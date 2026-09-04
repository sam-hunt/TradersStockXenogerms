using System.Collections.Generic;
using RimWorld;
using Verse;

namespace XenogermTraderStock.Patches
{
    // Pure decision behind Xenogerm_PawnIdeoDisallowsImplanting_Patch: would
    // implanting this xenogerm leave the pawn as one of the ideo's preferred
    // xenotypes? Kept free of game state so the headless suite can cover it.
    //
    // The prediction mirrors what Ideo.IsPreferredXenotype will read AFTER the
    // implant, per implant path:
    //
    //  * Preset (source.Preset - the trader comp, or a comp-less germ whose genes
    //    equal exactly one preset's and no template in the game's custom xenotype
    //    database): the implant patch stamps the def onto the tracker, so the pawn
    //    reads as that preset. Preferred iff the ideo's PreferredXenotypes list
    //    holds the def (Baseliner included - it is a XenotypeDef and can be
    //    preferred like any other). A preferred CUSTOM template sharing a preset's
    //    gene list never reaches this branch: such a template can only come from
    //    the xenotype editor (Precept_Xenotype's picker offers XenotypeDefs plus
    //    CharacterCardUtility's on-disk custom xenotype files - the gene assembler
    //    saves xenogerm TEMPLATES, a different file kind that never reaches an
    //    ideo), and every custom precept's template is seeded into the game's
    //    database at scenario start, where XenogermIdentity lets it claim the germ
    //    first, so the germ falls through to the custom branch below.
    //
    //  * Anything else: the tracker ends at Baseliner and resolves CustomXenotype
    //    by GENE MATCH against the database - the item's xenotypeName is never
    //    consulted - and IsPreferredXenotype checks that resolved template against
    //    the ideo's PreferredCustomXenotypes with the same matcher
    //    (GeneUtility.PawnIsCustomXenotype), which reads ENDOgenes for an
    //    inheritable template and xenogenes for any other. Which layer the germ's
    //    genes land on is the implant patch's germlineRetarget decision
    //    (GeneUtility_ImplantXenogermItem_Patch.WillRetargetGermline: an
    //    inheritable scenario template, the setting on, a rewritable germline).
    //    So: preferred iff a preferred template's passOnDirectly genes equal the
    //    germ's AND its inheritable flag names the layer the genes will land on -
    //    an inheritable template when the germline is being rewritten, a
    //    non-inheritable one when vanilla's xenogene implant stands. (With a
    //    retarget the xenogene layer holds only the pawn's restored pre-implant
    //    xenogenes, never the germ's genes, so a non-inheritable template cannot
    //    match then; without one the germline is untouched, so an inheritable
    //    template cannot.)
    public static class PreferredXenotypeGate
    {
        public static bool ImplantYieldsPreferred(
            XenogermSource source,
            bool germlineRetarget,
            List<GeneDef> germGenes,
            List<XenotypeDef> preferredXenotypes,
            List<CustomXenotype> preferredCustomXenotypes)
        {
            if (source.Preset != null)
            {
                return preferredXenotypes?.Contains(source.Preset) == true;
            }

            if (preferredCustomXenotypes == null || germGenes == null)
            {
                return false;
            }

            foreach (CustomXenotype custom in preferredCustomXenotypes)
            {
                if (custom != null
                    && custom.inheritable == germlineRetarget
                    && GenesMatch(germGenes, custom.genes))
                {
                    return true;
                }
            }

            return false;
        }

        // Vanilla's custom-xenotype matcher over two gene lists; shared with the
        // preset inference, which is the same rule aimed at XenotypeDefs.
        public static bool GenesMatch(List<GeneDef> germGenes, List<GeneDef> templateGenes)
        {
            return XenogermIdentity.GenesMatch(germGenes, templateGenes);
        }
    }
}
