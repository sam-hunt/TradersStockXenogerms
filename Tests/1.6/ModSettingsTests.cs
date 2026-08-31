using Verse;
using Xunit;

namespace XenogermTraderStock.Tests
{
    // Unit coverage for XenogermTraderStockSettings: a fresh instance's
    // field-initializer defaults, ResetToDefaults() restoring those same
    // values after mutation, and the per-xenotype sold ledger (Get/Set for
    // both the preset and custom dictionaries). Defaults are checked against
    // the Default* constants directly so the test can't drift from the
    // shipped defaults.
    [Collection("XenogermPricing")]
    public class ModSettingsTests
    {
        [Fact]
        public void NewInstance_FieldsMatchDefaultConstants()
        {
            var settings = new XenogermTraderStockSettings();

            Assert.Equal(XenogermTraderStockSettings.DefaultImplantGermlineAsEndogenes, settings.implantGermlineAsEndogenes);
            Assert.Equal(XenogermTraderStockSettings.DefaultBasePresetValue, settings.basePresetValue);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerMetabolism, settings.valuePerMetabolism);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerComplexity, settings.valuePerComplexity);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerArchite, settings.valuePerArchite);
            Assert.Equal(XenogermTraderStockSettings.DefaultSelectionStrategy, settings.selectionStrategy);
        }

        [Fact]
        public void ResetToDefaults_RestoresAllFieldsAfterMutation()
        {
            var settings = new XenogermTraderStockSettings
            {
                implantGermlineAsEndogenes = !XenogermTraderStockSettings.DefaultImplantGermlineAsEndogenes,
                basePresetValue = XenogermTraderStockSettings.DefaultBasePresetValue + 500f,
                valuePerMetabolism = XenogermTraderStockSettings.DefaultValuePerMetabolism + 5f,
                valuePerComplexity = XenogermTraderStockSettings.DefaultValuePerComplexity + 5f,
                valuePerArchite = XenogermTraderStockSettings.DefaultValuePerArchite + 50f,
                selectionStrategy = XenogermSelectionStrategy.Uniform,
            };

            settings.ResetToDefaults();

            Assert.Equal(XenogermTraderStockSettings.DefaultImplantGermlineAsEndogenes, settings.implantGermlineAsEndogenes);
            Assert.Equal(XenogermTraderStockSettings.DefaultBasePresetValue, settings.basePresetValue);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerMetabolism, settings.valuePerMetabolism);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerComplexity, settings.valuePerComplexity);
            Assert.Equal(XenogermTraderStockSettings.DefaultValuePerArchite, settings.valuePerArchite);
            Assert.Equal(XenogermTraderStockSettings.DefaultSelectionStrategy, settings.selectionStrategy);
        }

        [Fact]
        public void NewInstance_SoldLedgersAreEmpty()
        {
            var settings = new XenogermTraderStockSettings();

            Assert.NotNull(settings.soldXenotypes);
            Assert.NotNull(settings.soldCustomXenotypes);
            Assert.Empty(settings.soldXenotypes);
            Assert.Empty(settings.soldCustomXenotypes);
        }

        [Fact]
        public void GetXenotypeSold_UnseenEntryOrNullKey_IsNull()
        {
            var settings = new XenogermTraderStockSettings();

            Assert.Null(settings.GetXenotypeSold("Foo"));
            Assert.Null(settings.GetXenotypeSold(null));
        }

        [Fact]
        public void SetXenotypeSold_Upserts_AndIgnoresNullKey()
        {
            var settings = new XenogermTraderStockSettings();

            settings.SetXenotypeSold("Foo", true);
            Assert.True(settings.GetXenotypeSold("Foo"));
            Assert.Single(settings.soldXenotypes);

            // Overwrites the same entry rather than adding another.
            settings.SetXenotypeSold("Foo", false);
            Assert.False(settings.GetXenotypeSold("Foo"));
            Assert.Single(settings.soldXenotypes);

            // A null key is ignored: no throw, no entry.
            settings.SetXenotypeSold(null, true);
            Assert.Single(settings.soldXenotypes);
        }

        [Fact]
        public void GetCustomXenotypeSold_UnseenEntryOrNullKey_IsNull()
        {
            var settings = new XenogermTraderStockSettings();

            Assert.Null(settings.GetCustomXenotypeSold("Foo"));
            Assert.Null(settings.GetCustomXenotypeSold(null));
        }

        [Fact]
        public void SetCustomXenotypeSold_Upserts_AndIgnoresNullKey()
        {
            var settings = new XenogermTraderStockSettings();

            settings.SetCustomXenotypeSold("Foo", true);
            Assert.True(settings.GetCustomXenotypeSold("Foo"));
            Assert.Single(settings.soldCustomXenotypes);

            // Overwrites the same entry rather than adding another.
            settings.SetCustomXenotypeSold("Foo", false);
            Assert.False(settings.GetCustomXenotypeSold("Foo"));
            Assert.Single(settings.soldCustomXenotypes);

            // A null key is ignored: no throw, no entry.
            settings.SetCustomXenotypeSold(null, true);
            Assert.Single(settings.soldCustomXenotypes);
        }

        [Fact]
        public void PresetAndCustomSoldLedgers_AreIndependent()
        {
            var settings = new XenogermTraderStockSettings();

            settings.SetXenotypeSold("Foo", true);

            Assert.True(settings.GetXenotypeSold("Foo"));
            Assert.Null(settings.GetCustomXenotypeSold("Foo"));
        }

        [Fact]
        public void ResetToDefaults_ClearsBothSoldLedgers()
        {
            var settings = new XenogermTraderStockSettings();
            settings.SetXenotypeSold("Foo", true);
            settings.SetCustomXenotypeSold("Bar", true);

            settings.ResetToDefaults();

            Assert.Empty(settings.soldXenotypes);
            Assert.Empty(settings.soldCustomXenotypes);
        }

        // The trader count overrides store only deviations from the XML
        // defaults: a missing entry means "use the def's countRange", so a
        // fresh instance holds none and reset just empties the dictionary.
        [Fact]
        public void NewInstance_TraderCountOverridesAreEmpty()
        {
            var settings = new XenogermTraderStockSettings();

            Assert.NotNull(settings.traderCountRanges);
            Assert.Empty(settings.traderCountRanges);
        }

        [Fact]
        public void GetTraderCountRange_UnseenEntryOrNullKey_IsNull()
        {
            var settings = new XenogermTraderStockSettings();

            Assert.Null(settings.GetTraderCountRange("Orbital_Exotic"));
            Assert.Null(settings.GetTraderCountRange(null));
        }

        [Fact]
        public void SetTraderCountRange_Upserts_AndIgnoresNullKey()
        {
            var settings = new XenogermTraderStockSettings();

            settings.SetTraderCountRange("Orbital_Exotic", new IntRange(1, 3));
            Assert.Equal(new IntRange(1, 3), settings.GetTraderCountRange("Orbital_Exotic"));
            Assert.Single(settings.traderCountRanges);

            // Overwrites the same entry rather than adding another.
            settings.SetTraderCountRange("Orbital_Exotic", new IntRange(0, 5));
            Assert.Equal(new IntRange(0, 5), settings.GetTraderCountRange("Orbital_Exotic"));
            Assert.Single(settings.traderCountRanges);

            // A null key is ignored: no throw, no entry.
            settings.SetTraderCountRange(null, new IntRange(2, 4));
            Assert.Single(settings.traderCountRanges);
        }

        [Fact]
        public void RemoveTraderCountRange_DropsTheEntry_IgnoresNullAndAbsentKeys()
        {
            var settings = new XenogermTraderStockSettings();
            settings.SetTraderCountRange("Orbital_Exotic", new IntRange(1, 3));

            settings.RemoveTraderCountRange("Orbital_Exotic");
            Assert.Null(settings.GetTraderCountRange("Orbital_Exotic"));
            Assert.Empty(settings.traderCountRanges);

            // Removing an absent entry or a null key does not throw.
            settings.RemoveTraderCountRange("Orbital_Exotic");
            settings.RemoveTraderCountRange(null);
            Assert.Empty(settings.traderCountRanges);
        }

        [Fact]
        public void ResetToDefaults_ClearsTraderCountOverrides()
        {
            var settings = new XenogermTraderStockSettings();
            settings.SetTraderCountRange("Orbital_Exotic", new IntRange(1, 3));

            settings.ResetToDefaults();

            Assert.Empty(settings.traderCountRanges);
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
