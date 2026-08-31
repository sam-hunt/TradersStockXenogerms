using System.Collections.Generic;
using Verse;

namespace XenogermTraderStock
{
    // "Xenotypes for sale" settings section: the per-xenotype sold ledger
    // behind the toggle grid and its category filter rows (UI/XenotypeGridUI.cs
    // draws both).
    //
    // The ledger stores an EXPLICIT sold bool for every xenotype ever seen -
    // presets keyed by defName, player-scenario xenotypes by
    // CustomXenotype.name (they have no def). A missing entry means the
    // xenotype has never been loaded on this install;
    // XenotypeEligibility.SeedUnseen writes it the first time it shows up,
    // majority-voted from its category peers. Entries whose xenotype is no
    // longer loaded stay dormant rather than being pruned, so a temporarily
    // disabled mod keeps its choices. Read through XenotypeEligibility rather
    // than directly - it owns candidacy and seeding.
    public partial class XenogermTraderStockSettings
    {
        public Dictionary<string, bool> soldXenotypes = new Dictionary<string, bool>();
        public Dictionary<string, bool> soldCustomXenotypes = new Dictionary<string, bool>();

        private void ExposeXenotypeSettings()
        {
            Scribe_Collections.Look(ref soldXenotypes, "soldXenotypes", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref soldCustomXenotypes, "soldCustomXenotypes", LookMode.Value, LookMode.Value);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                // Scribe_Collections nulls the target when the node is absent
                // (settings files written before the ledger existed).
                soldXenotypes ??= new Dictionary<string, bool>();
                soldCustomXenotypes ??= new Dictionary<string, bool>();
            }
        }

        // Clearing the ledger IS the reset: the next SeedUnseen pass re-seeds
        // an empty ledger to the shipped defaults, because every majority vote
        // ties.
        private void ResetXenotypeSettings()
        {
            soldXenotypes.Clear();
            soldCustomXenotypes.Clear();
        }

        public bool? GetXenotypeSold(string defName)
        {
            return GetEntry(soldXenotypes, defName);
        }

        public void SetXenotypeSold(string defName, bool sold)
        {
            SetEntry(soldXenotypes, defName, sold);
        }

        public bool? GetCustomXenotypeSold(string name)
        {
            return GetEntry(soldCustomXenotypes, name);
        }

        public void SetCustomXenotypeSold(string name, bool sold)
        {
            SetEntry(soldCustomXenotypes, name, sold);
        }

        private static bool? GetEntry(Dictionary<string, bool> ledger, string key)
        {
            return key != null && ledger.TryGetValue(key, out bool sold) ? sold : (bool?)null;
        }

        private static void SetEntry(Dictionary<string, bool> ledger, string key, bool sold)
        {
            if (key != null)
            {
                ledger[key] = sold;
            }
        }

        private static void DrawXenotypesSection(Listing_Standard listing)
        {
            SectionHeader(listing, "XTS_XenotypesSection".Translate(), "XTS_XenotypesSectionDesc".Translate());
            XenotypeGridUI.Draw(listing);
            listing.Gap(SectionGap);
        }
    }
}
