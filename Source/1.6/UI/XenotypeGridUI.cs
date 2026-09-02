using System.Collections.Generic;
using RimWorld;
using Verse.Sound;
using UnityEngine;
using Verse;

namespace XenogermTraderStock
{
    // Settings-window grid of every xenotype the generator could sell, four per
    // row so a heavily modded xenotype list stays scannable, headed by one
    // tri-state filter row per xenotype category. Each cell reads and writes
    // the per-xenotype sold ledger directly and is always interactive - click
    // to toggle, or drag to paint one state across many cells, vanilla's
    // checkbox drag-painting; an unsold cell dims its icon and label so the
    // set actually on sale pops from the full roster at a glance.
    public static class XenotypeGridUI
    {
        public const int Columns = 4;
        private const float RowHeight = 28f;
        private const float IconSize = 24f;
        private const float CheckboxSize = 24f;
        private const float Pad = 4f;

        // Tooltip descriptor colours. Archite: the tip-title yellow nudged toward
        // ColorLibrary.Lime, so it reads as "special" without repeating the title.
        // Endogenes: the palette's very light brown, warm like the germline it
        // joins. Xenogenes: a pale cyan to contrast it.
        private static readonly Color ArchiteColor = new Color(0.8f, 0.95f, 0.35f);
        private static readonly Color GermlineColor = ColorLibrary.Beige;
        private static readonly Color XenogeneColor = new Color(0.55f, 0.85f, 0.95f);

        // Accent on the price list's two summary rows - the market-value
        // subtotal the components build to, and the final asking price after
        // vanilla's buying markup: full bright yellow, so both pop from the
        // uncolored component rows.
        private static readonly Color PriceAccentColor = Color.yellow;

        private static XenogermTraderStockSettings Settings => XenogermTraderStockMod.Settings;

        public static void Draw(Listing_Standard listing)
        {
            // Any candidate the ledger has not seen gets its entry before the
            // grid reads it (covers customs created mid-game too).
            XenotypeEligibility.SeedUnseen();

            // Re-enumerated every frame, presets included: an in-process
            // play-data reload (a mid-session language change) replaces every
            // def instance, so a per-process def cache would go stale - see
            // UniqueMeleeWeapons' StaticConstructorOnStartupUtility_CallAll_Patch
            // for the re-run hook the day this needs more than "don't cache".
            // Customs additionally change under us via the in-game editor.
            var cells = new List<Cell>();
            foreach (XenotypeDef def in XenotypeEligibility.CandidateXenotypes())
            {
                cells.Add(Cell.ForPreset(def));
            }
            foreach (CustomXenotype custom in XenotypeEligibility.CandidateCustomXenotypes())
            {
                cells.Add(Cell.ForCustom(custom));
            }

            if (cells.Count == 0)
            {
                listing.Label("XTS_NoXenotypes".Translate());
                return;
            }

            DrawFilterRow(listing, XenotypeEligibility.XenotypeCategory.Archite,
                "XTS_FilterArchite", "XTS_FilterArchiteDesc", cells);
            DrawFilterRow(listing, XenotypeEligibility.XenotypeCategory.Inheritable,
                "XTS_FilterInheritable", "XTS_FilterInheritableDesc", cells);
            DrawFilterRow(listing, XenotypeEligibility.XenotypeCategory.PlayerScenario,
                "XTS_FilterPlayerScenario", "XTS_FilterPlayerScenarioDesc", cells, alwaysShow: true);
            listing.Gap(6f);

            GameFont prevFont = Text.Font;
            TextAnchor prevAnchor = Text.Anchor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;

            for (int i = 0; i < cells.Count; i += Columns)
            {
                Rect row = listing.GetRect(RowHeight);
                float cellWidth = row.width / Columns;
                for (int col = 0; col < Columns && i + col < cells.Count; col++)
                {
                    Rect cellRect = new Rect(row.x + (col * cellWidth), row.y, cellWidth, row.height);
                    DrawCell(cellRect, cells[i + col]);
                }
            }

            Text.Anchor = prevAnchor;
            Text.Font = prevFont;
        }

