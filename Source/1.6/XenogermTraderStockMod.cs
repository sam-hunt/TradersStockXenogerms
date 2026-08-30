using HarmonyLib;
using UnityEngine;
using Verse;

namespace XenogermTraderStock
{
    public class XenogermTraderStockMod : Mod
    {
        // Setter is internal so the headless test suite can install a settings instance.
        public static XenogermTraderStockSettings Settings { get; internal set; }

        private Vector2 settingsScroll;
        private float settingsHeight;

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
                "XTS_IncludeArchite".Translate(),
                ref Settings.includeArchiteXenotypes,
                "XTS_IncludeArchiteDesc".Translate());

            listing.CheckboxLabeled(
                "XTS_IncludeInheritable".Translate(),
                ref Settings.includeInheritableXenotypes,
                "XTS_IncludeInheritableDesc".Translate());

            listing.CheckboxLabeled(
                "XTS_IncludePlayerCreated".Translate(),
                ref Settings.includePlayerCreatedXenotypes,
                "XTS_IncludePlayerCreatedDesc".Translate());

            listing.GapLine(16f);
            listing.Label("XTS_PricingSection".Translate());
            GameFont prevFont = Text.Font;
            Text.Font = GameFont.Tiny;
            listing.Label("XTS_PricingSectionDesc".Translate());
            Text.Font = prevFont;
            listing.Gap(6f);

            Settings.basePresetValue = SliderRow(listing,
                "XTS_BasePresetValue", "XTS_BasePresetValueDesc",
                Settings.basePresetValue,
                XenogermTraderStockSettings.DefaultBasePresetValue,
                XenogermTraderStockSettings.MinBasePresetValue,
                XenogermTraderStockSettings.MaxBasePresetValue);

            Settings.valuePerMetabolism = SliderRow(listing,
                "XTS_ValuePerMetabolism", "XTS_ValuePerMetabolismDesc",
                Settings.valuePerMetabolism,
                XenogermTraderStockSettings.DefaultValuePerMetabolism,
                XenogermTraderStockSettings.MinValuePerMetabolism,
                XenogermTraderStockSettings.MaxValuePerMetabolism);

            Settings.valuePerComplexity = SliderRow(listing,
                "XTS_ValuePerComplexity", "XTS_ValuePerComplexityDesc",
                Settings.valuePerComplexity,
                XenogermTraderStockSettings.DefaultValuePerComplexity,
                XenogermTraderStockSettings.MinValuePerComplexity,
                XenogermTraderStockSettings.MaxValuePerComplexity);

            Settings.valuePerArchite = SliderRow(listing,
                "XTS_ValuePerArchite", "XTS_ValuePerArchiteDesc",
                Settings.valuePerArchite,
                XenogermTraderStockSettings.DefaultValuePerArchite,
                XenogermTraderStockSettings.MinValuePerArchite,
                XenogermTraderStockSettings.MaxValuePerArchite);

            listing.GapLine(16f);
            listing.Label("XTS_XenotypesSection".Translate());
            Text.Font = GameFont.Tiny;
            listing.Label("XTS_XenotypesSectionDesc".Translate());
            Text.Font = prevFont;
            listing.Gap(6f);
            XenotypeGridUI.Draw(listing);

            settingsHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();

            if (Widgets.ButtonText(buttonRect, "XTS_ResetToDefaults".Translate()))
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
                label += "XTS_DefaultSuffix".Translate();
            }
            listing.Label(label, tooltip: descKey.Translate(defaultValue.ToString("F0")));
            return Mathf.Round((listing.Slider(value, min, max) - min) / step) * step + min;
        }
    }
}
