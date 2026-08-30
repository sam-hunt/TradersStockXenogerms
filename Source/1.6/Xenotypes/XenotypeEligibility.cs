using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    // Single source of truth for "may traders sell this xenotype?". Both the
    // stock generator and the settings grid read the *derived* state here, so
    // a category toggle (archite / inheritable / player-scenario) really does
    // override a per-xenotype opt-in rather than merely greying it out, and the
    // per-xenotype blacklist entry survives untouched underneath for when the
    // category is re-enabled.
    public static class XenotypeEligibility
    {
        private static XenogermTraderStockSettings Settings => XenogermTraderStockMod.Settings;

        // Which category toggle, if any, is currently suppressing a xenotype
        // with these traits. None means the per-xenotype toggle is live.
        public enum CategoryBlock
        {
            None,
            Archite,
            Inheritable,
            PlayerScenario,
        }

        // Pure, settings-driven category check shared by presets and custom
        // xenotypes. Archite wins over inheritable when both apply, matching the
        // order the toggles appear in the settings window.
        public static CategoryBlock GetCategoryBlock(XenogermTraderStockSettings settings,
            bool archite, bool inheritable, bool playerScenario)
        {
            if (playerScenario && !settings.includePlayerScenarioXenotypes)
            {
                return CategoryBlock.PlayerScenario;
            }
            if (archite && !settings.includeArchiteXenotypes)
            {
                return CategoryBlock.Archite;
            }
            if (inheritable && !settings.includeInheritableXenotypes)
            {
                return CategoryBlock.Inheritable;
            }
            return CategoryBlock.None;
        }

        // Derived sellable state from the raw inputs; the Def/CustomXenotype
        // overloads below are thin adapters over this so the logic is testable
        // headless (XenotypeDef.Archite needs live GeneCategoryDefOf).
        public static bool IsSellable(XenogermTraderStockSettings settings,
            bool archite, bool inheritable, bool playerScenario, bool excluded)
        {
            return !excluded
                && GetCategoryBlock(settings, archite, inheritable, playerScenario) == CategoryBlock.None;
        }

        // Xenotypes that can never appear in stock regardless of settings: the
        // Baseliner (nothing to implant), gene-less xenotypes, and anything
        // carrying a gene opted out via GeneExtension (VREA androids). These
        // are hidden from the grid outright rather than greyed like a category
        // block, since no setting can bring them back.
        public static bool IsCandidate(XenotypeDef xenotype)
        {
            return xenotype != XenotypeDefOf.Baseliner
                && !xenotype.genes.NullOrEmpty()
                && !ContainsExcludedGene(xenotype.genes);
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

        public static CategoryBlock GetCategoryBlock(XenotypeDef xenotype)
        {
            return GetCategoryBlock(Settings, xenotype.Archite, xenotype.inheritable, playerScenario: false);
        }

        public static CategoryBlock GetCategoryBlock(CustomXenotype xenotype)
        {
            return GetCategoryBlock(Settings, IsArchite(xenotype), xenotype.inheritable, playerScenario: true);
        }

        public static bool IsSellable(XenotypeDef xenotype)
        {
            return IsCandidate(xenotype)
                && IsSellable(Settings, xenotype.Archite, xenotype.inheritable, playerScenario: false,
                    excluded: Settings.IsXenotypeExcluded(xenotype.defName));
        }

        public static bool IsSellable(CustomXenotype xenotype)
        {
            return IsCandidate(xenotype)
                && IsSellable(Settings, IsArchite(xenotype), xenotype.inheritable, playerScenario: true,
                    excluded: Settings.IsCustomXenotypeExcluded(xenotype.name));
        }

        // CustomXenotype has no Archite property; mirror XenotypeDef.Archite's
        // gene-category test rather than biostatArc so the two agree on genes
        // that sit in the archite category but cost no archite capsules.
        public static bool IsArchite(CustomXenotype xenotype)
        {
            return xenotype.genes.Any(g => g.displayCategory == GeneCategoryDefOf.Archite);
        }

        // Presets in the order vanilla lists them (displayPriority descending),
        // then label to keep same-priority modded xenotypes stable.
        public static IEnumerable<XenotypeDef> CandidateXenotypes()
        {
            return DefDatabase<XenotypeDef>.AllDefsListForReading
                .Where(IsCandidate)
                .OrderByDescending(x => x.displayPriority)
                .ThenBy(x => x.LabelCap.ToString());
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