        // One tri-state bulk-edit row per category: checked = every member
        // sold, unchecked = none, partial = mixed. The row is a pure
        // derivation of the ledger, never separate state, and clicking keeps
        // vanilla CheckboxMulti's cycle (on/partial -> off, off -> on) writing
        // every member's entry. There is no row for Plain xenotypes - they
        // partition into no group worth bulk-editing - and a category with no
        // loaded members draws nothing, UNLESS alwaysShow (PlayerScenario:
        // scenario-dependent, so its absence this game is itself information
        // worth surfacing rather than hiding). Note the categories are
        // DISJOINT (XenotypeEligibility.Categorize picks one per xenotype),
        // so the rows never fight over a cell.
        private static void DrawFilterRow(Listing_Standard listing, XenotypeEligibility.XenotypeCategory category,
            string labelKey, string descKey, List<Cell> cells, bool alwaysShow = false)
        {
            int total = 0;
            int soldCount = 0;
            foreach (Cell cell in cells)
            {
                if (cell.Category != category)
                {
                    continue;
                }
                total++;
                if (cell.Sold)
                {
                    soldCount++;
                }
            }
            if (total == 0 && !alwaysShow)
            {
                return;
            }

            MultiCheckboxState state = total == 0 ? MultiCheckboxState.Off
                : soldCount == total ? MultiCheckboxState.On
                : soldCount == 0 ? MultiCheckboxState.Off
                : MultiCheckboxState.Partial;

            string label = labelKey.Translate();
            float height = Mathf.Max(Text.CalcHeight(label, listing.ColumnWidth - CheckboxSize), CheckboxSize);
            Rect rect = listing.GetRect(height);
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            TooltipHandler.TipRegion(rect, descKey.Translate());

            TextAnchor prevAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect labelRect = new Rect(rect.x, rect.y, rect.width - CheckboxSize, rect.height);
            Widgets.Label(labelRect, label);
            Text.Anchor = prevAnchor;

            Rect checkboxRect = new Rect(rect.xMax - CheckboxSize,
                rect.y + ((rect.height - CheckboxSize) / 2f), CheckboxSize, CheckboxSize);
            // paintable, like the thing-filter tree's category rows: a paint
            // stroke crossing the checkbox snaps the whole category to the
            // stroke's on/off state (the same globals the grid cells share).
            MultiCheckboxState newState = Widgets.CheckboxMulti(checkboxRect, state, paintable: true);
            // The label half of the row is a click target too, the way vanilla
            // CheckboxLabeled spans its whole row: same binary cycle CheckboxMulti
            // applies on a click (on/partial -> off, off -> on), same sounds.
            if (newState == state && Widgets.ButtonInvisible(labelRect))
            {
                newState = state == MultiCheckboxState.Off ? MultiCheckboxState.On : MultiCheckboxState.Off;
                (newState == MultiCheckboxState.On ? SoundDefOf.Checkbox_TurnedOn : SoundDefOf.Checkbox_TurnedOff)
                    .PlayOneShotOnCamera();
            }
            if (newState != state)
            {
                // CheckboxMulti only ever returns On or Off from a click.
                bool sold = newState == MultiCheckboxState.On;
                foreach (Cell cell in cells)
                {
                    if (cell.Category == category)
                    {
                        cell.SetSold(sold);
                    }
                }
            }

            listing.Gap(listing.verticalSpacing);
        }

