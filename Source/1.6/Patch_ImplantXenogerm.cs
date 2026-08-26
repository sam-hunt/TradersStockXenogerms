using HarmonyLib;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    // After xenogerm implantation, assigns the preset xenotype reference to the pawn.
    //
    // Vanilla implantation (GeneUtility.ImplantXenogermItem) adds genes and sets xenotypeName,
    // but leaves the pawn as "Baseliner" with a custom gene set. This patch uses SetXenotypeDirect
    // to assign the actual XenotypeDef reference, enabling:
    // - Ideology recognition (preferred xenotypes)
    // - Proper xenotype display in social/info panels
    // - "Naturalized" members of germline xenotypes (e.g., Impid) for social purposes
    [HarmonyPatch(typeof(GeneUtility), nameof(GeneUtility.ImplantXenogermItem))]
    public static class Patch_ImplantXenogermItem
    {
        public static void Postfix(Pawn pawn, Xenogerm xenogerm)
        {
            var comp = xenogerm.TryGetComp<CompXenotypeSource>();
            if (comp?.sourceXenotype != null && pawn.genes != null)
            {
                // SetXenotypeDirect only sets the xenotype field - it doesn't modify genes.
                // The genes were already added by the vanilla implantation method.
                pawn.genes.SetXenotypeDirect(comp.sourceXenotype);
            }
        }
    }
}
