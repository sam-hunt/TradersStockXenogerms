using HarmonyLib;
using UnityEngine;
using Verse;

namespace XenogermTraderStock
{
    // Mod entry point. Wires up settings and applies all Harmony patches at
    // startup; PatchAll discovers the classes under Patches/ via their
    // [HarmonyPatch] attributes. The settings window itself is drawn by
    // XenogermTraderStockSettings.DoWindowContents (Core/Settings/ has the
    // sections); this class owns only the lifecycle glue.
    public class XenogermTraderStockMod : Mod
    {
        // Setter is internal so the headless test suite can install a settings instance.
        public static XenogermTraderStockSettings Settings { get; internal set; }

        public XenogermTraderStockMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<XenogermTraderStockSettings>();

            var harmony = new Harmony("shunter.xenogermtraderstock");
            harmony.PatchAll();
            Log.Message("[Xenogerm Trader Stock] Mod loaded.");
        }

        public override string SettingsCategory()
        {
            return "XTS_SettingsCategory".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }
    }
}
