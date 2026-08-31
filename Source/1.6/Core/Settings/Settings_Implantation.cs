using Verse;

namespace XenogermTraderStock
{
    // "Implantation" settings section: how trader-sold xenogerms behave when
    // implanted (the GeneUtility.ImplantXenogermItem postfix reads it).
    public partial class XenogermTraderStockSettings
    {
        // Write a trader-sold germline (inheritable) xenogerm's genes into the
        // pawn's endogenes rather than as xenogenes, making a born member of
        // the xenotype (children inherit, later implants stack instead of
        // replacing). Free-standing on purpose: it acts at implant time on an
        // item the player already owns, so it must not depend on what the shop
        // grid currently has ticked.
        public const bool DefaultImplantGermlineAsEndogenes = false;
        public bool implantGermlineAsEndogenes = DefaultImplantGermlineAsEndogenes;

        private void ExposeImplantationSettings()
        {
            Scribe_Values.Look(ref implantGermlineAsEndogenes, "implantGermlineAsEndogenes", DefaultImplantGermlineAsEndogenes);
        }

        private void ResetImplantationSettings()
        {
            implantGermlineAsEndogenes = DefaultImplantGermlineAsEndogenes;
        }

        private void DrawImplantationSection(Listing_Standard listing)
        {
            SectionHeader(listing, "XTS_ImplantationSection".Translate(), "XTS_ImplantationSectionDesc".Translate());

            listing.CheckboxLabeled(
                "XTS_ImplantGermlineAsEndogenes".Translate(),
                ref implantGermlineAsEndogenes,
                "XTS_ImplantGermlineAsEndogenesDesc".Translate());

            listing.Gap(SectionGap);
        }
    }
}
