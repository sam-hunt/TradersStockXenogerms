using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace XenogermTraderStock.Patches
{
    // Lets an ideo with "preferred xenotype" precepts accept a xenogerm that
    // makes the pawn one of those xenotypes.
    //
    // Vanilla's Xenogerm.PawnIdeoDisallowsImplanting (behind the float menu, the
    // implant-target picker and Dialog_SelectXenogerm) asks only whether the pawn's
    // ideo holds a precept unwilling to "become a non-preferred xenotype". It never
    // looks at the germ: a vanilla xenogerm can only ever produce a custom
    // xenotype, which is never in PreferredXenotypes, so Ludeon made the ban
    // unconditional. Trader-sold preset xenogerms break that assumption - after
    // GeneUtility_ImplantXenogermItem_Patch the pawn IS the preset, and
    // Ideo.IsPreferredXenotype says so - yet the blanket ban still refuses them.
    // Vanilla already carves this exception for the one path that can name a
    // target xenotype: CompAbilityEffect_ReimplantXenogerm.PawnIdeoCanAccept
    // allows the reimplant when the caster is a preferred xenotype. This postfix
    // gives the item path the same rule, predicting the post-implant identity via
    // PreferredXenotypeGate (preset by def, resolved the same way the implant patch
    // will - XenogermIdentity - so gate and stamp can never disagree; custom by the
    // gene match vanilla uses, against the gene layer the implant patch's germline
    // decision says the genes will land on).
    //
    // The precept's OTHER gate - refusing to propagate the Bloodfeeder gene - is
    // a separate history event and stays in force: a preferred Sanguophage germ
    // is still refused by a pawn whose ideo forbids bloodfeeding.
    [HarmonyPatch(typeof(Xenogerm), nameof(Xenogerm.PawnIdeoDisallowsImplanting))]
    public static class Xenogerm_PawnIdeoDisallowsImplanting_Patch
    {
        public static void Postfix(Xenogerm __instance, Pawn selPawn, ref bool __result)
        {
            if (!__result)
            {
                return;
            }

            Ideo ideo = selPawn?.Ideo;
            if (ideo == null || __instance.GeneSet == null)
            {
                return;
            }

            List<GeneDef> genes = __instance.GeneSet.GenesListForReading;

            // Vanilla checks two independent refusals and returns a single bool; the
            // bloodfeeder one is not ours to relax, so re-derive it before overriding.
            if (genes.Contains(GeneDefOf.Bloodfeeder)
                && !IdeoUtility.DoerWillingToDo(HistoryEventDefOf.PropagateBloodfeederGene, selPawn))
            {
                return;
            }

            XenogermSource source = XenogermIdentity.Resolve(__instance);
            bool germlineRetarget = selPawn.genes != null
                && GeneUtility_ImplantXenogermItem_Patch.WillRetargetGermline(selPawn, source);
            if (PreferredXenotypeGate.ImplantYieldsPreferred(
                    source, germlineRetarget, genes, ideo.PreferredXenotypes, ideo.PreferredCustomXenotypes))
            {
                __result = false;
            }
        }
    }
}
