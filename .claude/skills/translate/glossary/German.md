# German — Xenogerm Trader Stock glossary

Family-shared mechanics (case declension via `lookup`/`decline`, article
helpers, `PostProcessed`'s `'s`-rewrite), style/corpus rules, and
vanilla-grounded common vocabulary (Cancel/Reset buttons, quality tiers,
"Traders will pay more/less for it.") live in the `l10n/` submodule at
`l10n/languages/German.md`, grounded across the weapon-mod sibling family
and consolidated with Better Traders Guild's 2026-08-10 pass. **That
upstream file corrects an earlier family claim that `GrammarResolverSimple`
implements no `lookup` function — `lookup` IS available in a plain Keyed
string (see its Pitfalls section).** Read the upstream file itself rather
than relying on any older, superseded description of this mechanic. This
file holds only what is specific to Xenogerm Trader Stock.

## Grounded terms

| English | German | Source |
|---|---|---|
| Xenogerm | Xenokeim | Biotech Keyed `Xenogerm`; DefInjected `ThingDef` `Xenogerm.label` |
| Xenotype | Xenotyp | Biotech Keyed `Xenotype` |
| Gene / Genes | Gen / Gene | Biotech Keyed `Gene`, `Genes` |
| Complexity | Komplexität | Biotech Keyed `Complexity` |
| Metabolism (metabolic efficiency) | metabolische Effizienz | Biotech Keyed `Metabolism` |
| Endogenes (germline genes) | Keimbahngene | Biotech Keyed `Endogenes` |
| Xenogenes | Xenogene | Biotech Keyed `Xenogenes` |
| archite gene(s) | Architgen / Architgene | Biotech Keyed `XenotypeBreaksLimits_Archites`; DefInjected `GeneDef` `ArchiteMetabolism.label` = "architischer Stoffwechsel" (confirms `architisch` as the attested adjective) |
| archite capsules | Architkapseln | Biotech Keyed `ArchitesRequired`, `NotEnoughArchites` |
| Default | Standard | Core Keyed `Default` |
| Reset to defaults | Auf Standard zurücksetzen | Core Keyed `ResetButton` = "Zurücksetzen"; l10n family table already has this exact phrase for "reset to defaults" |
| Sanguophage / Impid / Yttakin / Hussar / Pigskin / Dirtmole / Highmate | Sanguophage / Impid / Yttakin / Husar / Schweinehaut / Dreckwühler / Überpartner | Biotech DefInjected `XenotypeDef` labels |
| MarketValue | Marktwert | Core DefInjected `StatDef` `MarketValue.label` |
| SellPriceFactor | Verkaufspreis-Faktor | Core DefInjected `StatDef` `SellPriceFactor.label` |
| Silver | Silber | Core DefInjected `ThingDef` `Silver.label` |
| exotic goods trader | Händler exotischer Güter | Core DefInjected `TraderKindDef` `Orbital_Exotic.label` |
| scenario editor | Szenario-Editor | Core Keyed `ScenarioEditor` |
| player-created / custom xenotype | benutzerdefinierter Xenotyp | Biotech Keyed `MessageTooManyCustomXenotypes` ("benutzerdef. Xenotypen") |
| ideoligion / ideology | Ideologie | l10n family table (`l10n/languages/German.md`), not present in the extracted Core/Biotech data for this repo — Ideology DLC data wasn't pulled |
| Baseliner (xenotype label) | Normal | Biotech DefInjected `XenotypeDef` `Baseliner.label` |
| Deathrest | Todesschlaf | Biotech Keyed `Deathrest`, `Deathresting`; DefInjected `GeneDef` `Deathrest.label` |
| Deathrest capacity | Todesschlafkapazität | Biotech Keyed `DeathrestCapacity`, `DeathrestCapacityDesc` |
| Deathrest capacity serum | Todesschlafkapazitätsserum | Biotech DefInjected `ThingDef` `DeathrestCapacitySerum.label` |
| Sanguophage reimplant (ability) | Gene implantieren (verb: implantieren) | Biotech DefInjected `AbilityDef` `ReimplantXenogerm.label`/`.description` |
| scenario | Szenario | Core Keyed `ScenarioEditor`, `ScenarioTitle` |
| trader kind (bulk goods / exotic goods trader) | Großhändler / Händler exotischer Güter | Core DefInjected `TraderKindDef` `Orbital_BulkGoods.label`, `Orbital_Exotic.label` |

## Workshop title

**Xenokeime im Händlerbestand** — built from the grounded `Xenokeim`
(Biotech) and `Bestand`/`Händler` (a natural, gaming-idiomatic rendering of
"trader stock/inventory"; vanilla itself doesn't have a single-word
"trader stock" Keyed string to reuse verbatim). Short, searchable, and
mirrors the sense of the English title ("xenogerms in traders' stock")
without appending the English brand name.

## Phrasing decisions

- **"naturalized" (member) — needs native review.** No Ideology-DLC data
  was extracted for this pass (only Core + Biotech tars were pulled), so
  this term is ungrounded. Rendered as `'naturalisiert'` (ASCII-quoted,
  matching the English source's own quoting and this language's
  citation-quote style) as the most natural literal rendering; a native or
  an Ideology-tar grounding pass should confirm whether RimWorld's German
  Ideology data uses a different established term for this concept.
- "preset xenotype" is rendered as "vordefinierter Xenotyp" rather than a
  loanword "Preset-Xenotyp" — no vanilla precedent for "preset" as a
  loanword in this domain, and "vordefiniert" is standard German UI
  vocabulary.
- "germline xenotype" is expanded to "Xenotyp mit Keimbahngenen" (using
  the grounded `Endogenes` = `Keimbahngene`) rather than compounding into
  an unattested "Keimbahn-Xenotyp", to keep the noun phrase transparent.
- Percent in the description body is kept tight (`5%`), matching both the
  English source's own formatting and the upstream style file's note that
  vanilla de writes percentages tight (`{0}%`).
- **"Germline xenotypes" (`XTS_FilterInheritable`, `XTS_ImplantGermlineAsEndogenes`)
  is rendered as the compound "Keimbahn-Xenotypen"** rather than the earlier
  "Xenotyp mit Keimbahngenen" phrasing — these are short radio/filter/toggle
  labels (brief constraint), where English itself compounds ("Germline
  xenotypes"). The longer expanded form is kept for prose contexts
  (`XTS_FilterInheritableDesc` still spells out "vererbbare Xenotypen ...
  und Normal"). Flagged since it partially revisits the earlier phrasing
  decision below; the earlier decision's rationale (avoid an unattested
  compound in a full sentence) still holds for prose, this is only for
  short labels.
- **Baseliner, when referred to generically ("a baseliner") rather than as
  the proper `XenotypeDef` label, is rendered using its grounded label
  "Normal"** (`XTS_FilterInheritableDesc`: "... und Normal, dessen leerer
  Xenokeim den Träger wieder zu einem Normal macht") — needs native review to
  confirm this reads naturally rather than ambiguous with the adjective
  "normal".
- 2026-09-02 update pass: added the 42 new Keyed keys (xenotype filter
  grid, price breakdown tooltip, stock-quantity, implantation, and
  commonality-strategy sections), retranslated the 5 pricing-slider
  tooltip descriptions after the English dropped their "Default: {0}"
  clause, and removed the three retired `XTS_Include*` toggle keys.
