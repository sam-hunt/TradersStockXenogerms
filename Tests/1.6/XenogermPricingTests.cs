using System.Collections.Generic;
using Verse;
using Xunit;

namespace XenogermTraderStock.Tests
{
    // All pricing test classes share this collection so xunit serializes them:
    // XenogermTraderStockMod.Settings is a process-global static that
    // XenogermPricing reads through, and tests install/mutate their own
    // instance of it.
    [CollectionDefinition("XenogermPricing")]
    public class XenogermPricingCollection
    {
    }

    // Unit coverage for XenogermPricing.Calculate/EstimateMarketValue: the
    // metabolism/complexity/archite counting and the premium formula those
    // feed into. Expected numbers are derived in comments from the settings
    // values each test installs.
    [Collection("XenogermPricing")]
    public class XenogermPricingTests
    {
        public XenogermPricingTests()
        {
            TestHelpers.InstallDefaultSettings();
        }

        // ---- Calculate: gene counting -----------------------------------------

        [Fact]
        public void Calculate_NullGenes_YieldsZeroCountsAndBasePresetPremium()
        {
            var breakdown = XenogermPricing.Calculate(null);

            Assert.Equal(0, breakdown.AbsoluteMetabolism);
            Assert.Equal(0, breakdown.Complexity);
            Assert.Equal(0, breakdown.Archites);
            Assert.Equal(XenogermTraderStockSettings.DefaultBasePresetValue, breakdown.Premium);
        }

        [Fact]
        public void Calculate_EmptyGenes_MatchesNullGenes()
        {
            var breakdown = XenogermPricing.Calculate(new List<GeneDef>());

            Assert.Equal(0, breakdown.AbsoluteMetabolism);
            Assert.Equal(0, breakdown.Complexity);
            Assert.Equal(0, breakdown.Archites);
            Assert.Equal(XenogermTraderStockSettings.DefaultBasePresetValue, breakdown.Premium);
        }

        [Fact]
        public void Calculate_SumsMetabolismComplexityArchitesAcrossGenes()
        {
            var genes = new List<GeneDef>
            {
                TestHelpers.MakeGene(metabolism: 2, complexity: 1, archites: 0),
                TestHelpers.MakeGene(metabolism: -3, complexity: 2, archites: 1),
                TestHelpers.MakeGene(metabolism: 1, complexity: 0, archites: 2),
            };

            var breakdown = XenogermPricing.Calculate(genes);

            // Metabolism is summed by absolute value: |2| + |-3| + |1| = 6.
            Assert.Equal(6, breakdown.AbsoluteMetabolism);
            Assert.Equal(3, breakdown.Complexity);
            Assert.Equal(3, breakdown.Archites);
        }

        [Fact]
        public void Calculate_MetabolismUsesAbsoluteValue_NegativeAndPositiveContributeEqually()
        {
            var negative = XenogermPricing.Calculate(new List<GeneDef> { TestHelpers.MakeGene(metabolism: -5) });
            var positive = XenogermPricing.Calculate(new List<GeneDef> { TestHelpers.MakeGene(metabolism: 5) });

            Assert.Equal(5, negative.AbsoluteMetabolism);
            Assert.Equal(5, positive.AbsoluteMetabolism);
            Assert.Equal(negative.Premium, positive.Premium);
        }

        // ---- Calculate: premium formula ----------------------------------------

        [Fact]
        public void Calculate_PremiumFormulaMatchesSettingsValues()
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.basePresetValue = 1000f;
            settings.valuePerMetabolism = 10f;
            settings.valuePerComplexity = 15f;
            settings.valuePerArchite = 100f;

            var genes = new List<GeneDef>
            {
                TestHelpers.MakeGene(metabolism: 4, complexity: 3, archites: 2),
            };

            var breakdown = XenogermPricing.Calculate(genes);

            // 1000 + (4 x 10) + (3 x 15) + (2 x 100) = 1000 + 40 + 45 + 200 = 1285.
            Assert.Equal(1285f, breakdown.Premium);
        }

        [Fact]
        public void Calculate_ZeroedSettings_PremiumIsZero()
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.basePresetValue = 0f;
            settings.valuePerMetabolism = 0f;
            settings.valuePerComplexity = 0f;
            settings.valuePerArchite = 0f;

            var genes = new List<GeneDef>
            {
                TestHelpers.MakeGene(metabolism: 10, complexity: 10, archites: 10),
            };

            var breakdown = XenogermPricing.Calculate(genes);

            Assert.Equal(0f, breakdown.Premium);
        }

        // ---- EstimateMarketValue: base + premium -------------------------------

        [Fact]
        public void EstimateMarketValue_EqualsBaseXenogermValuePlusPremium()
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.basePresetValue = 500f;
            settings.valuePerMetabolism = 0f;
            settings.valuePerComplexity = 0f;
            settings.valuePerArchite = 0f;

            float value = XenogermPricing.EstimateMarketValue(null);

            // Base xenogerm value (20) + premium (500, no gene contributions).
            Assert.Equal(20f + 500f, value);
        }

        [Fact]
        public void EstimateMarketValue_IncludesGeneContributions()
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.basePresetValue = 1300f;
            settings.valuePerMetabolism = 10f;
            settings.valuePerComplexity = 15f;
            settings.valuePerArchite = 100f;

            var genes = new List<GeneDef>
            {
                TestHelpers.MakeGene(metabolism: 5, complexity: 4, archites: 1),
            };

            float value = XenogermPricing.EstimateMarketValue(genes);

            // 20 (base) + 1300 + (5x10) + (4x15) + (1x100) = 20 + 1300 + 50 + 60 + 100 = 1530.
            Assert.Equal(1530f, value);
        }
    }
}
