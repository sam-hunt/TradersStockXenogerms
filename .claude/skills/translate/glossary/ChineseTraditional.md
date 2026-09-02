# Xenogerm Trader Stock — Traditional Chinese glossary

Mod-specific coined terms and phrasing decisions for this repo's zh-Hant
translation. Shared mechanics, engine findings, and vanilla-grounded common
vocabulary live in `l10n/languages/ChineseTraditional.md` — check there
first before assuming a term is mod-specific.

## Grounded terms

| English | zh-Hant | Source key/def |
|---|---|---|
| Xenogerm | 異種細胞 | Biotech ThingDef `Xenogerm.label` |
| Xenotype | 異種人 | Biotech Keyed `Xenotype` |
| Gene / Genes | 基因 | Biotech Keyed `Gene`/`Genes` |
| Complexity | 複雜度 | Biotech Keyed `Complexity` |
| Metabolism (metabolic efficiency) | 代謝效率 | Biotech Keyed `Metabolism` |
| Endogenes (germline genes) | 種系基因 | Biotech Keyed `Endogenes` |
| Xenogenes | 異種基因 | Biotech Keyed `Xenogenes` |
| Archite (adjective/noun) | 遠古分子 | Biotech Keyed `ArchitesTotal`, `AnyNonArchite` |
| Archite capsules | 遠古分子膠囊 | Biotech Keyed `ArchitesRequired` |
| Archite genes | 遠古分子基因 | Biotech Keyed `AssemblingRequiresResearch`, `XenotypeBreaksLimits_Archites` |
| Inheritable (genes) | 可遺傳 | Biotech Keyed `GenesAreInheritable` |
| Player-created / custom xenotype | 自訂異種人 | Biotech Keyed `MessageTooManyCustomXenotypes` |
| Scenario editor | 腳本編輯器 | Core Keyed `ScenarioEditor` |
| Default (adj.) | 預設 | Core Keyed `Default` |
| Reset to defaults | 恢復為預設值 | shared vocab table (l10n `ChineseTraditional.md`), Core `RestoreToDefaultSettings` |
| Sanguophage / Impid / Yttakin / Hussar / Pigskin / Dirtmole / Highmate | 噬血者 / 魔人 / 依達人 / 驍騎人 / 豬皮人 / 泥鼴人 / 高魅人 | Biotech `XenotypeDef.label` |
| MarketValue (stat) | 市場價格 | Core StatDef `MarketValue.label` |
| SellPriceFactor (stat) | 售價倍率 | Core StatDef `SellPriceFactor.label` |
| Silver | 白銀 | Core ThingDef `Silver.label` |
| Trader (plain noun) | 商人 | Core Keyed `TraderCount`, `TraderHasNoMore` |
| Orbital exotic goods trader | 軌道外貿商 | Core TraderKindDef `Orbital_Exotic.label` |
| Biotech (DLC brand) | 「生機」 | Core Keyed `SimulateNotOwningBiotech` |
| Ideology / ideoligion | 理念 | shared vocab table (l10n `ChineseTraditional.md`), Ideology `CustomizeIdeoligion` |
| Baseliner (xenotype label) | 一般人類 | Biotech DefInjected `XenotypeDef/Baseliner.label` |
| Deathrest | 絕息 | Biotech Keyed `Deathrest`, GeneDef `Deathrest.label` |
| Deathrest capacity | 絕息容量 | Biotech Keyed `DeathrestCapacity` |
| Deathrest capacity serum | 絕息容量血清 | Biotech Keyed `DeathrestCapacityDesc` ("注射絕息容量血清可以增加絕息容量") |
| Sanguophage reimplant (ability, verb "implant genes") | 移植基因 | Biotech DefInjected `AbilityDef/ReimplantXenogerm.label` |
| Scenario (bare noun, source tag) | 腳本 | shared vocab table (l10n `ChineseTraditional.md`, orbital/trade table: storyteller/scenario/inventory row) |

## Workshop title

