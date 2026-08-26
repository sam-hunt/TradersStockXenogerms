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

            // Note: iconDef is for custom xenotypes (XenotypeIconDef).
            // Preset xenotypes use XenotypeDef.iconPath directly.
            // Leaving iconDef null - display uses xenotypeName.

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
