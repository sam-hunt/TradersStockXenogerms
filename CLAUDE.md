# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Traders Stock Xenogerms** is a RimWorld 1.6 mod that adds xenogerms for preset xenotypes to trader inventories, allowing players to purchase pre-made xenogerms as an alternative to collecting individual genes. Requires the Biotech DLC.

**Key Features:**

- Preset xenogerms added to orbital exotic goods trader inventories
- Proper xenotype assignment on implantation (not just gene copying)
- Weighted spawn rates based on xenogerm cost
- Configurable pricing formula via mod settings
- Gene Trader mod compatibility

**Key Technologies:** C# (.NET Framework 4.7.2), Harmony library, RimWorld modding API, XML patches

## Build Commands

```bash
# Build the mod (outputs to 1.6/Assemblies/ and deploys to RimWorld Mods folder)
dotnet build TradersStockXenogerms.sln -c Release

# Build only the main project
dotnet build Source/1.6/TradersStockXenogerms.csproj

# Clean build artifacts
dotnet clean TradersStockXenogerms.sln

# Clean deployed mod folder (use when Defs/Patches are renamed or deleted)
dotnet build Source/1.6/TradersStockXenogerms.csproj -t:CleanModFolder

# Full clean build (clean + rebuild)
./Scripts/clean-build.sh
```

The build system auto-detects the RimWorld installation path on Windows/Linux/Mac (including WSL targeting a Windows install). For CI builds without RimWorld installed, it falls back to the `Krafs.Rimworld.Ref` NuGet package.

### Deployment

The repo lives in `~/dev/TradersStockXenogerms`, separate from the RimWorld Mods folder. A post-build MSBuild target (`DeployToModFolder`) automatically copies only runtime files (About, Assemblies, Defs, Patches, Languages, LoadFolders.xml) to `$RIMWORLD_PATH/Mods/TradersStockXenogerms/`. It uses `SkipUnchangedFiles` for fast incremental builds.

**Important:** The deploy copies files but does not delete stale files. If you rename or delete a Def/Patch XML, run `dotnet build Source/1.6/TradersStockXenogerms.csproj -t:CleanModFolder` to wipe the deployed folder, then rebuild to redeploy cleanly.

**WSL Setup:** Requires `RIMWORLD_PATH` env var in `~/.bashrc` pointing to the Windows RimWorld install (e.g., `/mnt/c/Program Files (x86)/Steam/steamapps/common/RimWorld`). The csproj auto-detects `RimWorldWin64_Data` when the Linux data folder isn't found.

## Architecture

### Entry Point

`Source/1.6/TradersStockXenogermsMod.cs` — `TradersStockXenogermsMod` constructor loads settings and patches via `Harmony.PatchAll()` attribute discovery.

**Settings Access:** `TradersStockXenogermsMod.Settings` provides global access to mod configuration.

### Directory Structure

```
Source/1.6/
├── TradersStockXenogermsMod.cs         # Mod entry point, settings UI
├── ModSettings.cs                  # TradersStockXenogermsSettings (persisted config)
├── CompXenotypeSource.cs           # ThingComp storing source XenotypeDef
├── Patch_ImplantXenogerm.cs        # Harmony postfix for xenotype assignment
├── StockGenerator_Xenogerms.cs     # Trader stock generation with weighted rates
├── XenogermFactory.cs              # Creates xenogerm Things from definitions
├── XenogermPricing.cs              # Centralized pricing calculations
├── StatPart_XenogermValue.cs       # MarketValue stat calculation
├── StatPart_XenogermSellFactor.cs  # SellPriceFactor stat for archite bonus
├── Properties/
│   └── AssemblyInfo.cs             # Assembly version metadata

1.6/Patches/                        # XML patches
├── Patches_Traders.xml             # Adds StockGenerator_Xenogerms to trader defs
├── Patches_Xenogerm.xml            # Adds CompXenotypeSource and StatParts to xenogerm def
├── Patches_Stats.xml               # Stat definitions for pricing
└── Patches_GeneTrader.xml          # Gene Trader mod compatibility patch
```

### Core Mechanism

The mod stores a `XenotypeDef` reference on trader-sold xenogerms via `CompXenotypeSource`. When implanted, a Harmony postfix patch on `GeneUtility.ImplantXenogermItem` calls `SetXenotypeDirect()` to assign the preset xenotype to the pawn (vanilla only copies genes and display name).

