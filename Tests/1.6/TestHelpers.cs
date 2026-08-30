using System.Collections.Generic;
using System.Runtime.Serialization;
using Verse;

namespace XenogermTraderStock.Tests
{
    // Synthetic data builders. XenogermPricing is pure (it only reads
    // GeneDef.biostatMet / biostatCpx / biostatArc plus
    // XenogermTraderStockMod.Settings) and the GeneExtension gate only walks
    // GeneDef.modExtensions, so this only needs to populate those fields.
    internal static class TestHelpers
    {
        // Calling `new GeneDef()` runs Def's instance ctor, which assigns
        // debugRandomId via Verse.Rand.RangeInclusive — safe on its own, but
        // there's no reason to depend on Rand's static state being valid
        // outside a live game. GetUninitializedObject allocates without
        // running any constructor, matching the pattern UWU's TestHelpers
        // uses for ThingDef.
        //
        // exclude: null leaves modExtensions null (the common case for a gene
        // no mod has patched); true/false attaches a GeneExtension with that
        // excludeFromXenogermStock value.
        public static GeneDef MakeGene(int metabolism = 0, int complexity = 0, int archites = 0,
            string defName = null, bool? exclude = null)
        {
            var gene = (GeneDef)FormatterServices.GetUninitializedObject(typeof(GeneDef));
            gene.defName = defName ?? "TestGene";
            gene.biostatMet = metabolism;
            gene.biostatCpx = complexity;
            gene.biostatArc = archites;
            if (exclude.HasValue)
            {
                gene.modExtensions = new List<DefModExtension>
                {
                    new GeneExtension { excludeFromXenogermStock = exclude.Value },
                };
            }
            return gene;
        }

        // Installs a fresh, default-valued settings instance. The setter is
        // internal (XenogermTraderStockMod.Settings), exposed to this test
        // assembly via InternalsVisibleTo on the main project.
        public static XenogermTraderStockSettings InstallDefaultSettings()
        {
            var settings = new XenogermTraderStockSettings();
            XenogermTraderStockMod.Settings = settings;
            return settings;
        }
    }
}
