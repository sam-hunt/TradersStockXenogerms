using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace TradersStockXenogerms
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

        private IEnumerable<XenotypeDef> GetValidXenotypes()
        {
            var settings = TradersStockXenogermsMod.Settings;

            foreach (var xenotype in DefDatabase<XenotypeDef>.AllDefsListForReading)
            {
                // Skip Baseliner - no point selling that
                if (xenotype == XenotypeDefOf.Baseliner)
                    continue;

                // Skip xenotypes with no genes
                if (xenotype.genes.NullOrEmpty())
                    continue;

                // Filter archite xenotypes
                if (!settings.includeArchiteXenotypes && xenotype.Archite)
                    continue;

                // Filter inheritable xenotypes
                if (!settings.includeInheritableXenotypes && xenotype.inheritable)
                    continue;

                yield return xenotype;
            }
        }

        private IEnumerable<CustomXenotype> GetValidCustomXenotypes()
        {
            var settings = TradersStockXenogermsMod.Settings;

            // Only include custom xenotypes if the setting is enabled
            if (!settings.includePlayerCreatedXenotypes)
                yield break;

            // Access custom xenotypes from current game
            var customXenotypes = Current.Game?.customXenotypeDatabase?.customXenotypes;
            if (customXenotypes == null)
                yield break;

            foreach (var customXenotype in customXenotypes)
            {
                // Skip xenotypes with no genes
                if (customXenotype.genes.NullOrEmpty())
                    continue;

                // Filter archite xenotypes
                if (!settings.includeArchiteXenotypes && customXenotype.genes.Any(g => g.biostatArc > 0))
                    continue;

                // Filter inheritable xenotypes
                if (!settings.includeInheritableXenotypes && customXenotype.inheritable)
                    continue;

                yield return customXenotype;
            }
        }

        public override bool HandlesThingDef(ThingDef thingDef)
        {
            return thingDef == ThingDefOf.Xenogerm;
        }
    }
}
