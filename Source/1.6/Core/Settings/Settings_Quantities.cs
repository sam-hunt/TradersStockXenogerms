using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    // "Stock quantity" settings section: how many xenogerms each patched
    // trader kind rolls per visit. Stored as OVERRIDES keyed by
    // TraderKindDef.defName: no entry means the XML countRange on that
    // trader's StockGenerator_Xenogerms applies, so the XML patches stay the
    // single source of shipped defaults and a row dragged back onto its
    // default drops the entry rather than pinning today's XML value forever.
    // Read live at generation time (StockGenerator_Xenogerms), which is why
    // configurable counts need no def write-back and no restart - the change
    // shows on newly generated traders only, like every other setting here.
    public partial class XenogermTraderStockSettings
    {
        // Upper slider bound: comfortably above the largest shipped default
        // (the Gene Trader's 4) without letting a drag flood a trader.
        public const int MaxTraderCount = 8;

        public Dictionary<string, IntRange> traderCountRanges = new Dictionary<string, IntRange>();

        private void ExposeQuantitySettings()
        {
            Scribe_Collections.Look(ref traderCountRanges, "traderCountRanges", LookMode.Value, LookMode.Value);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                // Scribe_Collections nulls the target when the node is absent
                // (settings files written before the overrides existed).
                traderCountRanges ??= new Dictionary<string, IntRange>();
            }
        }

        private void ResetQuantitySettings()
        {
            traderCountRanges.Clear();
        }

        public IntRange? GetTraderCountRange(string traderDefName)
        {
            return traderDefName != null && traderCountRanges.TryGetValue(traderDefName, out IntRange range)
                ? range
                : (IntRange?)null;
        }

        public void SetTraderCountRange(string traderDefName, IntRange range)
        {
            if (traderDefName != null)
            {
                traderCountRanges[traderDefName] = range;
            }
        }

        public void RemoveTraderCountRange(string traderDefName)
        {
            if (traderDefName != null)
            {
                traderCountRanges.Remove(traderDefName);
            }
        }

        private void DrawQuantitySection(Listing_Standard listing)
        {
            SectionHeader(listing, "XTS_QuantitySection".Translate(), "XTS_QuantitySectionDesc".Translate());

            // One row per trader kind that actually carries the generator, so
            // the Gene Trader row exists exactly while that mod is loaded and
            // a third-party def someone patches the generator onto shows up
            // unasked. Ordered by label rather than def order, which is load
            // order and reshuffles when the mod list moves.
            foreach ((TraderKindDef trader, IntRange xmlDefault) in
                StockGenerator_Xenogerms.PatchedTraders().OrderBy(t => t.trader.LabelCap.ToString()))
            {
                TraderCountRow(listing, trader, xmlDefault);
            }

            listing.Gap(SectionGap);
        }

        // Left inset of the range slider under its full-width trader label, so
        // the slider reads as belonging to the name above it. Only the left
        // edge moves - ColumnWidth shrinks by the same amount so the right
        // edge stays flush with every other row in the window.
        private const float CountSliderIndent = 12f;

        // The row's whole separation, spent below the slider instead of
        // between label and slider: a range that sits tight under its own
        // label and clear of the next trader can't be misread as belonging to
        // the row below.
        private const float CountRowGap = 14f;

        // One labelled range row in the SliderRow style: "Trader: min~max"
        // with the "(default)" suffix while the effective range matches the
        // XML default, description as hover tooltip on the label. The label is
        // the row's only readout - CompactIntRange drops the centred grey one
        // vanilla's slider draws, which only repeated these same numbers.
        private void TraderCountRow(Listing_Standard listing, TraderKindDef trader, IntRange xmlDefault)
        {
            IntRange value = GetTraderCountRange(trader.defName) ?? xmlDefault;
            string label = "XTS_TraderCount".Translate(trader.LabelCap, value.min, value.max);
            if (value == xmlDefault)
            {
                label += "XTS_DefaultSuffix".Translate();
            }

            // Zeroed for the two rows below, which each trail a Gap of it, so
            // the label and the slider close up; the gap the row does want
            // goes on after the spacing is restored.
            float prevSpacing = listing.verticalSpacing;
            listing.verticalSpacing = 0f;

            listing.Label(label, tooltip: "XTS_TraderCountDesc".Translate(xmlDefault.min, xmlDefault.max));

            IntRange edited = value;
            listing.Indent(CountSliderIndent);
            listing.ColumnWidth -= CountSliderIndent;
            CompactIntRange.Draw(listing, ref edited, 0, MaxTraderCount);
            listing.ColumnWidth += CountSliderIndent;
            listing.Outdent(CountSliderIndent);

            listing.verticalSpacing = prevSpacing;
            listing.Gap(CountRowGap);

            if (edited != value)
            {
                if (edited == xmlDefault)
                {
                    RemoveTraderCountRange(trader.defName);
                }
                else
                {
                    SetTraderCountRange(trader.defName, edited);
                }
            }
        }
    }
}
