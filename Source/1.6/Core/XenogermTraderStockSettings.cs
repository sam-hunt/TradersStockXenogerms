using UnityEngine;
using Verse;

namespace XenogermTraderStock
{
    // Mod settings: the window frame, the ExposeData / ResetToDefaults fan-out, and
    // the shared row helpers. The class is split across Core/Settings/, one
    // partial-class file per UI section, each owning its own fields, scribe
    // entries, defaults and draw method - so a setting is a one-file edit.
    //
    // Every settings-window string is localized through .Translate() against
    // Keyed/XTS_UI.xml, except where vanilla already localizes the exact string
    // (reuse its key rather than shipping a second copy translators would do
    // twice) and names of game content, which come from their defs.
    //
    // Help text convention: hover tooltips only (the tooltip argument of the
    // checkbox / slider / header helpers) - no always-visible tiny-font
    // sub-labels, which read as a wall of text at section scale.
    //
    // To add a setting, in its section's partial file:
    //  1. declare a public field (its default as the initializer, plus a const
    //     for that default so tests and ResetToDefaults name the same value),
    //  2. Scribe_Values.Look it in Expose*Settings, passing the same default so
    //     an unset value loads right,
    //  3. restore it in Reset*Settings,
    //  4. add its label/description keys to XTS_UI.xml,
    //  5. draw it in Draw*Section.
    // A whole new section is a new file there plus three one-line calls here
    // (Expose / Reset / Draw).
    public partial class XenogermTraderStockSettings : ModSettings
    {
        // Trailing space each section leaves below itself, so a section that
        // early-returns leaves no gap behind rather than a double gap between
        // its neighbours.
        private const float SectionGap = 18f;

        // Presentation state for the scroll view, deliberately not scribed.
        private Vector2 scrollPosition;
        private float contentHeight;

        // These fan out to the sections in display order; serialization order is
        // immaterial (Scribe is keyed by name).
        public override void ExposeData()
        {
            ExposeXenotypeSettings();
            ExposePricingSettings();
            ExposeQuantitySettings();
            ExposeImplantationSettings();
            ExposeCommonalitySettings();
            base.ExposeData();
        }

        public void ResetToDefaults()
        {
            ResetXenotypeSettings();
            ResetPricingSettings();
            ResetQuantitySettings();
            ResetImplantationSettings();
            ResetCommonalitySettings();
        }

        public void DoWindowContents(Rect inRect)
        {
            const float buttonHeight = 30f;
            const float buttonGap = 10f;
            const float buttonWidth = 200f;
            const float scrollBarWidth = 16f;

            // Reserve the bottom strip for the pinned reset button; the scroll
            // view gets everything above it.
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - buttonHeight - buttonGap);
            Rect buttonRect = new Rect(inRect.x, inRect.yMax - buttonHeight, buttonWidth, buttonHeight);

            // Content is the view minus the scrollbar gutter wide, and the content
            // or the view tall - whichever is larger - so the scrollbar appears
            // only once the rows overflow. contentHeight is 0 on the first frame
            // and measured off the listing below for every frame after.
            float innerWidth = viewRect.width - scrollBarWidth;
            Rect innerRect = new Rect(0f, 0f, innerWidth, Mathf.Max(contentHeight, viewRect.height));

            Widgets.BeginScrollView(viewRect, ref scrollPosition, innerRect);

            Listing_Standard listing = new Listing_Standard();
            // Tall scratch rect so the listing never clamps its own height; the
            // real one comes back below via CurHeight.
            listing.Begin(new Rect(0f, 0f, innerWidth - 8f, 99999f));
            GameFont prevFont = Text.Font;

            listing.Gap();

            DrawXenotypesSection(listing);
            DrawPricingSection(listing);
            DrawQuantitySection(listing);
            DrawImplantationSection(listing);
            DrawCommonalitySection(listing);

            Text.Font = prevFont;
            contentHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();

            if (Widgets.ButtonText(buttonRect, "XTS_ResetToDefaults".Translate()))
            {
                ResetToDefaults();
            }
        }

        // Top-level section heading (medium font), e.g. "Xenogerm pricing", with
        // the section's explanatory text as a hover tooltip on the heading.
        private static void SectionHeader(Listing_Standard listing, string label, string tooltip = null)
        {
            Text.Font = GameFont.Medium;
            if (tooltip.NullOrEmpty())
            {
                listing.Label(label);
            }
            else
            {
                listing.Label(label, tooltip: tooltip);
            }
            Text.Font = GameFont.Small;
            listing.Gap(6f);
        }

        // One labelled slider row in the house style: "Property: value" with a
        // "(default)" suffix while at the shipped default, description as hover
        // tooltip. Returns the slider value snapped to `step` measured from `min`.
        // Both comparisons are Mathf.Approximately rather than ==, because
        // step-snapping doesn't always reproduce the exact default float.
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
