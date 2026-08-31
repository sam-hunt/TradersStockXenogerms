using System;
using System.Linq;
using Xunit;

namespace XenogermTraderStock.Tests
{
    // XenogermCommonality.Weights is the pure pool-price -> spawn-weight math
    // behind the "Xenotype commonality by price" setting. No settings or game
    // state involved: weights depend only on the price list and the strategy.
    public class XenogermCommonalityTests
    {
        // Odd-length, sorted, distinct: median is Pool[2], extremes at the ends.
        private static readonly float[] Pool = { 100f, 500f, 1000f, 2000f, 4000f };

        [Fact]
        public void InversePrice_WeightsAreReciprocalPrices()
        {
            float[] weights = XenogermCommonality.Weights(Pool, XenogermSelectionStrategy.InversePrice);

            for (int i = 0; i < Pool.Length; i++)
            {
                Assert.Equal(1f / Pool[i], weights[i]);
            }
        }

        [Fact]
        public void Price_WeightsAreThePricesThemselves()
        {
            float[] weights = XenogermCommonality.Weights(Pool, XenogermSelectionStrategy.Price);

            for (int i = 0; i < Pool.Length; i++)
            {
                Assert.Equal(Pool[i], weights[i]);
            }
        }

        [Fact]
        public void SoftInversePrice_FavoursCheap_ButFlatterThanInverse()
        {
            float[] soft = XenogermCommonality.Weights(Pool, XenogermSelectionStrategy.SoftInversePrice);
            float[] inverse = XenogermCommonality.Weights(Pool, XenogermSelectionStrategy.InversePrice);

            Assert.True(soft[0] > soft[Pool.Length - 1], "Cheapest should still outweigh priciest.");

            // The cheap:expensive ratio is the bias; softened must bias less.
            float softRatio = soft[0] / soft[Pool.Length - 1];
            float inverseRatio = inverse[0] / inverse[Pool.Length - 1];
            Assert.True(softRatio < inverseRatio,
                $"Softened ratio ({softRatio}) should be flatter than inverse ({inverseRatio}).");
        }

        [Fact]
        public void SqrtPrice_FavoursExpensive_ButFlatterThanLinear()
        {
            float[] sqrt = XenogermCommonality.Weights(Pool, XenogermSelectionStrategy.SqrtPrice);
            float[] linear = XenogermCommonality.Weights(Pool, XenogermSelectionStrategy.Price);

            Assert.True(sqrt[Pool.Length - 1] > sqrt[0], "Priciest should still outweigh cheapest.");

            // The expensive:cheap ratio is the bias; sqrt must bias less than linear.
            float sqrtRatio = sqrt[Pool.Length - 1] / sqrt[0];
            float linearRatio = linear[Pool.Length - 1] / linear[0];
            Assert.True(sqrtRatio < linearRatio,
                $"Square-root ratio ({sqrtRatio}) should be flatter than linear ({linearRatio}).");
        }

        [Fact]
        public void BellCurve_PeaksAtTheMedian_AndFallsTowardBothExtremes()
        {
            float[] weights = XenogermCommonality.Weights(Pool, XenogermSelectionStrategy.BellCurve);

            const int median = 2;
            for (int i = 0; i < Pool.Length; i++)
            {
                if (i != median)
                {
                    Assert.True(weights[i] < weights[median],
                        $"Weight at index {i} ({weights[i]}) should sit below the median's ({weights[median]}).");
                }
            }
            Assert.Equal(1f, weights[median]);
        }

        [Fact]
        public void BellCurve_DegeneratesToUniform_WhenAllPricesEqual()
        {
            var flat = new[] { 1320f, 1320f, 1320f };

            float[] weights = XenogermCommonality.Weights(flat, XenogermSelectionStrategy.BellCurve);

            Assert.All(weights, w => Assert.Equal(1f, w));
        }

        [Fact]
        public void Uniform_IgnoresPricesEntirely()
        {
            float[] weights = XenogermCommonality.Weights(Pool, XenogermSelectionStrategy.Uniform);

            Assert.All(weights, w => Assert.Equal(1f, w));
        }

        [Fact]
        public void EveryStrategy_YieldsPositiveFiniteWeights_EvenOnDegeneratePrices()
        {
            // A zero price can only come from another mod zeroing market value;
            // the floor must keep every strategy's weights usable regardless.
            var degenerate = new[] { 0f, 20f, 1320f };

            foreach (XenogermSelectionStrategy strategy in
                Enum.GetValues(typeof(XenogermSelectionStrategy)).Cast<XenogermSelectionStrategy>())
            {
                float[] weights = XenogermCommonality.Weights(degenerate, strategy);

                Assert.All(weights, w => Assert.True(w > 0f && !float.IsInfinity(w) && !float.IsNaN(w),
                    $"{strategy} produced unusable weight {w}."));
            }
        }
    }
}
