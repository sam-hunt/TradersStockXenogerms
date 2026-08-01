using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TradersStockXenogerms
{
    /// <summary>
    /// Centralized pricing calculations for xenogerms.
    /// Used by both StockGenerator (for spawn weighting) and StatPart (for market value display).
    /// </summary>
    public static class XenogermPricing
    {
        /// <summary>
        /// Base xenogerm value from vanilla ThingDef.
        /// </summary>
        public const float BaseXenogermValue = 20f;

        /// <summary>
        /// Breakdown of pricing components for a xenogerm.
        /// </summary>
        public struct PricingBreakdown
        {
            public int AbsoluteMetabolism;
            public int Complexity;
            public int Archites;
            public float Premium;
        }

        private static TradersStockXenogermsSettings Settings => TradersStockXenogermsMod.Settings;

        /// <summary>
        /// Calculates the pricing breakdown for a set of genes.
        /// Returns the raw stats and the calculated premium (excluding base xenogerm value).
        /// </summary>
        public static PricingBreakdown Calculate(IEnumerable<GeneDef> genes)
        {
            var breakdown = new PricingBreakdown();

            if (genes != null)
            {
                foreach (var gene in genes)
                {
                    breakdown.AbsoluteMetabolism += Math.Abs(gene.biostatMet);
                    breakdown.Complexity += gene.biostatCpx;
                    breakdown.Archites += gene.biostatArc;
                }
            }

            breakdown.Premium = Settings.basePresetValue
                + (breakdown.AbsoluteMetabolism * Settings.valuePerMetabolism)
                + (breakdown.Complexity * Settings.valuePerComplexity)
                + (breakdown.Archites * Settings.valuePerArchite);

            return breakdown;
        }

        /// <summary>
        /// Estimates the full market value of a xenogerm (base value + premium).
        /// Used for spawn weighting in stock generation.
        /// </summary>
        public static float EstimateMarketValue(IEnumerable<GeneDef> genes)
        {
            return BaseXenogermValue + Calculate(genes).Premium;
        }
    }
}
