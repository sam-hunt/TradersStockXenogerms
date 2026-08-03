using System.Collections.Generic;
using Verse;
using Xunit;

namespace TradersStockXenogerms.Tests
{
    // StockGenerator_Xenogerms.GenerateThings weights spawn selection by
    // `1f / XenogermPricing.EstimateMarketValue(genes)` — cheaper xenogerms
    // spawn more often. GenerateThings itself needs a PlanetTile, a Faction,
    // and DefDatabase<XenotypeDef> contents, so it isn't headless-testable;
    // what's pure is the weight formula it feeds RandomElementByWeight. These
    // tests exercise that formula directly against XenogermPricing.
    [Collection("XenogermPricing")]
    public class SpawnWeightTests
    {
        public SpawnWeightTests()
        {
            TestHelpers.InstallDefaultSettings();
        }

        private static float Weight(IEnumerable<GeneDef> genes)
        {
            return 1f / XenogermPricing.EstimateMarketValue(genes);
        }

        [Fact]
        public void Weight_IsInverselyProportionalToMarketValue()
        {
            var cheap = new List<GeneDef> { TestHelpers.MakeGene(metabolism: 1, complexity: 0, archites: 0) };
            var expensive = new List<GeneDef> { TestHelpers.MakeGene(metabolism: 1, complexity: 0, archites: 5) };

            float cheapWeight = Weight(cheap);
            float expensiveWeight = Weight(expensive);

            Assert.True(cheapWeight > expensiveWeight,
                $"Expected cheap ({cheapWeight}) to outweigh expensive ({expensiveWeight}).");
        }

        [Fact]
        public void Weight_DecreasesMonotonicallyAsArchiteCountIncreases()
        {
            float previousWeight = float.PositiveInfinity;

            for (int archites = 0; archites <= 4; archites++)
            {
                var genes = new List<GeneDef> { TestHelpers.MakeGene(archites: archites) };
                float weight = Weight(genes);

                Assert.True(weight < previousWeight,
                    $"Weight at {archites} archites ({weight}) did not decrease from the previous tier ({previousWeight}).");
                previousWeight = weight;
            }
        }

        [Fact]
        public void Weight_IsNeverZeroOrNegative_EvenWithAllPricingKnobsZeroed()
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.basePresetValue = 0f;
            settings.valuePerMetabolism = 0f;
            settings.valuePerComplexity = 0f;
            settings.valuePerArchite = 0f;

            // Market value floors at BaseXenogermValue (20) even with every
            // premium knob zeroed, so the weight stays finite and positive.
            float weight = Weight(null);

            Assert.True(weight > 0f);
            Assert.Equal(1f / XenogermPricing.BaseXenogermValue, weight);
        }

        [Fact]
        public void Weight_EqualGeneSetsProduceEqualWeight()
        {
            var a = new List<GeneDef> { TestHelpers.MakeGene(metabolism: 3, complexity: 2, archites: 1) };
            var b = new List<GeneDef> { TestHelpers.MakeGene(metabolism: 3, complexity: 2, archites: 1) };

            Assert.Equal(Weight(a), Weight(b));
        }
    }
}
