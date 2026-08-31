using Verse;

namespace XenogermTraderStock
{
    // Seeds a sold-ledger entry for every xenotype loaded this launch, so the
    // first settings-window open and the first trader roll read a fully
    // populated ledger instead of each writing part of it.
    //
    // Once per process is enough for THIS state: entries are keyed by
    // defName/name strings, and an in-process play-data reload (a mid-session
    // language change replaces every def instance) cannot change the set of
    // names - only a mod-list change can, and that restarts the game. State
    // derived from def INSTANCES must never hide behind this attribute; see
    // UniqueMeleeWeapons' StaticConstructorOnStartupUtility_CallAll_Patch for
    // the re-run hook the day that changes. Custom xenotypes are per-game
    // besides, which is why SeedUnseen also runs from the settings grid and
    // the stock generator.
    [StaticConstructorOnStartup]
    public static class XenotypeLedgerStartup
    {
        static XenotypeLedgerStartup()
        {
            XenotypeEligibility.SeedUnseen();
        }
    }
}
