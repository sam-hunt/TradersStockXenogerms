using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    // After xenogerm implantation, assigns the preset xenotype reference to the pawn.
    //
    // Vanilla implantation (GeneUtility.ImplantXenogermItem) resets the pawn to Baseliner,
    // copies the item's xenotypeName and iconDef onto the gene tracker, then adds the genes
    // as xenogenes. The result is a "custom" xenotype that merely shares the preset's name.
    //
    // This postfix runs inside Recipe_ImplantXenogerm.ApplyOnPawn, immediately after the
    // vanilla method returns and before anything else can observe the pawn, and leaves the
    // gene tracker's identity fields exactly as PawnGenerator leaves them for a generated
    // pawn of that xenotype:
    //   xenotype     = the preset def   (SetXenotypeDirect; genes are untouched)
    //   xenotypeName = null             (cleared by SetXenotypeDirect; label falls through to the def)
    //   iconDef      = null             (cleared here; XenotypeIcon falls through to the def's icon)
    //
    // SetXenotypeDirect deliberately does not clear iconDef (only SetXenotype does, and that
    // would also wipe the freshly implanted xenogenes), so it must be nulled explicitly. The
    // item's iconDef is never null in practice: Xenogerm.ExposeData backfills
    // XenotypeIconDefOf.Basic on load, which is how the "custom xenotype" icon leaked onto
    // pawns implanted from a xenogerm that had been through a save/load cycle.
    //
    // Not touched: Pawn_GeneTracker.hybrid. It describes the pawn's germline (set by
    // PregnancyUtility for a child of two inheritable xenotypes, read only for inheritance),
    // which a xenogerm implant does not change - vanilla leaves it alone too.
    //
    // This enables:
    // - Ideology recognition (preferred xenotypes)
    // - Proper xenotype display in social/info panels (label, icon, info-card link)
    // - "Naturalized" members of germline xenotypes (e.g., Impid) for social purposes
    //
    // Optionally (settings.ImplantsGermlineAsEndogenes), a germline xenotype's genes are
    // then moved from xenogenes into the pawn's endogenes - what PawnGenerator does for a
    // born member of an inheritable xenotype (SetXenotype adds them as endogenes) - so
    // children inherit them and a later xenogerm implant stacks on top rather than wiping
    // them. Vanilla's own implant keeps everything as xenogenes, hence opt-in.
    [HarmonyPatch(typeof(GeneUtility), nameof(GeneUtility.ImplantXenogermItem))]
    public static class Patch_ImplantXenogermItem
    {
        public static void Postfix(Pawn pawn, Xenogerm xenogerm)
        {
            var comp = xenogerm.TryGetComp<CompXenotypeSource>();
            if (comp?.sourceXenotype == null || pawn.genes == null)
            {
                return;
            }

            // Mirrors GeneUtility.ReimplantXenogerm's field handling: set the xenotype, then
            // assign the identity fields explicitly. Genes stay as the xenogenes vanilla added.
            pawn.genes.SetXenotypeDirect(comp.sourceXenotype);
            pawn.genes.iconDef = null;

            if (comp.sourceXenotype.inheritable && XenogermTraderStockMod.Settings.ImplantsGermlineAsEndogenes)
            {
                RetargetToEndogenes(pawn, xenogerm);
            }
        }

        // Re-adds the implant's genes as endogenes. Vanilla has just SetXenotype(Baseliner)'d
        // the pawn (every prior xenogene gone) and added exactly the xenogerm's genes as
        // xenogenes, so clearing the xenogene list removes the implant and nothing else.
        //
        // A conflicting germline gene the pawn already had (its own skin or hair colour,
        // say) is removed first. Two endogenes in conflict are not resolved by arrival
        // order but by display order (GeneUtility.Overrides -> GenesInOrder), so leaving
        // the pawn's own gene in place could let it win over the implant's and the pawn
        // would keep, e.g., a fair skin under an Impid germline. Xenogenes never had this
        // problem because a xenogene always overrides an endogene.
        //
        // Consequences that follow from having no xenogenes, exactly as for a born member:
        // no xenogerm can be extracted from the pawn (GeneUtility.CanAbsorbXenogerm), and
        // the pawn no longer counts as having an implanted part for that purpose.
        private static void RetargetToEndogenes(Pawn pawn, Xenogerm xenogerm)
        {
            pawn.genes.ClearXenogenes();
            List<Gene> endogenes = pawn.genes.Endogenes;
            foreach (GeneDef gene in xenogerm.GeneSet.GenesListForReading)
            {
                for (int i = endogenes.Count - 1; i >= 0; i--)
                {
                    Gene existing = endogenes[i];
                    // An identical gene already in the germline stays put; AddGene
                    // below is a no-op for it (Pawn_GeneTracker.HasEndogene).
                    if (existing.def != gene && existing.def.ConflictsWith(gene))
                    {
                        pawn.genes.RemoveGene(existing);
                    }
                }
                pawn.genes.AddGene(gene, xenogene: false);
            }
        }
    }
}
