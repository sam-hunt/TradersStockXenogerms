using HarmonyLib;
using RimWorld;
using Verse;

namespace XenogermTraderStock.Patches
{
    // Companion to GeneUtility_ImplantXenogermItem_Patch's deathrest-capacity
    // carryover, for the sanguophage reimplant ability. Vanilla ReimplantXenogerm
    // clears the recipient's xenogenes and re-adds the caster's by def, so a
    // recipient who already had deathrest gets a fresh Gene_Deathrest whose
    // PostAdd resets capacity to 1 - discarding the recipient's serum
    // investment. Capacity is the recipient's own: what is restored is what THIS
    // pawn had before the operation, never the caster's (the caster's serums
    // fed the caster's body, and vanilla transfers genes by def only).
    //
    // The caster is left alone: it keeps its gene instances, except in the
    // would-die-from-reimplanting branch where vanilla deliberately reverts it
    // to Baseliner - no deathrest gene remains to carry capacity on.
    [HarmonyPatch(typeof(GeneUtility), nameof(GeneUtility.ReimplantXenogerm))]
    public static class GeneUtility_ReimplantXenogerm_Patch
    {
        public static void Prefix(Pawn recipient, out int __state)
        {
            __state = XenogermTraderStockMod.Settings.preserveDeathrestCapacity
                ? DeathrestCapacityCarryover.Snapshot(recipient)
                : 0;
        }

        public static void Postfix(Pawn recipient, int __state)
        {
            DeathrestCapacityCarryover.Restore(recipient, __state);
        }
    }
}
