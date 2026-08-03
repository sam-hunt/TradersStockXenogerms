using Xunit;

namespace TradersStockXenogerms.Tests
{
    // Unit coverage for TradersStockXenogermsSettings: a fresh instance's
    // field-initializer defaults, and ResetToDefaults() restoring those same
    // values after mutation. Both are checked against the Default* constants
    // directly so the test can't drift from the shipped defaults.
    [Collection("XenogermPricing")]
    public class ModSettingsTests
    {
        [Fact]
        public void NewInstance_FieldsMatchDefaultConstants()
        {
            var settings = new TradersStockXenogermsSettings();

            Assert.Equal(TradersStockXenogermsSettings.DefaultIncludeArchiteXenotypes, settings.includeArchiteXenotypes);
            Assert.Equal(TradersStockXenogermsSettings.DefaultIncludeInheritableXenotypes, settings.includeInheritableXenotypes);
            Assert.Equal(TradersStockXenogermsSettings.DefaultIncludePlayerCreatedXenotypes, settings.includePlayerCreatedXenotypes);
            Assert.Equal(TradersStockXenogermsSettings.DefaultBasePresetValue, settings.basePresetValue);
            Assert.Equal(TradersStockXenogermsSettings.DefaultValuePerMetabolism, settings.valuePerMetabolism);
            Assert.Equal(TradersStockXenogermsSettings.DefaultValuePerComplexity, settings.valuePerComplexity);
            Assert.Equal(TradersStockXenogermsSettings.DefaultValuePerArchite, settings.valuePerArchite);
        }

        [Fact]
        public void ResetToDefaults_RestoresAllFieldsAfterMutation()
        {
            var settings = new TradersStockXenogermsSettings
            {
                includeArchiteXenotypes = !TradersStockXenogermsSettings.DefaultIncludeArchiteXenotypes,
                includeInheritableXenotypes = !TradersStockXenogermsSettings.DefaultIncludeInheritableXenotypes,
                includePlayerCreatedXenotypes = !TradersStockXenogermsSettings.DefaultIncludePlayerCreatedXenotypes,
                basePresetValue = TradersStockXenogermsSettings.DefaultBasePresetValue + 500f,
                valuePerMetabolism = TradersStockXenogermsSettings.DefaultValuePerMetabolism + 5f,
                valuePerComplexity = TradersStockXenogermsSettings.DefaultValuePerComplexity + 5f,
                valuePerArchite = TradersStockXenogermsSettings.DefaultValuePerArchite + 50f,
            };

            settings.ResetToDefaults();

            Assert.Equal(TradersStockXenogermsSettings.DefaultIncludeArchiteXenotypes, settings.includeArchiteXenotypes);
            Assert.Equal(TradersStockXenogermsSettings.DefaultIncludeInheritableXenotypes, settings.includeInheritableXenotypes);
            Assert.Equal(TradersStockXenogermsSettings.DefaultIncludePlayerCreatedXenotypes, settings.includePlayerCreatedXenotypes);
            Assert.Equal(TradersStockXenogermsSettings.DefaultBasePresetValue, settings.basePresetValue);
            Assert.Equal(TradersStockXenogermsSettings.DefaultValuePerMetabolism, settings.valuePerMetabolism);
            Assert.Equal(TradersStockXenogermsSettings.DefaultValuePerComplexity, settings.valuePerComplexity);
            Assert.Equal(TradersStockXenogermsSettings.DefaultValuePerArchite, settings.valuePerArchite);
        }

        [Fact]
        public void DefaultValues_FallWithinTheirSliderRanges()
        {
            // Guards against a future default edit landing outside the slider
            // range it's meant to be clamped by.
            Assert.InRange(TradersStockXenogermsSettings.DefaultBasePresetValue,
                TradersStockXenogermsSettings.MinBasePresetValue, TradersStockXenogermsSettings.MaxBasePresetValue);
            Assert.InRange(TradersStockXenogermsSettings.DefaultValuePerMetabolism,
                TradersStockXenogermsSettings.MinValuePerMetabolism, TradersStockXenogermsSettings.MaxValuePerMetabolism);
            Assert.InRange(TradersStockXenogermsSettings.DefaultValuePerComplexity,
                TradersStockXenogermsSettings.MinValuePerComplexity, TradersStockXenogermsSettings.MaxValuePerComplexity);
            Assert.InRange(TradersStockXenogermsSettings.DefaultValuePerArchite,
                TradersStockXenogermsSettings.MinValuePerArchite, TradersStockXenogermsSettings.MaxValuePerArchite);
        }
    }
}
