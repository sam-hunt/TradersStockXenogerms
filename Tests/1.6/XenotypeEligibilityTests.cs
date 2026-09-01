using System.Collections.Generic;
using RimWorld;
using Verse;
using Xunit;

namespace XenogermTraderStock.Tests
{
    // Unit coverage for the pure members of XenotypeEligibility: category
    // precedence (Categorize), the majority-vote seeding rule (SeedValue),
    // and the GeneExtension opt-out gate. The XenotypeDef overloads need a
    // live DefDatabase (XenotypeDefOf.Baseliner) and are out of scope for
    // this headless suite; CustomXenotype is a plain IExposable, so its
    // IsCandidate overload is covered.
    [Collection("XenogermPricing")]
    public class XenotypeEligibilityTests
    {
        [Fact]
        public void Categorize_NoTraits_IsPlain()
        {
            var category = XenotypeEligibility.Categorize(archite: false, inheritable: false, playerScenario: false);

            Assert.Equal(XenotypeEligibility.XenotypeCategory.Plain, category);
        }

        [Fact]
        public void Categorize_ArchiteOnly_IsArchite()
        {
            var category = XenotypeEligibility.Categorize(archite: true, inheritable: false, playerScenario: false);

            Assert.Equal(XenotypeEligibility.XenotypeCategory.Archite, category);
        }

        [Fact]
        public void Categorize_InheritableOnly_IsInheritable()
        {
            var category = XenotypeEligibility.Categorize(archite: false, inheritable: true, playerScenario: false);

            Assert.Equal(XenotypeEligibility.XenotypeCategory.Inheritable, category);
        }

        [Fact]
        public void Categorize_PlayerScenarioOnly_IsPlayerScenario()
        {
            var category = XenotypeEligibility.Categorize(archite: false, inheritable: false, playerScenario: true);

            Assert.Equal(XenotypeEligibility.XenotypeCategory.PlayerScenario, category);
        }

        [Fact]
        public void Categorize_PlayerScenarioWinsOverArchiteAndInheritable()
        {
            var category = XenotypeEligibility.Categorize(archite: true, inheritable: true, playerScenario: true);

            Assert.Equal(XenotypeEligibility.XenotypeCategory.PlayerScenario, category);
        }

        [Fact]
        public void Categorize_ArchiteWinsOverInheritable()
        {
            var category = XenotypeEligibility.Categorize(archite: true, inheritable: true, playerScenario: false);

            Assert.Equal(XenotypeEligibility.XenotypeCategory.Archite, category);
        }

        [Fact]
        public void SeedValue_MajoritySoldPeers_IsTrue()
        {
            var ledger = new List<(XenotypeEligibility.XenotypeCategory category, bool sold)>
            {
                (XenotypeEligibility.XenotypeCategory.Archite, true),
                (XenotypeEligibility.XenotypeCategory.Archite, true),
                (XenotypeEligibility.XenotypeCategory.Archite, false),
            };

            bool seeded = XenotypeEligibility.SeedValue(XenotypeEligibility.XenotypeCategory.Archite,
                gatesAsInheritable: false, ledger: ledger);

            Assert.True(seeded);
        }

        [Fact]
        public void SeedValue_MajorityUnsoldPeers_IsFalse()
        {
            var ledger = new List<(XenotypeEligibility.XenotypeCategory category, bool sold)>
            {
                (XenotypeEligibility.XenotypeCategory.Archite, false),
                (XenotypeEligibility.XenotypeCategory.Archite, false),
                (XenotypeEligibility.XenotypeCategory.Archite, true),
            };

            bool seeded = XenotypeEligibility.SeedValue(XenotypeEligibility.XenotypeCategory.Archite,
                gatesAsInheritable: false, ledger: ledger);

            Assert.False(seeded);
        }

