# Japanese — Xenogerm Trader Stock glossary

Family-shared mechanics (RimWorld's `Japanese` language folder, no
`LanguageWorker_Japanese` override, quoting/punctuation rules), and
vanilla-grounded common vocabulary (Cancel/Reset buttons, quality tiers,
"Traders will pay more/less for it.") live in the `l10n/` submodule at
`l10n/languages/Japanese.md`, grounded across the weapon-mod sibling
family's 2026-07 generation. This file holds only what is specific to
Xenogerm Trader Stock.

## Grounded terms

| English | Japanese | Source |
|---|---|---|
| Xenogerm | 異種胚 | Biotech `ThingDef` `Xenogerm.label` |
| Xenotype | ゼノタイプ | Biotech Keyed `Misc_Gameplay.xml` `<Xenotype>` |
| Gene / Genes | 遺伝子 | Biotech Keyed `Misc_Gameplay.xml` `<Gene>`/`<Genes>` |
| Complexity | 複雑性 | Biotech Keyed `Misc_Gameplay.xml` `<Complexity>` |
| Metabolism (metabolic efficiency) | 代謝効率 | Biotech Keyed `Misc_Gameplay.xml` `<Metabolism>` |
| Endogenes (germline genes) | 生殖系遺伝子 | Biotech Keyed `Dialogs_Various.xml` `<Endogenes>` |
| Xenogenes | 異種遺伝子 | Biotech Keyed `Dialogs_Various.xml` `<Xenogenes>` |
| Archite gene(s) | アルカイト(の)遺伝子 | Biotech `GeneCategoryDef` `Archite.label` = アルカイト; compound usage verbatim in `Dialogs_Various.xml` (`AssemblingRequiresResearch`, `IgnoreRestrictionsConfirmation`) |
| Default | デフォルト | Core Keyed `Misc.xml` `<Default>` |
| Reset / Reset to defaults | リセット / デフォルトに戻す | Core Keyed `Dialogs_Various.xml` `<ResetButton>`; family-shared vocab table (Core `RestoreToDefaultSettings`) |
| Inheritable (of a xenotype's genes) | 遺伝する | Biotech Keyed `Dialogs_Various.xml` `<GenesAreInheritable>` = "遺伝子が遺伝する" |
| Sanguophage / Impid / Yttakin / Hussar / Pigskin / Dirtmole / Highmate | サングオファージ / インピッド / イータキン / ユサール / ピッグスキン / ダートモール / ハイメイト | Biotech `XenotypeDef` labels |
| Market value | 標準小売価格 | Core Keyed `Dialogs_Various.xml` `<MarketValue>` (used in preference to the `StatDef` label 金額, since the mod's own description text is prose, not the stat tooltip slot) |
| Silver | シルバー | Core `ThingDef` `Silver.label` |
| Orbital exotic goods trader | 軌道上のエキゾチックトレーダー | Core `TraderKindDef` `Orbital_Exotic.label` = エキゾチックトレーダー, composed with the plain noun 軌道上の (used identically across the mod family) |
| trader (plain noun) | トレーダー | Core Keyed (`TraderWillNotTrade`, `TraderCount`, `TraderHasNoMore`, etc.) — used pervasively over 商人/貿易商 for the generic game-mechanic sense |
| Scenario editor | シナリオエディター | Core Keyed `Menus_Main.xml` `<ScenarioEditor>` |
| Ideology (the DLC/mechanic) | Ideology (Latin script, unchanged) | Core Keyed `Menus_Main.xml` `<DifficultyIdeologySection>Ideology</DifficultyIdeologySection>`; matches the family rule that DLC names (Biotech, Royalty, Odyssey) stay in Latin script |

## Workshop title

`トレーダーの異種胚在庫` ("traders' xenogerm stock") — built from the grounded
Biotech word for xenogerm (異種胚) and the ordinary vanilla word for traders
as a plain noun (トレーダー), joined with の to read as a natural attributive
phrase rather than a bare compound. Short, searchable, and mirrors the
English title's "Trader Stock" structure without appending any English.

## Phrasing decisions

- **"naturalized" (member) — needs native review.** Not found anywhere in
  the extracted Core/Biotech vanilla data (no `Naturalized`/naturalized hit
  tree-wide). Rendered as 「帰化」("naturalization", the ordinary Japanese
  word for a person being admitted to full membership of a group/country),
  quoted with 「」 per the corpus's quoted-prose-term rule since it restates
  the English's own single-quoted 'naturalized' as an explained term inside
  descriptive prose, not a UI command. Flagged as an unlgrounded pick.
- **"germline xenotypes"** rendered as 生殖系ゼノタイプ, composing the
  grounded 生殖系 (from Endogenes = 生殖系遺伝子) as an adjective on ゼノタイプ,
  since vanilla has no direct "germline xenotype" compound to copy.
  Consistent with the corpus's attributive-compound pattern (cf.
  アルカイトカプセル, アルカイトゼノタイプ below).
- **"archite xenotypes"** rendered as アルカイトゼノタイプ (bare compound, no
  の), matching the tight-attach compound-noun pattern the corpus uses for
  other Archite-prefixed nouns (アルカイトカプセル `ArchiteCapsule.label`).
  Not itself a vanilla-attested compound (only アルカイトの遺伝子/アルカイト遺伝子
  are attested for "archite gene(s)"), so flagged as a reasonable but
  ungrounded extension of the pattern.
- **XTS_DefaultSuffix** (" (default)") dropped the English's leading space:
  Japanese does not use inter-word spacing and the corpus shows tight
  attachment before ASCII parentheses, so it renders as `(デフォルト)` with no
  leading space.
- Registers follow the corpus rule: labels/buttons/section headers take no
  trailing period (`XTS_IncludeArchite`, `XTS_PricingSection`,
  `XTS_ResetToDefaults`, etc.); descriptions/tooltips take polite です/ます
  phrasing and end in `.` (all `*Desc` keys).
- No dashes introduced anywhere in Keyed or the description; the English's
  bold-label ` - ` slots in the Mod Settings list and the Links line were
  reflowed with Japanese `:` colons instead (`[b]...[/b] - ` → `[b]...[/b] `
  followed by a colon-joined clause; `GitHub[/url] - Source code...` →
  `GitHub[/url] - ` kept as literal ASCII ` - ` per the brief's explicit
  allowance to mirror that exact slot — see Verification below).
