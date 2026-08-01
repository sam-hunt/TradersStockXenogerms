using Verse;

namespace TradersStockXenogerms
{
    public class TradersStockXenogermsSettings : ModSettings
    {
        public bool includeArchiteXenotypes = true;
        public bool includeInheritableXenotypes = true;
        public bool includePlayerCreatedXenotypes = false;

        // Pricing constants with defaults matching original values
        public float basePresetValue = DefaultBasePresetValue;
        public float valuePerMetabolism = DefaultValuePerMetabolism;
        public float valuePerComplexity = DefaultValuePerComplexity;
        public float valuePerArchite = DefaultValuePerArchite;

        // Default values
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

        public override void ExposeData()
        {
            Scribe_Values.Look(ref includeArchiteXenotypes, "includeArchiteXenotypes", true);
            Scribe_Values.Look(ref includeInheritableXenotypes, "includeInheritableXenotypes", true);
            Scribe_Values.Look(ref includePlayerCreatedXenotypes, "includePlayerCreatedXenotypes", false);

            Scribe_Values.Look(ref basePresetValue, "basePresetValue", DefaultBasePresetValue);
            Scribe_Values.Look(ref valuePerMetabolism, "valuePerMetabolism", DefaultValuePerMetabolism);
            Scribe_Values.Look(ref valuePerComplexity, "valuePerComplexity", DefaultValuePerComplexity);
            Scribe_Values.Look(ref valuePerArchite, "valuePerArchite", DefaultValuePerArchite);

            base.ExposeData();
        }
    }
}
