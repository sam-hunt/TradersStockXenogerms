using Xunit;

namespace XenogermTraderStock.Tests
{
    // Unit coverage for the pure overloads of XenotypeEligibility
    // (GetCategoryBlock/IsSellable taking raw settings + trait bools). The
    // XenotypeDef/CustomXenotype overloads need a live DefDatabase and are out
    // of scope for this headless suite.
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
    }
}
