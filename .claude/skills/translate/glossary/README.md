# Glossary — TSX-specific terminology

These per-language files (`Russian.md`, `Japanese.md`, `ChineseSimplified.md`,
`Korean.md`, `German.md`, `Spanish.md`, `French.md`,
`PortugueseBrazilian.md`) are meant to hold everything about a language's
translation that is specific to Traders Stock Xenogerms: mod-coined terms,
the localized Workshop title (`TSX_SettingsCategory`), and worked phrasing
decisions tied to this mod's own `StatDef`/`ThingDef` patches (e.g. any
restructuring `MarketValue`/`SellPriceFactor`/`Xenogerm` prose needs to fit a
target language's contraction or case rules).

**None of that exists yet.** This mod has run no Biotech-grounded generation
pass in any language — every file below is a placeholder carrying only the
mechanics/style status inherited from the weapon-mod sibling family's
2026-07 generation passes (none of which touched Biotech or xenogerm/
xenotype vocabulary) plus a "no pass has run" note. The first real pass for
a language should ground xenogerm, xenotype, gene, archite gene, gene
complexity, metabolism, and the other terms named in this skill's grounding
section against the Biotech tar, and record the results in that language's
file here.

Family-shared, mod-independent findings — LanguageWorker mechanics, style
and corpus rules, and vanilla-grounded common vocabulary (quality tiers,
Cancel/Reset buttons, "Traders will pay more/less for it.", and so on) — live
upstream in the `l10n/` submodule at `l10n/languages/<Language>.md`
(canonical checkout: `~/dev/rimworld-l10n`), since they apply to any mod in
the family, not just this one. Do not duplicate them here.

This mod patches a vanilla `StatDef` (`MarketValue`, `SellPriceFactor`) and a
vanilla `ThingDef` (`Xenogerm`) rather than owning either — a future
generation pass should confirm it is grounding against the right def
*type*'s official label before reusing a vanilla term (see this skill's
grounding-domain section).

When a future translation pass coins a new TSX-specific term, record it
here. If a pass instead surfaces a correction to shared mechanics or
vocabulary, send that fix upstream to the l10n repo rather than duplicating
it here.
