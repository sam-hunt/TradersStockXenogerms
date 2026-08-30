using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    public static class XenogermFactory
    {
        public static Xenogerm CreateForXenotype(XenotypeDef xenotype)
        {
            var xenogerm = (Xenogerm)ThingMaker.MakeThing(ThingDefOf.Xenogerm);

            // Initialize gene set from xenotype
            if (!xenotype.genes.NullOrEmpty())
            {
                foreach (var gene in xenotype.genes)
                {
                    xenogerm.GeneSet.AddGene(gene);
                }
            }

            // Set display name (shows as "Hussar xenogerm" etc)
            xenogerm.xenotypeName = xenotype.label;

            // Vanilla xenogerm items always carry a non-null iconDef (the gene assembler
            // picks one, and Xenogerm.ExposeData backfills Basic on load). There is no
            // XenotypeIconDef for a preset xenotype - presets draw from XenotypeDef.iconPath -
            // so use Basic here too, keeping the item identical before and after a save/load.
            // GeneUtility_ImplantXenogermItem_Patch clears it on the pawn so the preset's own icon shows.
            xenogerm.iconDef = XenotypeIconDefOf.Basic;

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

            // Initialize gene set from custom xenotype
            if (!customXenotype.genes.NullOrEmpty())
            {
                foreach (var gene in customXenotype.genes)
                {
                    xenogerm.GeneSet.AddGene(gene);
                }
            }

            // Set display name
            xenogerm.xenotypeName = customXenotype.name;

            // Set icon from custom xenotype
            xenogerm.iconDef = customXenotype.iconDef;

            // Note: No CompXenotypeSource set - pawn will get genes but not a preset xenotype reference
            // This matches vanilla behavior for player-crafted xenogerms

            return xenogerm;
        }
    }
}