        private static void DrawCell(Rect rect, Cell cell)
        {
            Rect checkboxRect = new Rect(rect.x + Pad, rect.y + ((rect.height - CheckboxSize) / 2f), CheckboxSize, CheckboxSize);
            Rect iconRect = new Rect(checkboxRect.xMax + Pad, rect.y + ((rect.height - IconSize) / 2f), IconSize, IconSize);
            Rect labelRect = new Rect(iconRect.xMax + Pad, rect.y, rect.xMax - iconRect.xMax - (2f * Pad), rect.height);

            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                TooltipHandler.TipRegion(rect, new TipSignal(() => BuildTooltip(cell), cell.TooltipId));
            }

            // The whole cell is one click-and-paint target, the shape vanilla
            // CheckboxLabeled(paintable: true) takes: a press toggles, a drag
            // paints the first toggle's state across every cell the stroke
            // crosses (the trade dialog's checkbox-column behaviour - vanilla
            // hangs the painted state off the cursor and ends the stroke on
            // mouse-up in WidgetsOnGUI). ToggleInvisibleDraggable owns the
            // turned-on/off sounds, so no manual click sound here.
            bool sold = cell.Sold;
            bool checkOn = sold;
            Widgets.ToggleInvisibleDraggable(rect, ref checkOn, doMouseoverSound: true, paintable: true);
            if (checkOn != sold)
            {
                cell.SetSold(checkOn);
            }
            Widgets.CheckboxDraw(checkboxRect.x, checkboxRect.y, checkOn, disabled: false, CheckboxSize);

            // Dim by the post-click state so the cell follows the click in the
            // same frame.
            Color prevColor = GUI.color;
            GUI.color = checkOn ? XenotypeDef.IconColor : XenotypeDef.IconColor * Widgets.InactiveColor;
            GUI.DrawTexture(iconRect, cell.Icon);
            GUI.color = checkOn ? prevColor : Widgets.InactiveColor;
            Widgets.Label(labelRect, cell.Label().Truncate(labelRect.width));
            GUI.color = prevColor;
        }

