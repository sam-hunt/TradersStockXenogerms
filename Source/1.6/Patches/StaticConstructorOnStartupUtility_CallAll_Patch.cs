using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace XenogermTraderStock.Patches
{
    // Stamps GeneExtension { blocksGermlineRetarget = true } onto Vanilla Races
    // Expanded - Lycanthrope's morph-family genes after every def load, so the
    // implant patch's germline retarget skips pawns carrying morph machinery in
    // their germline (see GeneUtility_ImplantXenogermItem_Patch.
    // GermlineIsRewritable for the failure this prevents).
    //
    // Why not an XML patch: the per-xenotype morph genes
    // (VRE_Morphs_<XenotypeDefName>, one generated per loaded xenotype, each
    // granting the VRE_Morph ability) are built at runtime by Lycanthrope's
    // GeneDefGenerator.ImpliedGeneDefs postfix, field-by-field with a
    // hard-coded modExtensions list. There is no PatchOperation target: the
    // template is a MorphGeneTemplateDef with no modExtensions field, and the
    // generator copies nothing from the template's extensions anyway.
    //
    // Why CallAll and not [StaticConstructorOnStartup]: the stamp is
    // def-INSTANCE state, and an in-process play-data reload (a mid-session
    // language change) replaces every def instance while static ctors run once
    // per process - the stamp would silently vanish. CallAll executes at the
    // end of every play-data load, implied defs included, so fresh instances
    // are restamped. (XenotypeLedgerStartup's comment states the ledger-side
    // of this same rule; the patch target is vanilla, so the foreign-target
    // patch-timing hazard does not apply.)
    //
    // Why a defName prefix: it is Lycanthrope's own identification rule - its
    // morph machinery classifies genes via defName.Contains("VRE_Morphs")
    // (morph-snapshot maintenance, its germline-reimplant filter), and the
    // generator names every gene "VRE_Morphs_" + the xenotype's defName. The
    // five static morph-CONDITION genes (Nocturnal/Adulthood/Seasonal/Damage/
    // RandomMorphing) share the prefix and are stamped too: they grant no
    // ability, so blocking on them alone is merely conservative, and every
    // shipped morph carrier pairs them with a generated gene anyway.
    //
    // blocksGermlineRetarget only, never excludeFromXenogermStock: Wolfman and
    // Lycan xenogerms are legitimate stock. The flag protects pawns that
    // ALREADY carry morph machinery in the germline, not the xenotypes the
    // genes appear in.
    //
    // A gene that already has a GeneExtension is left alone: within one def
    // generation only a player's local XML patch can have put it there, and
    // that configuration wins.
    [HarmonyPatch(typeof(StaticConstructorOnStartupUtility), nameof(StaticConstructorOnStartupUtility.CallAll))]
    public static class StaticConstructorOnStartupUtility_CallAll_Patch
    {
        private const string MorphGenePrefix = "VRE_Morphs_";

        [HarmonyPostfix]
        public static void Postfix()
        {
            foreach (GeneDef gene in DefDatabase<GeneDef>.AllDefsListForReading)
            {
                if (!gene.defName.StartsWith(MorphGenePrefix)
                    || gene.GetModExtension<GeneExtension>() != null)
                {
                    continue;
                }

                gene.modExtensions ??= new List<DefModExtension>();
                gene.modExtensions.Add(new GeneExtension { blocksGermlineRetarget = true });
            }
        }
    }
}
