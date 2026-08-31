using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using RimWorld;
using Verse;

namespace XenogermTraderStock
{
    // Dev-mode debug action: spawn a trader-style xenogerm for any xenotype at
    // the clicked cell. Deliberately bypasses XenotypeEligibility so xenogerms
    // that stock generation would never produce - category toggles, per-xenotype
    // blacklist, GeneExtension opt-outs like VREA androids - can still be put in
    // hand for testing; entries eligibility currently filters out carry vanilla's
    // " [NO]" suffix but spawn anyway. Only Baseliner and gene-less xenotypes are
    // skipped: an empty gene set cannot form a meaningful xenogerm item.
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

            // Presets in the same order as the settings grid: vanilla display
            // priority, then label for same-priority modded xenotypes.
            foreach (var xenotype in DefDatabase<XenotypeDef>.AllDefsListForReading
                .Where(x => x != XenotypeDefOf.Baseliner && !x.genes.NullOrEmpty())
                .OrderByDescending(x => x.displayPriority)
                .ThenBy(x => x.LabelCap.ToString()))
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

        private static void PlaceAtMouseCell(Thing thing)
        {
            GenPlace.TryPlaceThing(thing, UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near);
        }
    }
}
