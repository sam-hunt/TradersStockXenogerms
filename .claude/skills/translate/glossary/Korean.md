# Korean — Xenogerm Trader Stock glossary

Family-shared mechanics — most notably the josa (particle) marker rules
(`Verse.LanguageWorker_Korean.ReplaceJosa`), which apply to any Keyed
string this mod's settings window uses, not just combat/rulepack text — and
vanilla-grounded common vocabulary (Cancel/Reset buttons, quality tiers,
"Traders will pay more/less for it.") live in the `l10n/` submodule at
`l10n/languages/Korean.md`, grounded across the weapon-mod sibling family's
2026-07 generation and cross-checked against PersonaWeaponsUnbound's
independent Korean pass the same day. This file holds only what is specific
to Xenogerm Trader Stock.

## Grounded terms

| English | Korean | Source |
|---|---|---|
| xenogerm | 변형이식체 | Biotech `ThingDef` `Xenogerm.label` |
| xenotype | 인종형 | Biotech Keyed `Xenotype` |
| gene / genes | 유전자 | Biotech Keyed `Gene`/`Genes` |
| complexity | 복잡도 | Biotech Keyed `Complexity` |
| metabolism (metabolic efficiency) | 대사 효율 | Biotech Keyed `Metabolism` |
| endogenes / germline genes | 생식유전자 | Biotech Keyed `Endogenes` |
| xenogenes | 변형인자 | Biotech Keyed `Xenogenes` |
| archite / archite gene(s) | 초월입자 / 초월입자 유전자 | Biotech Keyed `ArchitesTotal`/`ArchitesRequired`; `GeneCategoryDef` `Archite.label` |
| inheritable | 유전 가능 | Biotech Keyed `GenesAreInheritable` |
| default | 기본값 | Core Keyed `Default` |
| reset to defaults | 기본값 복원 | Core `RestoreToDefaultSettings` (per family glossary; distinct from `ResetButton`=초기화, a different generic key) |
| Sanguophage | 생귀오파지 | Biotech `XenotypeDef` `Sanguophage.label` |
| Impid | 임피드 | Biotech `XenotypeDef` `Impid.label` |
| Yttakin | 이타킨 | Biotech `XenotypeDef` `Yttakin.label` |
| Hussar | 후사르 | Biotech `XenotypeDef` `Hussar.label` |
| Pigskin | 피그스킨 | Biotech `XenotypeDef` `Pigskin.label` |
| Dirtmole | 더트몰 | Biotech `XenotypeDef` `Dirtmole.label` |
| Highmate | 하이메이트 | Biotech `XenotypeDef` `Highmate.label` |
| market value | 시장 가치 | Core `StatDef` `MarketValue.label` |
| silver | 은 | Core `ThingDef` `Silver.label` |
| trader / orbital exotic goods trader | 상인 / 궤도 희귀품 상선 | Core Keyed trader vocab; `TraderKindDef` `Orbital_Exotic.label`=희귀품 상선 (family glossary: caravan traders=상인, orbital kinds=상선) |
| scenario editor | 시나리오 편집 | Core Keyed `ScenarioEditor` |
| ideology (the game mechanic) | 이념 | Biotech Keyed `ITabs.xml` (`IdeoExposurePoints*` strings use 이념 for the specific belief system a pawn joins) |

## Workshop title

`XTS_SettingsCategory` = **상인 변형이식체 재고** ("trader xenogerm stock").
Built from the grounded Biotech term for xenogerm (변형이식체) and the
ordinary vanilla word for traders (상인), in the order that reads most
naturally as a Korean noun-phrase title (modifier-modifier-head), conveying
"xenogerms in traders' stock" concisely for Workshop search.

## Phrasing decisions

- **"naturalized" (English source uses scare quotes: 'naturalized' members)
  has no vanilla Korean anchor** — grepped Biotech/Core Keyed and DefInjected
  for `naturaliz*` with zero hits. Rendered as 귀화 (the ordinary Korean word
  for naturalization/immigration status), kept in the same single-quote
  scare-quotes the English uses. **Needs native review.**
- **"Biotech" (DLC name) is not present in the extracted Keyed/DefInjected
  dump** (only found `RoomRequirementNoBiotechBuildings` → 생명공학 건물 없음,
  a generic "biotech buildings" phrase, not the DLC's own label). Used
  바이오테크, the commonly recognized Steam/Ludeon transliteration for the
  DLC's official Korean storefront name, on ordinary-word confidence rather
  than a grounded Keyed/DefInjected hit. **Flag for native review** if a more
  authoritative in-game DLC-name string turns up later.
- "preset" (as in "preset xenotype", "base preset value") has no vanilla
  Korean anchor either; rendered as 프리셋, a loanword in common use in the
  Korean RimWorld community. Low risk, but flagged since it is a coinage,
  not a grounded term.
- "player-created xenotypes" avoided a literal "player" (플레이어) and instead
  used 직접 생성한/직접 만든 ("that you created yourself"), matching Korean's
  general avoidance of possessive/agent pronouns noted in the family style
  guide.
- No josa markers were needed anywhere in XTS_UI.xml or the description: all
  placeholders (`{0}`) sit before either an invariant particle (none used)
  or no particle at all (numeric labels use fixed suffixes like `당`, `개당`,
  `점당`-equivalents that don't inflect), so the digit-josa fallback pitfall
  never applies here.
- No dashes were introduced anywhere; the description's Mod Settings list
  items use ` - ` after each bold label, mirroring the exact English slot per
  the brief's allowance.
