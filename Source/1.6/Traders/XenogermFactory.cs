using System.Collections.Generic;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    public static class XenogermFactory
    {
        public static Xenogerm CreateForXenotype(XenotypeDef xenotype)
        {
            var xenogerm = (Xenogerm)ThingMaker.MakeThing(ThingDefOf.Xenogerm);

            // MakeThing leaves GeneSetHolderBase.geneSet null (vanilla only creates it
            // in Xenogerm.Initialize and Notify_DebugSpawned), so AddGene on a fresh
            // item NREs. Initialize with no genepacks builds the empty GeneSet and sets
            // the display name ("Hussar xenogerm" etc) and icon in one vanilla call.
            //
            // Vanilla xenogerm items always carry a non-null iconDef (the gene assembler
            // picks one, and Xenogerm.ExposeData backfills Basic on load). There is no
            // XenotypeIconDef for a preset xenotype - presets draw from XenotypeDef.iconPath -
            // so use Basic here too, keeping the item identical before and after a save/load.
            // GeneUtility_ImplantXenogermItem_Patch clears it on the pawn so the preset's own icon shows.
            xenogerm.Initialize(new List<Genepack>(), xenotype.label, XenotypeIconDefOf.Basic);

            if (!xenotype.genes.NullOrEmpty())
            {
                foreach (var gene in xenotype.genes)
                {
                    xenogerm.GeneSet.AddGene(gene);
                }
            }

            // Store xenotype reference for implantation
            var comp = xenogerm.TryGetComp<CompXenotypeSource>();
            if (comp != null)
            {
                comp.sourceXenotype = xenotype;
            }

            return xenogerm;
        }

        public static Xenogerm CreateForCustomXenotype(CustomXenotype customXenotype)
        {
            var xenogerm = (Xenogerm)ThingMaker.MakeThing(ThingDefOf.Xenogerm);

            // Same geneSet bootstrap as CreateForXenotype; a custom xenotype carries
            // its own XenotypeIconDef, so that icon ships on the item directly.
            xenogerm.Initialize(new List<Genepack>(), customXenotype.name, customXenotype.iconDef);

            if (!customXenotype.genes.NullOrEmpty())
            {
                foreach (var gene in customXenotype.genes)
                {
                    xenogerm.GeneSet.AddGene(gene);
                }
            }

            // Note: No CompXenotypeSource set - pawn will get genes but not a preset xenotype reference
            // This matches vanilla behavior for player-crafted xenogerms

            return xenogerm;
        }
    }
}
