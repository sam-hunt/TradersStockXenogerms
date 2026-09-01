# TODOs

- review pass of all player facing english strings
- review pass of steam workshop description
- confirm readme's claim of removal save-safety now we've added more features
- README yellow "Status: In Development" badge -> Steam stat badge row: subscribers/downloads/favorites/views keyed on the PublishedFileId, per the siblings.
- commit steam published file id to About/ post-release

## Follow-ups from the 2026-08-31 overnight pass (items 1-8 landed, one commit each)

- In-game eyeball of the new settings-grid interactions (2026-09-01): filter-row label click cycles the tri-state box; drag-painting across grid cells sets them all to the first toggle's state (cursor shows the painted checkbox while dragging) and snaps a category row's checkbox when the stroke crosses it.
- Playtest the endogene retarget: implant a trader-sold Impid/Yttakin xenogerm with the toggle on; confirm skin/hair follow the implant (previous germline is replaced wholesale; the pawn's own skin/hair colour genes return only when the xenotype has none), children inherit, the gene tab shows endogenes, and a pawn with no remaining xenogenes can't have a xenogerm extracted.
- Playtest the deathrest-capacity carryover: serum a sanguophage's capacity up, reimplant its xenogerm into a pawn who also had serum-raised capacity (recipient keeps their own number, not the caster's); implant any trader-sold or crafted xenogerm granting deathrest into a serum-raised deathrester (capacity survives, no "capacity changed" message); toggle the setting off and confirm vanilla reset returns.
- Playtest the xenogene-preservation fix: implant a germline (Impid/Yttakin) xenogerm into a pawn with existing xenogenes (e.g., a Hussar) — the xenogenes must survive on top of the new germline (gene tab shows both layers, xenogene wins visual conflicts, extraction still offered); implant a Baseliner xenogerm into the same kind of pawn — both layers wiped, plain baseliner out.