### Key Classes

| Class | Purpose |
|-------|---------|
| `TradersStockXenogermsMod` | Mod entry point, Harmony init, settings UI |
| `TradersStockXenogermsSettings` | Persisted mod settings (pricing, toggles) |
| `CompXenotypeSource` | ThingComp storing source XenotypeDef on xenogerms |
| `Patch_ImplantXenogermItem` | Harmony postfix assigning xenotype after implantation |
| `StockGenerator_Xenogerms` | Generates xenogerms for traders with weighted spawn rates |
| `XenogermFactory` | Creates xenogerm Things from XenotypeDef or CustomXenotype |
| `XenogermPricing` | Centralized pricing calculations (used by StockGenerator and StatPart) |
| `StatPart_XenogermValue` | Calculates MarketValue based on genes |
| `StatPart_XenogermSellFactor` | Adds SellPriceFactor bonus for archite xenogerms |

### Key Patterns

**Harmony Patching:** All patches use `[HarmonyPatch]` attributes for automatic discovery via `Harmony.PatchAll()`.

**Pricing via StatParts:** Market value and sell factor are calculated dynamically using `StatPart` classes rather than fixed values. This allows xenogerm prices to reflect their gene composition. Only xenogerms with `CompXenotypeSource.sourceXenotype != null` get premium pricing — player-crafted xenogerms retain the base 20 silver value.

**Stock Generation:** `StockGenerator_Xenogerms` is injected into trader defs via XML patches. It queries `DefDatabase<XenotypeDef>` at generation time, filtering by mod settings (archite, inheritable, player-created toggles) and weighting spawn probability inversely by price.

### Pricing Formula

```
MarketValue = 20 (base) + basePresetValue + (|metabolism| × valuePerMetabolism) + (complexity × valuePerComplexity) + (archites × valuePerArchite)
SellPriceFactor = 0.05 (base) + (archites × 0.035)
```

Default pricing settings:
- `basePresetValue`: 1300 (range: 0-3000)
- `valuePerMetabolism`: 10 (range: 0-50)
- `valuePerComplexity`: 15 (range: 0-75)
- `valuePerArchite`: 100 (range: 0-500)

## Save Compatibility

- **Adding mid-game**: New xenogerms appear at traders. Existing xenogerms work normally.
- **Removing mid-game**: CompXenotypeSource data ignored on load. Xenogerms remain valid with stored genes. Implantation gives custom xenotype (graceful fallback).
- **XenotypeDef removed**: `Scribe_Defs.Look` returns null. Xenogerm keeps genes, implants as custom xenotype.

## Debugging

1. **Enable RimWorld Dev Mode:** Settings → Dev Mode → Logging
2. **Log locations:**
   - **Windows:** `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`
   - **WSL:** `/mnt/c/Users/*/AppData/LocalLow/Ludeon Studios/RimWorld by Ludeon Studios/Player.log`
3. **Logging:** Use `Log.Message("[TradersStockXenogerms] ...")` for mod-specific logs
4. **Inspect RimWorld API:** `monodis "/mnt/c/.../RimWorldWin64_Data/Managed/Assembly-CSharp.dll"`

## Harmony Patch Examples

**Postfix Pattern:**

```csharp
[HarmonyPatch(typeof(TargetClass), nameof(TargetClass.MethodName))]
public static class TargetClass_MethodName_Postfix
{
    [HarmonyPostfix]
    public static void Postfix(TargetClass __instance, ref ReturnType __result)
    {
        // __instance: object method was called on
        // __result: return value (modifiable with ref)
    }
}
```

**Prefix Pattern (for skipping original):**

```csharp
[HarmonyPrefix]
public static bool Prefix(ref ReturnType __result)
{
    __result = newValue;
    return false; // Skip original method
}
```

## Key RimWorld APIs

- `XenotypeDef` — Preset xenotype definition with gene list
- `Xenogerm` — Item storing genes (inherits `GeneSetHolderBase`)
- `GeneSet.AddGene()` — Add gene to xenogerm
- `Pawn_GeneTracker.SetXenotypeDirect()` — Set xenotype field without modifying genes
- `DefDatabase<XenotypeDef>.AllDefsListForReading` — All loaded xenotypes
- `Current.Game.customXenotypeDatabase` — Player-created xenotypes from scenario editor
