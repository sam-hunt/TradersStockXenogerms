# Glossary — XTS-specific terminology

These per-language files (`Russian.md`, `Japanese.md`, `ChineseSimplified.md`,
`Korean.md`, `German.md`, `Spanish.md`, `French.md`,
`PortugueseBrazilian.md`, `ChineseTraditional.md`) are meant to hold everything about a language's
translation that is specific to Xenogerm Trader Stock: mod-coined terms,
the localized Workshop title (`XTS_SettingsCategory`), and worked phrasing
decisions tied to this mod's own `StatDef`/`ThingDef` patches (e.g. any
restructuring `MarketValue`/`SellPriceFactor`/`Xenogerm` prose needs to fit a
target language's contraction or case rules).

**Status (2026-08-26):** the initial machine-assisted generation pass has run
for all nine roster languages, grounding xenogerm, xenotype, gene, archite
gene, gene complexity, metabolism and the trader/market-value vocabulary
against the Core + Biotech tars. Each language file records its grounded
term table, the localized Workshop title (`XTS_SettingsCategory`), and any
phrasing decisions or terms flagged for native review. Later passes should
extend those tables rather than re-derive them.

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

When a future translation pass coins a new XTS-specific term, record it
here. If a pass instead surfaces a correction to shared mechanics or
vocabulary, send that fix upstream to the l10n repo rather than duplicating
it here.