        private static string BuildTooltip(Cell cell)
        {
            // Mirrors the pawn bio's xenotype hover: coloured title, short
            // description, subtle grey footnotes.
            var breakdown = XenogermPricing.Calculate(cell.Genes);
            float marketValue = XenogermPricing.BaseXenogermValue + breakdown.Premium;
            float buyPrice = marketValue * XenogermPricing.VanillaBuyMarkup;
            var settings = Settings;

            // Title carries the parenthesized price, same shape as the cell text.
            string text = (cell.LabelCap + " (" + buyPrice.ToStringMoney() + ")")
                .Colorize(ColoredText.TipSectionTitleColor)
                + "\n\n" + cell.Description;

            // One coloured line per category the xenotype falls into, so a
            // glance at the hover says why it prices and seeds the way it does.
            string descriptors = BuildDescriptors(cell);
            if (!descriptors.NullOrEmpty())
            {
                text += "\n\n" + descriptors;
            }

            // Formula transparency: one line per pricing component, its silver
            // amount hung off a right-aligned column. The list closes with the
            // market-value subtotal, vanilla's buying markup (shown as the
            // silver it adds, "+40%", so every uncolored row stays additive
            // and only the yellow rows total) and the final asking price,
            // worded with the same vanilla keys the trade dialog's own price
            // tooltip uses (MarketValue stat label,
            // "Buying", "FinalPrice") so the two hovers visibly agree - and
            // needing no XTS strings of their own. The two summary rows are
            // the accents, in bright yellow, colorized after alignment so
            // padding is measured on plain text. Component rows stay entirely
            // uncolored, which rules out Resolve(): its CurrencyRegex
            // gold-tints every $ amount. Each Translate is ToString()d
            // (RawText, tags kept) rather than left to ride the + chain as a
            // bare TaggedString, whose implicit string conversion is
            // RawText.StripTags() - one leak decolorizes the entire tooltip.
            var priceRows = new List<(string label, string money, bool accent)>
            {
                ("XTS_PriceBase".Translate().ToString(),
                    XenogermPricing.BaseXenogermValue.ToStringMoney(), false),
                ("XTS_PricePreset".Translate().ToString(),
                    settings.basePresetValue.ToStringMoney(), false),
                ("XTS_PriceMetabolism".Translate(
                        breakdown.AbsoluteMetabolism, settings.valuePerMetabolism.ToString("F0")).ToString(),
                    (breakdown.AbsoluteMetabolism * settings.valuePerMetabolism).ToStringMoney(), false),
                ("XTS_PriceComplexity".Translate(
                        breakdown.Complexity, settings.valuePerComplexity.ToString("F0")).ToString(),
                    (breakdown.Complexity * settings.valuePerComplexity).ToStringMoney(), false),
                ("XTS_PriceArchite".Translate(
                        breakdown.Archites, settings.valuePerArchite.ToString("F0")).ToString(),
                    (breakdown.Archites * settings.valuePerArchite).ToStringMoney(), false),
                (StatDefOf.MarketValue.LabelCap.ToString(), marketValue.ToStringMoney(), true),
                ("+" + (XenogermPricing.VanillaBuyMarkup - 1f).ToStringPercent()
                        + " (" + "Buying".Translate().ToString() + ")",
                    (buyPrice - marketValue).ToStringMoney(), false),
                ("FinalPrice".Translate().ToString(), buyPrice.ToStringMoney(), true),
            };
            List<string> priceLines = AlignPriceColumn(priceRows.ConvertAll(row => (row.label, row.money)));
            for (int i = 0; i < priceLines.Count; i++)
            {
                if (priceRows[i].accent)
                {
                    priceLines[i] = priceLines[i].Colorize(PriceAccentColor);
                }
            }
            text += "\n\n" + string.Join("\n", priceLines);

            // Last line: where the xenotype comes from, as the info card's Source
            // row shows it (vanilla's Stat_Source_Label + the content pack name).
            // Player-scenario xenotypes have no pack, so they get a keyed
            // "Scenario" word instead, keeping the line on every cell.
            string source = cell.SourceName;
            if (!source.NullOrEmpty())
            {
                text += "\n\n" + ("Stat_Source_Label".Translate() + ": " + source)
                    .Colorize(ColoredText.SubtleGrayColor);
            }
            return text;
        }

        private static string BuildDescriptors(Cell cell)
        {
            var lines = new List<string>(3);
            if (cell.Archite)
            {
                lines.Add("XTS_XenotypeArchite".Translate().Colorize(ArchiteColor));
            }
            // Every xenotype's genes land on exactly one side of the germline:
            // an inheritable xenotype implants endogenes, any other xenogenes.
            // Baseliner's xenogerm has no genes to land anywhere - it wipes
            // instead of implanting - so it gets neither line.
            if (!cell.Genes.NullOrEmpty())
            {
                lines.Add(cell.Inheritable
                    ? "XTS_XenotypeEndogenes".Translate().Colorize(GermlineColor)
                    : "XTS_XenotypeXenogenes".Translate().Colorize(XenogeneColor));
            }
            return string.Join("\n", lines);
        }

