using System.Collections.Generic;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    // Which preset xenotype does this xenogerm produce? The single answer read by
    // the implant stamp (GeneUtility_ImplantXenogermItem_Patch) and the ideology
    // gate (Xenogerm_PawnIdeoDisallowsImplanting_Patch). Pricing deliberately does
    // NOT use it - see the StatParts.
    //
    // Two sources, in order:
    //
    //  1. CompXenotypeSource.sourceXenotype - stamped by XenogermFactory on every
    //     trader-sold preset germ. Authoritative when present.
    //
    //  2. Gene-set inference. Comp state does not survive every path an item can
    //     take: ReSplice Core's xenogerm duplicator builds its copy with
    //     ThingMaker.MakeThing and copies only xenotypeName, iconDef and the gene
    //     list, so the copy of a Hussar germ has a null comp and would implant as a
    //     custom xenotype merely named "hussar" - same genes, no ideology
    //     recognition, no germline retarget. Any other cloner that copies fields
    //     rather than comps loses the def the same way. So a comp-less germ whose
    //     genes equal exactly one preset's gene list IS that preset. This is
    //     vanilla's own notion of identity - Pawn_GeneTracker.CustomXenotype and
    //     GeneUtility.PawnIsCustomXenotype resolve a pawn's custom xenotype by gene
    //     match and never by name - extended from custom templates to preset defs.
    //     A side effect by design: a player who assembles exactly Hussar's genes at
    //     the gene assembler gets a Hussar, whatever they typed in the name box.
    //
    // Inference only names what vanilla cannot. Before any preset is considered,
    // the game's custom xenotype database (Current.Game.customXenotypeDatabase -
    // the templates the player authored in the xenotype editor, seeded from the
    // scenario's starting pawns and the chosen ideos' xenotype precepts) gets first
    // claim: if a non-inheritable template there matches the germ's genes, the
    // resolver returns null and vanilla's implant runs untouched - the pawn's
    // CustomXenotype resolves to that template exactly as it would without this
    // mod. A player who went to the trouble of authoring a preset clone and not
    // picking the preset has expressed a preference, whatever the reason (an
    // interaction with some other mod, say), and this mod should not overrule it.
    // Non-inheritable only: an inheritable template is matched against ENDOgenes,
    // which a xenogene implant never writes, so vanilla could not label that
    // outcome anyway and deferring would trade a correct preset for a nameless
    // custom. The comp path is unaffected - a trader-sold germ stamps its def
    // regardless - so a bought germ and its comp-less copy can diverge in a game
    // holding such a clone; that is the player's template winning, by design.
    //
    // The germ's xenotypeName is a tie-breaker only. Two loaded presets sharing one
    // gene list (a mod re-shipping a vanilla xenotype under its own defName) are
    // split by comparing the name to each candidate's label / defName; a tie the
    // name cannot break resolves to null and the germ implants as vanilla would.
    // Name-first matching was rejected: a player working towards a preset one
    // genepack at a time ("hussar", "hussar2", "hussar3") ends with a germ that is
    // a Hussar in every way vanilla can measure, and the name they happened to type
    // last should not decide that.
    //
    // Never infers a gene-less preset. Baseliner's empty germ is the
    // baseliner-conversion item - its implant wipes both gene layers - and it is
    // only ever created with the comp set (XenogermFactory / the debug spawner).
    // Vanilla cannot produce an empty xenogerm, but a modded or malformed one must
    // not turn into a germline wipe by accident.
    public static class XenogermIdentity
    {
        public static XenotypeDef Resolve(Xenogerm xenogerm)
        {
            if (xenogerm == null)
            {
                return null;
            }

            XenotypeDef stored = xenogerm.TryGetComp<CompXenotypeSource>()?.sourceXenotype;
            if (stored != null)
            {
                return stored;
            }

            GeneSet geneSet = xenogerm.GeneSet;
            if (geneSet == null)
            {
                return null;
            }

            return InferPreset(
                geneSet.GenesListForReading,
                xenogerm.xenotypeName,
                DefDatabase<XenotypeDef>.AllDefsListForReading,
                Current.Game?.customXenotypeDatabase?.customXenotypes);
        }

        // Pure over its inputs so the headless suite can cover it. Returns null when
        // a non-inheritable custom template claims the genes; otherwise the one
        // candidate whose genes match the germ's (see GenesMatch); with several, the
        // one whose label or defName equals xenotypeName (case-insensitive);
        // otherwise null.
        public static XenotypeDef InferPreset(
            List<GeneDef> germGenes,
            string xenotypeName,
            IEnumerable<XenotypeDef> candidates,
            IEnumerable<CustomXenotype> customTemplates = null)
        {
            if (germGenes == null || germGenes.Count == 0 || candidates == null)
            {
                return null;
            }

            if (ClaimedByCustomTemplate(germGenes, customTemplates))
            {
                return null;
            }

            XenotypeDef single = null;
            List<XenotypeDef> ties = null;
            foreach (XenotypeDef candidate in candidates)
            {
                if (candidate?.genes == null || candidate.genes.Count == 0
                    || !GenesMatch(germGenes, candidate.genes))
                {
                    continue;
                }

                if (single == null)
                {
                    single = candidate;
                }
                else
                {
                    ties ??= new List<XenotypeDef> { single };
                    ties.Add(candidate);
                }
            }

            if (ties == null)
            {
                return single;
            }

            return BreakTieByName(ties, xenotypeName);
        }

        private static bool ClaimedByCustomTemplate(List<GeneDef> germGenes, IEnumerable<CustomXenotype> customTemplates)
        {
            if (customTemplates == null)
            {
                return false;
            }

            foreach (CustomXenotype custom in customTemplates)
            {
                if (custom != null && !custom.inheritable && GenesMatch(germGenes, custom.genes))
                {
                    return true;
                }
            }

            return false;
        }

        private static XenotypeDef BreakTieByName(List<XenotypeDef> ties, string xenotypeName)
        {
            if (xenotypeName.NullOrEmpty())
            {
                return null;
            }

            string name = xenotypeName.Trim();
            XenotypeDef named = null;
            foreach (XenotypeDef candidate in ties)
            {
                if (!NameMatches(candidate, name))
                {
                    continue;
                }

                if (named != null)
                {
                    return null;
                }

                named = candidate;
            }

            return named;
        }

        private static bool NameMatches(XenotypeDef def, string name)
        {
            return string.Equals(def.label, name, System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(def.defName, name, System.StringComparison.OrdinalIgnoreCase);
        }

        // GeneUtility.PawnIsCustomXenotype's rule, applied to two gene lists: every
        // passOnDirectly gene on either side must appear on the other. Genes with
        // passOnDirectly=false (vanilla: none shipped, but modded ones exist) are
        // ignored by both, exactly as vanilla ignores them.
        public static bool GenesMatch(List<GeneDef> germGenes, List<GeneDef> templateGenes)
        {
            if (germGenes == null || templateGenes == null)
            {
                return false;
            }

            foreach (GeneDef gene in templateGenes)
            {
                if (gene.passOnDirectly && !germGenes.Contains(gene))
                {
                    return false;
                }
            }

            foreach (GeneDef gene in germGenes)
            {
                if (gene.passOnDirectly && !templateGenes.Contains(gene))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
