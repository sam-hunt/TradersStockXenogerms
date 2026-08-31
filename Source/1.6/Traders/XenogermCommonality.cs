using System;
using System.Collections.Generic;

namespace XenogermTraderStock
{
    // How a trader picks which xenotypes its xenogerm stock is for.
    public enum XenogermSelectionStrategy
    {
        InversePrice,     // cheaper = more common
        SoftInversePrice, // 1/sqrt(price): cheap still favoured, gentler slope (shipped default)
        Price,            // pricier = more common
        SqrtPrice,        // sqrt(price): pricey still favoured, gentler slope
        BellCurve,        // gaussian around the pool's median price
        Uniform,          // every sellable option equally likely
    }

    // Pure weight math feeding StockGenerator_Xenogerms' RandomElementByWeight.
    // Headless-testable, so System.Math only, no game state. Every strategy is
    // stateless by design: weights depend only on the pool's prices, never on
    // earlier picks, so repeated draws stay independent (no round-robin or
    // roll-with-removal - a stock roll has no memory to keep).
    public static class XenogermCommonality
    {
        // Weights for the pool, parallel to `prices`. Pool-relative on purpose:
        // the bell curve centres on THIS pool's median, so it adapts to whatever
        // mods and settings put on sale rather than to fixed silver amounts.
        public static float[] Weights(IReadOnlyList<float> prices, XenogermSelectionStrategy strategy)
        {
            var weights = new float[prices.Count];
            switch (strategy)
            {
                case XenogermSelectionStrategy.Uniform:
                    for (int i = 0; i < weights.Length; i++)
                    {
                        weights[i] = 1f;
                    }
                    break;
                case XenogermSelectionStrategy.Price:
                    for (int i = 0; i < weights.Length; i++)
                    {
                        weights[i] = FloorPrice(prices[i]);
                    }
                    break;
                case XenogermSelectionStrategy.SqrtPrice:
                    for (int i = 0; i < weights.Length; i++)
                    {
                        weights[i] = (float)Math.Sqrt(FloorPrice(prices[i]));
                    }
                    break;
                case XenogermSelectionStrategy.SoftInversePrice:
                    for (int i = 0; i < weights.Length; i++)
                    {
                        weights[i] = 1f / (float)Math.Sqrt(FloorPrice(prices[i]));
                    }
                    break;
                case XenogermSelectionStrategy.BellCurve:
                    FillBellCurve(prices, weights);
                    break;
                case XenogermSelectionStrategy.InversePrice:
                default:
                    for (int i = 0; i < weights.Length; i++)
                    {
                        weights[i] = 1f / FloorPrice(prices[i]);
                    }
                    break;
            }
            return weights;
        }

        // Prices are >= the base item value (20) in practice; the floor only
        // guards a modded zero from producing zero or infinite weights.
        private static float FloorPrice(float price)
        {
            return Math.Max(price, 1f);
        }

        // exp(-(price - median)^2 / 2sigma^2), sigma a quarter of the pool's
        // price spread: the pool's extremes land at two sigma (~14% of the
        // peak), a recognisable bell without making the edges unbuyable. exp is
        // never zero at that range, so RandomElementByWeight's total stays
        // positive. A pool with no spread degenerates to uniform.
        private static void FillBellCurve(IReadOnlyList<float> prices, float[] weights)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            for (int i = 0; i < prices.Count; i++)
            {
                min = Math.Min(min, prices[i]);
                max = Math.Max(max, prices[i]);
            }

            float sigma = (max - min) / 4f;
            if (sigma <= 0f)
            {
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = 1f;
                }
                return;
            }

            float median = Median(prices);
            for (int i = 0; i < weights.Length; i++)
            {
                float distance = prices[i] - median;
                weights[i] = (float)Math.Exp(-(distance * distance) / (2f * sigma * sigma));
            }
        }

        private static float Median(IReadOnlyList<float> prices)
        {
            var sorted = new float[prices.Count];
            for (int i = 0; i < prices.Count; i++)
            {
                sorted[i] = prices[i];
            }
            Array.Sort(sorted);

            int mid = sorted.Length / 2;
            return sorted.Length % 2 == 1
                ? sorted[mid]
                : (sorted[mid - 1] + sorted[mid]) / 2f;
        }
    }
}
