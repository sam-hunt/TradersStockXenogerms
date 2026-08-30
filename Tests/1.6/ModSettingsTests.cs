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
            Assert.Equal(XenogermTraderStockSettings.DefaultIncludePlayerScenarioXenotypes, settings.includePlayerScenarioXenotypes);
            Assert.Equal(XenogermTraderStockSettings.DefaultImplantGermlineAsEndogenes, settings.implantGermlineAsEndogenes);
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
                includePlayerScenarioXenotypes = !XenogermTraderStockSettings.DefaultIncludePlayerScenarioXenotypes,
                implantGermlineAsEndogenes = !XenogermTraderStockSettings.DefaultImplantGermlineAsEndogenes,
                basePresetValue = XenogermTraderStockSettings.DefaultBasePresetValue + 500f,
                valuePerMetabolism = XenogermTraderStockSettings.DefaultValuePerMetabolism + 5f,
                valuePerComplexity = XenogermTraderStockSettings.DefaultValuePerComplexity + 5f,
                valuePerArchite = XenogermTraderStockSettings.DefaultValuePerArchite + 50f,
            };

            settings.ResetToDefaults();

            Assert.Equal(XenogermTraderStockSettings.DefaultIncludeArchiteXenotypes, settings.includeArchiteXenotypes);
            Assert.Equal(XenogermTraderStockSettings.DefaultIncludeInheritableXenotypes, settings.includeInheritableXenotypes);
            Assert.Equal(XenogermTraderStockSettings.DefaultIncludePlayerScenarioXenotypes, settings.includePlayerScenarioXenotypes);
            Assert.Equal(XenogermTraderStockSettings.DefaultImplantGermlineAsEndogenes, settings.implantGermlineAsEndogenes);
            Assert.Equal(XenogermTraderStockSettings.DefaultBasePresetValue, settings.basePresetValue);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerMetabolism, settings.valuePerMetabolism);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerComplexity, settings.valuePerComplexity);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerArchite, settings.valuePerArchite);
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, true)]
        public void ImplantsGermlineAsEndogenes_RequiresBothInheritableAndTheToggle(
            bool includeInheritable, bool implantAsEndogenes, bool expected)
        {
            // The window greys the toggle out (shown unchecked) while inheritable
            // xenotypes are off; the implant patch must agree with what is shown.
            var settings = new XenogermTraderStockSettings
            {
                includeInheritableXenotypes = includeInheritable,
                implantGermlineAsEndogenes = implantAsEndogenes,
            };

            Assert.Equal(expected, settings.ImplantsGermlineAsEndogenes);
        }

        [Fact]
        public void NewInstance_ExclusionSetsAreEmpty()
        {
            var settings = new XenogermTraderStockSettings();

            Assert.NotNull(settings.excludedXenotypes);
            Assert.NotNull(settings.excludedCustomXenotypes);
            Assert.Empty(settings.excludedXenotypes);
            Assert.Empty(settings.excludedCustomXenotypes);
        }

        [Fact]
        public void SetXenotypeExcluded_AddsAndRemoves_IdempotentlyAndIgnoresNull()
        {
            var settings = new XenogermTraderStockSettings();

            settings.SetXenotypeExcluded("Foo", true);
            Assert.True(settings.IsXenotypeExcluded("Foo"));
            Assert.Single(settings.excludedXenotypes);

            // Adding again is idempotent.
            settings.SetXenotypeExcluded("Foo", true);
            Assert.Single(settings.excludedXenotypes);

            settings.SetXenotypeExcluded("Foo", false);
            Assert.False(settings.IsXenotypeExcluded("Foo"));
            Assert.Empty(settings.excludedXenotypes);

            // Removing an absent entry does not throw.
            settings.SetXenotypeExcluded("Foo", false);
            Assert.Empty(settings.excludedXenotypes);

            // A null key is ignored: no throw, no entry.
            settings.SetXenotypeExcluded(null, true);
            Assert.Empty(settings.excludedXenotypes);
        }

        [Fact]
        public void SetCustomXenotypeExcluded_AddsAndRemoves_IdempotentlyAndIgnoresNull()
        {
            var settings = new XenogermTraderStockSettings();

            settings.SetCustomXenotypeExcluded("Foo", true);
            Assert.True(settings.IsCustomXenotypeExcluded("Foo"));
            Assert.Single(settings.excludedCustomXenotypes);

            // Adding again is idempotent.
            settings.SetCustomXenotypeExcluded("Foo", true);
            Assert.Single(settings.excludedCustomXenotypes);

            settings.SetCustomXenotypeExcluded("Foo", false);
            Assert.False(settings.IsCustomXenotypeExcluded("Foo"));
            Assert.Empty(settings.excludedCustomXenotypes);

            // Removing an absent entry does not throw.
            settings.SetCustomXenotypeExcluded("Foo", false);
            Assert.Empty(settings.excludedCustomXenotypes);

            // A null key is ignored: no throw, no entry.
            settings.SetCustomXenotypeExcluded(null, true);
            Assert.Empty(settings.excludedCustomXenotypes);
        }

        [Fact]
        public void PresetAndCustomExclusionSets_AreIndependent()
        {
            var settings = new XenogermTraderStockSettings();

            settings.SetXenotypeExcluded("Foo", true);

            Assert.True(settings.IsXenotypeExcluded("Foo"));
            Assert.False(settings.IsCustomXenotypeExcluded("Foo"));
        }

        [Fact]
        public void ResetToDefaults_ClearsBothExclusionSets()
        {
            var settings = new XenogermTraderStockSettings();
            settings.SetXenotypeExcluded("Foo", true);
            settings.SetCustomXenotypeExcluded("Bar", true);

            settings.ResetToDefaults();

            Assert.Empty(settings.excludedXenotypes);
            Assert.Empty(settings.excludedCustomXenotypes);
        }

        [Theory]
        [InlineData(XenogermTraderStockSettings.DefaultBasePresetValue, XenogermTraderStockSettings.MinBasePresetValue, XenogermTraderStockSettings.StepBasePresetValue)]
        [InlineData(XenogermTraderStockSettings.DefaultValuePerMetabolism, XenogermTraderStockSettings.MinValuePerMetabolism, XenogermTraderStockSettings.StepValuePerMetabolism)]
        [InlineData(XenogermTraderStockSettings.DefaultValuePerComplexity, XenogermTraderStockSettings.MinValuePerComplexity, XenogermTraderStockSettings.StepValuePerComplexity)]
        [InlineData(XenogermTraderStockSettings.DefaultValuePerArchite, XenogermTraderStockSettings.MinValuePerArchite, XenogermTraderStockSettings.StepValuePerArchite)]
        public void DefaultValues_SitOnTheirSliderStepGrid(float defaultValue, float min, float step)
        {
            // The slider snaps to `step` measured from `min`, so a default off the
            // grid could never be re-landed by dragging and its "(default)" suffix
            // would only ever show on a fresh install.
            Assert.True(step > 0f);
            Assert.Equal(0f, (defaultValue - min) % step);
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
