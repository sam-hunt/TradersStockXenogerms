using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using RimWorld;
using Verse;
using XenogermTraderStock.Patches;

namespace XenogermTraderStock
{
    // Dev-mode debug actions. Spawn: a trader-style xenogerm for any xenotype at
    // the clicked cell. Deliberately bypasses XenotypeEligibility so xenogerms
    // that stock generation would never produce - category toggles, per-xenotype
    // blacklist, GeneExtension opt-outs like VREA androids - can still be put in
    // hand for testing; entries eligibility currently filters out carry vanilla's
    // " [NO]" suffix but spawn anyway. Only gene-less xenotypes are skipped (an
    // empty gene set cannot form a meaningful xenogerm item), with the one
    // exception eligibility itself makes: Baseliner, whose empty xenogerm is the
    // baseliner-conversion item.
    public static class XenogermDebugActions
    {
        // The debug menu's DebugActionNode tree is built ONCE per play-data load
        // (Dialog_Debug.rootNode is static; only PlayDataLoader.ResetStaticDataPre
        // clears it), so these nodes outlive save switches. Preset defs are
        // load-stable, but their [NO] state tracks live settings via labelGetter.
        // Custom xenotypes belong to Current.Game and so cannot be baked into the
        // cached tree; they get a single node opening a legacy
        // Dialog_DebugOptionListLister, which is rebuilt fresh on every open.
        [DebugAction("Xenogerm Trader Stock", "Spawn preset xenogerm", false, false, true, false, false, 0, false,
            allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static List<DebugActionNode> SpawnPresetXenogerm()
        {
            var nodes = new List<DebugActionNode>();

            // Presets in the same order as the settings grid: Baseliner pinned
            // first, then vanilla display priority, then label.
            foreach (var xenotype in XenotypeEligibility.InDisplayOrder(
                DefDatabase<XenotypeDef>.AllDefsListForReading
                    .Where(x => x == XenotypeDefOf.Baseliner || !x.genes.NullOrEmpty())))
            {
                var node = new DebugActionNode(xenotype.LabelCap, DebugActionType.ToolMap, delegate
                {
                    PlaceAtMouseCell(XenogermFactory.CreateForXenotype(xenotype));
                });
                node.labelGetter = () => XenotypeEligibility.IsSellable(xenotype)
                    ? node.label
                    : node.label + " [NO]";
                nodes.Add(node);
            }

            nodes.Add(new DebugActionNode("Custom xenotype...", DebugActionType.Action,
                OpenCustomXenotypeMenu));

            return nodes;
        }

        // Player-scenario customs from the live game database - the same source
        // stock generation sells from (includes VREA android projects). Built
        // fresh per open, so the inline [NO] suffix never goes stale.
        private static void OpenCustomXenotypeMenu()
        {
            var options = new List<DebugMenuOption>();
            var customXenotypes = Current.Game?.customXenotypeDatabase?.customXenotypes;
            if (customXenotypes != null)
            {
                foreach (var custom in customXenotypes
                    .Where(c => !c.genes.NullOrEmpty())
                    .OrderBy(c => c.name))
                {
                    string label = custom.name.CapitalizeFirst();
                    if (!XenotypeEligibility.IsSellable(custom))
                    {
                        label += " [NO]";
                    }
                    options.Add(new DebugMenuOption(label, DebugMenuOptionMode.Tool, delegate
                    {
                        PlaceAtMouseCell(XenogermFactory.CreateForCustomXenotype(custom));
                    }));
                }
            }

            if (options.Count == 0)
            {
                Messages.Message("No custom xenotypes in this game.", MessageTypeDefOf.RejectInput,
                    historical: false);
                return;
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }

        // Vanilla's Devtool menu on the genes tab offers "Reset genes to base
        // xenotype" only while the pawn has no custom name (!UniqueXenotype), and
        // its reset re-applies genes.Xenotype - a XenotypeDef - so a pawn whose
        // identity is a CustomXenotype template has no reset at all: the option is
        // hidden, and were it shown it would reset the pawn to a bare Baseliner.
        // This tool fills that gap. Click a pawn, then pick the template BY NAME
        // from the game's custom xenotype database (the scenario / ideo templates
        // that XenogermIdentity resolves against); nothing is inferred from the
        // pawn's genes, so the player always sees which of Biotech's several
        // "custom xenotype" notions they are applying. The pawn is rebuilt as a
        // freshly generated member of the template: an inheritable template becomes
        // the germline (children inherit it), a non-inheritable one the xenogene
        // layer over a colour-only germline, exactly as PawnGenerator would lay it
        // down, and the tracker is left at Baseliner + the template's name and icon,
        // where GeneUtility.PawnIsCustomXenotype then matches it by genes.
        //
        // Also the repair for pawns hit by the pre-1.0.3 bug (a trader-sold germ of
        // an inheritable scenario xenotype implanted as xenogenes): picking that
        // template moves the genes into the germline the pawn should have had.
        [DebugAction("Xenogerm Trader Stock", "Reset genes to scenario xenotype", false, false, true, false, false, 0, false,
            actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ResetGenesToScenarioXenotype(Pawn pawn)
        {
            if (pawn.genes == null)
            {
                Messages.Message("Pawn has no genes.", MessageTypeDefOf.RejectInput, historical: false);
                return;
            }

            var customXenotypes = Current.Game?.customXenotypeDatabase?.customXenotypes;
            var options = new List<DebugMenuOption>();
            if (customXenotypes != null)
            {
                foreach (var custom in customXenotypes
                    .Where(c => !c.genes.NullOrEmpty())
                    .OrderBy(c => c.name))
                {
                    string label = custom.name.CapitalizeFirst()
                        + (custom.inheritable ? " (germline, " : " (xenogenes, ")
                        + custom.genes.Count + " genes)";
                    options.Add(new DebugMenuOption(label, DebugMenuOptionMode.Action, delegate
                    {
                        ResetTo(pawn, custom);
                    }));
                }
            }

            if (options.Count == 0)
            {
                Messages.Message("No custom xenotypes in this game.", MessageTypeDefOf.RejectInput,
                    historical: false);
                return;
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }

        private static void ResetTo(Pawn pawn, CustomXenotype custom)
        {
            Pawn_GeneTracker genes = pawn.genes;
            int deathrestCapacity = DeathrestCapacityCarryover.Snapshot(pawn);

            GeneLayerRebuild.Apply(genes,
                custom.inheritable ? custom.genes : null,
                custom.inheritable ? null : custom.genes);

            // The shape vanilla gives a custom identity: Baseliner def, template
            // name and icon; CustomXenotype then resolves by gene match.
            genes.SetXenotypeDirect(XenotypeDefOf.Baseliner);
            genes.xenotypeName = custom.name;
            genes.iconDef = custom.IconDef;

            DeathrestCapacityCarryover.Restore(pawn, deathrestCapacity);

            Messages.Message(pawn.LabelShortCap + ": genes reset to " + custom.name.CapitalizeFirst()
                + (custom.inheritable ? " (germline)." : " (xenogenes)."),
                pawn, MessageTypeDefOf.TaskCompletion, historical: false);
        }

        private static void PlaceAtMouseCell(Thing thing)
        {
            GenPlace.TryPlaceThing(thing, UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near);
        }
    }
}
