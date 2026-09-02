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
| Baseliner | 일반인 | Biotech DefInjected `XenotypeDef/Baseliner.label` |
| deathrest | 죽음안식 | Biotech Keyed `Deathrest`; GeneDef `Deathrest.label` |
| deathrest capacity | 죽음안식 수용량 | Biotech Keyed `DeathrestCapacity` |
| deathrest capacity serum | 죽음안식 수용 혈청 | Biotech DefInjected `ThingDef/DeathrestCapacitySerum.label` |
| sanguophage reimplant (ability) | 유전자 이식 | Biotech DefInjected `AbilityDef/ReimplantXenogerm.label` |
| scenario | 시나리오 | Core Keyed `ScenarioEditor` (시나리오 편집) |
| custom (xenotype) | 사용자 지정 (인종형) | Biotech Keyed `Custom`, `MessageTooManyCustomXenotypes` |

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
- **2026-09-02 update pass:** added 42 new keys (xenotype filter grid, price
  breakdown tooltip rows, stock quantity, implantation, and commonality
  sections) and retranslated the five pricing-section descriptions after the
  English dropped their trailing "Default: {0}" clause; removed the
  superseded `XTS_IncludeArchite`/`IncludeInheritable`/`IncludePlayerScenario`
  toggle keys (replaced by the per-xenotype sold ledger grid). No dashes
  introduced (0 dashes in the file vs. vanilla ko Keyed's ~30/100k chars).
- "Germline xenotypes" (`XTS_FilterInheritable`) rendered as 생식유전자
  인종형 ("germline-gene xenotypes"), built on the grounded Endogenes
  rendering (생식유전자) rather than coining a separate "germline" word; no
  vanilla string uses this exact compound, so it needs native review.
- "Xenotype commonality by price" (`XTS_CommonalitySection`) rendered as
  가격별 인종형 출현 빈도 ("occurrence frequency of xenotype by price").
  "Commonality" has no vanilla anchor; 출현 빈도 is the ordinary term for
  spawn/appearance frequency in Korean RimWorld modding usage. Flagged for
  native review.
- Commonality strategy names (Inverse/Inverse root/Linear/Square
  root/Bell curve/Uniform) use ordinary Korean math vocabulary (역비례,
  역제곱근, 선형, 제곱근, 종형 곡선, 균등); none have a vanilla anchor, but
  all are standard textbook terms, so risk is low.
- "Sanguophage reimplant" reuses the `ReimplantXenogerm` ability's own label
  verb (유전자 이식) per the update brief's instruction, rather than coining
  a separate "재이식" word.
