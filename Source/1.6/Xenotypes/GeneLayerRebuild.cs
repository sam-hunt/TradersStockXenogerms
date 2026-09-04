using System.Collections.Generic;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    // Rebuilds both of a pawn's gene layers from scratch, the way PawnGenerator
    // lays them down for a freshly generated pawn: the given endogenes as the
    // germline, the pawn's own skin and hair colour kept only when that germline
    // supplies none (PawnGenerator's backfill, with the pawn's genes instead of
    // random ones), then the given xenogenes on top. Shared by the implant patch's
    // germline retarget and the dev-mode "reset genes to scenario xenotype" tool.
    //
    // Order matters twice. Xenogenes are cleared BEFORE the colour genes are read,
    // because GetMelaninGene / GetHairColorGene scan all genes and a coloured
    // xenogene (a Hussar's red skin) is not the pawn's own colouring. And the new
    // xenogenes are added LAST, after the colour backfill, because a coloured
    // xenogene present during the backfill would suppress it and a born member's
    // germline always carries its colouring.
    //
    // Endogenes are replaced wholesale, not merged: a merge would also let old
    // genes win conflicts, because two endogenes resolve by display order
    // (GeneUtility.Overrides -> GenesInOrder), not arrival order. Duplicates across
    // the two layers are vanilla-legal; the xenogene copy always overrides.
    //
    // Gene INSTANCES are recreated, so per-gene state (hemogen level, cooldowns)
    // resets - exactly what a vanilla implant does to every xenogene it re-adds.
    // Deathrest capacity, the one value worth more than that rule, is the
    // caller's business (DeathrestCapacityCarryover).
    //
    // Clears hybrid: the germline is now uniformly one xenotype (or empty), so
    // the pawn is no longer a germline hybrid. The flag is read for gene
    // inheritance (PregnancyUtility treats a hybrid germline as inheritable) and
    // by the CustomXenotype matcher, so leaving it stale has real effects.
    public static class GeneLayerRebuild
    {
        public static void Apply(Pawn_GeneTracker genes, List<GeneDef> endogenes, List<GeneDef> xenogenes)
        {
            genes.ClearXenogenes();

            GeneDef melanin = genes.GetMelaninGene();
            GeneDef hairColor = genes.GetHairColorGene();

            List<Gene> oldEndogenes = genes.Endogenes;
            for (int i = oldEndogenes.Count - 1; i >= 0; i--)
            {
                genes.RemoveGene(oldEndogenes[i]);
            }

            if (endogenes != null)
            {
                foreach (GeneDef gene in endogenes)
                {
                    genes.AddGene(gene, xenogene: false);
                }
            }

            if (genes.GetMelaninGene() == null && melanin != null)
            {
                genes.AddGene(melanin, xenogene: false);
            }

            if (genes.GetHairColorGene() == null && hairColor != null)
            {
                genes.AddGene(hairColor, xenogene: false);
            }

            if (xenogenes != null)
            {
                foreach (GeneDef gene in xenogenes)
                {
                    genes.AddGene(gene, xenogene: true);
                }
            }

            genes.hybrid = false;
        }
    }
}
