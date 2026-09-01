using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    // Single source of truth for "may traders sell this xenotype?". The answer
    // lives in the settings' per-xenotype sold ledger; this class owns
    // candidacy (which xenotypes can appear at all), the category partition the
    // filter rows and the seeding share, and the seeding itself - the majority
    // vote that writes a ledger entry the first time a xenotype is seen. The
    // stock generator and the settings grid both read through here; the filter
    // rows above the grid are pure derivations of the ledger (a tri-state
    // summary plus bulk edits), never separate state.
    public static class XenotypeEligibility
    {
        private static XenogermTraderStockSettings Settings => XenogermTraderStockMod.Settings;

        // Disjoint grouping: every candidate lands in exactly ONE category, the
        // first that applies, so the filter rows partition the grid and the
        // seeding vote has an unambiguous peer group. The precedence mirrors
        // the retired category toggles': a scenario xenotype groups by origin
        // over gene content, an archite one by its price-defining genes over
        // inheritability.
        public enum XenotypeCategory
        {
            PlayerScenario,
            Archite,
            Inheritable,
            Plain,
        }

        // Pure so it is testable headless; the Def/CustomXenotype overloads
        // below are thin adapters (XenotypeDef.Archite needs live
        // GeneCategoryDefOf).
        public static XenotypeCategory Categorize(bool archite, bool inheritable, bool playerScenario)
        {
            if (playerScenario)
            {
                return XenotypeCategory.PlayerScenario;
            }
            if (archite)
            {
                return XenotypeCategory.Archite;
            }
            if (inheritable)
            {
                return XenotypeCategory.Inheritable;
            }
            return XenotypeCategory.Plain;
        }

        public static XenotypeCategory Categorize(XenotypeDef xenotype)
        {
            return Categorize(xenotype.Archite, GatesAsInheritable(xenotype), playerScenario: false);
        }

        public static XenotypeCategory Categorize(CustomXenotype xenotype)
        {
            return Categorize(IsArchite(xenotype), xenotype.inheritable, playerScenario: true);
        }

        // Seed value for a xenotype the ledger has never seen: the majority
        // sold state of its category peers, because the player's own pattern
        // is a better guess than any fixed default - a player who ticked every
        // archite xenotype wants a new archite mod's on sale too, and one who
        // cleared the group wants it kept out. Ties (the empty first-run
        // ledger included) fall back to the shipped default: germline-
        // rewriting xenotypes start unsold - implanting one converts the pawn
        // outright - and everything else starts sold.
        public static bool SeedValue(XenotypeCategory category, bool gatesAsInheritable,
            IEnumerable<(XenotypeCategory category, bool sold)> ledger)
        {
            int balance = 0;
            foreach ((XenotypeCategory peerCategory, bool sold) in ledger)
            {
                if (peerCategory == category)
                {
                    balance += sold ? 1 : -1;
                }
            }
            return balance != 0 ? balance > 0 : !gatesAsInheritable;
        }

        // Ledger reconciliation: every live candidate without an entry gets
        // one, voted against the PRE-pass ledger so simultaneous newcomers (a
        // new mod's whole roster) all get the same answer instead of voting on
        // each other. Only live candidates vote: entries whose xenotype is no
        // longer loaded stay dormant - never consulted, never voting, never
        // pruned - so a temporarily disabled mod gets its choices back on
        // re-enable. Idempotent and cheap once seeded; runs at startup
        // (XenotypeLedgerStartup), from the settings grid and from stock
        // generation - the custom pool is per-game, so startup alone cannot
        // cover it.
        public static void SeedUnseen()
        {
            XenogermTraderStockSettings settings = Settings;
            var snapshot = new List<(XenotypeCategory category, bool sold)>();
            var unseenPresets = new List<XenotypeDef>();
            var unseenCustoms = new List<CustomXenotype>();

            foreach (XenotypeDef xenotype in CandidateXenotypes())
            {
                bool? sold = settings.GetXenotypeSold(xenotype.defName);
                if (sold.HasValue)
                {
                    snapshot.Add((Categorize(xenotype), sold.Value));
                }
                else
                {
                    unseenPresets.Add(xenotype);
                }
            }
            foreach (CustomXenotype custom in CandidateCustomXenotypes())
            {
                bool? sold = settings.GetCustomXenotypeSold(custom.name);
                if (sold.HasValue)
                {
                    snapshot.Add((Categorize(custom), sold.Value));
                }
                else
                {
                    unseenCustoms.Add(custom);
                }
            }

            foreach (XenotypeDef xenotype in unseenPresets)
            {
                settings.SetXenotypeSold(xenotype.defName,
                    SeedValue(Categorize(xenotype), GatesAsInheritable(xenotype), snapshot));
            }
            foreach (CustomXenotype custom in unseenCustoms)
            {
                settings.SetCustomXenotypeSold(custom.name,
                    SeedValue(Categorize(custom), custom.inheritable, snapshot));
            }
        }

        // Xenotypes that can never appear in stock regardless of settings:
        // gene-less xenotypes and anything carrying a gene opted out via
        // GeneExtension (VREA androids). These are hidden from the grid
        // outright rather than shown unticked, since no setting can bring them
        // back. Baseliner is the deliberate exception to the gene-less rule:
        // its empty xenogerm is the "make this pawn a baseliner" item -
        // vanilla implantation wipes all xenogenes before consulting the
        // germ's (empty) gene list, and the implant patch clears the germline
        // too.
        public static bool IsCandidate(XenotypeDef xenotype)
        {
            return xenotype == XenotypeDefOf.Baseliner
                || (!xenotype.genes.NullOrEmpty() && !ContainsExcludedGene(xenotype.genes));
        }

        public static bool IsCandidate(CustomXenotype xenotype)
        {
            return !xenotype.genes.NullOrEmpty() && !ContainsExcludedGene(xenotype.genes);
        }

        // Pure so it is testable headless: Def.GetModExtension only walks the
        // modExtensions list and tolerates it being null.
        public static bool ContainsExcludedGene(IEnumerable<GeneDef> genes)
        {
            return genes.Any(g => g.GetModExtension<GeneExtension>()?.excludeFromXenogermStock == true);
        }

        // Whether any of these genes forbids the wholesale germline rewrite of
        // a pawn carrying it as an endogene (the implant patch's retarget).
        // excludeFromXenogermStock implies the block - germline machinery too
        // alien to sell is also too alien to overwrite - while
        // blocksGermlineRetarget alone leaves the xenotype sellable: IsCandidate
        // deliberately never reads it. Pure like ContainsExcludedGene.
        public static bool BlocksGermlineRetarget(IEnumerable<GeneDef> genes)
        {
            return genes.Any(g =>
            {
                GeneExtension ext = g.GetModExtension<GeneExtension>();
                return ext != null && (ext.excludeFromXenogermStock || ext.blocksGermlineRetarget);
            });
        }

        // Baseliner's def is not inheritable (there are no genes to inherit),
        // but its xenogerm always rewrites the germline - the implant patch's
        // unconditional retarget - so it categorizes and seed-defaults as
        // inheritable: the conservative unsold default is about germline
        // rewriting, not gene inheritance per se.
        public static bool GatesAsInheritable(XenotypeDef xenotype)
        {
            return xenotype.inheritable || xenotype == XenotypeDefOf.Baseliner;
        }

        public static bool IsSellable(XenotypeDef xenotype)
        {
            if (!IsCandidate(xenotype))
            {
                return false;
            }
            bool? sold = Settings.GetXenotypeSold(xenotype.defName);
            if (!sold.HasValue)
            {
                // Belt and braces: the grid, the generator and startup all
                // seed before reading, so a miss here means a call path that
                // forgot to - seed everything rather than guess one entry.
                SeedUnseen();
                sold = Settings.GetXenotypeSold(xenotype.defName);
            }
            return sold ?? false;
        }

        public static bool IsSellable(CustomXenotype xenotype)
        {
            if (!IsCandidate(xenotype))
            {
                return false;
            }
            bool? sold = Settings.GetCustomXenotypeSold(xenotype.name);
            if (!sold.HasValue)
            {
                SeedUnseen();
                sold = Settings.GetCustomXenotypeSold(xenotype.name);
            }
            return sold ?? false;
        }

        // CustomXenotype has no Archite property; mirror XenotypeDef.Archite's
        // gene-category test rather than biostatArc so the two agree on genes
        // that sit in the archite category but cost no archite capsules.
        public static bool IsArchite(CustomXenotype xenotype)
        {
            return xenotype.genes.Any(g => g.displayCategory == GeneCategoryDefOf.Archite);
        }

        // Presets in the order vanilla lists them (displayPriority descending),
        // then label to keep same-priority modded xenotypes stable. Baseliner is
        // pinned first outright: vanilla's displayPriority 1000 is already the
        // ceiling, but a modded xenotype outranking it would otherwise displace
        // the one entry every pool wants on top. Shared by the settings grid and
        // the debug spawner so the pools always agree.
        public static IEnumerable<XenotypeDef> InDisplayOrder(IEnumerable<XenotypeDef> xenotypes)
        {
            return xenotypes
                .OrderByDescending(x => x == XenotypeDefOf.Baseliner)
                .ThenByDescending(x => x.displayPriority)
                .ThenBy(x => x.LabelCap.ToString());
        }

        public static IEnumerable<XenotypeDef> CandidateXenotypes()
        {
            return InDisplayOrder(DefDatabase<XenotypeDef>.AllDefsListForReading.Where(IsCandidate));
        }

        // The live game's database while playing (the one the generator reads);
        // the on-disk xenotype files from the main menu so the grid isn't empty.
        public static IEnumerable<CustomXenotype> CandidateCustomXenotypes()
        {
            var source = Current.Game?.customXenotypeDatabase?.customXenotypes
                ?? CharacterCardUtility.CustomXenotypesForReading;
            return source?.Where(IsCandidate) ?? Enumerable.Empty<CustomXenotype>();
        }
    }
}