        // Peers of a different category must not vote: a ledger stuffed with
        // sold Archite entries and no Plain entries at all is an exact tie
        // (zero same-category votes) for a Plain newcomer, so it falls back
        // to !gatesAsInheritable rather than being dragged along by Archite.
        [Fact]
        public void SeedValue_PeersOfOtherCategoriesDoNotVote()
        {
            var ledger = new List<(XenotypeEligibility.XenotypeCategory category, bool sold)>
            {
                (XenotypeEligibility.XenotypeCategory.Archite, true),
                (XenotypeEligibility.XenotypeCategory.Archite, true),
                (XenotypeEligibility.XenotypeCategory.Archite, true),
            };

            bool seeded = XenotypeEligibility.SeedValue(XenotypeEligibility.XenotypeCategory.Plain,
                gatesAsInheritable: false, ledger: ledger);

            Assert.True(seeded);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void SeedValue_ExactTie_FallsBackToNotGatesAsInheritable(bool gatesAsInheritable, bool expected)
        {
            var ledger = new List<(XenotypeEligibility.XenotypeCategory category, bool sold)>
            {
                (XenotypeEligibility.XenotypeCategory.Inheritable, true),
                (XenotypeEligibility.XenotypeCategory.Inheritable, false),
            };

            bool seeded = XenotypeEligibility.SeedValue(XenotypeEligibility.XenotypeCategory.Inheritable,
                gatesAsInheritable, ledger);

            Assert.Equal(expected, seeded);
        }

        // The empty ledger is the shipped-defaults case: a fresh install has
        // no votes at all, so every category falls back to !gatesAsInheritable
        // - germline-rewriting xenotypes seed unsold, everything else sold.
        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void SeedValue_EmptyLedger_FallsBackToNotGatesAsInheritable(bool gatesAsInheritable, bool expected)
        {
            var ledger = new List<(XenotypeEligibility.XenotypeCategory category, bool sold)>();

            bool seeded = XenotypeEligibility.SeedValue(XenotypeEligibility.XenotypeCategory.Plain,
                gatesAsInheritable, ledger);

            Assert.Equal(expected, seeded);
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
        public void BlocksGermlineRetarget_NoExtensions_IsFalse()
        {
            var genes = new List<GeneDef> { TestHelpers.MakeGene(), TestHelpers.MakeGene() };

            Assert.False(XenotypeEligibility.BlocksGermlineRetarget(genes));
        }

        [Fact]
        public void BlocksGermlineRetarget_BothFlagsOff_IsFalse()
        {
            var genes = new List<GeneDef> { TestHelpers.MakeGene(exclude: false, blockRetarget: false) };

            Assert.False(XenotypeEligibility.BlocksGermlineRetarget(genes));
        }

        [Fact]
        public void BlocksGermlineRetarget_OneBlockingGeneAmongMany_IsTrue()
        {
            var genes = new List<GeneDef>
            {
                TestHelpers.MakeGene(),
                TestHelpers.MakeGene(blockRetarget: true),
                TestHelpers.MakeGene(),
            };

            Assert.True(XenotypeEligibility.BlocksGermlineRetarget(genes));
        }

        // excludeFromXenogermStock implies the retarget block: germline
        // machinery too alien to sell is also too alien to overwrite.
        [Fact]
        public void BlocksGermlineRetarget_ExcludedGene_AlsoBlocks()
        {
            var genes = new List<GeneDef> { TestHelpers.MakeGene(exclude: true) };

            Assert.True(XenotypeEligibility.BlocksGermlineRetarget(genes));
        }

        // The implication does not run the other way: a retarget-blocking gene
        // (VRE Lycanthrope's morph family) leaves its xenotypes fully sellable.
        [Fact]
        public void ContainsExcludedGene_RetargetBlockedGene_IsFalse()
        {
            var genes = new List<GeneDef> { TestHelpers.MakeGene(blockRetarget: true) };

            Assert.False(XenotypeEligibility.ContainsExcludedGene(genes));
        }

        [Fact]
        public void IsCandidate_CustomXenotypeWithRetargetBlockedGene_IsTrue()
        {
            var custom = new CustomXenotype { name = "Lycanthrope project" };
            custom.genes.Add(TestHelpers.MakeGene(blockRetarget: true));

            Assert.True(XenotypeEligibility.IsCandidate(custom));
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
