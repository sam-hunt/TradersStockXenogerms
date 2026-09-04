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

    // Records which xenotype a trader-sold xenogerm was created from: a preset
    // XenotypeDef, or the NAME of a player-scenario CustomXenotype. Stamped by
    // XenogermFactory; read raw by the pricing StatParts (the "this is trader
    // stock" marker) and, for the def, by XenogermIdentity as the authoritative
    // identity. Exactly one of the two fields is set on a trader-sold germ; both
    // are null on anything else (assembler output, ReSplice duplicates, germs
    // from saves that predate the comp).
    //
    // The custom source is a name, not a reference or a copy, because a
    // CustomXenotype is neither a Def (Scribe_Defs cannot point at it) nor
    // ILoadReferenceable (Scribe_References has no load ID to write). A
    // Scribe_Deep copy would only snapshot fields the live template already has,
    // and the live template in Current.Game.customXenotypeDatabase is what the
    // pawn is matched against afterwards (GeneUtility.PawnIsCustomXenotype), so
    // it is the only thing worth resolving to. Which template a germ yields is
    // decided by GENE SET, vanilla's own rule for custom identity; the stored
    // name is a tie-breaker between templates sharing one gene list and the
    // pricing marker, nothing more - see XenogermIdentity.
    //
    // Save-safe: Scribe_Defs.Look returns null gracefully if the XenotypeDef is
    // missing (e.g., if a mod providing the xenotype is removed); the name is a
    // plain string that resolves to nothing when no such template is loaded.
    public class CompXenotypeSource : ThingComp
    {
        public XenotypeDef sourceXenotype;
        public string sourceCustomName;

        // Whether this germ was created by the trader stock generator (or the
        // debug spawner) rather than assembled or copied. Only trader stock earns
        // the premium price: a hand-assembled or duplicated gene-for-gene match
        // implants as the same xenotype but must stay base-priced, or
        // buy-one-copy-many becomes a silver printer.
        public bool IsTraderSold => sourceXenotype != null || !sourceCustomName.NullOrEmpty();

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref sourceXenotype, "sourceXenotype");
            Scribe_Values.Look(ref sourceCustomName, "sourceCustomName");
        }
    }
}
