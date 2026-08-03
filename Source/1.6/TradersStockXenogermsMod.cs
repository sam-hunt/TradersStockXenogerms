using HarmonyLib;
using UnityEngine;
using Verse;

namespace TradersStockXenogerms
{
    public class TradersStockXenogermsMod : Mod
    {
        // Setter is internal so the headless test suite can install a settings instance.
        public static TradersStockXenogermsSettings Settings { get; internal set; }

        private Vector2 settingsScroll;
        private float settingsHeight;

        public TradersStockXenogermsMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<TradersStockXenogermsSettings>();

            var harmony = new Harmony("shunter.tradersstockxenogerms");
            harmony.PatchAll();
            Log.Message("[TradersStockXenogerms] Mod loaded.");
        }

        public override string SettingsCategory()
        {
            return "TSX_SettingsCategory".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            const float buttonHeight = 30f;
            const float buttonGap = 10f;
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - buttonHeight - buttonGap);
            Rect buttonRect = new Rect(inRect.x, inRect.yMax - buttonHeight, 200f, buttonHeight);

            // Self-measuring scroll view: innerRect height comes from the previous
            // frame's CurHeight, so a scrollbar only appears once content overflows.
            float innerWidth = viewRect.width - 16f;
            Rect innerRect = new Rect(0f, 0f, innerWidth, Mathf.Max(settingsHeight, viewRect.height));
            Widgets.BeginScrollView(viewRect, ref settingsScroll, innerRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(new Rect(0f, 0f, innerWidth - 8f, 99999f));

            listing.CheckboxLabeled(
                "TSX_IncludeArchite".Translate(),
                ref Settings.includeArchiteXenotypes,
                "TSX_IncludeArchiteDesc".Translate());

            listing.CheckboxLabeled(
                "TSX_IncludeInheritable".Translate(),
                ref Settings.includeInheritableXenotypes,
                "TSX_IncludeInheritableDesc".Translate());

            listing.CheckboxLabeled(
                "TSX_IncludePlayerCreated".Translate(),
                ref Settings.includePlayerCreatedXenotypes,
                "TSX_IncludePlayerCreatedDesc".Translate());

            listing.GapLine(16f);
            listing.Label("TSX_PricingSection".Translate());
            GameFont prevFont = Text.Font;
            Text.Font = GameFont.Tiny;
            listing.Label("TSX_PricingSectionDesc".Translate());
            Text.Font = prevFont;
            listing.Gap(6f);

            Settings.basePresetValue = SliderRow(listing,
                "TSX_BasePresetValue", "TSX_BasePresetValueDesc",
                Settings.basePresetValue,
                TradersStockXenogermsSettings.DefaultBasePresetValue,
                TradersStockXenogermsSettings.MinBasePresetValue,
                TradersStockXenogermsSettings.MaxBasePresetValue);

            Settings.valuePerMetabolism = SliderRow(listing,
                "TSX_ValuePerMetabolism", "TSX_ValuePerMetabolismDesc",
                Settings.valuePerMetabolism,
                TradersStockXenogermsSettings.DefaultValuePerMetabolism,
                TradersStockXenogermsSettings.MinValuePerMetabolism,
                TradersStockXenogermsSettings.MaxValuePerMetabolism);

            Settings.valuePerComplexity = SliderRow(listing,
                "TSX_ValuePerComplexity", "TSX_ValuePerComplexityDesc",
                Settings.valuePerComplexity,
                TradersStockXenogermsSettings.DefaultValuePerComplexity,
                TradersStockXenogermsSettings.MinValuePerComplexity,
                TradersStockXenogermsSettings.MaxValuePerComplexity);

            Settings.valuePerArchite = SliderRow(listing,
                "TSX_ValuePerArchite", "TSX_ValuePerArchiteDesc",
                Settings.valuePerArchite,
                TradersStockXenogermsSettings.DefaultValuePerArchite,
                TradersStockXenogermsSettings.MinValuePerArchite,
                TradersStockXenogermsSettings.MaxValuePerArchite);

            settingsHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();

            if (Widgets.ButtonText(buttonRect, "TSX_ResetToDefaults".Translate()))
            {
                Settings.ResetToDefaults();
            }
        }

        // House-style slider row: label carries the current value plus a "(default)"
        // suffix while at the shipped default (Mathf.Approximately, not ==, because
        // step-snapping doesn't always reproduce the exact default float), description
        // as hover tooltip. Returns the slider value snapped to step, measured from min.
        private static float SliderRow(Listing_Standard listing, string labelKey, string descKey,
            float value, float defaultValue, float min, float max, float step = 1f)
        {
            string label = labelKey.Translate(value.ToString("F0"));
            if (Mathf.Approximately(value, defaultValue))
            {
                label += "TSX_DefaultSuffix".Translate();
            }
            listing.Label(label, tooltip: descKey.Translate(defaultValue.ToString("F0")));
            return Mathf.Round((listing.Slider(value, min, max) - min) / step) * step + min;
        }
    }
}
