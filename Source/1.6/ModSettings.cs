using Verse;

namespace TradersStockXenogerms
{
    public class TradersStockXenogermsSettings : ModSettings
    {
        public bool includeArchiteXenotypes = DefaultIncludeArchiteXenotypes;
        public bool includeInheritableXenotypes = DefaultIncludeInheritableXenotypes;
        public bool includePlayerCreatedXenotypes = DefaultIncludePlayerCreatedXenotypes;

        // Pricing constants with defaults matching original values
        public float basePresetValue = DefaultBasePresetValue;
        public float valuePerMetabolism = DefaultValuePerMetabolism;
        public float valuePerComplexity = DefaultValuePerComplexity;
        public float valuePerArchite = DefaultValuePerArchite;

        // Default values
        public const bool DefaultIncludeArchiteXenotypes = true;
        public const bool DefaultIncludeInheritableXenotypes = true;
        public const bool DefaultIncludePlayerCreatedXenotypes = false;
        public const float DefaultBasePresetValue = 1300f;
        public const float DefaultValuePerMetabolism = 10f;
        public const float DefaultValuePerComplexity = 15f;
        public const float DefaultValuePerArchite = 100f;

        // Slider ranges
        public const float MinBasePresetValue = 0f;
        public const float MaxBasePresetValue = 3000f;
        public const float MinValuePerMetabolism = 0f;
        public const float MaxValuePerMetabolism = 50f;
        public const float MinValuePerComplexity = 0f;
        public const float MaxValuePerComplexity = 75f;
        public const float MinValuePerArchite = 0f;
        public const float MaxValuePerArchite = 500f;

        public void ResetToDefaults()
        {
            includeArchiteXenotypes = DefaultIncludeArchiteXenotypes;
            includeInheritableXenotypes = DefaultIncludeInheritableXenotypes;
            includePlayerCreatedXenotypes = DefaultIncludePlayerCreatedXenotypes;
            basePresetValue = DefaultBasePresetValue;
            valuePerMetabolism = DefaultValuePerMetabolism;
            valuePerComplexity = DefaultValuePerComplexity;
            valuePerArchite = DefaultValuePerArchite;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref includeArchiteXenotypes, "includeArchiteXenotypes", DefaultIncludeArchiteXenotypes);
            Scribe_Values.Look(ref includeInheritableXenotypes, "includeInheritableXenotypes", DefaultIncludeInheritableXenotypes);
            Scribe_Values.Look(ref includePlayerCreatedXenotypes, "includePlayerCreatedXenotypes", DefaultIncludePlayerCreatedXenotypes);

            Scribe_Values.Look(ref basePresetValue, "basePresetValue", DefaultBasePresetValue);
            Scribe_Values.Look(ref valuePerMetabolism, "valuePerMetabolism", DefaultValuePerMetabolism);
            Scribe_Values.Look(ref valuePerComplexity, "valuePerComplexity", DefaultValuePerComplexity);
            Scribe_Values.Look(ref valuePerArchite, "valuePerArchite", DefaultValuePerArchite);

            base.ExposeData();
        }
    }
}
