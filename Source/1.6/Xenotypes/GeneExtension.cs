using Verse;

namespace XenogermTraderStock
{
    // Opt-out markers for genes that interact badly with the xenogerm trade.
    // Gene-level rather than xenotype-level on purpose: custom xenotypes have
    // no Def to patch, and a xenotype built from android hardware is an android
    // whatever it is called. Vanilla has no "organic humanlike" concept to lean
    // on instead — canGenerateInGeneSet=false sits on Core's hair-colour
    // endogenes (would drop Impid/Waster/Highmate) and selectionWeight=0 on
    // Hemogenic (would drop Sanguophage) — so the markers have to be explicit.
    //
    // Two flags of different strength:
    //
    // excludeFromXenogermStock — this gene must never travel in a trader-sold
    // xenogerm. Any xenotype containing it — preset or player-scenario — is
    // dropped at the candidate stage (XenotypeEligibility.IsCandidate): hidden
    // from the settings grid and never generated as stock. Implies the retarget
    // block below: germline machinery too alien to sell is also too alien to
    // overwrite. Shipped for Vanilla Races Expanded - Android in
    // Patches_VREAndroid.xml.
    //
    // blocksGermlineRetarget — a pawn whose ENDOgenes include this gene keeps
    // its germline: the implant patch skips the wholesale endogene rewrite
    // (vanilla implant behaviour plus the identity stamp instead). Xenotypes
    // containing the gene stay sellable, and implanting one into an unflagged
    // pawn still retargets — the flag protects the gene where it already
    // lives, not the xenotypes it appears in. For genes whose removal leaves
    // live machinery behind: VRE Lycanthrope's morph genes grant an ability
    // marked dontModifyAbilityOnGeneRemoval, so the gizmo would survive the
    // wipe and later resurrect the pre-implant germline (or throw) when cast.
    // Stamped onto that mod's morph genes at runtime by
    // StaticConstructorOnStartupUtility_CallAll_Patch - they are generated
    // defs no XML patch can reach.
    //
    // Other mods, or a player's local patch, can flag their own genes the same
    // way:
    //
    //   <modExtensions>
    //     <li Class="XenogermTraderStock.GeneExtension">
    //       <excludeFromXenogermStock>true</excludeFromXenogermStock>
    //     </li>
    //   </modExtensions>
    public class GeneExtension : DefModExtension
    {
        public bool excludeFromXenogermStock;

        public bool blocksGermlineRetarget;
    }
}
