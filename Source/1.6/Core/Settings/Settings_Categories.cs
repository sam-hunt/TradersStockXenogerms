using Verse;

namespace XenogermTraderStock
{
    // "Xenotype categories" settings section: the whole-group switches that feed
    // XenotypeEligibility.GetCategoryBlock (archite / inheritable / player-
    // scenario), plus the germline implant behaviour that only means something
    // while inheritable xenogerms can be sold at all.
    public partial class XenogermTraderStockSettings
    {
        public const bool DefaultIncludeArchiteXenotypes = true;
        public bool includeArchiteXenotypes = DefaultIncludeArchiteXenotypes;

        public const bool DefaultIncludeInheritableXenotypes = false;
        public bool includeInheritableXenotypes = DefaultIncludeInheritableXenotypes;

        public const bool DefaultIncludePlayerScenarioXenotypes = true;
        public bool includePlayerScenarioXenotypes = DefaultIncludePlayerScenarioXenotypes;

        // Write a trader-sold germline (inheritable) xenogerm's genes into the
        // pawn's endogenes rather than as xenogenes, making a born member of the
        // xenotype (children inherit, later implants stack instead of replacing).
        // Read it through ImplantsGermlineAsEndogenes, never directly.
        public const bool DefaultImplantGermlineAsEndogenes = false;
        public bool implantGermlineAsEndogenes = DefaultImplantGermlineAsEndogenes;

        // Derived state: the toggle is gated on inheritable xenotypes being sold
        // (the window greys it out and shows it unchecked otherwise), so the
        // implant patch must see the same effective value the player does. The
        // stored flag survives underneath for when the gate reopens.
        public bool ImplantsGermlineAsEndogenes => includeInheritableXenotypes && implantGermlineAsEndogenes;

        private void ExposeCategorySettings()
        {
            Scribe_Values.Look(ref includeArchiteXenotypes, "includeArchiteXenotypes", DefaultIncludeArchiteXenotypes);
            Scribe_Values.Look(ref includeInheritableXenotypes, "includeInheritableXenotypes", DefaultIncludeInheritableXenotypes);
            Scribe_Values.Look(ref includePlayerScenarioXenotypes, "includePlayerScenarioXenotypes", DefaultIncludePlayerScenarioXenotypes);
            Scribe_Values.Look(ref implantGermlineAsEndogenes, "implantGermlineAsEndogenes", DefaultImplantGermlineAsEndogenes);
        }

        private void ResetCategorySettings()
        {
            includeArchiteXenotypes = DefaultIncludeArchiteXenotypes;
            includeInheritableXenotypes = DefaultIncludeInheritableXenotypes;
            includePlayerScenarioXenotypes = DefaultIncludePlayerScenarioXenotypes;
            implantGermlineAsEndogenes = DefaultImplantGermlineAsEndogenes;
        }

        private void DrawCategoriesSection(Listing_Standard listing)
        {
            SectionHeader(listing, "XTS_CategoriesSection".Translate(), "XTS_CategoriesSectionDesc".Translate());

            listing.CheckboxLabeled(
                "XTS_IncludeArchite".Translate(),
                ref includeArchiteXenotypes,
                "XTS_IncludeArchiteDesc".Translate());

            listing.CheckboxLabeled(
                "XTS_IncludeInheritable".Translate(),
                ref includeInheritableXenotypes,
                "XTS_IncludeInheritableDesc".Translate());

            // Gated on the row above: without germline xenogerms in stock there is
            // nothing for it to act on, so it greys out and shows unchecked.
            CheckboxLabeledGated(listing,
                "XTS_ImplantGermlineAsEndogenes".Translate(),
                ref implantGermlineAsEndogenes,
                "XTS_ImplantGermlineAsEndogenesDesc".Translate("XTS_IncludeInheritable".Translate()),
                enabled: includeInheritableXenotypes);

            listing.CheckboxLabeled(
                "XTS_IncludePlayerScenario".Translate(),
                ref includePlayerScenarioXenotypes,
                "XTS_IncludePlayerScenarioDesc".Translate());

            listing.Gap(SectionGap);
        }
    }
}
