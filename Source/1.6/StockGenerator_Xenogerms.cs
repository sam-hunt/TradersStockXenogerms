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
            var validXenotypes = GetValidXenotypes().ToList();
            var validCustomXenotypes = GetValidCustomXenotypes().ToList();

            int totalOptions = validXenotypes.Count + validCustomXenotypes.Count;
            if (totalOptions == 0)
            {
                yield break;
            }

            // Build combined list with weights for weighted selection
            var weightedOptions = new List<(object xenotype, bool isCustom, float weight)>();

            // Inverse weighting - cheaper xenogerms spawn more frequently
            foreach (var xenotype in validXenotypes)
            {
                float weight = 1f / XenogermPricing.EstimateMarketValue(xenotype.genes);
                weightedOptions.Add((xenotype, false, weight));
            }

            foreach (var customXenotype in validCustomXenotypes)
            {
                float weight = 1f / XenogermPricing.EstimateMarketValue(customXenotype.genes);
                weightedOptions.Add((customXenotype, true, weight));
            }

            int count = countRange.RandomInRange;
            for (int i = 0; i < count; i++)
            {
                var selected = weightedOptions.RandomElementByWeight(opt => opt.weight);

                if (selected.isCustom)
                {
                    yield return XenogermFactory.CreateForCustomXenotype((CustomXenotype)selected.xenotype);
                }
                else
                {
                    yield return XenogermFactory.CreateForXenotype((XenotypeDef)selected.xenotype);
                }
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
    }
}
