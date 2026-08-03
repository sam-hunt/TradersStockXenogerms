using System.Text;
using RimWorld;
using Verse;

namespace TradersStockXenogerms
{
    // Adds market value to xenogerms that have a preset xenotype source (trader-sold xenogerms).
    // Player-crafted xenogerms (no CompXenotypeSource) retain their base 20 silver value.
    // Pricing formula is defined in XenogermPricing and configurable via mod settings.
    public class StatPart_XenogermValue : StatPart
    {
        private static TradersStockXenogermsSettings Settings => TradersStockXenogermsMod.Settings;

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!req.HasThing)
                return;

            if (!(req.Thing is Xenogerm xenogerm))
                return;

            // Only add value for preset xenotype xenogerms (from traders)
            var comp = xenogerm.TryGetComp<CompXenotypeSource>();
            if (comp?.sourceXenotype == null)
                return;

            var geneSet = xenogerm.GeneSet;
            if (geneSet == null)
                return;

            val += XenogermPricing.Calculate(geneSet.GenesListForReading).Premium;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!req.HasThing)
                return null;

            if (!(req.Thing is Xenogerm xenogerm))
                return null;

            var comp = xenogerm.TryGetComp<CompXenotypeSource>();
            if (comp?.sourceXenotype == null)
                return null;

            var geneSet = xenogerm.GeneSet;
            if (geneSet == null)
                return null;

            var breakdown = XenogermPricing.Calculate(geneSet.GenesListForReading);
            var sb = new StringBuilder();

            // Show xenotype name
            sb.AppendLine($"Preset xenotype ({comp.sourceXenotype.label}): +{Settings.basePresetValue}");

            // Show metabolism contribution
            if (breakdown.AbsoluteMetabolism > 0)
            {
                float metabolismValue = breakdown.AbsoluteMetabolism * Settings.valuePerMetabolism;
                sb.AppendLine($"Genetic metabolism ({breakdown.AbsoluteMetabolism}): +{metabolismValue}");
            }

            // Show complexity contribution
            if (breakdown.Complexity > 0)
            {
                float complexityValue = breakdown.Complexity * Settings.valuePerComplexity;
                sb.AppendLine($"Genetic complexity ({breakdown.Complexity}): +{complexityValue}");
            }

            // Show archite contribution
            if (breakdown.Archites > 0)
            {
                float architeValue = breakdown.Archites * Settings.valuePerArchite;
                sb.AppendLine($"Archite genes ({breakdown.Archites}): +{architeValue}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
