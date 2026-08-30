# TODOs

- Source line is skipped for player-scenario xenotypes (no content pack) in the mod setting xenotype grid item tooltip; add a "Source: scenario" line for symmetry.
- review pass of all player facing english strings
- review pass of steam workshop description
- confirm readme's claim of removal save-safety now we've added more features
- README yellow "Status: In Development" badge -> Steam stat badge row: subscribers/downloads/favorites/views keyed on the PublishedFileId, per the siblings.
- commit steam published file id to About/ post-release

## Follow-ups from the 2026-08-31 overnight pass (items 1-8 landed, one commit each)

- Tooltip archite line is text-only ("Contains archite genes"): `TipSignal` carries a string, so no icon can ride in a hover - decide whether to draw a small archite badge in the grid cell itself instead.
- In-game eyeball of the new tooltip colours (archite lime-yellow is a hand-picked `Color(0.8, 0.95, 0.35)`, no palette entry; germline `ColorLibrary.Beige`; player-scenario `FactionDefOf.PlayerColony.DefaultColor`) and of the greyed endogene checkbox under a switched-off inheritable toggle.
- Playtest the endogene retarget: implant a trader-sold Impid/Yttakin xenogerm with the toggle on; confirm skin/hair follow the implant (previous germline is replaced wholesale; the pawn's own skin/hair colour genes return only when the xenotype has none), children inherit, the gene tab shows endogenes, and the pawn can no longer have a xenogerm extracted.
- Section descriptions moved from tiny-font sub-labels to header hover tooltips per the family convention - confirm the "Xenotypes for sale" how-to (untick / greyed / hover) is discoverable enough from a header hover.
