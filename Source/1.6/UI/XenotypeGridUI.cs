using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse.Sound;
using UnityEngine;
using Verse;

namespace XenogermTraderStock
{
    // Settings-window grid of every xenotype the generator could sell, four per
    // row so a heavily modded xenotype list stays scannable. Each cell shows the
    // derived sellable state (XenotypeEligibility), not the raw blacklist entry:
    // a xenotype suppressed by a category toggle draws greyed and unchecked, and
    // its own toggle is inert until the category comes back, at which point the
    // stored per-xenotype choice reappears untouched.
    public static class XenotypeGridUI
    {
        public const int Columns = 4;
        private const float RowHeight = 28f;
        private const float IconSize = 24f;
        private const float CheckboxSize = 24f;
        private const float Pad = 4f;

        private static List<XenotypeDef> cachedPresets;

        // Tooltip descriptor colours. Archite: the tip-title yellow nudged toward
        // ColorLibrary.Lime, so it reads as "special" without repeating the title.
        // Germline: the palette's very light brown. Player-scenario: the player
        // faction's own colour, straight from its def so it tracks any retint.
        private static readonly Color ArchiteColor = new Color(0.8f, 0.95f, 0.35f);
        private static readonly Color GermlineColor = ColorLibrary.Beige;
        private static Color PlayerScenarioColor => FactionDefOf.PlayerColony.DefaultColor;

        private static XenogermTraderStockSettings Settings => XenogermTraderStockMod.Settings;

