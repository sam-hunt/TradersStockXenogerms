using System.Runtime.Serialization;
using Verse;

namespace TradersStockXenogerms.Tests
{
    // Synthetic data builders for pricing tests. XenogermPricing is pure (it
    // only reads GeneDef.biostatMet / biostatCpx / biostatArc plus
    // TradersStockXenogermsMod.Settings), so this only needs to populate those
    // three fields.
    internal static class TestHelpers
    {
        // Calling `new GeneDef()` runs Def's instance ctor, which assigns
        // debugRandomId via Verse.Rand.RangeInclusive — safe on its own, but
        // there's no reason to depend on Rand's static state being valid
        // outside a live game. GetUninitializedObject allocates without
        // running any constructor, matching the pattern UWU's TestHelpers
        // uses for ThingDef.
        public static GeneDef MakeGene(int metabolism = 0, int complexity = 0, int archites = 0, string defName = null)
        {
            var gene = (GeneDef)FormatterServices.GetUninitializedObject(typeof(GeneDef));
            gene.defName = defName ?? "TestGene";
            gene.biostatMet = metabolism;
            gene.biostatCpx = complexity;
            gene.biostatArc = archites;
            return gene;
        }

        // Installs a fresh, default-valued settings instance. The setter is
        // internal (TradersStockXenogermsMod.Settings), exposed to this test
        // assembly via InternalsVisibleTo on the main project.
        public static TradersStockXenogermsSettings InstallDefaultSettings()
        {
            var settings = new TradersStockXenogermsSettings();
            TradersStockXenogermsMod.Settings = settings;
            return settings;
        }
    }
}
