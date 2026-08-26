using Xunit;

namespace XenogermTraderStock.Tests
{
    // Unit coverage for XenogermTraderStockSettings: a fresh instance's
    // field-initializer defaults, and ResetToDefaults() restoring those same
    // values after mutation. Both are checked against the Default* constants
    // directly so the test can't drift from the shipped defaults.
    [Collection("XenogermPricing")]
    public class ModSettingsTests
    {
        [Fact]
        public void NewInstance_FieldsMatchDefaultConstants()
        {
            var settings = new XenogermTraderStockSettings();

            Assert.Equal(XenogermTraderStockSettings.DefaultIncludeArchiteXenotypes, settings.includeArchiteXenotypes);
            Assert.Equal(XenogermTraderStockSettings.DefaultIncludeInheritableXenotypes, settings.includeInheritableXenotypes);
            Assert.Equal(XenogermTraderStockSettings.DefaultIncludePlayerCreatedXenotypes, settings.includePlayerCreatedXenotypes);
            Assert.Equal(XenogermTraderStockSettings.DefaultBasePresetValue, settings.basePresetValue);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerMetabolism, settings.valuePerMetabolism);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerComplexity, settings.valuePerComplexity);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerArchite, settings.valuePerArchite);
        }

        [Fact]
        public void ResetToDefaults_RestoresAllFieldsAfterMutation()
        {
            var settings = new XenogermTraderStockSettings
            {
                includeArchiteXenotypes = !XenogermTraderStockSettings.DefaultIncludeArchiteXenotypes,
                includeInheritableXenotypes = !XenogermTraderStockSettings.DefaultIncludeInheritableXenotypes,
                includePlayerCreatedXenotypes = !XenogermTraderStockSettings.DefaultIncludePlayerCreatedXenotypes,
                basePresetValue = XenogermTraderStockSettings.DefaultBasePresetValue + 500f,
                valuePerMetabolism = XenogermTraderStockSettings.DefaultValuePerMetabolism + 5f,
                valuePerComplexity = XenogermTraderStockSettings.DefaultValuePerComplexity + 5f,
                valuePerArchite = XenogermTraderStockSettings.DefaultValuePerArchite + 50f,
            };

            settings.ResetToDefaults();

            Assert.Equal(XenogermTraderStockSettings.DefaultIncludeArchiteXenotypes, settings.includeArchiteXenotypes);
            Assert.Equal(XenogermTraderStockSettings.DefaultIncludeInheritableXenotypes, settings.includeInheritableXenotypes);
            Assert.Equal(XenogermTraderStockSettings.DefaultIncludePlayerCreatedXenotypes, settings.includePlayerCreatedXenotypes);
            Assert.Equal(XenogermTraderStockSettings.DefaultBasePresetValue, settings.basePresetValue);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerMetabolism, settings.valuePerMetabolism);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerComplexity, settings.valuePerComplexity);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerArchite, settings.valuePerArchite);
        }

        [Fact]
        public void DefaultValues_FallWithinTheirSliderRanges()
        {
            // Guards against a future default edit landing outside the slider
            // range it's meant to be clamped by.
            Assert.InRange(XenogermTraderStockSettings.DefaultBasePresetValue,
                XenogermTraderStockSettings.MinBasePresetValue, XenogermTraderStockSettings.MaxBasePresetValue);
            Assert.InRange(XenogermTraderStockSettings.DefaultValuePerMetabolism,
                XenogermTraderStockSettings.MinValuePerMetabolism, XenogermTraderStockSettings.MaxValuePerMetabolism);
            Assert.InRange(XenogermTraderStockSettings.DefaultValuePerComplexity,
                XenogermTraderStockSettings.MinValuePerComplexity, XenogermTraderStockSettings.MaxValuePerComplexity);
            Assert.InRange(XenogermTraderStockSettings.DefaultValuePerArchite,
                XenogermTraderStockSettings.MinValuePerArchite, XenogermTraderStockSettings.MaxValuePerArchite);
        }
    }
}
