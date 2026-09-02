# Russian — Xenogerm Trader Stock glossary

Family-shared mechanics, style/corpus rules, and vanilla-grounded common
vocabulary (Cancel button, report/inspect string register, quality tiers)
live in the `l10n/` submodule at `l10n/languages/Russian.md` — originally
grounded from UniqueWeaponsUnbound's PR #6 native review, extended across
the weapon-mod sibling family. This file holds only what is specific to
Xenogerm Trader Stock.

## Grounded terms

| English | Russian | Source |
|---|---|---|
| Xenogerm | Ксеносемя | Biotech Keyed `Xenogerm`; DefInjected `ThingDef` `Xenogerm.label` |
| Xenotype | Ксенотип | Biotech Keyed `Xenotype` |
| Gene / Genes | ген / гены | Biotech Keyed `Gene` / `Genes` |
| Complexity | Сложность | Biotech Keyed `Complexity` |
| Metabolism (metabolic efficiency) | эффективность метаболизма | Biotech Keyed `Metabolism` |
| Endogenes (germline genes) | наследуемые гены | Biotech Keyed `Endogenes` |
| Xenogenes | ксеногены | Biotech Keyed `Xenogenes` |
| Archite genes | архитные гены | Biotech Keyed `XenotypeBreaksLimits_Archites` |
| Inheritable (genes/xenotype) | наследуется / наследуемые гены | Biotech Keyed `GenesAreInheritable` |
| Default | По умолчанию | Core Keyed `Default` |
| Reset (button) | Сбросить | Core Keyed `ResetButton` |
| Reset to defaults | Восстановить по умолчанию | family-grounded (`l10n/languages/Russian.md` vocabulary table, Core `RestoreToDefaultSettings`) |
| Sanguophage | гемофаг | Biotech DefInjected `XenotypeDef/XenotypeDefs.xml` |
| Impid | импид | same |
| Yttakin | иттакин | same |
| Hussar | гусар | same |
| Pigskin | свиночеловек | same |
| Dirtmole | кроточеловек | same |
| Highmate | ангел | same |
| Market value | рыночная стоимость | Core DefInjected `StatDef` `MarketValue.label` |
| Silver | серебро | Core DefInjected `ThingDef` `Silver.label` |
| Orbital exotic goods trader(s) | торговцы экзотикой | Core DefInjected `TraderKindDef` `Orbital_Exotic.label` (plural per family note) |
| trader (plain noun) | торговец | family vocabulary table |
| Scenario editor | редактор сценариев | Core Keyed `ScenarioEditor` |
| Ideology (concept, ordinary word) | идеология | Core Keyed `Ideology.description`, Biotech Keyed `IdeoExposurePointsTooltip*Description` |
| Baseliner | первозданный | Biotech DefInjected `XenotypeDef/XenotypeDefs.xml` `Baseliner.label` |
| Deathrest | торпор | Biotech Keyed `Deathrest`/`Deathresting`; DefInjected `GeneDef` `Deathrest.label` |
| Deathrest capacity | торпорная вместимость | Biotech Keyed `DeathrestCapacity` |
| Deathrest capacity serum | сыворотка торпорной вместимости | Biotech DefInjected `ThingDef` `DeathrestCapacitySerum.label` |
| Sanguophage reimplant (ability) | вживление генов | Biotech DefInjected `AbilityDef` `ReimplantXenogerm.label` |
| Implant (verb, xenogerm) | вживить / вживление | Biotech Keyed `NoXenogermImplanted`, `SelectXenogerm`-adjacent strings; `ReimplantXenogerm.description` |
| Germline (genes) | наследуемые гены | Biotech Keyed `Endogenes` ("germline genes"), `EndogenesDesc` |
| Custom xenotype (player-created) | созданный ксенотип | Biotech Keyed `CreateXenotype` ("Создать ксенотип"); consistent with prior `XTS_IncludePlayerScenario` phrasing |
| Scenario (custom xenotype source) | сценарий | Core Keyed `ScenarioTitle`/`ScenarioEditor` |

