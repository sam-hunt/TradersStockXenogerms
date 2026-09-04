using System.Collections.Generic;
using RimWorld;
using Verse;

namespace XenogermTraderStock.Patches
{
    // Pure decision behind Xenogerm_PawnIdeoDisallowsImplanting_Patch: would
    // implanting this xenogerm leave the pawn as one of the ideo's preferred
    // xenotypes? Kept free of game state so the headless suite can cover it.
    //
    // The prediction mirrors what Ideo.IsPreferredXenotype will read AFTER the
    // implant, per implant path:
    //
    //  * Preset (sourceXenotype resolved by XenogermIdentity - the trader comp, or
    //    a comp-less germ whose genes equal exactly one preset's and no
    //    non-inheritable template in the game's custom xenotype database): the
    //    implant patch stamps the def onto the tracker, so the pawn reads as that
    //    preset. Preferred iff the ideo's PreferredXenotypes list holds the def
    //    (Baseliner included - it is a XenotypeDef and can be preferred like any
    //    other). A preferred CUSTOM template sharing a preset's gene list never
    //    reaches this branch: such a template can only come from the xenotype
    //    editor (Precept_Xenotype's picker offers XenotypeDefs plus
    //    CharacterCardUtility's on-disk custom xenotype files - the gene assembler
    //    saves xenogerm TEMPLATES, a different file kind that never reaches an
    //    ideo), and every custom precept's template is seeded into the game's
    //    database at scenario start, where XenogermIdentity lets it claim the germ
    //    first, so the germ falls through to the custom branch below.
    //
    //  * Anything else (custom-template or player-crafted): vanilla leaves the
    //    tracker at Baseliner with the germ's genes as xenogenes. The tracker then
    //    resolves CustomXenotype by GENE MATCH against the database - the item's
    //    xenotypeName is never consulted - and IsPreferredXenotype checks that
    //    resolved template against the ideo's PreferredCustomXenotypes with the
    //    same matcher (GeneUtility.PawnIsCustomXenotype). So: preferred iff a
    //    preferred custom template's passOnDirectly genes equal the germ's. A
    //    template marked inheritable is matched against ENDOgenes, which a
    //    xenogene implant never writes, so no xenogerm can turn a pawn into one -
    //    those templates grant no exception here.
    public static class PreferredXenotypeGate
    {
        public static bool ImplantYieldsPreferred(
            XenotypeDef sourceXenotype,
            List<GeneDef> germGenes,
            List<XenotypeDef> preferredXenotypes,
            List<CustomXenotype> preferredCustomXenotypes)
        {
            if (sourceXenotype != null)
            {
                return preferredXenotypes?.Contains(sourceXenotype) == true;
            }

            if (preferredCustomXenotypes == null || germGenes == null)
            {
                return false;
            }

            foreach (CustomXenotype custom in preferredCustomXenotypes)
            {
                if (!custom.inheritable && GenesMatch(germGenes, custom.genes))
                {
                    return true;
                }
            }

            return false;
        }

        // Vanilla's custom-xenotype matcher over two gene lists; shared with the
        // preset inference, which is the same rule aimed at XenotypeDefs.
        public static bool GenesMatch(List<GeneDef> germGenes, List<GeneDef> templateGenes)
        {
            return XenogermIdentity.GenesMatch(germGenes, templateGenes);
        }
    }
}
