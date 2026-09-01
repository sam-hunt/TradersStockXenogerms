# TODOs

- review pass of all player facing english strings
- review pass of steam workshop description
- confirm readme's claim of removal save-safety now we've added more features
- README yellow "Status: In Development" badge -> Steam stat badge row: subscribers/downloads/favorites/views keyed on the PublishedFileId, per the siblings.
- commit steam published file id to About/ post-release

## Follow-ups from the 2026-08-31 overnight pass (items 1-8 landed, one commit each)

- Playtest the endogene retarget: implant a trader-sold Impid/Yttakin xenogerm with the toggle on; confirm skin/hair follow the implant (previous germline is replaced wholesale; the pawn's own skin/hair colour genes return only when the xenotype has none), children inherit, the gene tab shows endogenes, and a pawn with no remaining xenogenes can't have a xenogerm extracted.
- Playtest the deathrest-capacity carryover: serum a sanguophage's capacity up, reimplant its xenogerm into a pawn who also had serum-raised capacity (recipient keeps their own number, not the caster's); implant any trader-sold or crafted xenogerm granting deathrest into a serum-raised deathrester (capacity survives, no "capacity changed" message); toggle the setting off and confirm vanilla reset returns.
- Playtest the xenogene-preservation fix: implant a germline (Impid/Yttakin) xenogerm into a pawn with existing xenogenes (e.g., a Hussar) — the xenogenes must survive on top of the new germline (gene tab shows both layers, xenogene wins visual conflicts, extraction still offered); implant a Baseliner xenogerm into the same kind of pawn — both layers wiped, plain baseliner out.
- Playtest the identity-layer rule (2026-09-01): implant a germline (Impid/Dirtmole) xenogerm into a Sanguophage — pawn stays labeled/iconed Sanguophage while the germline underneath becomes the implant; into a born Impid carrying a stray dev-mode xenogene — relabeled to the implant's xenotype, stray xenogene kept; into a grown-up "Hybrid"-labeled pawn — relabeled, and a later pregnancy behaves as a pure germline (hybrid flag cleared); into a pawn that earlier received a vanilla custom xenogerm — the custom name/icon survive the retarget.
- Playtest the VRE Lycanthrope retarget guard (2026-09-02): with Lycanthrope loaded, implant a trader-sold Impid xenogerm into a Wolfman/Lycan pawn — germline must be left alone (vanilla xenogene implant + identity stamp, morph gizmo still works before and after); implant a trader-sold Wolfman xenogerm into a plain colonist — full retarget, pawn morphs like a born Wolfman; check the gene tab's morph genes still show Lycanthrope's own icon/background (our extension is appended, not replacing theirs).