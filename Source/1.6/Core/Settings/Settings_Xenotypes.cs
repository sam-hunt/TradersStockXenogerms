using System.Collections.Generic;
using Verse;

namespace XenogermTraderStock
{
    // "Xenotypes for sale" settings section: the per-xenotype opt-outs behind the
    // toggle grid (UI/XenotypeGridUI.cs draws it).
    //
    // The grid presents a whitelist (checked = sold) but the sets below store a
    // BLACKLIST, so the default is "everything on" and a xenotype added or
    // removed by another mod needs no migration: unknown names are simply never
    // matched. Presets are keyed by defName, player-scenario xenotypes by
    // CustomXenotype.name (they have no def). Read through XenotypeEligibility
    // rather than directly - these are only one input to the derived sellable
    // state, and a category toggle above overrides them without touching them.
    public partial class XenogermTraderStockSettings
    {
        public HashSet<string> excludedXenotypes = new HashSet<string>();
        public HashSet<string> excludedCustomXenotypes = new HashSet<string>();

        private void ExposeXenotypeSettings()
        {
            Scribe_Collections.Look(ref excludedXenotypes, "excludedXenotypes", LookMode.Value);
            Scribe_Collections.Look(ref excludedCustomXenotypes, "excludedCustomXenotypes", LookMode.Value);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                // Scribe_Collections nulls the target when the node is absent
                // (settings files written before the blacklist existed).
                excludedXenotypes ??= new HashSet<string>();
                excludedCustomXenotypes ??= new HashSet<string>();
            }
        }

        private void ResetXenotypeSettings()
        {
            excludedXenotypes.Clear();
            excludedCustomXenotypes.Clear();
        }

        public bool IsXenotypeExcluded(string defName)
        {
            return excludedXenotypes.Contains(defName);
        }

        public void SetXenotypeExcluded(string defName, bool excluded)
        {
            SetMembership(excludedXenotypes, defName, excluded);
        }

        public bool IsCustomXenotypeExcluded(string name)
        {
            return excludedCustomXenotypes.Contains(name);
        }

        public void SetCustomXenotypeExcluded(string name, bool excluded)
        {
            SetMembership(excludedCustomXenotypes, name, excluded);
        }

        private static void SetMembership(HashSet<string> set, string key, bool member)
        {
            if (key == null)
            {
                return;
            }
            if (member)
            {
                set.Add(key);
            }
            else
            {
                set.Remove(key);
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
