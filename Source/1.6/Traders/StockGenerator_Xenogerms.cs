using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace XenogermTraderStock
{
    public class StockGenerator_Xenogerms : StockGenerator
    {
        public override IEnumerable<Thing> GenerateThings(PlanetTile forTile, Faction faction = null)
        {
            // The custom pool is per-game, so the startup seeding pass cannot
            // have covered it: give every unseen xenotype its ledger entry
            // before eligibility is read.
            XenotypeEligibility.SeedUnseen();

            // Combined preset + custom pool with a parallel price list: the
            // commonality strategy is pool-relative (the bell curve centres on
            // the pool's median price), so weights are computed in one pass over
            // everything sellable rather than per option.
            var options = new List<(object xenotype, bool isCustom)>();
            var prices = new List<float>();

            foreach (var xenotype in GetValidXenotypes())
            {
                options.Add((xenotype, false));
                prices.Add(XenogermPricing.EstimateMarketValue(xenotype.genes));
            }

            foreach (var customXenotype in GetValidCustomXenotypes())
            {
                options.Add((customXenotype, true));
                prices.Add(XenogermPricing.EstimateMarketValue(customXenotype.genes));
            }

            if (options.Count == 0)
            {
                yield break;
            }

            float[] weights = XenogermCommonality.Weights(prices,
                XenogermTraderStockMod.Settings.selectionStrategy);

            // The settings override is read live, keyed by the owning trader
            // kind (StockGenerator.trader, set by ResolveReferences), with the
            // XML countRange as the shipped default. Nothing is ever written
            // back onto the def, so a settings change needs no restart.
            IntRange range = XenogermTraderStockMod.Settings.GetTraderCountRange(trader?.defName) ?? countRange;
            int count = range.RandomInRange;
            for (int i = 0; i < count; i++)
            {
                int index = Enumerable.Range(0, options.Count).RandomElementByWeight(j => weights[j]);
                var selected = options[index];

                yield return selected.isCustom
                    ? XenogermFactory.CreateForCustomXenotype((CustomXenotype)selected.xenotype)
                    : XenogermFactory.CreateForXenotype((XenotypeDef)selected.xenotype);
            }
        }

        private static IEnumerable<XenotypeDef> GetValidXenotypes()
        {
            return DefDatabase<XenotypeDef>.AllDefsListForReading.Where(XenotypeEligibility.IsSellable);
        }

        private static IEnumerable<CustomXenotype> GetValidCustomXenotypes()
        {
            var customXenotypes = Current.Game?.customXenotypeDatabase?.customXenotypes;
            return customXenotypes?.Where(XenotypeEligibility.IsSellable) ?? Enumerable.Empty<CustomXenotype>();
        }

        public override bool HandlesThingDef(ThingDef thingDef)
        {
            return thingDef == ThingDefOf.Xenogerm;
        }

        // Every trader kind carrying this generator, with the generator's XML
        // countRange as that trader's shipped default. Feeds the "Stock
        // quantity" settings rows, so a trader patched by another mod (or the
        // Gene Trader def, present only while that mod is loaded) gets its row
        // without this mod knowing its name.
        public static IEnumerable<(TraderKindDef trader, IntRange xmlCountRange)> PatchedTraders()
        {
            foreach (TraderKindDef trader in DefDatabase<TraderKindDef>.AllDefsListForReading)
            {
                foreach (StockGenerator generator in trader.stockGenerators)
                {
                    if (generator is StockGenerator_Xenogerms xenogerms)
                    {
                        yield return (trader, xenogerms.countRange);
                    }
                }
            }
        }
    }
}
