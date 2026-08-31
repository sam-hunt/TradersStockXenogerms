using Verse;

namespace XenogermTraderStock
{
    // "Xenogerm pricing" settings section: the four terms of the market-value
    // formula XenogermPricing evaluates (see CLAUDE.md, Pricing Formula). Values
    // are silver, read live by the StatParts and the stock generator, so a change
    // shows in the xenotype grid's prices immediately and on newly generated
    // xenogerms thereafter.
    public partial class XenogermTraderStockSettings
    {
        public const float DefaultBasePresetValue = 1300f;
        public float basePresetValue = DefaultBasePresetValue;

        public const float DefaultValuePerMetabolism = 10f;
        public float valuePerMetabolism = DefaultValuePerMetabolism;

        public const float DefaultValuePerComplexity = 15f;
        public float valuePerComplexity = DefaultValuePerComplexity;

        public const float DefaultValuePerArchite = 100f;
        public float valuePerArchite = DefaultValuePerArchite;

        // Slider ranges and snap steps. Steps are sized to the silver a single
        // notch moves on a typical xenogerm (a few dozen silver against a
        // ~1,500 price), not to the raw unit: a 1-silver notch on a 0-3000 range
        // is unlandable by mouse and meaningless in play. Every default must sit
        // on its step grid so the "(default)" suffix is reachable by dragging.
        public const float MinBasePresetValue = 0f;
        public const float MaxBasePresetValue = 5000f;
        public const float StepBasePresetValue = 50f;
        public const float MinValuePerMetabolism = 0f;
        public const float MaxValuePerMetabolism = 100f;
        public const float StepValuePerMetabolism = 5f;
        public const float MinValuePerComplexity = 0f;
        public const float MaxValuePerComplexity = 100f;
        public const float StepValuePerComplexity = 5f;
        public const float MinValuePerArchite = 0f;
        public const float MaxValuePerArchite = 1000f;
        public const float StepValuePerArchite = 25f;

        private void ExposePricingSettings()
        {
            Scribe_Values.Look(ref basePresetValue, "basePresetValue", DefaultBasePresetValue);
            Scribe_Values.Look(ref valuePerMetabolism, "valuePerMetabolism", DefaultValuePerMetabolism);
            Scribe_Values.Look(ref valuePerComplexity, "valuePerComplexity", DefaultValuePerComplexity);
            Scribe_Values.Look(ref valuePerArchite, "valuePerArchite", DefaultValuePerArchite);
        }

        private void ResetPricingSettings()
        {
            basePresetValue = DefaultBasePresetValue;
            valuePerMetabolism = DefaultValuePerMetabolism;
            valuePerComplexity = DefaultValuePerComplexity;
            valuePerArchite = DefaultValuePerArchite;
        }

        private void DrawPricingSection(Listing_Standard listing)
        {
            SectionHeader(listing, "XTS_PricingSection".Translate(), "XTS_PricingSectionDesc".Translate());

            basePresetValue = SliderRow(listing,
                "XTS_BasePresetValue", "XTS_BasePresetValueDesc",
                basePresetValue, DefaultBasePresetValue,
                MinBasePresetValue, MaxBasePresetValue, StepBasePresetValue);

            valuePerMetabolism = SliderRow(listing,
                "XTS_ValuePerMetabolism", "XTS_ValuePerMetabolismDesc",
                valuePerMetabolism, DefaultValuePerMetabolism,
                MinValuePerMetabolism, MaxValuePerMetabolism, StepValuePerMetabolism);

            valuePerComplexity = SliderRow(listing,
                "XTS_ValuePerComplexity", "XTS_ValuePerComplexityDesc",
                valuePerComplexity, DefaultValuePerComplexity,
                MinValuePerComplexity, MaxValuePerComplexity, StepValuePerComplexity);

            valuePerArchite = SliderRow(listing,
                "XTS_ValuePerArchite", "XTS_ValuePerArchiteDesc",
                valuePerArchite, DefaultValuePerArchite,
                MinValuePerArchite, MaxValuePerArchite, StepValuePerArchite);

            listing.Gap(SectionGap);
        }
    }
}
