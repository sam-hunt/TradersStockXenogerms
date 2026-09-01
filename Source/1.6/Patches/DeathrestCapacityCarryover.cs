using RimWorld;
using Verse;

namespace XenogermTraderStock.Patches
{
    // Shared capture/restore for the deathrest-capacity carryover, used by the
    // prefix/postfix pairs on GeneUtility.ImplantXenogermItem and
    // GeneUtility.ReimplantXenogerm.
    //
    // Deathrest capacity is pure per-instance state: a private int on
    // Gene_Deathrest, default 1, raised only by deathrest capacity serums
    // calling OffsetCapacity on the live gene. Vanilla always re-adds genes by
    // def (GeneMaker.MakeGene builds a fresh instance) and
    // Gene_Deathrest.PostAdd unconditionally Reset()s capacity to 1, so every
    // implant or reimplant that grants deathrest again silently discards the
    // pawn's serum investment. There is no vanilla precedent for carrying gene
    // instance state across a remove/re-add - the only correct pattern is to
    // read the value before the wipe and write it back after PostAdd has run,
    // which is what these two calls do around the patched methods.
    //
    // Deliberately capacity-only: bound deathrest buildings also live on the
    // gene instance, but rebinding costs nothing (the pawn just deathrests
    // again), while capacity costs ~1000+ silver per serum. Generic carryover
    // of arbitrary (modded) gene state is off the table - it would need
    // ExposeData round-trips outside a load pass, with loadID and
    // Scribe_References hazards.
    internal static class DeathrestCapacityCarryover
    {
        // The pawn's current deathrest capacity, or 0 when it has no deathrest
        // gene (capacity itself is always >= 1). Serums target
        // GetFirstGeneOfType<Gene_Deathrest>, so the first gene is the one
        // whose capacity holds the investment.
        public static int Snapshot(Pawn pawn)
        {
            return pawn?.genes?.GetFirstGeneOfType<Gene_Deathrest>()?.DeathrestCapacity ?? 0;
        }

        // Raises the pawn's deathrest capacity back to the snapshot. Offset
        // rather than assignment because that is the only public mutator, and
        // max semantics rather than overwrite so a deathrest gene that
        // survived untouched (an endogene outside the wipe) is left alone.
        // No notification: to the player nothing changed.
        public static void Restore(Pawn pawn, int capacity)
        {
            if (capacity <= 0)
            {
                return;
            }

            Gene_Deathrest gene = pawn?.genes?.GetFirstGeneOfType<Gene_Deathrest>();
            if (gene != null && gene.DeathrestCapacity < capacity)
            {
                gene.OffsetCapacity(capacity - gene.DeathrestCapacity, sendNotification: false);
            }
        }
    }
}
