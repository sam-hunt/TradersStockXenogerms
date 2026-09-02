# Brazilian Portuguese — Xenogerm Trader Stock glossary

Family-shared mechanics (mandatory but unsupported contractions,
`(a)`-hedged gender, `[X_possessive]` unusability), style/corpus rules, and
vanilla-grounded common vocabulary (Cancel/Reset buttons, quality tiers,
"Traders will pay more/less for it.") live in the `l10n/` submodule at
`l10n/languages/PortugueseBrazilian.md`, grounded across the weapon-mod
sibling family's 2026-07 generation. This file holds only what is specific
to Xenogerm Trader Stock.

## Grounded terms

| English | vanilla pt-BR | Source key/def |
|---|---|---|
| Xenogerm | xenogerminador | Biotech `Keyed/Misc_Gameplay.xml` `<Xenogerm>`; `ThingDef` `Xenogerm.label` |
| Xenotype | Xenótipo | Biotech `Keyed/Misc_Gameplay.xml` `<Xenotype>` |
| Gene / Genes | gene / genes | Biotech `Keyed/Misc_Gameplay.xml` `<Gene>`/`<Genes>` |
| Complexity | Complexidade | Biotech `Keyed/Misc_Gameplay.xml` `<Complexity>` |
| Metabolism (metabolic efficiency) | eficiência metabólica | Biotech `Keyed/Misc_Gameplay.xml` `<Metabolism>` |
| Endogenes (germline genes) | genes de linha germinativa | Biotech `Keyed/Dialogs_Various.xml` `<Endogenes>` |
| Xenogenes | xenogenes | Biotech `Keyed/Dialogs_Various.xml` `<Xenogenes>` |
| Archite (adj/noun) | arquita | Biotech `GeneCategoryDef` `Archite.label`; `GeneDef` `ArchiteMetabolism.label` = "metabolismo arquita" |
| Inheritable | hereditário | Biotech `Keyed/Dialogs_Various.xml` `<GenesAreInheritable>` = "Os genes são hereditários" |
| Sanguophage / Impid / Yttakin / Hussar / Pigskin / Dirtmole / Highmate | sanguófago / diabrete / yttakin / hussardo / pele de porco / cavaterra / alto companheiro | Biotech `XenotypeDef/XenotypeDefs.xml` |
| MarketValue (StatDef sense) | valor de mercado | Core `StatDef/Stats_Basics_General.xml` `MarketValue.label` |
| MarketValue (Keyed sense) | Preço base | Core `Keyed/Dialogs_Various.xml` `<MarketValue>` — not used here; the mod's own text is about the StatDef/formula sense, so "valor de mercado" was used throughout |
| SellPriceFactor | multiplicador do preço de venda | Core `StatDef/Stats_Basics_General.xml` `SellPriceFactor.label` |
| Silver | prata | Core `ThingDef/Items_Resource_Stuff.xml` `Silver.label` |
| Orbital exotic goods trader(s) | comerciantes de produtos exóticos | Core `TraderKindDef/TraderKinds_Orbital_Misc.xml` `Orbital_Exotic.label` |
| Default | Padrão | Core `Keyed/Misc.xml` `<Default>` |
| Scenario editor | editor de cenário | Core `Keyed/Menus_Main.xml` `<ScenarioEditor>` = "Editor de Cenário" (Title Case as a menu name; lowercased here as an ordinary noun phrase inside prose, per pt-BR's per-def-type casing convention) |
| Ideology | ideologia | Core `Keyed/Dialog_StatsReports.xml` `<StatsReport_Ideoligion>` |
| Scenario (short tag) | cenário | Core `Keyed/Menus_Main.xml` `<ScenariosCustom>`/`<ScenarioEditor>` family, lowercased as an ordinary noun outside the menu-name context |
| Custom xenotype | xenótipo personalizado | Biotech `Keyed/...` `<MessageTooManyCustomXenotypes>` = "xenótipos personalizados" |
| Deathrest (gene) | descanso mortal | Biotech `DefInjected/GeneDef/*` `Deathrest.label` |
| Deathrest capacity | capacidade do descanso mortal | Biotech `DefInjected/ThingDef/*` `DeathrestCasket.description` ("...depende da capacidade do descanso mortal da pessoa...") |
| Deathrest capacity serum | soro de capacidade do descanso mortal | Biotech `DefInjected/ThingDef/*` `DeathrestCapacitySerum.label` |
| Sanguophage reimplant (ability) | reimplante (verb: implantar) | Biotech `DefInjected/AbilityDef/*` `ReimplantXenogerm.label` = "implantar genes"; its description uses the verb "implantar" for the act, so "reimplante" is a natural derived noun rather than a literal vanilla label |
| Baseliner (as a name, not the adjective sense) | Padrão | Biotech `DefInjected/XenotypeDef/*` `Baseliner.label` = "padrão" (vanilla lowercases it as a descriptive adjective; capitalized here to match the Title Case treatment given to other xenotype names like Diabrete, Yttakin) |
| Pawn (generic, in mod prose) | colono | Core `Keyed` usage throughout (`ColonistNeedsRescue`, `BreakRiskMinorDesc`, etc.) consistently renders "pawn"-in-context as "colono" |

## Workshop title

`XTS_SettingsCategory` = **Estoque de Xenogerminadores de Comerciantes**
("stock of traders' xenogerms"). Built directly from the grounded Biotech
noun `xenogerminador` and the ordinary word for traders, `comerciantes`
(itself grounded via the Core `Orbital_Exotic` trader-kind label), mirroring
the English title's sense of "xenogerms in traders' stock" without
appending any English brand text.

## Phrasing decisions

- **"naturalized" (member) is ungrounded** — no `naturaliz*` string exists
  anywhere in the extracted Biotech Keyed/DefInjected data. Rendered as the
  literal cognate `"naturalizados"` (quoted, matching the English's quoted
  `'naturalized'`, per the family's ASCII-straight-quote rule). **Needs
  native review.**
- `<Default>`=`Padrão` (Core) is a singular noun; `XTS_ResetToDefaults` uses
  the natural plural `Restaurar padrões` since English "defaults" is plural
  and Core's own `ResetButton`=`Restaurar` supplied the verb.
- No dashes were needed anywhere in this pass — reflowed with commas/colons
  per the family's zero-dash pt-BR rule; the description's `[b]label[/b] - `
  slots mirror the English's own ASCII ` - ` exactly, per the brief's
  allowance.
- FAQ answers use `R:` (Resposta) consistently for both questions, matching
  question labels `P:` (Pergunta) — no vanilla precedent for this Q&A shape
  exists in the mod-domain data, so this is an editorial choice, not a
  grounded term.
- **2026-09-02 update pass:** added 42 new keys (implantation, quantity,
  commonality-strategy and price-breakdown sections) and retranslated the 5
  pricing-slider tooltips whose English dropped the trailing "Default: {0}"
  clause; removed the three retired Include* toggle keys. "Vanilla" (as in
  "Vanilla recreates the deathrest gene...") is ungrounded as a standalone
  term; rendered as "o jogo base" (the base game), a natural and unambiguous
  phrase, not a literal vanilla string. **Needs native review.**
  `XTS_ImplantGermlineAsEndogenes` reads as literally repetitive
  ("xenótipos de linha germinativa como genes de linha germinativa")
  because "endogenes" is grounded as "genes de linha germinativa" and the
  label already says "xenótipos de linha germinativa" — kept as-is for
  terminological accuracy rather than shortened, since no shorter grounded
  synonym for "endogenes" exists. The six math-strategy labels (Inverso,
  Raiz inversa, Linear, Raiz quadrada, Curva em sino, Uniforme) are ordinary
  Portuguese mathematical vocabulary, not vanilla-grounded (no RimWorld
  Keyed source uses them), per the brief's allowance.
