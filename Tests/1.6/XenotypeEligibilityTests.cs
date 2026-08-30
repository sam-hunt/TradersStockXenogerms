using System.Collections.Generic;
using RimWorld;
using Verse;
using Xunit;

namespace XenogermTraderStock.Tests
{
    // Unit coverage for the pure overloads of XenotypeEligibility
    // (GetCategoryBlock/IsSellable taking raw settings + trait bools) and the
    // GeneExtension opt-out gate. The XenotypeDef overloads need a live
    // DefDatabase (XenotypeDefOf.Baseliner) and are out of scope for this
    // headless suite; CustomXenotype is a plain IExposable, so its overload is
    // covered.
    [Collection("XenogermPricing")]
    public class XenotypeEligibilityTests
    {
        [Fact]
        public void GetCategoryBlock_DefaultsAndNoTraits_IsNone()
        {
            var settings = TestHelpers.InstallDefaultSettings();

            var block = XenotypeEligibility.GetCategoryBlock(settings,
                archite: false, inheritable: false, playerCreated: false);

            Assert.Equal(XenotypeEligibility.CategoryBlock.None, block);
        }

        [Fact]
        public void IsSellable_DefaultsAndNoTraitsAndNotExcluded_IsTrue()
        {
            var settings = TestHelpers.InstallDefaultSettings();

            bool sellable = XenotypeEligibility.IsSellable(settings,
                archite: false, inheritable: false, playerCreated: false, excluded: false);

            Assert.True(sellable);
        }

        [Fact]
        public void GetCategoryBlock_ArchiteExcludedByToggle_ReturnsArchite()
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.includeArchiteXenotypes = false;

            var block = XenotypeEligibility.GetCategoryBlock(settings,
                archite: true, inheritable: false, playerCreated: false);

            Assert.Equal(XenotypeEligibility.CategoryBlock.Archite, block);
        }

        [Fact]
        public void GetCategoryBlock_InheritableExcludedByToggle_ReturnsInheritable()
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.includeInheritableXenotypes = false;

            var block = XenotypeEligibility.GetCategoryBlock(settings,
                archite: false, inheritable: true, playerCreated: false);

