using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TradersStockXenogerms
{
    // Centralized pricing calculations for xenogerms.
    // Used by both StockGenerator (for spawn weighting) and StatPart (for market value display).
    public static class XenogermPricing
    {
        // Base xenogerm value from vanilla ThingDef.
        public const float BaseXenogermValue = 20f;

        // Breakdown of pricing components for a xenogerm.
        public struct PricingBreakdown
        {
            public int AbsoluteMetabolism;
            public int Complexity;
            public int Archites;
            public float Premium;
        }

        private static TradersStockXenogermsSettings Settings => TradersStockXenogermsMod.Settings;

        // Calculates the pricing breakdown for a set of genes.
        // Returns the raw stats and the calculated premium (excluding base xenogerm value).
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

        // Estimates the full market value of a xenogerm (base value + premium).
        // Used for spawn weighting in stock generation.
        public static float EstimateMarketValue(IEnumerable<GeneDef> genes)
        {
            return BaseXenogermValue + Calculate(genes).Premium;
        }
    }
}
