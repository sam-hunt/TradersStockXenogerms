# TODOs

- review pass of all player facing english strings
- review pass of steam workshop description
- confirm readme's claim of removal save-safety now we've added more features
- README yellow "Status: In Development" badge -> Steam stat badge row: subscribers/downloads/favorites/views keyed on the PublishedFileId, per the siblings.
- commit steam published file id to About/ post-release

## Follow-ups from the 2026-08-31 overnight pass (items 1-8 landed, one commit each)

- `/translate update` across all nine languages: 12 new keys missing everywhere (endogene toggle, categories header, archite/player-scenario descriptors, plus the older grid keys) and `XTS_IncludePlayerScenario(+Desc)` renamed but left stale on purpose; glossary "player-created" rows and the non-English Workshop settings bullets (renamed setting, new endogene bullet, corrected defaults) need the same pass.
- Tooltip archite line is text-only ("Contains archite genes"): `TipSignal` carries a string, so no icon can ride in a hover - decide whether to draw a small archite badge in the grid cell itself instead.
- In-game eyeball of the new tooltip colours (archite lime-yellow is a hand-picked `Color(0.8, 0.95, 0.35)`, no palette entry; germline `ColorLibrary.Beige`; player-scenario `FactionDefOf.PlayerColony.DefaultColor`) and of the greyed endogene checkbox under a switched-off inheritable toggle.
- Playtest the endogene retarget: implant a trader-sold Impid/Yttakin xenogerm with the toggle on; confirm skin/hair follow the implant (pawn's conflicting endogenes are removed first), children inherit, the gene tab shows endogenes, and the pawn can no longer have a xenogerm extracted.
- Tooltip Source line is skipped for player-scenario xenotypes (no content pack) - decide whether a "Source: scenario" line is wanted for symmetry.
- Section descriptions moved from tiny-font sub-labels to header hover tooltips per the family convention - confirm the "Xenotypes for sale" how-to (untick / greyed / hover) is discoverable enough from a header hover.
