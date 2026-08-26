# Spanish — Xenogerm Trader Stock glossary

Family-shared mechanics (`de el`→`del`/`a el`→`al` hand-contraction,
`[X_possessive]` singular-only usage, inverted opening punctuation), style/
corpus rules, and vanilla-grounded common vocabulary (Cancel/Reset buttons,
quality tiers, "Traders will pay more/less for it.") live in the `l10n/`
submodule at `l10n/languages/Spanish.md`, grounded across the weapon-mod
sibling family's 2026-07 generation. This file holds only what is specific
to Xenogerm Trader Stock.

## Grounded terms

| English | Spanish | Source |
|---|---|---|
| xenogerm | xenogermen | Biotech Keyed `Misc_Gameplay.xml` `<Xenogerm>`; ThingDef `Xenogerm.label` (lowercase in DefInjected) |
| xenotype | xenotipo | Biotech Keyed `Misc_Gameplay.xml` `<Xenotype>` |
| gene / genes | gen / genes | Biotech Keyed `Misc_Gameplay.xml` `<Gene>`/`<Genes>` |
| complexity | complejidad | Biotech Keyed `Misc_Gameplay.xml` `<Complexity>` |
| metabolism (metabolic efficiency) | eficiencia metabólica | Biotech Keyed `Misc_Gameplay.xml` `<Metabolism>` |
| endogenes / germline genes | genes de línea germinal | Biotech Keyed `Dialogs_Various.xml` `<Endogenes>` |
| archite / archite gene | arquita / gen arquita | Biotech Keyed `ArchitesTotal`, `ArchitesRequiredDesc`, `XenotypeBreaksLimits_Archites` ("genes arquita") — postposed noun-as-modifier, not an adjective |
| inheritable | hereditario | Biotech Keyed `Dialogs_Various.xml` `GenesAreInheritable` = "Genes hereditarios" |
| player-created / scenario editor | creado por el jugador / editor de escenarios | Core Keyed `Menus_Main.xml` `<ScenarioEditor>` |
| default | por defecto | Core Keyed `Misc.xml` `<Default>` |
| reset to defaults | Restablecer valores por defecto | family-shared vocab table in `l10n/languages/Spanish.md` (Core `Reset`/`RestoreToDefaultSettings` both collapse to "Restablecer"; "to defaults" built from `Default`) |
| Sanguophage / Impid / Yttakin / Hussar / Pigskin / Dirtmole / Highmate | sanguífago / diablillo / yttakin / húsar / pielcerdo / terra-topo / súcubo | Biotech DefInjected `XenotypeDef/XenotypeDefs.xml` |
| market value (running prose, e.g. "the market value formula") | valor de mercado | Core DefInjected `StatDef/Stats_Basics_General.xml` `MarketValue.label` — narrative/formula sense, distinct from the Core Keyed `<MarketValue>` = "Precio base" trap noted in `l10n/languages/Spanish.md` (that's the specific dialog field label, not used in this mod's strings) |
| silver | plata | Core DefInjected `ThingDef/Items_Resource_Stuff.xml` `Silver.label` |
| orbital exotic goods trader | comerciante orbital de productos exóticos | Core DefInjected `TraderKindDef/TraderKinds_Orbital_Misc.xml` `Orbital_Exotic.label` |
| ideology (recognition system) | ideoligión | family-shared vocab table in `l10n/languages/Spanish.md` (Ideology `Relic`/`RelicOf`/`IdeoligionOf` all coin "ideoligión"; not independently re-extracted this pass, applied per that table) |

## Workshop title

`XTS_SettingsCategory` = **Xenogérmenes para comerciantes**. Built from the
grounded Biotech noun `xenogermen` plus the ordinary word for traders
(`comerciantes`, same noun used in the grounded `Orbital_Exotic` trader
label), in the sense "xenogerms for traders['] stock" — short, natural,
and searchable on Workshop. Avoided a literal "stock/existencias" word
since it made the title noticeably longer without adding clarity.

## Phrasing decisions

- **"naturalized" is ungrounded — needs native review.** No official
  Spanish rendering of "naturalized" (in the Biotech xenotype/ideology
  sense) turned up in the extracted Biotech or Core Keyed/DefInjected data.
  Used the plain, most natural word `naturalizados`, quoted in ASCII
  straight quotes per style (matching the English's single-quoted
  `'naturalized'`), consistent with vanilla's convention of quoting cited
  terms. Flag for native-speaker confirmation.
- **Plural of `xenogermen` is `xenogérmenes` (attested).** The Biotech tar
  uses `xenogérmenes` a dozen times alongside ~70 singular `xenogermen` hits,
  following the regular stress-shift pattern (`origen`→`orígenes`). The
  2026-08-26 first draft wrote the plural unaccented; lead review corrected
  every instance (Keyed, Workshop title and description) to `xenogérmenes`.
- **"archite" used as a postposed noun-modifier, not an adjective**
  (`xenotipos arquita`, `gen arquita`), mirroring vanilla's own
  `genes arquita` / `cápsulas de arquita` pattern rather than coining an
  adjective form like `arquítico`.
- **Numbers in the Workshop description use Spanish period thousands
  separators** (`1.500`, `3.200`) rather than the English source's commas,
  per standard Castilian numeral formatting; this is prose, not a
  game-rendered value, so no engine formatting applies.
- **No dashes introduced.** The English " - " after bold Mod Settings
  labels and in the Links line was replaced with a colon throughout,
  per the family's zero-dash rule for Spanish.
