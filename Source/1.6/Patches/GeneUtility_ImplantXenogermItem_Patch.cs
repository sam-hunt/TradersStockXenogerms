using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace XenogermTraderStock.Patches
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
    // Optionally (settings.implantGermlineAsEndogenes), a germline xenotype's genes
    // replace the pawn's endogenes - what PawnGenerator produces for a born member of an
    // inheritable xenotype (SetXenotype adds them as endogenes) - so children inherit them
    // and a later xenogerm implant stacks on top rather than wiping them. Vanilla's own
    // implant keeps everything as xenogenes, hence a setting (default on).
    [HarmonyPatch(typeof(GeneUtility), nameof(GeneUtility.ImplantXenogermItem))]
    public static class GeneUtility_ImplantXenogermItem_Patch
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

            // Baseliner bypasses the germline setting: its gene-less xenogerm has
            // exactly one honest meaning - make this pawn a baseliner - and with
            // no genes to add there is no "keep it as xenogenes" alternative for
            // the setting to choose. Vanilla has already wiped the xenogenes
            // (SetXenotype clears them before consulting the empty gene list);
            // the retarget clears the germline too, adding nothing back except
            // the pawn's own skin/hair colour, which Baseliner never supplies.
            // Without this, an Impid implanted with a baseliner xenogerm would
            // be relabeled Baseliner while keeping every Impid germline gene.
            if (comp.sourceXenotype == XenotypeDefOf.Baseliner
                || (comp.sourceXenotype.inheritable && XenogermTraderStockMod.Settings.implantGermlineAsEndogenes))
            {
                RetargetToEndogenes(pawn, xenogerm);
            }
        }

        // Rebuilds the pawn's germline as the implant's genes. Vanilla has just
        // SetXenotype(Baseliner)'d the pawn (every prior xenogene gone) and added exactly
        // the xenogerm's genes as xenogenes, so clearing the xenogene list removes the
        // implant and nothing else. The previous endogenes are then replaced wholesale,
        // not merged: the pawn becomes a born member of the xenotype, not a hybrid of it
        // and whatever germline it had before - and a merge would also let old genes win
        // conflicts, because two endogenes resolve by display order (GeneUtility.Overrides
        // -> GenesInOrder), not arrival order.
        //
        // Skin and hair colour are the exception PawnGenerator itself makes: after
        // SetXenotype it backfills a random melanin / hair-colour gene whenever the
        // xenotype supplies none (GetMelaninGene / GetHairColorGene == null). Here the
        // pawn's own genes are re-added instead of random ones - an Impid germline still
        // turns the pawn red, but a xenotype with no skin gene leaves the pawn's natural
        // colouring alone, exactly the born-member look.
        //
        // Consequences that follow from having no xenogenes, exactly as for a born member:
        // no xenogerm can be extracted from the pawn (GeneUtility.CanAbsorbXenogerm), and
        // the pawn no longer counts as having an implanted part for that purpose.
        private static void RetargetToEndogenes(Pawn pawn, Xenogerm xenogerm)
        {
            Pawn_GeneTracker genes = pawn.genes;
            genes.ClearXenogenes();

            GeneDef melanin = genes.GetMelaninGene();
            GeneDef hairColor = genes.GetHairColorGene();

            List<Gene> endogenes = genes.Endogenes;
            for (int i = endogenes.Count - 1; i >= 0; i--)
            {
                genes.RemoveGene(endogenes[i]);
            }

            foreach (GeneDef gene in xenogerm.GeneSet.GenesListForReading)
            {
                genes.AddGene(gene, xenogene: false);
            }

            if (genes.GetMelaninGene() == null && melanin != null)
            {
                genes.AddGene(melanin, xenogene: false);
            }

            if (genes.GetHairColorGene() == null && hairColor != null)
            {
                genes.AddGene(hairColor, xenogene: false);
            }
        }
    }
}
