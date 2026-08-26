# French — Xenogerm Trader Stock glossary

Family-shared mechanics (`LanguageWorker_French`'s automatic elision/
contraction, `vous` formality, `[X_possessive]` unusability), style/corpus
rules, and vanilla-grounded common vocabulary (Cancel/Reset buttons, quality
tiers, "Traders will pay more/less for it.") live in the `l10n/` submodule
at `l10n/languages/French.md`. This file holds only what is specific to
Xenogerm Trader Stock.

## Grounded terms

| English | French | Source |
|---|---|---|
| Xenogerm | xénogerme | Biotech `Keyed/Misc_Gameplay.xml` `<Xenogerm>`, ThingDef `Xenogerm.label` |
| Xenotype | xénotype | Biotech `Keyed/Misc_Gameplay.xml` `<Xenotype>` |
| Gene / Genes | gène / gènes | Biotech `Keyed/Misc_Gameplay.xml` `<Gene>`/`<Genes>` |
| Complexity | complexité | Biotech `Keyed/Misc_Gameplay.xml` `<Complexity>` |
| Metabolism (= metabolic efficiency) | efficacité métabolique | Biotech `Keyed/Misc_Gameplay.xml` `<Metabolism>` |
| Endogenes (germline genes) | gènes germinaux | Biotech `Keyed/Dialogs_Various.xml` `<Endogenes>` |
| Xenogenes | xénogènes | Biotech `Keyed/Dialogs_Various.xml` `<Xenogenes>` |
| Inheritable (xenotype) | héréditaire | Biotech `Keyed/Dialogs_Various.xml` `GenesAreInheritable` ("Les gènes sont héréditaires") |
| Archite gene(s) | gène archite / gènes archites | Biotech `Items_Various.xml` `ArchiteCapsule.description` ("un gène archite", noun apposition, no linking preposition); `GeneCategoryDef` `Archite.label` = "archite" |
| Reset to defaults | Réinitialiser les valeurs par défaut | Core `Keyed/Dialogs_Various.xml` `ResetButton`("Réinitialiser") + `Default`("Par défaut") pattern, family-confirmed vocab table |
| Default | par défaut | Core `Keyed/Misc.xml` `<Default>` |
| Traders (plain noun) | commerçants | Core: `DismissTrader`, `AlertTatteredApparelDesc`, `NeedWarmClothesDesc2`, `ColonyCount` all use "commerçant(s)" as the ordinary noun; "marchand" also appears (`ChooseOrbitalTraderKind`) but "commerçant" is the consistent plain-noun choice used across settings-style prose |
| Sanguophage / Impid / Yttakin / Hussar / Pigskin / Dirtmole / Highmate | sanguophage / diabloïde / yttakin / hussard / homme-cochon / homme-taupe / psycharmeur | Biotech `DefInjected/XenotypeDef/XenotypeDefs.xml`, lowercase per vanilla label style |
| Orbital exotic goods trader | vendeur de produits exotiques (en orbite) | Core `TraderKindDef` `Orbital_Exotic.label` |
| Scenario editor | éditeur de scénario | Core `Keyed/Menus_Main.xml` `<ScenarioEditor>` |
| Ideology (ordinary word) | idéologie | Biotech `Keyed/ITabs.xml` `IdeoExposureSectionHeader` etc. |
| Market value (StatDef) | valeur marchande | Core `StatDef` `MarketValue.label` — NOTE: the Keyed `<MarketValue>` slot instead renders "Prix de base"; this mod's settings text is StatDef-flavoured prose ("market value formula"), so `valeur marchande` was used, not the Keyed slot's "Prix de base" |
| Silver (as a value/amount) | argent | Core `Items_Resource_Stuff.xml` `Silver.label` |

## Workshop title

`XTS_SettingsCategory` = **Xénogermes des commerçants**. Built from the
grounded Biotech noun for xenogerm (xénogerme → plural xénogermes) and the
grounded ordinary word for traders (commerçants), in the sense "xenogerms
[found in the stock] of traders" — short, natural, and searchable, avoiding
the ambiguous "marchand" alternative that Core uses inconsistently.

## Phrasing decisions

- **"naturalized" (member) has no official French rendering anywhere in
  Core or Biotech** — grepped `natural*` tree-wide and found only unrelated
  hits (`grossesse naturelle`). Used "naturalisés" (ASCII-quoted, mirroring
  the English scare-quotes) as the most natural cognate rendering. **Needs
  native review.**
- `ArchitesRequired` is a distinct concept ("Capsules d'archites", the
  Keyed label for archite-capsule cost) from "archite gene(s)" — grounded
  separately from `ArchiteCapsule.description`'s "un gène archite" instead.
- Settings toggle descriptions use `commerçants` (plain trader noun)
  throughout, not `marchands`, per the grounded-terms table above.
- No new dashes introduced beyond the two slots the brief explicitly
  permits mirroring (bold-label ` - ` separators in the Mod Settings list
  and the GitHub Links line of the Steam description) — the Keyed file and
  the rest of the description body use colons/commas/restructuring instead,
  consistent with `l10n/languages/French.md`'s "dashes are rare" finding.
