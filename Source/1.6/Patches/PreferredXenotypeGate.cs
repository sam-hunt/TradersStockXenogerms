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
    //  * Trader-sold preset (CompXenotypeSource set): the implant patch stamps the
    //    source def onto the tracker, so the pawn reads as that preset. Preferred
    //    iff the ideo's PreferredXenotypes list holds the def (Baseliner included -
    //    it is a XenotypeDef and can be preferred like any other).
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

        // GeneUtility.PawnIsCustomXenotype's rule, applied to a gene list instead of a
        // pawn layer: every passOnDirectly gene on either side must appear on the
        // other. Genes with passOnDirectly=false (vanilla: none shipped, but modded
        // ones exist) are ignored by both, exactly as vanilla ignores them.
        public static bool GenesMatch(List<GeneDef> germGenes, List<GeneDef> templateGenes)
        {
            if (templateGenes == null)
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
