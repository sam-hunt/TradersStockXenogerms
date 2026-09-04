using System.Collections.Generic;
using System.Runtime.Serialization;
using RimWorld;
using Verse;
using Xunit;

namespace XenogermTraderStock.Tests
{
    // Unit coverage for XenogermIdentity.Infer and .GenesMatch - the pure gene-set
    // inference behind a comp-less germ's identity (see the class comment on
    // XenogermIdentity for why the comp can go missing). Resolve(Xenogerm) is out
    // of scope: it touches DefDatabase and a live Thing.
    public class XenogermIdentityTests
    {
        private static XenotypeDef MakeXenotype(string defName, string label, List<GeneDef> genes)
        {
            var def = (XenotypeDef)FormatterServices.GetUninitializedObject(typeof(XenotypeDef));
            def.defName = defName;
            def.label = label;
            def.genes = genes;
            return def;
        }

        [Fact]
        public void SingleCandidate_ExactGeneMatch_ReturnsCandidate()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef hussar = MakeXenotype("Hussar", "hussar", new List<GeneDef> { a });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "hussar3", new List<XenotypeDef> { hussar });

            Assert.Same(hussar, result.Preset);
        }

        [Fact]
        public void SingleCandidate_GenesInDifferentOrder_StillMatches()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");
            XenotypeDef candidate = MakeXenotype("Candidate", "candidate", new List<GeneDef> { a, b });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { b, a }, "nonsense", new List<XenotypeDef> { candidate });

            Assert.Same(candidate, result.Preset);
        }

        [Fact]
        public void NoCandidate_GermHasExtraGene_ReturnsNull()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");
            XenotypeDef candidate = MakeXenotype("Candidate", "candidate", new List<GeneDef> { a });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a, b }, "candidate", new List<XenotypeDef> { candidate });

            Assert.True(result.IsNone);
        }

        [Fact]
        public void NoCandidate_GermMissingGene_ReturnsNull()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");
            XenotypeDef candidate = MakeXenotype("Candidate", "candidate", new List<GeneDef> { a, b });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "candidate", new List<XenotypeDef> { candidate });

            Assert.True(result.IsNone);
        }

        [Fact]
        public void EmptyGermGenes_ReturnsNull_EvenAgainstGeneLessCandidate()
        {
            // The Baseliner guard: an empty germ must never resolve to a gene-less preset.
            XenotypeDef baseliner = MakeXenotype("Baseliner", "baseliner", new List<GeneDef>());

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef>(), "baseliner", new List<XenotypeDef> { baseliner });

            Assert.True(result.IsNone);
        }

        [Fact]
        public void Candidates_WithNullOrEmptyGenes_AreSkippedNotMatched()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef nullGenes = MakeXenotype("NullGenes", "nullGenes", null);
            XenotypeDef emptyGenes = MakeXenotype("EmptyGenes", "emptyGenes", new List<GeneDef>());
            XenotypeDef match = MakeXenotype("Match", "match", new List<GeneDef> { a });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "nonsense", new List<XenotypeDef> { nullGenes, emptyGenes, match });

            Assert.Same(match, result.Preset);
        }

        [Fact]
        public void Tie_NameMatchesOneCandidatesLabel_ReturnsThatCandidate()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef hussar = MakeXenotype("HussarDef", "hussar", new List<GeneDef> { a });
            XenotypeDef other = MakeXenotype("OtherDef", "other", new List<GeneDef> { a });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "Hussar", new List<XenotypeDef> { hussar, other });

            Assert.Same(hussar, result.Preset);
        }

        [Fact]
        public void Tie_NameMatchesOneCandidatesDefName_ReturnsThatCandidate()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef hussar = MakeXenotype("HussarDef", "labelOne", new List<GeneDef> { a });
            XenotypeDef other = MakeXenotype("OtherDef", "labelTwo", new List<GeneDef> { a });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "HussarDef", new List<XenotypeDef> { hussar, other });

            Assert.Same(hussar, result.Preset);
        }

        [Fact]
        public void Tie_NameMatchesNeitherCandidate_ReturnsNull()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef one = MakeXenotype("D1", "labelOne", new List<GeneDef> { a });
            XenotypeDef two = MakeXenotype("D2", "labelTwo", new List<GeneDef> { a });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "Nonsense", new List<XenotypeDef> { one, two });

            Assert.True(result.IsNone);
        }

        [Fact]
        public void Tie_NameIsNull_ReturnsNull()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef one = MakeXenotype("D1", "labelOne", new List<GeneDef> { a });
            XenotypeDef two = MakeXenotype("D2", "labelTwo", new List<GeneDef> { a });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, null, new List<XenotypeDef> { one, two });

            Assert.True(result.IsNone);
        }

        [Fact]
        public void Tie_NameIsWhitespace_ReturnsNull()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef one = MakeXenotype("D1", "labelOne", new List<GeneDef> { a });
            XenotypeDef two = MakeXenotype("D2", "labelTwo", new List<GeneDef> { a });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "   ", new List<XenotypeDef> { one, two });

            Assert.True(result.IsNone);
        }

        [Fact]
        public void Tie_BothCandidatesShareMatchingLabel_ReturnsNull()
        {
            // The name cannot break a tie it matches on both sides of.
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef one = MakeXenotype("D1", "Hussar", new List<GeneDef> { a });
            XenotypeDef two = MakeXenotype("D2", "Hussar", new List<GeneDef> { a });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "Hussar", new List<XenotypeDef> { one, two });

            Assert.True(result.IsNone);
        }

        [Fact]
        public void ThreeCandidates_NonMatchingThirdDoesNotInterfereWithTieBreak()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");
            XenotypeDef hussar = MakeXenotype("D1", "Hussar", new List<GeneDef> { a });
            XenotypeDef other = MakeXenotype("D2", "Other", new List<GeneDef> { a });
            XenotypeDef unrelated = MakeXenotype("D3", "Unrelated", new List<GeneDef> { b });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "Hussar", new List<XenotypeDef> { hussar, other, unrelated });

            Assert.Same(hussar, result.Preset);
        }

        [Fact]
        public void CandidateExtraNonPassOnDirectlyGene_StillMatches()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef extra = TestHelpers.MakeGene(defName: "Extra", passOnDirectly: false);
            XenotypeDef candidate = MakeXenotype("Candidate", "candidate", new List<GeneDef> { a, extra });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "nonsense", new List<XenotypeDef> { candidate });

            Assert.Same(candidate, result.Preset);
        }

        [Fact]
        public void GermExtraNonPassOnDirectlyGene_StillMatches()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef extra = TestHelpers.MakeGene(defName: "Extra", passOnDirectly: false);
            XenotypeDef candidate = MakeXenotype("Candidate", "candidate", new List<GeneDef> { a });

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a, extra }, "nonsense", new List<XenotypeDef> { candidate });

            Assert.Same(candidate, result.Preset);
        }

        [Fact]
        public void CustomTemplate_NonInheritableMatch_IsTheSource_OverPreset()
        {
            // The player's own template names this germ, not the preset it clones; the
            // source is not inheritable, so the implant patch leaves the germline alone.
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef hussar = MakeXenotype("Hussar", "hussar", new List<GeneDef> { a });
            var clone = new CustomXenotype { name = "Clone", inheritable = false, genes = new List<GeneDef> { a } };

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "hussar", new List<XenotypeDef> { hussar }, new List<CustomXenotype> { clone });

            Assert.Same(clone, result.Custom);
            Assert.Null(result.Preset);
            Assert.False(result.Inheritable);
        }

        [Fact]
        public void CustomTemplate_InheritableMatch_IsTheSource_OverPreset()
        {
            // The player's inheritable template wins over a preset sharing its genes,
            // whatever the germ is called: the retarget delivers it as a germline.
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef impid = MakeXenotype("Impid", "impid", new List<GeneDef> { a });
            var clone = new CustomXenotype { name = "Clone", inheritable = true, genes = new List<GeneDef> { a } };

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "impid", new List<XenotypeDef> { impid }, new List<CustomXenotype> { clone });

            Assert.Same(clone, result.Custom);
            Assert.Null(result.Preset);
            Assert.True(result.Inheritable);
        }

        [Fact]
        public void CustomTemplate_InheritableMatch_NoPresets_IsTheSource()
        {
            // The commenter's case: a colony's own germline xenotype, sold by the trader.
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");
            var colony = new CustomXenotype { name = "Colony", inheritable = true, genes = new List<GeneDef> { a, b } };

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { b, a }, "Colony", new List<XenotypeDef>(), new List<CustomXenotype> { colony });

            Assert.Same(colony, result.Custom);
        }

        [Fact]
        public void CustomTemplate_InheritableMatch_GenesInDifferentOrderAndOtherName_StillTheSource()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");
            var colony = new CustomXenotype { name = "Colony", inheritable = true, genes = new List<GeneDef> { a, b } };

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { b, a }, "colony2", null, new List<CustomXenotype> { colony });

            Assert.Same(colony, result.Custom);
        }

        [Fact]
        public void CustomTemplate_Tie_NameMatchesInheritableOne_ReturnsIt()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            var germline = new CustomXenotype { name = "Germline", inheritable = true, genes = new List<GeneDef> { a } };
            var implant = new CustomXenotype { name = "Implant", inheritable = false, genes = new List<GeneDef> { a } };

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "germline", null, new List<CustomXenotype> { implant, germline });

            Assert.Same(germline, result.Custom);
        }

        [Fact]
        public void CustomTemplate_Tie_NameMatchesNonInheritableOne_ReturnsIt()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef impid = MakeXenotype("Impid", "impid", new List<GeneDef> { a });
            var germline = new CustomXenotype { name = "Germline", inheritable = true, genes = new List<GeneDef> { a } };
            var implant = new CustomXenotype { name = "Implant", inheritable = false, genes = new List<GeneDef> { a } };

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "Implant", new List<XenotypeDef> { impid }, new List<CustomXenotype> { germline, implant });

            Assert.Same(implant, result.Custom);
            Assert.False(result.Inheritable);
        }

        [Fact]
        public void CustomTemplate_Tie_NameMatchesNeither_ClaimsGermForVanilla_NotPreset()
        {
            // Templates matched, so the preset never gets a look; the unbreakable tie is vanilla's.
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef impid = MakeXenotype("Impid", "impid", new List<GeneDef> { a });
            var one = new CustomXenotype { name = "One", inheritable = true, genes = new List<GeneDef> { a } };
            var two = new CustomXenotype { name = "Two", inheritable = true, genes = new List<GeneDef> { a } };

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "impid", new List<XenotypeDef> { impid }, new List<CustomXenotype> { one, two });

            Assert.True(result.IsNone);
        }

        [Fact]
        public void CustomTemplate_WithNullOrEmptyGenes_IsSkipped()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            XenotypeDef impid = MakeXenotype("Impid", "impid", new List<GeneDef> { a });
            var empty = new CustomXenotype { name = "Empty", inheritable = true, genes = new List<GeneDef>() };
            var nullGenes = new CustomXenotype { name = "NullGenes", inheritable = true, genes = null };

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "impid", new List<XenotypeDef> { impid }, new List<CustomXenotype> { empty, nullGenes });

            Assert.Same(impid, result.Preset);
        }

        [Fact]
        public void EmptyGermGenes_NeverMatchesGeneLessTemplate()
        {
            var empty = new CustomXenotype { name = "Empty", inheritable = true, genes = new List<GeneDef>() };

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef>(), "Empty", new List<XenotypeDef>(), new List<CustomXenotype> { empty });

            Assert.True(result.IsNone);
        }

        [Fact]
        public void Source_None_IsNotInheritable()
        {
            Assert.False(XenogermSource.None.Inheritable);
            Assert.True(XenogermSource.None.IsNone);
        }

        [Fact]
        public void Source_NonInheritablePreset_IsNotInheritable()
        {
            XenotypeDef hussar = MakeXenotype("Hussar", "hussar", new List<GeneDef>());
            Assert.False(XenogermSource.Of(hussar).Inheritable);
        }

        [Fact]
        public void CustomTemplate_NonMatchingGenes_DoesNotClaimGerm()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");
            XenotypeDef hussar = MakeXenotype("Hussar", "hussar", new List<GeneDef> { a });
            var other = new CustomXenotype { name = "Other", inheritable = false, genes = new List<GeneDef> { b } };

            XenogermSource result = XenogermIdentity.Infer(
                new List<GeneDef> { a }, "hussar", new List<XenotypeDef> { hussar }, new List<CustomXenotype> { other, null });

            Assert.Same(hussar, result.Preset);
        }

        [Fact]
        public void NullGermGenes_ReturnsNull()
        {
            XenotypeDef candidate = MakeXenotype("Candidate", "candidate", new List<GeneDef> { TestHelpers.MakeGene(defName: "A") });

            XenogermSource result = XenogermIdentity.Infer(null, "candidate", new List<XenotypeDef> { candidate });

            Assert.True(result.IsNone);
        }

        [Fact]
        public void NullCandidates_ReturnsNull()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");

            XenogermSource result = XenogermIdentity.Infer(new List<GeneDef> { a }, "candidate", null);

            Assert.True(result.IsNone);
        }

        [Fact]
        public void GenesMatch_IdenticalLists_ReturnsTrue()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");

            bool result = XenogermIdentity.GenesMatch(new List<GeneDef> { a, b }, new List<GeneDef> { b, a });

            Assert.True(result);
        }

        [Fact]
        public void GenesMatch_NullGermGenes_ReturnsFalse()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");

            bool result = XenogermIdentity.GenesMatch(null, new List<GeneDef> { a });

            Assert.False(result);
        }

        [Fact]
        public void GenesMatch_NullTemplateGenes_ReturnsFalse()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");

            bool result = XenogermIdentity.GenesMatch(new List<GeneDef> { a }, null);

            Assert.False(result);
        }

        [Fact]
        public void GenesMatch_ExtraGeneOnGermSide_ReturnsFalse()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");

            bool result = XenogermIdentity.GenesMatch(new List<GeneDef> { a, b }, new List<GeneDef> { a });

            Assert.False(result);
        }

        [Fact]
        public void GenesMatch_ExtraGeneOnTemplateSide_ReturnsFalse()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");

            bool result = XenogermIdentity.GenesMatch(new List<GeneDef> { a }, new List<GeneDef> { a, b });

            Assert.False(result);
        }

        [Fact]
        public void GenesMatch_NonPassOnDirectlyExtrasOnBothSides_AreIgnored()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef germOnly = TestHelpers.MakeGene(defName: "GermOnly", passOnDirectly: false);
            GeneDef templateOnly = TestHelpers.MakeGene(defName: "TemplateOnly", passOnDirectly: false);

            bool result = XenogermIdentity.GenesMatch(
                new List<GeneDef> { a, germOnly }, new List<GeneDef> { a, templateOnly });

            Assert.True(result);
        }
    }
}
