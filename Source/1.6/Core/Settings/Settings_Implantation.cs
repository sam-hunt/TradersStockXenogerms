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
        public const bool DefaultImplantGermlineAsEndogenes = true;
        public bool implantGermlineAsEndogenes = DefaultImplantGermlineAsEndogenes;

        // Restore a pawn's deathrest capacity (bought with ~1000-silver serums,
        // stored on the Gene_Deathrest instance) after an implant or sanguophage
        // reimplant recreates the gene - vanilla's Gene_Deathrest.PostAdd resets
        // capacity to 1 unconditionally. Applies to EVERY xenogerm implantation,
        // not just trader-sold items: the reset is a vanilla-wide annoyance and
        // gating it on our comp would leave the same pawn inconsistent between
        // a bought and a crafted xenogerm.
        public const bool DefaultPreserveDeathrestCapacity = true;
        public bool preserveDeathrestCapacity = DefaultPreserveDeathrestCapacity;

        private void ExposeImplantationSettings()
        {
            Scribe_Values.Look(ref implantGermlineAsEndogenes, "implantGermlineAsEndogenes", DefaultImplantGermlineAsEndogenes);
            Scribe_Values.Look(ref preserveDeathrestCapacity, "preserveDeathrestCapacity", DefaultPreserveDeathrestCapacity);
        }

        private void ResetImplantationSettings()
        {
            implantGermlineAsEndogenes = DefaultImplantGermlineAsEndogenes;
            preserveDeathrestCapacity = DefaultPreserveDeathrestCapacity;
        }

        private void DrawImplantationSection(Listing_Standard listing)
        {
            SectionHeader(listing, "XTS_ImplantationSection".Translate(), "XTS_ImplantationSectionDesc".Translate());

            listing.CheckboxLabeled(
                "XTS_ImplantGermlineAsEndogenes".Translate(),
                ref implantGermlineAsEndogenes,
                "XTS_ImplantGermlineAsEndogenesDesc".Translate());

            listing.CheckboxLabeled(
                "XTS_PreserveDeathrestCapacity".Translate(),
                ref preserveDeathrestCapacity,
                "XTS_PreserveDeathrestCapacityDesc".Translate());

            listing.Gap(SectionGap);
        }
    }
}
