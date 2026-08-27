---
name: rimworld-logs
description: Find and read RimWorld's Player.log, or disassemble the RimWorld API to inspect vanilla types and method signatures. Use when debugging runtime behaviour, chasing an exception, or checking what a vanilla method actually does.
---

# RimWorld Logs and API Inspection

## Reading the log

1. **Enable RimWorld Dev Mode:** Settings → Dev Mode → Logging
2. **Log locations:**
   - **Windows:** `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`
   - **WSL:** `/mnt/c/Users/*/AppData/LocalLow/Ludeon Studios/RimWorld by Ludeon Studios/Player.log`
   - **Linux (Steam):** `~/.config/unity3d/Ludeon Studios/RimWorld by Ludeon Studios/Player.log`

Our own messages are prefixed with the mod's display name, `[Xenogerm Trader Stock]` — the same
prefix RimWorld itself uses when it reports XML patch or def-load failures *about* the mod
(`[Xenogerm Trader Stock] Patch operation ... failed`), and the convention across the sibling
mods — so a single grep for `Xenogerm Trader Stock` isolates both our output and the game's
complaints about us. Note the 1.6 Player.log carries no stack frames or `(Filename:` locators: a
`Log.Error` is just its message line(s), so don't wait for a trace that will never appear.

## Inspecting the RimWorld API

```bash
monodis "/mnt/c/.../RimWorldWin64_Data/Managed/Assembly-CSharp.dll"
```

The local RimWorld installation is the source of truth for API shapes — prefer it over the
`Krafs.Rimworld.Ref` NuGet package, which is only the CI fallback. `ilspycmd` works too when you
need method bodies rather than signatures.
