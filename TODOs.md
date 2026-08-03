# TODOs

Freeform backlog of ideas — not commitments.

- Generate machine-assisted translations for the sibling repos' language set
  (ChineseSimplified, French, German, Japanese, Korean, PortugueseBrazilian,
  Russian, Spanish) via the `translate` skill. English is currently the only
  language; the checker/sidecar infrastructure is already in place.
- Consider a Steam Workshop preview image (About/Preview.png is absent).
- Evaluate whether `Scripts/test-windows.sh` is still necessary or the suite can
  run natively with `dotnet test Tests/1.6/TradersStockXenogerms.Tests.csproj` — the idiomatic
  pattern BetterTradersGuild uses (its CLAUDE.md warns the Windows-interop script
  corrupts shared `obj/` incremental state; ArchotechAndroidHardware verified
  native runs work and dropped the script, AAH 9bc240f). `DeployToModFolder` is
  already Release-gated here, so Debug `dotnet test` builds won't redeploy.