        // Vanilla tooltips are a single rich-text label and IMGUI markup has no
        // alignment or tab stops, so the money column is right-aligned the only
        // way text allows: space padding. The column sits on the tooltip's own
        // right edge rather than the widest price row, and each candidate line
        // is measured COMPOSED - fragment widths summed separately drift from
        // the renderer's layout of the whole line, which showed up as visibly
        // ragged edges. Residual quantization is under one space width.
        // ActiveTip.DrawTooltip sets GameFont.Small before resolving the tip
        // text, so CalcSize here measures the font the tooltip draws with; the
        // font is still pinned locally because BuildTooltip's other callers
        // (none today) owe no such guarantee.
        private static List<string> AlignPriceColumn(List<(string label, string money)> rows)
        {
            // ActiveTip.TipRect wraps text wider than 260px, and the 4px box
            // padding rides outside that (ContractedBy(-4), undone at draw
            // time) - so 260 is the true content width the description wraps
            // to, and a line padded up to it lands flush without wrapping.
            const float tipContentWidth = 260f;
            const int minGapSpaces = 2;

            GameFont prevFont = Text.Font;
            Text.Font = GameFont.Small;

            float spaceWidth = Text.CalcSize("$ $").x - Text.CalcSize("$$").x;
            var lines = new List<string>(rows.Count);
            foreach ((string label, string money) in rows)
            {
                // First guess from fragment widths, then settle on the widest
                // composed line that still fits the content width.
                int spaces = Mathf.Max(minGapSpaces, Mathf.FloorToInt(
                    (tipContentWidth - Text.CalcSize(label).x - Text.CalcSize(money).x) / spaceWidth));
                while (spaces > minGapSpaces
                    && Text.CalcSize(label + new string(' ', spaces) + money).x > tipContentWidth)
                {
                    spaces--;
                }
                while (Text.CalcSize(label + new string(' ', spaces + 1) + money).x <= tipContentWidth)
                {
                    spaces++;
                }
                lines.Add(label + new string(' ', spaces) + money);
            }

            Text.Font = prevFont;
            return lines;
        }

        // Preset and player-scenario xenotypes flattened to what a cell draws, so
        // the grid loop doesn't branch on the source type.
        private readonly struct Cell
        {
            private readonly XenotypeDef def;
            private readonly CustomXenotype custom;

            private Cell(XenotypeDef def, CustomXenotype custom)
            {
                this.def = def;
                this.custom = custom;
            }

            public static Cell ForPreset(XenotypeDef def) => new Cell(def, null);
            public static Cell ForCustom(CustomXenotype custom) => new Cell(null, custom);

            public List<GeneDef> Genes => def != null ? def.genes : custom.genes;
            public bool Archite => def != null ? def.Archite : XenotypeEligibility.IsArchite(custom);
            public bool Inheritable => def != null ? def.inheritable : custom.inheritable;
            public string SourceName => def != null
                ? def.modContentPack?.Name
                : "XTS_XenotypeSourceScenario".Translate().ToString();
            public Texture2D Icon => def != null ? def.Icon : custom.IconDef.Icon;
            public string LabelCap => def != null ? def.LabelCap : custom.name.CapitalizeFirst();
            public int TooltipId => def != null ? def.GetHashCode() : custom.name.GetHashCode();

            public string Description => def != null
                ? (def.descriptionShort.NullOrEmpty() ? def.description : def.descriptionShort)
                : "XTS_CustomXenotypeDesc".Translate().ToString();

            public XenotypeEligibility.XenotypeCategory Category => def != null
                ? XenotypeEligibility.Categorize(def)
                : XenotypeEligibility.Categorize(custom);

            // The ledger is seeded before the grid reads it (Draw's first
            // call), so the null fallback is never the shown state in practice.
            public bool Sold => (def != null
                ? Settings.GetXenotypeSold(def.defName)
                : Settings.GetCustomXenotypeSold(custom.name)) ?? false;

            public void SetSold(bool sold)
            {
                if (def != null)
                {
                    Settings.SetXenotypeSold(def.defName, sold);
                }
                else
                {
                    Settings.SetCustomXenotypeSold(custom.name, sold);
                }
            }

            // Price is recomputed per frame from the live slider values so the
            // grid tracks the pricing sliders above it as they move. It is the
            // shelf price (market value x vanilla's flat buying markup), not
            // raw market value: in a trader-stock context players read this
            // number as what they will pay, and the markup part is constant -
            // the trade session's own modifiers (negotiator, difficulty) only
            // ever discount it.
            public string Label()
            {
                return LabelCap + " (" + XenogermPricing.EstimateBuyPrice(Genes).ToStringMoney() + ")";
            }
        }
    }
}