`XTS_SettingsCategory` = **商人異種細胞庫存** ("trader xenogerm stock"). Built
from the grounded 商人 (trader, plain noun) + 異種細胞 (xenogerm) + 庫存
(stock/inventory, attested in Core Keyed `TraderNotVisitedYet`'s "庫存不明").
Compound noun-stacking in this order mirrors the English title's word order
and stays short and searchable; a possessive 的 was dropped as unneeded
in a title.

## Phrasing decisions

- **"naturalized" member — NOT grounded.** No vanilla zh-Hant string renders
  this concept (Biotech/Core Keyed and DefInjected both searched, no hit).
  Used 「歸化」 (naturalize/naturalized, standard PRC/Taiwan civics term) in
  corner quotes, mirroring how vanilla quotes other bare technical terms.
  **Needs native review.**
- "archite xenotype" (a xenotype containing archite genes, not a def field)
  has no direct vanilla analogue — composed as 遠古分子異種人 by the same
  attributive pattern vanilla uses for 遠古分子基因 (archite genes) and the
  bare adjectival 遠古分子 in `AnyNonArchite`. Used consistently for
  `XTS_IncludeArchite`/`XTS_IncludeArchiteDesc` and the Workshop description.
- "inheritable xenotype" (germline xenotype, not a per-gene toggle) rendered
  as 可遺傳異種人 (using the grounded 可遺傳 from `GenesAreInheritable`) in
  the Keyed labels, and as 種系異種人 (using the grounded 種系基因 root) in
  running prose describing "germline xenotypes (e.g., Impid, Yttakin)" —
  both read naturally; the Keyed label favors the "inheritable" framing
  since that is literally the toggle's English name, the prose favors the
  "germline" framing since that is the English word actually used there.
- "player-created" rendered 玩家自訂 (using the grounded 自訂 from
  `MessageTooManyCustomXenotypes`'s 自訂異種人) rather than a literal
  "player-made" coinage.
- The English " - " slot after bold `[b]` labels in the Mod Settings list
  and the Links line was rendered as a full-width colon （：）rather than
  mirrored as an ASCII dash: mirroring is permitted by the brief, but this
  mod's tree is small enough that the 4 occurrences alone pushed dash
  density to ~14x the language's measured vanilla baseline (13.35/100k
  chars, comments stripped). The colon reading is also the more natural
  zh-Hant label:value form (see the shared file's "terse label:value
  templates use full-width ：" finding). No other dashes/hyphens appear
  anywhere in the deliverables.
- Parentheses kept ASCII `( )` throughout, including `XTS_DefaultSuffix`
  (" (預設)"), per the shared file's measured 528:0 ASCII-vs-full-width
  finding — the one exception being full-width （）is never used here.
- 2026-09-02 update pass: the settings window dropped the three
  `XTS_Include*` toggles for a per-xenotype sold ledger and grid, added a
  price-breakdown tooltip, quantity/implantation/commonality sections, and
  dropped the "Default: {0}" clause from the four pricing sliders' tooltips.
  Retranslated those four descs without the clause; deleted the three
  removed toggle pairs; added the 42 new keys. Zero dashes introduced (ratio
  0x against the documented 13.35/100k vanilla baseline).
- "Germline xenotypes" (`XTS_FilterInheritable`, a filter-row label whose
  English literally says "Germline", not "Inheritable") rendered as the
  prose framing 種系異種人 rather than the label framing 可遺傳異種人, per
  this file's existing "germline vs inheritable framing" note — English
  itself picked the germline wording for this slot.
- "Xenotypes for sale" (`XTS_XenotypesSection`, section header) rendered as
  出售異種人. Not independently grounded (no vanilla section header matches
  this shape); chosen for brevity and to mirror the existing section-header
  compound-noun style (異種細胞定價, 庫存數量). Flagging for native review.
- "Inverse root" / "Square root" strategy labels (`XTS_StrategySoftInversePrice`,
  `XTS_StrategySqrtPrice`) rendered as 反平方根 / 平方根 — ordinary Chinese
  maths vocabulary, not vanilla-grounded (no vanilla string names either
  concept). Flagging for native review alongside the other strategy labels
  (反比/線性/鐘形曲線/均等), which are likewise plain maths/statistics terms
  with no closer vanilla analogue to ground against.
- 2026-09-02 Workshop description refresh (release rewrite): Starjack grounded
  as 星族 via Odyssey `XenotypeDef/Starjack.label` (tar-extracted from the
  live install's Odyssey Languages pack). Section headers (`About`,
  `Xenogerm Trading`, `Pricing`, `Compatibility`, `FAQ`, `Links`) were kept
  as literal English in the prior pass of this file; reused that convention
  rather than translating them, for internal consistency across the page.
  "Hero art by X" (new Links line) rendered 主視覺美術由X繪製 (no vanilla
  analogue; plain descriptive rendering). "Q:"/"A:" continue to render as
  問：/答： per the prior pass. New pricing figures (1500~1700 / 1700~2000 /
  ~3600 silver) formatted with the file's existing locale convention: comma
  thousands separators, 至 for a two-value range, 約 for a single
  approximate value — not the literal `~` glyph, since the earlier pass had
  already committed to this locale form for the same slot. No new dashes
  introduced.
- Kept the brief's "1 / price" and "√price" literal-symbol forms as
  "1 / 價格" and "√價格" (translating the word, keeping the symbol and the
  ASCII slash's surrounding spaces) rather than converting to a full-width
  reading, since these sit inside otherwise-Chinese sentences as an inline
  math expression.
