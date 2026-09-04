using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    public class CompProperties_XenotypeSource : CompProperties
    {
        public CompProperties_XenotypeSource()
        {
            compClass = typeof(CompXenotypeSource);
        }
    }

    // Stores a reference to the XenotypeDef that a xenogerm was created from.
    // Added to xenogerms sold by traders so that implantation can assign the
    // preset xenotype to the pawn (via GeneUtility_ImplantXenogermItem_Patch).
    //
    // Player-crafted xenogerms don't have this comp set. They implant as a custom
    // xenotype unless their gene list equals exactly one preset's, in which case
    // XenogermIdentity infers the def - the comp is the fast, authoritative path
    // and the only one pricing consults.
    //
    // Save-safe: Scribe_Defs.Look returns null gracefully if the XenotypeDef is
    // missing (e.g., if a mod providing the xenotype is removed).
    public class CompXenotypeSource : ThingComp
    {
        public XenotypeDef sourceXenotype;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref sourceXenotype, "sourceXenotype");
        }
    }
}
