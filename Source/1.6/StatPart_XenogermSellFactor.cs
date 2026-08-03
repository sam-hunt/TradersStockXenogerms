using System.Text;
using RimWorld;
using Verse;

namespace TradersStockXenogerms
{
    // Increases the SellPriceFactor for xenogerms based on archite gene count.
    // This makes archite xenogerms more valuable when sold, compensating for their rarity.
    // Base SellPriceFactor (0.05) is set via XML; this adds the archite bonus.
    public class StatPart_XenogermSellFactor : StatPart
    {
        // Bonus to SellPriceFactor per archite gene
        // With 8 archites: 8 × 0.035 = 0.28 bonus, total 0.33 SellPriceFactor
        private const float BonusPerArchite = 0.035f;

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!req.HasThing)
                return;

            if (!(req.Thing is Xenogerm xenogerm))
                return;

            // Only add bonus for preset xenotype xenogerms (from traders)
            var comp = xenogerm.TryGetComp<CompXenotypeSource>();
            if (comp?.sourceXenotype == null)
                return;

            var geneSet = xenogerm.GeneSet;
            if (geneSet == null)
                return;

            int archites = geneSet.ArchitesTotal;
            if (archites <= 0)
                return;

            val += archites * BonusPerArchite;
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

            int archites = geneSet.ArchitesTotal;
            if (archites <= 0)
                return null;

            float bonus = archites * BonusPerArchite;
            return $"Archite genes ({archites}): +{bonus:P0}";
        }
    }
}
