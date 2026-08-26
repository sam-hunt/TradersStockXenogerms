# TODOs

Freeform backlog of ideas — not commitments.

- Generate machine-assisted translations for the sibling repos' language set
  (ChineseSimplified, French, German, Japanese, Korean, PortugueseBrazilian,
  Russian, Spanish) via the `translate` skill. English is currently the only
  language; the checker/sidecar infrastructure is already in place. The same
  pass should also create `.steamworkshop/Description/<Language>.txt` per
  language, localize each title per `.steamworkshop/README.md`'s convention
  (leaning on that language's vanilla Biotech term for xenogerm and its
  ordinary word for traders, no English brand appended), and sync each
  language's `XTS_SettingsCategory` Keyed value to its title line. The
  Workshop structure/process (README, English.txt, release/translate skill
  updates) landed 2026-08-18; only `English.txt` exists so far.
- Consider a Steam Workshop preview image (About/Preview.png is absent).
- After the first Workshop publish, swap the README's temporary yellow
  "Status: In Development" badge for the family's Steam stat badge row
  (subscribers/downloads/favorites/views keyed on the PublishedFileId, as in
  the sibling READMEs); the release skill no longer touches README.md.
