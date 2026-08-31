using Verse;

namespace XenogermTraderStock
{
    // "Xenotype commonality by price" settings section: which weighting strategy the
    // stock generator uses to pick xenotypes for a trader's xenogerms
    // (XenogermCommonality evaluates it). Read at generation time, so a change
    // shows on newly generated traders only.
    public partial class XenogermTraderStockSettings
    {
        public const XenogermSelectionStrategy DefaultSelectionStrategy = XenogermSelectionStrategy.SoftInversePrice;
        public XenogermSelectionStrategy selectionStrategy = DefaultSelectionStrategy;

        private void ExposeCommonalitySettings()
        {
            Scribe_Values.Look(ref selectionStrategy, "selectionStrategy", DefaultSelectionStrategy);
        }

        private void ResetCommonalitySettings()
        {
            selectionStrategy = DefaultSelectionStrategy;
        }

        private void DrawCommonalitySection(Listing_Standard listing)
        {
            SectionHeader(listing, "XTS_CommonalitySection".Translate(), "XTS_CommonalitySectionDesc".Translate());

            StrategyRadio(listing, XenogermSelectionStrategy.InversePrice,
                "XTS_StrategyInversePrice", "XTS_StrategyInversePriceDesc");
            StrategyRadio(listing, XenogermSelectionStrategy.SoftInversePrice,
                "XTS_StrategySoftInversePrice", "XTS_StrategySoftInversePriceDesc");
            StrategyRadio(listing, XenogermSelectionStrategy.Price,
                "XTS_StrategyPrice", "XTS_StrategyPriceDesc");
            StrategyRadio(listing, XenogermSelectionStrategy.SqrtPrice,
                "XTS_StrategySqrtPrice", "XTS_StrategySqrtPriceDesc");
            StrategyRadio(listing, XenogermSelectionStrategy.BellCurve,
                "XTS_StrategyBellCurve", "XTS_StrategyBellCurveDesc");
            StrategyRadio(listing, XenogermSelectionStrategy.Uniform,
                "XTS_StrategyUniform", "XTS_StrategyUniformDesc");

            listing.Gap(SectionGap);
        }

        // One radio row per strategy, description as hover tooltip. The shipped
        // default's label carries the same "(default)" suffix the sliders use,
        // permanently: on a radio group it marks which OPTION is the default,
        // not whether the current value matches it.
        private void StrategyRadio(Listing_Standard listing, XenogermSelectionStrategy strategy,
            string labelKey, string descKey)
        {
            string label = labelKey.Translate();
            if (strategy == DefaultSelectionStrategy)
            {
                label += "XTS_DefaultSuffix".Translate();
            }
            if (listing.RadioButton(label, selectionStrategy == strategy, 0f, descKey.Translate()))
            {
                selectionStrategy = strategy;
            }
        }
    }
}