        public static void Draw(Listing_Standard listing)
        {
            // Defs are fixed after load; custom xenotypes can change under us
            // (in-game xenotype editor), so those are re-read every frame.
            cachedPresets ??= XenotypeEligibility.CandidateXenotypes().ToList();

            var cells = new List<Cell>(cachedPresets.Count);
            foreach (XenotypeDef def in cachedPresets)
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

        private static void DrawCell(Rect rect, Cell cell)
        {
            var block = cell.Block;
            bool live = block == XenotypeEligibility.CategoryBlock.None;
            // Derived state: a category block always presents as "not sold".
            bool sellable = live && !cell.Excluded;

            Rect checkboxRect = new Rect(rect.x + Pad, rect.y + ((rect.height - CheckboxSize) / 2f), CheckboxSize, CheckboxSize);
            Rect iconRect = new Rect(checkboxRect.xMax + Pad, rect.y + ((rect.height - IconSize) / 2f), IconSize, IconSize);
            Rect labelRect = new Rect(iconRect.xMax + Pad, rect.y, rect.xMax - iconRect.xMax - (2f * Pad), rect.height);

            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                // Tooltip whether or not the cell is live - the description and
                // the reason it's greyed out are exactly what a blocked cell needs.
                TooltipHandler.TipRegion(rect, new TipSignal(() => BuildTooltip(cell), cell.TooltipId));
            }

            if (live)
            {
                bool checkOn = sellable;
                Widgets.Checkbox(checkboxRect.position, ref checkOn, CheckboxSize);
                if (Widgets.ButtonInvisible(new Rect(iconRect.x, rect.y, rect.xMax - iconRect.x, rect.height)))
                {
                    checkOn = !checkOn;
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
                if (checkOn != sellable)
                {
                    cell.SetExcluded(!checkOn);
                }
            }
            else
            {
                Widgets.CheckboxDraw(checkboxRect.x, checkboxRect.y, active: false, disabled: true, CheckboxSize);
            }

            Color prevColor = GUI.color;
            GUI.color = live ? XenotypeDef.IconColor : XenotypeDef.IconColor * Widgets.InactiveColor;
            GUI.DrawTexture(iconRect, cell.Icon);
            GUI.color = live ? prevColor : Widgets.InactiveColor;
            Widgets.Label(labelRect, cell.Label().Truncate(labelRect.width));
            GUI.color = prevColor;
        }

        private static string BuildTooltip(Cell cell)
        {
            // Mirrors the pawn bio's xenotype hover: coloured title, short
            // description, subtle grey footnotes.
            var breakdown = XenogermPricing.Calculate(cell.Genes);
            float price = XenogermPricing.BaseXenogermValue + breakdown.Premium;
            var settings = Settings;

            string text = cell.LabelCap.Colorize(ColoredText.TipSectionTitleColor)
                + "\n\n" + cell.Description;

            // One coloured line per category the xenotype falls into, so a
            // glance at the hover says why it prices and gates the way it does.
            string descriptors = BuildDescriptors(cell);
            if (!descriptors.NullOrEmpty())
            {
                text += "\n\n" + descriptors;
            }

            text += "\n\n" + "XTS_XenotypePriceBreakdown".Translate(
                    price.ToStringMoney(),
                    XenogermPricing.BaseXenogermValue.ToString("F0"),
                    settings.basePresetValue.ToString("F0"),
                    breakdown.AbsoluteMetabolism, settings.valuePerMetabolism.ToString("F0"),
                    breakdown.Complexity, settings.valuePerComplexity.ToString("F0"),
                    breakdown.Archites, settings.valuePerArchite.ToString("F0"))
                    .Colorize(ColoredText.SubtleGrayColor);

            // The reason a cell is greyed out is the one line a blocked cell's
            // hover exists for, so it goes in red rather than footnote grey.
            string blockKey = BlockSettingKey(cell.Block);
            if (blockKey != null)
            {
                text += "\n\n" + "XTS_XenotypeBlockedBy".Translate(blockKey.Translate())
                    .Colorize(ColorLibrary.RedReadable);
            }

            // Last line: where the xenotype comes from, as the info card's Source
            // row shows it (vanilla's Stat_Source_Label + the content pack name).
            // Player-scenario xenotypes have no pack; their cyan descriptor above
            // already says where they come from.
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
            if (cell.Inheritable)
            {
                // Vanilla's own line from the xenotype info card (Biotech Keyed),
                // so it is already localized everywhere.
                lines.Add("GenesAreInheritable".Translate().Colorize(GermlineColor));
            }
            if (cell.IsPlayerScenario)
            {
                lines.Add("XTS_XenotypePlayerScenario".Translate().Colorize(PlayerScenarioColor));
            }
            return string.Join("\n", lines);
        }

        private static string BlockSettingKey(XenotypeEligibility.CategoryBlock block)
        {
            switch (block)
            {
                case XenotypeEligibility.CategoryBlock.Archite: return "XTS_IncludeArchite";
                case XenotypeEligibility.CategoryBlock.Inheritable: return "XTS_IncludeInheritable";
                case XenotypeEligibility.CategoryBlock.PlayerScenario: return "XTS_IncludePlayerScenario";
                default: return null;
            }
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
            public bool IsPlayerScenario => def == null;
            public string SourceName => def?.modContentPack?.Name;
            public Texture2D Icon => def != null ? def.Icon : custom.IconDef.Icon;
            public string LabelCap => def != null ? def.LabelCap : custom.name.CapitalizeFirst();
            public int TooltipId => def != null ? def.GetHashCode() : custom.name.GetHashCode();

            public string Description => def != null
                ? (def.descriptionShort.NullOrEmpty() ? def.description : def.descriptionShort)
                : "XTS_CustomXenotypeDesc".Translate().ToString();

            public XenotypeEligibility.CategoryBlock Block => def != null
                ? XenotypeEligibility.GetCategoryBlock(def)
                : XenotypeEligibility.GetCategoryBlock(custom);

            public bool Excluded => def != null
                ? Settings.IsXenotypeExcluded(def.defName)
                : Settings.IsCustomXenotypeExcluded(custom.name);

            public void SetExcluded(bool excluded)
            {
                if (def != null)
                {
                    Settings.SetXenotypeExcluded(def.defName, excluded);
                }
                else
                {
                    Settings.SetCustomXenotypeExcluded(custom.name, excluded);
                }
            }

            // Price is recomputed per frame from the live slider values so the
            // grid tracks the pricing sliders above it as they move.
            public string Label()
            {
                return LabelCap + " (" + XenogermPricing.EstimateMarketValue(Genes).ToStringMoney() + ")";
            }
        }
    }
}
