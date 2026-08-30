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

            // Gated on the row above: without germline xenogerms in stock there is
            // nothing for it to act on, so it greys out and shows unchecked.
            CheckboxLabeledGated(listing,
                "XTS_ImplantGermlineAsEndogenes".Translate(),
                ref Settings.implantGermlineAsEndogenes,
                "XTS_ImplantGermlineAsEndogenesDesc".Translate("XTS_IncludeInheritable".Translate()),
                enabled: Settings.includeInheritableXenotypes);

            listing.CheckboxLabeled(
                "XTS_IncludePlayerScenario".Translate(),
                ref Settings.includePlayerScenarioXenotypes,
                "XTS_IncludePlayerScenarioDesc".Translate());

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
                XenogermTraderStockSettings.MaxBasePresetValue,
                XenogermTraderStockSettings.StepBasePresetValue);

            Settings.valuePerMetabolism = SliderRow(listing,
                "XTS_ValuePerMetabolism", "XTS_ValuePerMetabolismDesc",
                Settings.valuePerMetabolism,
                XenogermTraderStockSettings.DefaultValuePerMetabolism,
                XenogermTraderStockSettings.MinValuePerMetabolism,
                XenogermTraderStockSettings.MaxValuePerMetabolism,
                XenogermTraderStockSettings.StepValuePerMetabolism);

            Settings.valuePerComplexity = SliderRow(listing,
                "XTS_ValuePerComplexity", "XTS_ValuePerComplexityDesc",
                Settings.valuePerComplexity,
                XenogermTraderStockSettings.DefaultValuePerComplexity,
                XenogermTraderStockSettings.MinValuePerComplexity,
                XenogermTraderStockSettings.MaxValuePerComplexity,
                XenogermTraderStockSettings.StepValuePerComplexity);

            Settings.valuePerArchite = SliderRow(listing,
                "XTS_ValuePerArchite", "XTS_ValuePerArchiteDesc",
                Settings.valuePerArchite,
                XenogermTraderStockSettings.DefaultValuePerArchite,
                XenogermTraderStockSettings.MinValuePerArchite,
                XenogermTraderStockSettings.MaxValuePerArchite,
                XenogermTraderStockSettings.StepValuePerArchite);

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

        // Checkbox whose prerequisite may be off. GUI.enabled alone is not enough
        // for that state: it only fades the visuals, while RimWorld's invisible-
        // button hit test ignores it, so a "greyed" checkbox would still toggle on
        // click. When gated off this draws a genuinely non-interactive checkbox,
        // shown UNCHECKED (the effective state, since the feature can't run) while
        // the stored value stays untouched and reappears once re-enabled.
        private static void CheckboxLabeledGated(Listing_Standard listing, string label, ref bool value,
            string tooltip, bool enabled)
        {
            if (enabled)
            {
                listing.CheckboxLabeled(label, ref value, tooltip);
                return;
            }

            // Mirror Listing_Standard.CheckboxLabeled's rect/tooltip handling, but
            // draw through Widgets' disabled path with a throwaway unchecked state.
            bool prevGuiEnabled = GUI.enabled;
            GUI.enabled = false;
            float height = Text.CalcHeight(label, listing.ColumnWidth);
            Rect rect = listing.GetRect(height);
            if (!tooltip.NullOrEmpty())
            {
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
                TooltipHandler.TipRegion(rect, tooltip);
            }
            bool shownUnchecked = false;
            Widgets.CheckboxLabeled(rect, label, ref shownUnchecked, disabled: true);
            listing.Gap(listing.verticalSpacing);
            GUI.enabled = prevGuiEnabled;
        }

        // House-style slider row: label carries the current value plus a "(default)"
        // suffix while at the shipped default (Mathf.Approximately, not ==, because
        // step-snapping doesn't always reproduce the exact default float), description
        // as hover tooltip. Returns the slider value snapped to step, measured from min;
        // the Step* constants beside each range on the settings class size the notch.
        private static float SliderRow(Listing_Standard listing, string labelKey, string descKey,
            float value, float defaultValue, float min, float max, float step)
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