## Workshop title

`XTS_SettingsCategory` = **Ксеносемена у торговцев** ("Xenogerms at
traders'"). Built from the grounded Biotech term for xenogerm
(ксеносемя, plural ксеносемена) and the ordinary word for traders
(торговец/торговцы), in the possessive-location sense the brief asked for
("xenogerms in traders' stock") without becoming a literal noun-pile
translation of "stock". Short, natural as a settings-menu/Workshop title,
and searchable on the mod's core nouns.

## Phrasing decisions

- **"naturalized" (member) — ungrounded, needs native review.** No official
  Russian rendering of this Biotech mechanic term (endogene-only xenotype
  implanted via xenogerm) was found anywhere in the extracted Core or
  Biotech data for this language. Rendered as «натурализованных» (guillemets
  per the family's citation convention for a term-of-art) in both the Keyed
  desc and the Workshop description. Flag for native review.
- **"preset xenotype"** (this mod's own contrast against player-created
  custom xenotypes) rendered as "стандартный ксенотип" (standard xenotype)
  throughout the settings labels/descs and the Workshop description's
  pricing/trading sections, for consistency. Not a vanilla-grounded phrase
  (vanilla has no need to distinguish preset vs. custom xenotypes in this
  way) — flag as a mod-coined term, but low-risk since it is transparent.
- **"Xenogerm pricing" (section header)** rendered as "Ценообразование
  ксеносемян" (pricing/pricing-formation of xenogerms) rather than a
  literal "Цена ксеносемян" (price of xenogerms), since the section covers
  the whole pricing formula and its settings, not a single price.
- Numerals in the pricing bullets (~1,500 / ~1,600 to 1,850 / ~3,200) use
  plain digit grouping without English's comma separators (1500, 1600,
  1850, 3200). The "1,600 to 1,850" range is rendered "от ~1600 до 1850"
  (from ... to ...) rather than an en dash, per the brief's no-new-dash
  rule for the description file.
- **2026-09-02 update pass:** refreshed the settings-window Keyed file for
  the removal of the three coarse include-toggles (archite/inheritable/
  player-scenario) in favour of the per-xenotype sold-ledger grid, its
  category filter rows, price-breakdown tooltip, quantity/implantation/
  commonality sections. "Commonality" strategy labels (Inverse, Inverse
  root, Linear, Square root, Bell curve, Uniform) are mod-specific, not
  vanilla terms; rendered with ordinary Russian maths vocabulary
  (Обратная, Обратный корень, Линейная, Квадратный корень,
  Колоколообразная кривая, Равномерная). The "1 / price" and "√price"
  formula fragments keep "цена" uninflected after the fraction bar/root
  (matching English's un-inflected "price"), but "proportional to price"
  prose without a fraction bar takes the grammatical dative ("пропорционален
  цене/√цене") — a deliberate split, flagged here in case a future pass
  wants one convention throughout.
- **2026-09-02 Workshop description refresh:** rewrote the body to match the
  release English (dropped the removed "Параметры мода" settings-recap
  section and the sell-price-factor sentence, no longer present upstream;
  added the LLM/Combat Extended FAQ entries and the hero-art credit line).
  "Hero art by X" rendered as "Главная иллюстрация: X" (no dash, per the
  no-new-dashes rule — English has none there either). The GitHub link's
  " - " kept as a literal hyphen, matching how the prior pass already
  rendered it (not converted to a colon). "Was this made using LLMs!?"
  answer fragment "Some of the grunt code, and most localizations." kept as
  an elliptical noun phrase ("Часть рутинного кода и большинство
  переводов.") rather than inventing a verb or a dash to bridge it.
- **"Vanilla" (the base game, no mods) has no grounded Russian rendering**
  in either tar; `XTS_PreserveDeathrestCapacityDesc` avoids the word
  entirely ("Обычно игра..." / "the game normally...") rather than
  inventing a term. Flag for native review if a mod-family convention for
  "vanilla" emerges later.
