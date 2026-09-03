using System.Collections.Generic;
using System.Runtime.Serialization;
using RimWorld;
using Verse;
using Xunit;
using XenogermTraderStock.Patches;

namespace XenogermTraderStock.Tests
{
    // Unit coverage for the pure prediction behind the preferred-xenotype implant
    // gate. XenotypeDefs are allocated uninitialized (reference identity is all the
    // preset branch reads); CustomXenotype is a plain IExposable.
    public class PreferredXenotypeGateTests
    {
        private static XenotypeDef MakeXenotype(string defName)
        {
            var def = (XenotypeDef)FormatterServices.GetUninitializedObject(typeof(XenotypeDef));
            def.defName = defName;
            return def;
        }

        private static CustomXenotype MakeCustom(bool inheritable, params GeneDef[] genes)
        {
            return new CustomXenotype { name = "Custom", inheritable = inheritable, genes = new List<GeneDef>(genes) };
        }

        [Fact]
        public void Preset_InPreferredList_YieldsPreferred()
        {
            XenotypeDef impid = MakeXenotype("Impid");

            bool result = PreferredXenotypeGate.ImplantYieldsPreferred(
                impid, new List<GeneDef>(), new List<XenotypeDef> { MakeXenotype("Hussar"), impid }, null);

            Assert.True(result);
        }

        [Fact]
        public void Preset_NotInPreferredList_YieldsNonPreferred()
        {
            bool result = PreferredXenotypeGate.ImplantYieldsPreferred(
                MakeXenotype("Impid"), new List<GeneDef>(), new List<XenotypeDef> { MakeXenotype("Hussar") }, null);

            Assert.False(result);
        }

        [Fact]
        public void Preset_IgnoresCustomMatches()
        {
            GeneDef gene = TestHelpers.MakeGene(defName: "A");
            var germ = new List<GeneDef> { gene };

            bool result = PreferredXenotypeGate.ImplantYieldsPreferred(
                MakeXenotype("Impid"), germ, new List<XenotypeDef>(), new List<CustomXenotype> { MakeCustom(false, gene) });

            Assert.False(result);
        }

        [Fact]
        public void Custom_ExactGeneMatch_YieldsPreferred()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");

            bool result = PreferredXenotypeGate.ImplantYieldsPreferred(
                null, new List<GeneDef> { b, a }, new List<XenotypeDef>(), new List<CustomXenotype> { MakeCustom(false, a, b) });

            Assert.True(result);
        }

        [Fact]
        public void Custom_ExtraGermGene_YieldsNonPreferred()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");

            bool result = PreferredXenotypeGate.ImplantYieldsPreferred(
                null, new List<GeneDef> { a, b }, new List<XenotypeDef>(), new List<CustomXenotype> { MakeCustom(false, a) });

            Assert.False(result);
        }

        [Fact]
        public void Custom_MissingTemplateGene_YieldsNonPreferred()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef b = TestHelpers.MakeGene(defName: "B");

            bool result = PreferredXenotypeGate.ImplantYieldsPreferred(
                null, new List<GeneDef> { a }, new List<XenotypeDef>(), new List<CustomXenotype> { MakeCustom(false, a, b) });

            Assert.False(result);
        }

        [Fact]
        public void Custom_NonPassOnDirectlyGenes_AreIgnoredOnBothSides()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");
            GeneDef germOnly = TestHelpers.MakeGene(defName: "GermOnly", passOnDirectly: false);
            GeneDef templateOnly = TestHelpers.MakeGene(defName: "TemplateOnly", passOnDirectly: false);

            bool result = PreferredXenotypeGate.ImplantYieldsPreferred(
                null, new List<GeneDef> { a, germOnly }, new List<XenotypeDef>(), new List<CustomXenotype> { MakeCustom(false, a, templateOnly) });

            Assert.True(result);
        }

        [Fact]
        public void Custom_InheritableTemplate_GrantsNoException()
        {
            GeneDef a = TestHelpers.MakeGene(defName: "A");

            bool result = PreferredXenotypeGate.ImplantYieldsPreferred(
                null, new List<GeneDef> { a }, new List<XenotypeDef>(), new List<CustomXenotype> { MakeCustom(true, a) });

            Assert.False(result);
        }

        [Fact]
        public void Custom_EmptyPreferredLists_YieldsNonPreferred()
        {
            bool result = PreferredXenotypeGate.ImplantYieldsPreferred(
                null, new List<GeneDef> { TestHelpers.MakeGene() }, new List<XenotypeDef>(), new List<CustomXenotype>());

            Assert.False(result);
        }
    }
}
