using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace XenogermTraderStock.Patches
{
    // After xenogerm implantation, assigns the preset xenotype reference to the pawn.
    //
    // Vanilla implantation (GeneUtility.ImplantXenogermItem) resets the pawn to Baseliner,
    // copies the item's xenotypeName and iconDef onto the gene tracker, then adds the genes
    // as xenogenes. The result is a "custom" xenotype that merely shares the preset's name.
    //
    // This postfix runs inside Recipe_ImplantXenogerm.ApplyOnPawn, immediately after the
    // vanilla method returns and before anything else can observe the pawn, and leaves the
    // gene tracker's identity fields exactly as PawnGenerator leaves them for a generated
    // pawn of that xenotype:
    //   xenotype     = the preset def   (SetXenotypeDirect; genes are untouched)
    //   xenotypeName = null             (cleared by SetXenotypeDirect; label falls through to the def)
    //   iconDef      = null             (cleared here; XenotypeIcon falls through to the def's icon)
    //
    // SetXenotypeDirect deliberately does not clear iconDef (only SetXenotype does, and that
    // would also wipe the freshly implanted xenogenes), so it must be nulled explicitly. The
    // item's iconDef is never null in practice: Xenogerm.ExposeData backfills
    // XenotypeIconDefOf.Basic on load, which is how the "custom xenotype" icon leaked onto
    // pawns implanted from a xenogerm that had been through a save/load cycle.
    //
    // Not touched: Pawn_GeneTracker.hybrid. It describes the pawn's germline (set by
    // PregnancyUtility for a child of two inheritable xenotypes, read only for inheritance),
    // which a xenogerm implant does not change - vanilla leaves it alone too.
    //
    // This enables:
    // - Ideology recognition (preferred xenotypes)
    // - Proper xenotype display in social/info panels (label, icon, info-card link)
    // - "Naturalized" members of germline xenotypes (e.g., Impid) for social purposes
    //
    // Independently of all of the above (settings.preserveDeathrestCapacity, and unlike
    // everything else here not gated on the item being trader-sold), the pawn's
    // deathrest capacity is captured before and restored after the implant - see
    // DeathrestCapacityCarryover; GeneUtility_ReimplantXenogerm_Patch covers the
    // sanguophage reimplant ability the same way.
    //
    // Optionally (settings.implantGermlineAsEndogenes), a germline xenotype's genes
    // replace the pawn's endogenes - what PawnGenerator produces for a born member of an
    // inheritable xenotype (SetXenotype adds them as endogenes) - so children inherit them
    // and a later xenogerm implant stacks on top rather than wiping them. The xenogenes
    // the pawn carried before the implant survive it (a germline rewrite acts on the
    // germline layer only), except for the Baseliner conversion, which wipes both layers.
    // Vanilla's own implant keeps everything as xenogenes, hence a setting (default on).
    [HarmonyPatch(typeof(GeneUtility), nameof(GeneUtility.ImplantXenogermItem))]
    public static class GeneUtility_ImplantXenogermItem_Patch
    {
        // Cross-call state handed from Prefix to Postfix: what the pawn carried
        // before vanilla's wipe that must be rebuilt afterwards.
        public class ImplantState
        {
            // Pre-implant xenogene defs to rebuild after a germline retarget; null
            // when no retarget is coming or the pawn had none.
            public List<GeneDef> preservedXenogenes;

            // Pre-implant deathrest capacity; 0 when the pawn had no deathrest gene.
            public int deathrestCapacity;
        }

        // Two captures, both of state vanilla destroys before the postfix can see it:
        //
        // Xenogenes: a germline implant acts on the germline layer only, so the
        // xenogenes the pawn carried before the implant must survive it - but vanilla
        // wipes them first (SetXenotype(Baseliner) clears the xenogene list). Their
        // defs are snapshotted whenever the retarget is going to run. Defs, not Gene
        // instances: vanilla has already destroyed the instances (RemoveGene/
        // PostRemove) by the time the postfix runs, so per-gene state (resource
        // levels, cooldowns) resets - exactly what a vanilla implant does to every
        // xenogene it re-creates. Baseliner never snapshots (inheritable is false on
        // its def): its conversion item deliberately wipes both layers.
        //
        // Deathrest capacity: the one per-instance value worth more than that rule -
        // see DeathrestCapacityCarryover. Captured for EVERY implant, trader-sold or
        // not, so a bought and a crafted xenogerm treat the same pawn the same way.
        public static void Prefix(Pawn pawn, Xenogerm xenogerm, out ImplantState __state)
        {
            __state = null;
            if (pawn.genes == null)
            {
                return;
            }

            XenogermTraderStockSettings settings = XenogermTraderStockMod.Settings;
            int deathrestCapacity = settings.preserveDeathrestCapacity
                ? DeathrestCapacityCarryover.Snapshot(pawn)
                : 0;

            List<GeneDef> preservedXenogenes = null;
            var comp = xenogerm.TryGetComp<CompXenotypeSource>();
            if (comp?.sourceXenotype != null
                && comp.sourceXenotype.inheritable
                && settings.implantGermlineAsEndogenes
                && GermlineIsRewritable(pawn))
            {
                List<Gene> xenogenes = pawn.genes.Xenogenes;
                if (xenogenes.Count > 0)
                {
                    preservedXenogenes = new List<GeneDef>(xenogenes.Count);
                    foreach (Gene gene in xenogenes)
                    {
                        preservedXenogenes.Add(gene.def);
                    }
                }
            }

            if (deathrestCapacity > 0 || preservedXenogenes != null)
            {
                __state = new ImplantState
                {
                    preservedXenogenes = preservedXenogenes,
                    deathrestCapacity = deathrestCapacity,
                };
            }
        }

        public static void Postfix(Pawn pawn, Xenogerm xenogerm, ImplantState __state)
        {
            if (pawn.genes == null)
            {
                return;
            }

            var comp = xenogerm.TryGetComp<CompXenotypeSource>();
            if (comp?.sourceXenotype == null)
            {
                // Not one of ours; only the deathrest carryover applies.
                RestoreDeathrestCapacity(pawn, __state);
                return;
            }

            // Mirrors GeneUtility.ReimplantXenogerm's field handling: set the xenotype, then
            // assign the identity fields explicitly. Genes stay as the xenogenes vanilla added.
            pawn.genes.SetXenotypeDirect(comp.sourceXenotype);
            pawn.genes.iconDef = null;

            // Baseliner bypasses the germline setting: its gene-less xenogerm has
            // exactly one honest meaning - make this pawn a baseliner - and with
            // no genes to add there is no "keep it as xenogenes" alternative for
            // the setting to choose. Vanilla has already wiped the xenogenes
            // (SetXenotype clears them before consulting the empty gene list);
            // the retarget clears the germline too, adding nothing back except
            // the pawn's own skin/hair colour, which Baseliner never supplies.
            // Without this, an Impid implanted with a baseliner xenogerm would
            // be relabeled Baseliner while keeping every Impid germline gene.
            // The prefix never snapshots for Baseliner either: the conversion
            // wipes both layers, so a Hussar comes out a plain baseliner.
            if (GermlineIsRewritable(pawn)
                && (comp.sourceXenotype == XenotypeDefOf.Baseliner
                    || (comp.sourceXenotype.inheritable && XenogermTraderStockMod.Settings.implantGermlineAsEndogenes)))
            {
                RetargetToEndogenes(pawn, xenogerm, __state?.preservedXenogenes);
            }

            // Last, once the final gene set exists: if a deathrest gene came through
            // (as a fresh endogene from the retarget, a restored xenogene, or vanilla's
            // own re-add), its capacity has been PostAdd-reset to 1 - put it back.
            RestoreDeathrestCapacity(pawn, __state);
        }

        private static void RestoreDeathrestCapacity(Pawn pawn, ImplantState state)
        {
            if (state != null)
            {
                DeathrestCapacityCarryover.Restore(pawn, state.deathrestCapacity);
            }
        }

        // Never rewrite the germline of a pawn whose endogenes include a gene opted
        // out of the xenogerm trade (GeneExtension.excludeFromXenogermStock). Those
        // genes are germline machinery no organic xenogerm accounts for - VREA android
        // hardware lives as endogenes (androids are inheritable XenotypeDefs) and
        // carries per-instance state (reactor charge, stored name, decay timers), so
        // the wholesale endogene replace would destroy the pawn outright. Defense in
        // depth, not a live hole: VREA closes every shipped entry point (the xenogerm
        // float-menu option is disabled for androids, and an AvailableOnNow postfix
        // over every RecipeWorker subclass blocks the surgery - its disallowedRecipes
        // list is an XML Def, not something a player can toggle in game). This guard
        // makes the extension's promise hold anyway, for implant paths nobody audited:
        // another mod calling ImplantXenogermItem directly, or a non-VREA geneline
        // flagged with the same extension. Such a pawn gets vanilla implant behaviour
        // plus the identity stamp, nothing more - the Baseliner conversion included.
        private static bool GermlineIsRewritable(Pawn pawn)
        {
            return !XenotypeEligibility.ContainsExcludedGene(pawn.genes.Endogenes.Select(g => g.def));
        }

        // Rebuilds the pawn's germline as the implant's genes. Vanilla has just
        // SetXenotype(Baseliner)'d the pawn (every prior xenogene gone) and added exactly
        // the xenogerm's genes as xenogenes, so clearing the xenogene list removes the
        // implant and nothing else. The previous endogenes are then replaced wholesale,
        // not merged: the pawn becomes a born member of the xenotype, not a hybrid of it
        // and whatever germline it had before - and a merge would also let old genes win
        // conflicts, because two endogenes resolve by display order (GeneUtility.Overrides
        // -> GenesInOrder), not arrival order.
        //
        // Skin and hair colour are the exception PawnGenerator itself makes: after
        // SetXenotype it backfills a random melanin / hair-colour gene whenever the
        // xenotype supplies none (GetMelaninGene / GetHairColorGene == null). Here the
        // pawn's own genes are re-added instead of random ones - an Impid germline still
        // turns the pawn red, but a xenotype with no skin gene leaves the pawn's natural
        // colouring alone, exactly the born-member look.
        //
        // The pawn's pre-implant xenogenes (snapshotted by the prefix) are restored last:
        // a germline implant rewrites the germline layer only, so a Hussar implanted with
        // a Yttakin germline keeps the Hussar xenogenes on top of the new endogenes -
        // just as vanilla-implanting a Hussar xenogerm into a born Yttakin would. Last,
        // because GetMelaninGene/GetHairColorGene scan ALL genes: restoring earlier would
        // let a coloured xenogene suppress the germline's own colour backfill, and a
        // born member's germline always carries its colouring. Duplicates across the two
        // layers are vanilla-legal; the xenogene copy always overrides (GeneDef.Overrides
        // returns true whenever the xenogene side of a conflict is the caller).
        //
        // Consequences when no xenogenes remain, exactly as for a born member:
        // no xenogerm can be extracted from the pawn (GeneUtility.CanAbsorbXenogerm), and
        // the pawn no longer counts as having an implanted part for that purpose. A pawn
        // whose pre-implant xenogenes were restored keeps them extractable as before.
        private static void RetargetToEndogenes(Pawn pawn, Xenogerm xenogerm, List<GeneDef> preservedXenogenes)
        {
            Pawn_GeneTracker genes = pawn.genes;
            genes.ClearXenogenes();

            GeneDef melanin = genes.GetMelaninGene();
            GeneDef hairColor = genes.GetHairColorGene();

            List<Gene> endogenes = genes.Endogenes;
            for (int i = endogenes.Count - 1; i >= 0; i--)
            {
                genes.RemoveGene(endogenes[i]);
            }

            foreach (GeneDef gene in xenogerm.GeneSet.GenesListForReading)
            {
                genes.AddGene(gene, xenogene: false);
            }

            if (genes.GetMelaninGene() == null && melanin != null)
            {
                genes.AddGene(melanin, xenogene: false);
            }

            if (genes.GetHairColorGene() == null && hairColor != null)
            {
                genes.AddGene(hairColor, xenogene: false);
            }

            if (preservedXenogenes != null)
            {
                foreach (GeneDef gene in preservedXenogenes)
                {
                    genes.AddGene(gene, xenogene: true);
                }
            }
        }
    }
}