            Assert.Equal(XenotypeEligibility.CategoryBlock.Inheritable, block);
        }

        [Fact]
        public void GetCategoryBlock_PlayerCreatedExcludedByDefaultToggle_ReturnsPlayerCreated()
        {
            // includePlayerCreatedXenotypes defaults to false, so this needs
            // no explicit mutation of the fresh settings.
            var settings = TestHelpers.InstallDefaultSettings();

            var block = XenotypeEligibility.GetCategoryBlock(settings,
                archite: false, inheritable: false, playerCreated: true);

            Assert.Equal(XenotypeEligibility.CategoryBlock.PlayerCreated, block);
        }

        [Fact]
        public void GetCategoryBlock_AllThreeTraitsAndTogglesOff_PlayerCreatedWinsPrecedence()
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.includeArchiteXenotypes = false;
            settings.includeInheritableXenotypes = false;
            settings.includePlayerCreatedXenotypes = false;

            var block = XenotypeEligibility.GetCategoryBlock(settings,
                archite: true, inheritable: true, playerCreated: true);

            Assert.Equal(XenotypeEligibility.CategoryBlock.PlayerCreated, block);
        }

        [Fact]
        public void GetCategoryBlock_ArchiteAndInheritableTraitsBothOff_ArchiteWinsPrecedence()
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.includeArchiteXenotypes = false;
            settings.includeInheritableXenotypes = false;

            var block = XenotypeEligibility.GetCategoryBlock(settings,
                archite: true, inheritable: true, playerCreated: false);

            Assert.Equal(XenotypeEligibility.CategoryBlock.Archite, block);
        }

        [Fact]
        public void IsSellable_ExcludedEvenWithAllCategoriesAllowed_IsFalse()
        {
            var settings = TestHelpers.InstallDefaultSettings();

            bool sellable = XenotypeEligibility.IsSellable(settings,
                archite: false, inheritable: false, playerCreated: false, excluded: true);

            Assert.False(sellable);
        }

        [Fact]
        public void IsSellable_CategoryBlockedEvenWhenNotExcluded_IsFalse()
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.includeArchiteXenotypes = false;

            bool sellable = XenotypeEligibility.IsSellable(settings,
                archite: true, inheritable: false, playerCreated: false, excluded: false);

            Assert.False(sellable);
        }

        [Fact]
        public void IsSellable_NotExcludedAndNoCategoryBlock_IsTrue()
        {
            var settings = TestHelpers.InstallDefaultSettings();

            bool sellable = XenotypeEligibility.IsSellable(settings,
                archite: true, inheritable: true, playerCreated: false, excluded: false);

            Assert.True(sellable);
        }

        // The blacklist input is untouched by category toggles: flipping the
        // archite toggle back on restores sellability for a xenotype whose
        // excluded flag never changed, with no other input needing to move.
        [Theory]
        [InlineData(false, false, false)] // archite disallowed, not excluded -> blocked by category
        [InlineData(true, false, true)]   // archite allowed, not excluded -> sellable
        [InlineData(false, true, false)]  // archite disallowed, excluded -> blocked (both reasons)
        [InlineData(true, true, false)]   // archite allowed, excluded -> blocked by blacklist alone
        public void IsSellable_ArchiteToggleRestoresSellability_WhenExcludedFlagUnchanged(
            bool includeArchiteXenotypes, bool excluded, bool expectedSellable)
        {
            var settings = TestHelpers.InstallDefaultSettings();
            settings.includeArchiteXenotypes = includeArchiteXenotypes;

            bool sellable = XenotypeEligibility.IsSellable(settings,
                archite: true, inheritable: false, playerCreated: false, excluded: excluded);

            Assert.Equal(expectedSellable, sellable);
        }
    

        [Fact]
        public void ContainsExcludedGene_NoExtensions_IsFalse()
        {
            var genes = new List<GeneDef> { TestHelpers.MakeGene(), TestHelpers.MakeGene() };

            Assert.False(XenotypeEligibility.ContainsExcludedGene(genes));
        }

        [Fact]
        public void ContainsExcludedGene_ExtensionWithFlagOff_IsFalse()
        {
            var genes = new List<GeneDef> { TestHelpers.MakeGene(exclude: false) };

            Assert.False(XenotypeEligibility.ContainsExcludedGene(genes));
        }

        [Fact]
        public void ContainsExcludedGene_OneFlaggedGeneAmongMany_IsTrue()
        {
            var genes = new List<GeneDef>
            {
                TestHelpers.MakeGene(),
                TestHelpers.MakeGene(exclude: true),
                TestHelpers.MakeGene(),
            };

            Assert.True(XenotypeEligibility.ContainsExcludedGene(genes));
        }

        [Fact]
        public void IsCandidate_CustomXenotypeWithFlaggedGene_IsFalse()
        {
            var custom = new CustomXenotype { name = "Android project" };
            custom.genes.Add(TestHelpers.MakeGene());
            custom.genes.Add(TestHelpers.MakeGene(exclude: true));

            Assert.False(XenotypeEligibility.IsCandidate(custom));
        }

        [Fact]
        public void IsCandidate_CustomXenotypeWithOrdinaryGenes_IsTrue()
        {
            var custom = new CustomXenotype { name = "Organic" };
            custom.genes.Add(TestHelpers.MakeGene());

            Assert.True(XenotypeEligibility.IsCandidate(custom));
        }

        [Fact]
        public void IsCandidate_GenelessCustomXenotype_IsFalse()
        {
            var custom = new CustomXenotype { name = "Empty" };

            Assert.False(XenotypeEligibility.IsCandidate(custom));
        }
    }
}
