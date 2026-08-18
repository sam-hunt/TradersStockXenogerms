---
name: translate
description: Generate, update, or audit mod localization (Keyed today; DefInjected once the mod ships its own defs) for a target language, grounded in vanilla RimWorld terminology — particularly Biotech's xenotype/gene vocabulary. Use when asked to add a language, update translations, or check translation freshness.
argument-hint: "[language, e.g. German | update | check]"
---

# Translate

Produce or refresh localization files for Traders Stock Xenogerms. English is
the source of truth; every other language derives from it.

**The family-wide process lives in the `l10n/` submodule — load these first,
and only these** (progressive disclosure; if `l10n/` is empty, run
`git submodule update --init`):

- `l10n/process.md` — non-negotiables, file/format conventions, terminology
  grounding method, and the generation / update / audit workflows. This is
  the workflow authority; follow it step by step.
- `l10n/languages/<Language>.md` — the target language's engine mechanics,
  style rules, and vanilla-grounded common vocabulary. Read ONLY the target
  language's file.
- `glossary/<Language>.md` (beside this file) — this mod's own coined-term
  table for the target language. Read it in the same pass.
- `l10n/lessons.md` — cross-language lessons; read when generating a new
  language, skim otherwise.
- `l10n/workshop.md` — Steam Workshop description/title conventions;
  `.steamworkshop/README.md` names this mod's anchor term and title-coupling
  key (`TSX_SettingsCategory`).

**Where learnings land:** mod-independent findings (engine mechanics, a
language's grammar rule, corpus style facts) go in the `l10n/` submodule —
edit the canonical checkout at `~/dev/rimworld-l10n`, commit there, then bump
the pin here. Mod-specific findings (coined terms, phrasing decisions) go in
`glossary/<Language>.md`.

## This mod's translation surface

- English Keyed source: `1.6/Languages/English/Keyed/TSX_UI.xml` — a single
  file covering the mod settings window (pricing sliders, toggles, reset
  button) and any other player-facing prose this mod owns. Every key is
  `TSX_`-prefixed. There is no second Keyed file.
- **`TSX_SettingsCategory` is not a plain UI string, it is the localized
  Workshop title.** Its value is that language's Steam Workshop page title
  and must stay in sync with the title line (line 1) of
  `.steamworkshop/Description/<Language>.txt` — translate it per the title
  convention in `.steamworkshop/README.md` (fully localized, leaning on that
  language's vanilla Biotech term for xenogerm and its ordinary word for
  traders, no English brand appended), never leave it as the English string.
  See the CLAUDE.md localization note for the coupling rule.
- **This mod ships no Defs of its own.** `1.6/Patches/**` are XML Patches
  (`PatchOperationAdd`/`PatchOperationSequence`) that bolt comps and
  StatParts (`CompXenotypeSource`, `StatPart_XenogermValue`,
  `StatPart_XenogermSellFactor`) onto vanilla `Xenogerm`/`TraderKindDef`/
  `StatDef` entries — none of them add a `label`, `description`, or any
  other translatable field. So the DefInjected surface is currently
  **empty**, guarded by the `Scripts/expected-injections.json` sidecar
  rather than a hand-maintained "nothing to do here" note. If this mod ever
  ships its own Def subclass (a custom `StockGenerator` variant, say, with
  its own label), translate it per language via DefInjected exactly as the
  sibling mods do — `l10n/process.md`'s DefInjected mechanics apply
  unchanged the day that happens.
- **No gated compat load roots exist today** (no `MayRequire`-gated defs, no
  `1.6/Mods/<Name>/` folders) — but the checker and `LoadFolders.xml` idiom
  already support them, following sister mod `BetterTradersGuild`. If this
  mod ever ships a def gated on an optional mod or a second DLC, its
  DefInjected translations must live under that def's own gated root, never
  the main `1.6` tree — the checker enforces the placement in both
  directions and names the owning root in its errors.

## This mod's grounding domain

Domain DLC: **Biotech** (plus Core) — this repo's `REQUIRED_DLCS` is
`{"Biotech"}`: it's a hard dependency (without it the mod's defs do not load
at all), and it ships no other MayRequire-gated content behind a second DLC.
Ground against the Core + Biotech tars; Odyssey and Royalty are the
weapon-mod siblings' domain, not this one's. Terms that MUST be grounded
before use: xenogerm, xenotype, gene, archite gene, gene complexity,
metabolism (the gene stat, not the pawn need), inheritable vs.
non-inheritable genes, custom xenotype, and orbital-trader/market-value
vocabulary ("Traders will pay more/less for it." and similar phrasing,
market value, silver) — the vanilla-grounded answers live in
`l10n/languages/<Language>.md`; this mod's own coined terms (none yet — see
each glossary file's status note) live in `glossary/<Language>.md`. **No
language pass in this repo has yet run a Biotech-grounded generation**;
treat every glossary file as a style/mechanics status note only until an
actual generation pass grounds xenogerm/xenotype vocabulary against the
Biotech tar and records it there.

This mod patches both a `StatDef` (`MarketValue`, `SellPriceFactor`) and a
`ThingDef` (`Xenogerm`) — if either ever grows a translatable field, confirm
which def *type*'s official label you're grounding against, not just the
term (a def field's official label can differ across the def types that
share its name or concept — see `l10n/lessons.md`). And when Biotech's own
xenotype/gene Keyed data disagrees with Core's generic item/trader
vocabulary, Biotech wins — it's the nearer domain analog.

## Workflows

Follow `l10n/process.md`'s Initial generation / Update pass / Audit-only
workflows verbatim. This mod's specifics on top:

- The checker: `python3 Scripts/check-translations.py` (`--strict` for new
  languages). Sidecar regen: `python3
  Scripts/refresh-translation-expectations.py` (game must be closed; drives
  the deployed L10nProbe).
- There is currently no DefInjected surface to enumerate — confirm that
  against the `Scripts/expected-injections.json` sidecar (its `required`
  subset should come back empty) rather than assuming it, since the sidecar
  becomes the authority the moment a def with a translatable field ships.
- The public roster (and credits) is CONTRIBUTING.md's localization table —
  update it in the same commit as any language addition or native review.
