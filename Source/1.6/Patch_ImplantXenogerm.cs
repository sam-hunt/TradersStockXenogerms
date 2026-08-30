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
        }
    }
}
