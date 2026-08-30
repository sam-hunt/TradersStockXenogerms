using Verse;

namespace XenogermTraderStock
{
    // Opt-out marker for genes that must never travel in a trader-sold
    // xenogerm. Any xenotype containing such a gene — preset or player-created
    // — is dropped at the candidate stage (XenotypeEligibility.IsCandidate):
    // hidden from the settings grid and never generated as stock.
    //
    // Gene-level rather than xenotype-level on purpose: custom xenotypes have
    // no Def to patch, and a xenotype built from android hardware is an android
    // whatever it is called. Vanilla has no "organic humanlike" concept to lean
    // on instead — canGenerateInGeneSet=false sits on Core's hair-colour
    // endogenes (would drop Impid/Waster/Highmate) and selectionWeight=0 on
    // Hemogenic (would drop Sanguophage) — so the marker has to be explicit.
    //
    // Shipped for Vanilla Races Expanded - Android in Patches_VREAndroid.xml.
    // Other mods, or a player's local patch, can flag their own synthetic /
    // animal / insect genelines the same way:
    //
    //   <modExtensions>
    //     <li Class="XenogermTraderStock.GeneExtension">
    //       <excludeFromXenogermStock>true</excludeFromXenogermStock>
    //     </li>
    //   </modExtensions>
    public class GeneExtension : DefModExtension
    {
        public bool excludeFromXenogermStock;
    }
}
