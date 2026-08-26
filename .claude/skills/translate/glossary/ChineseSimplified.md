# Simplified Chinese — Xenogerm Trader Stock glossary

Family-shared mechanics (RimWorld's `ChineseSimplified` language folder,
full-width punctuation, curly-quote conventions), and vanilla-grounded
common vocabulary (quality tiers, "Traders will pay more for it.") live in
the `l10n/` submodule at `l10n/languages/ChineseSimplified.md`, grounded
across the weapon-mod sibling family's 2026-07 generation. This file holds
only what is specific to Xenogerm Trader Stock.

## Grounded terms

| English | Simplified Chinese | Source |
|---|---|---|
| Xenogerm | 异种胚芽 | Biotech Keyed `Xenogerm`; DefInjected `ThingDef Xenogerm.label` |
| Xenotype | 异种人 | Biotech Keyed `Xenotype` |
| Gene(s) | 基因 | Biotech Keyed `Gene`/`Genes` |
| Complexity | 复杂度 | Biotech Keyed `Complexity` |
| Metabolism | 代谢率 | Biotech Keyed `Metabolism` |
| Endogenes (germline genes) | 系谱基因 | Biotech Keyed `Endogenes` |
| Xenogenes | 异种基因 | Biotech Keyed `Xenogenes` |
| Archite (category/adjective) | 超凡 | Biotech `GeneCategoryDef Archite.label` |
| archite gene(s) | 超凡基因 | derived from `Archite.label`=超凡 + `Gene`=基因 (no single vanilla string states this compound; consistent with `ArchiteMetabolism`=超凡代谢 pattern) |
| Archites (the microscopic devices) | 超凡微械 | Biotech Keyed `ArchitesTotal`/`ArchitesRequiredDesc` |
| Sanguophage | 赫血种 | Biotech `XenotypeDef Sanguophage.label` |
| Impid | 炎魔种 | Biotech `XenotypeDef Impid.label` |
| Yttakin | 毛绒种 | Biotech `XenotypeDef Yttakin.label` |
| Hussar | 骠骑种 | Biotech `XenotypeDef Hussar.label` |
| Pigskin | 猪猡种 | Biotech `XenotypeDef Pigskin.label` |
| Dirtmole | 土鼹种 | Biotech `XenotypeDef Dirtmole.label` |
| Highmate | 优侣种 | Biotech `XenotypeDef Highmate.label` |
| MarketValue | 市场价值 | Core `StatDef MarketValue.label`, Keyed `MarketValue` |
| SellPriceFactor | 出售价格乘数 | Core `StatDef SellPriceFactor.label` |
| Silver | 白银 | Core `ThingDef Silver.label` |
| orbital exotic goods trader (`Orbital_Exotic`) | 稀有品贸易商 (轨道-) | Core `TraderKindDef Orbital_Exotic.label` |
| trader (plain noun) | 贸易商 | Core Keyed `TraderHasNoMore`, `TraderWillNotTrade`, etc. |
| Reset to defaults / restore default settings | 还原默认设置 | Core Keyed `RestoreToDefaultSettings` |
| Default | 默认 | Core Keyed `Default` |

## Workshop title

`XTS_SettingsCategory` = **贸易商异种胚芽库存** ("traders' xenogerm stock"),
built from the grounded 贸易商 (trader) and 异种胚芽 (xenogerm) plus 库存
(stock/inventory) to render the mod's "xenogerms in traders' stock" sense.
Short, searchable on either keyword, no English brand appended.

## Phrasing decisions

- **"archite gene(s)"** has no single vanilla-attested compound string;
  built by composing `GeneCategoryDef Archite.label`=超凡 with `Gene`=基因,
  following the same compounding pattern vanilla itself uses for
  `ArchiteMetabolism.label`=超凡代谢. Flagged for native review, though the
  compositional grounding is solid.
- **"naturalized" (member)** — not found anywhere in the extracted Core or
  Biotech Simplified Chinese data (Ideology-adjacent concept, but the
  literal word never appears verbatim in either DLC's zh corpus). Rendered
  as 归化 (the ordinary Chinese word for naturalized/citizenship-by-adoption),
  set in curly quotes "归化" per the family's citation-quoting convention.
  **Needs native review.**
- **"ideology" (recognition)** — rendered as 文化 (bare noun, ideoligion),
  matching the family's Ideology DLC grounding (`ButtonShowAllIdeoligions`
  etc. use 文化); no dedicated Ideology vanilla data was pulled for this
  pass since the mod only references the concept in passing prose.
- **"germline xenotypes"** in `XTS_IncludeInheritableDesc` is rendered as
  系谱异种人 (from `Endogenes`=系谱基因) rather than repeating 可遗传, to
  distinguish the parenthetical restatement from the setting's own label
  (可遗传异种人) while keeping both terms transparently linked.
- Colon usage follows the family style file: ASCII `: ` in label/value
  templates (`XTS_BasePresetValue`=预设基础价值: {0}, all `Default: {0}`
  descriptions), full-width ： nowhere in this mod's terse templates, and
  plain prose sentences in the Workshop description use no colon except
  where mirroring the English's own `Q:`/`A:` and bulleted `[b]Label[/b] - `
  slots (kept as literal ASCII ` - ` per the brief's exception).
- No new dashes introduced beyond the two English-mirrored ` - ` slots
  (Mod Settings list, Links line) that the brief explicitly permits.
