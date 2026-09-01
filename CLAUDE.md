# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Xenogerm Trader Stock** is a RimWorld 1.6 mod that adds xenogerms for preset xenotypes to trader inventories, allowing players to purchase pre-made xenogerms as an alternative to collecting individual genes. Requires the Biotech DLC.

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
dotnet build XenogermTraderStock.sln -c Release

# Build only the main project
dotnet build Source/1.6/XenogermTraderStock.csproj

# Full clean rebuild
dotnet clean XenogermTraderStock.sln && dotnet build XenogermTraderStock.sln -c Release

# Run the test suite (native; vstest hosts the net472 suite via mono)
dotnet test Tests/1.6/XenogermTraderStock.Tests.csproj
```

The build system auto-detects the RimWorld installation path on Windows/Linux/Mac (including WSL targeting a Windows install). For CI builds without RimWorld installed, it falls back to the `Krafs.Rimworld.Ref` NuGet package.

### Deployment

The repo lives in `~/dev/XenogermTraderStock`, separate from the RimWorld Mods folder. The csproj's `StageMod` target is the **single source of truth** for what files ship: its ItemGroup feeds both the post-build local deploy (`DeployToModFolder` → `StageMod`, an atomic wipe+recopy of `$RIMWORLD_PATH/Mods/XenogermTraderStock/`, so renamed/deleted files never linger) and the CI release, which invokes the same target with `-p:StageDir=...` so the release zip cannot drift from local deploys. Add/remove shipped files only in that ItemGroup.

A machine-local Claude Code Stop hook (`.claude/hooks/sync-mod.sh`, untracked) rebuilds and redeploys after any turn that touched mod files, so the deployed copy stays fresh without manual builds.

**WSL Setup:** Requires `RIMWORLD_PATH` env var in `~/.bashrc` pointing to the Windows RimWorld install (e.g., `/mnt/c/Program Files (x86)/Steam/steamapps/common/RimWorld`). The csproj auto-detects `RimWorldWin64_Data` when the Linux data folder isn't found.

## Architecture

### Entry Point

`Source/1.6/Core/XenogermTraderStockMod.cs` — `XenogermTraderStockMod` constructor loads settings and patches via `Harmony.PatchAll()` attribute discovery.

**Patch-timing hazard (other mods' methods):** that `PatchAll()` runs from the `Mod` subclass constructor — BEFORE any defs are loaded. Applying a detour JIT-compiles the target and runs its declaring type's static ctor, so a patch targeting ANOTHER MOD's method can permanently break that mod when its cctor resolves defs (the BetterTradersGuild v1.1.0 CWTL incident). The current patch targets vanilla `GeneUtility` (safe); before ever adding a foreign-target patch, defer its application until after defs load — worked example: BetterTradersGuild's `Core/DeferredModPatches.cs`.

**Settings Access:** `XenogermTraderStockMod.Settings` provides global access to mod configuration.

**Settings window (family pattern):** `XenogermTraderStockSettings` is a partial class split across `Core/Settings/Settings_*.cs`, one file per section owning its fields, `Expose*Settings`, `Reset*Settings` and `Draw*Section`; the frame file holds `DoWindowContents`, the fan-outs and the shared `SectionHeader` / `SliderRow` helpers. Help text is hover-tooltip only (section descriptions ride on the medium-font header) — no always-visible tiny-font sub-labels. Adding a setting is a one-file edit; a new section is a new file plus three one-line calls in the frame.

### Directory Structure

```
Source/1.6/                         # Family layout: one folder per concern, root namespace
│                                   # XenogermTraderStock everywhere except Patches/ (.Patches)
├── Core/
│   ├── XenogermTraderStockMod.cs       # Mod entry point, Harmony init; delegates the window to Settings
│   ├── XenogermTraderStockSettings.cs  # Settings frame: scroll/reset window, Expose/Reset fan-out, row helpers
│   └── Settings/                       # One partial-class file per settings section (fields, scribe, defaults, draw)
│       ├── Settings_Commonality.cs     # Stock-selection strategy radio group
│       ├── Settings_Implantation.cs    # Endogene implant toggle (free-standing behaviour, not a stock filter)
│       ├── Settings_Pricing.cs         # Pricing sliders: defaults, ranges, snap steps
│       ├── Settings_Quantities.cs      # Per-trader-kind xenogerm count overrides (XML countRange = default)
│       └── Settings_Xenotypes.cs       # Per-xenotype sold ledger + the grid section
├── Comps/
│   └── CompXenotypeSource.cs           # ThingComp storing source XenotypeDef
├── Debug/
│   └── XenogermDebugActions.cs         # Dev-mode spawn-xenogerm action; bypasses XenotypeEligibility
├── Patches/                            # Harmony patches, named <Type>_<Method>_Patch
│   └── GeneUtility_ImplantXenogermItem_Patch.cs  # Postfix: xenotype assignment (+ endogene retarget)
├── Stats/
│   ├── XenogermPricing.cs              # Centralized pricing calculations
│   ├── StatPart_XenogermValue.cs       # MarketValue stat calculation
│   └── StatPart_XenogermSellFactor.cs  # SellPriceFactor stat for archite bonus
├── Traders/
│   ├── StockGenerator_Xenogerms.cs     # Trader stock generation with weighted rates
│   ├── XenogermCommonality.cs          # Stateless price->spawn-weight strategies (inverse/inverse-root/linear/sqrt/bell/uniform)
│   └── XenogermFactory.cs              # Creates xenogerm Things from definitions
├── Xenotypes/
│   ├── XenotypeEligibility.cs          # Sellable state: candidacy, category partition, ledger seeding
│   ├── XenotypeLedgerStartup.cs        # Once-per-launch SeedUnseen pass (defName-keyed, reload-safe)
│   └── GeneExtension.cs                # DefModExtension opt-out: genes that bar a xenotype from stock
├── UI/
│   └── XenotypeGridUI.cs               # Settings-window xenotype toggle grid + tri-state category filter rows
├── Properties/
│   └── AssemblyInfo.cs                 # Assembly version metadata

1.6/Patches/                        # XML patches
├── Patches_Traders.xml             # Adds StockGenerator_Xenogerms to trader defs
├── Patches_Xenogerm.xml            # Adds CompXenotypeSource and StatParts to xenogerm def
├── Patches_Stats.xml               # Stat definitions for pricing
├── Patches_GeneTrader.xml          # Gene Trader mod compatibility patch
└── Patches_VREAndroid.xml          # Flags VREA android genes with GeneExtension (never sold)

1.6/Languages/English/Keyed/
└── XTS_UI.xml                      # All player-facing strings (XTS_ key prefix)

Scripts/
├── check-translations.py           # Deterministic localization validator (CI release gate)
├── refresh-translation-expectations.py  # Regenerates the sidecar via ../L10nProbe game boot
└── expected-injections.json        # Checked-in DefInjected expectations sidecar
```

### .claude layout

`.gitignore` tracks only `.claude/skills/` (shared: `release`, `translate`, `rimworld-logs`); `.claude/hooks/` and `.claude/settings.local.json` (Stop-hook wiring, permissions) stay machine-local.

### Core Mechanism

The mod stores a `XenotypeDef` reference on trader-sold xenogerms via `CompXenotypeSource`. When implanted, a Harmony postfix patch on `GeneUtility.ImplantXenogermItem` calls `SetXenotypeDirect()` and nulls `iconDef`, leaving the gene tracker's identity fields (`xenotype`, `xenotypeName`, `iconDef`) exactly as `PawnGenerator` leaves them for a generated pawn of that xenotype (vanilla only copies genes, display name, and the item's icon — which `Xenogerm.ExposeData` backfills to the custom-xenotype `Basic` icon on load). `hybrid` is deliberately untouched: it describes the germline, which an implant does not change.

**Germline retarget (default on):** with `Settings.implantGermlineAsEndogenes` (free-standing on purpose: it acts at implant time on items the player already owns, so it must not depend on what the shop grid currently has ticked) a trader-sold xenogerm for an *inheritable* xenotype has the pawn's endogenes replaced wholesale by its genes after the identity fields are set — what `PawnGenerator` produces for a born member, not a merge (a merge would also let old genes win conflicts: two endogenes resolve by display order, `GeneUtility.Overrides` → `GenesInOrder`, not arrival order). The pawn's own melanin/hair-colour genes are re-added only when the xenotype supplies none — the backfill `PawnGenerator` does with random ones. Side effects match a born member: no xenogenes, so no xenogerm extraction.

**Baseliner xenogerm:** Baseliner is `XenotypeEligibility.IsCandidate`'s one gene-less exception and is pinned first in every pool (`InDisplayOrder`, shared by grid and debug spawner). It gates as *inheritable* (`GatesAsInheritable`) despite `inheritable=false` on the def, because its implant always rewrites the germline — so it groups under the germline filter row and seeds unsold by default like the inheritable xenotypes. Its empty xenogerm is the baseliner-conversion item: vanilla implantation wipes all xenogenes unconditionally (`SetXenotype` clears before consulting the empty gene list — verified safe end-to-end, no gene-count gate in the float-menu/bill/recipe chain), and the patch always runs the germline retarget for it regardless of the setting (no genes means no "keep as xenogenes" alternative for the setting to choose; skin/hair colour survives via the retarget's own backfill). Prices at base + `basePresetValue` (no gene stats).

### Key Classes

| Class | Purpose |
|-------|---------|
| `XenogermTraderStockMod` | Mod entry point, Harmony init |
| `XenogermTraderStockSettings` | Persisted mod settings and the settings window (partial class, one file per section under `Core/Settings/`) |
| `CompXenotypeSource` | ThingComp storing source XenotypeDef on xenogerms |
| `GeneUtility_ImplantXenogermItem_Patch` | Harmony postfix assigning xenotype after implantation |
| `StockGenerator_Xenogerms` | Generates xenogerms for traders with weighted spawn rates |
| `XenotypeEligibility` | Single source of truth for sellable state: candidacy, category partition, sold-ledger seeding |
| `GeneExtension` | `DefModExtension` for GeneDefs; `excludeFromXenogermStock` bars any xenotype containing the gene |
| `XenotypeGridUI` | 4-column settings grid of xenotype toggles (icon, name, live shelf price, description tooltip) + tri-state category filter rows |
| `XenogermFactory` | Creates xenogerm Things from XenotypeDef or CustomXenotype |
| `XenogermPricing` | Centralized pricing calculations (used by StockGenerator and StatPart) |
| `StatPart_XenogermValue` | Calculates MarketValue based on genes |
| `StatPart_XenogermSellFactor` | Adds SellPriceFactor bonus for archite xenogerms |

### Key Patterns

**Harmony Patching:** All patches use `[HarmonyPatch]` attributes for automatic discovery via `Harmony.PatchAll()`.

**Pricing via StatParts:** Market value and sell factor are calculated dynamically using `StatPart` classes rather than fixed values. This allows xenogerm prices to reflect their gene composition. Only xenogerms with `CompXenotypeSource.sourceXenotype != null` get premium pricing — player-crafted xenogerms retain the base 20 silver value.

**Stock Generation:** `StockGenerator_Xenogerms` is injected into trader defs via XML patches. It queries `DefDatabase<XenotypeDef>` at generation time, filtering through `XenotypeEligibility.IsSellable` and weighting spawn probability by the settings-chosen `XenogermCommonality` strategy (default: inverse root, 1/√price; all strategies are stateless and pool-relative — the bell curve centres on the pool's median). The per-visit count comes from the settings' per-trader override (`traderCountRanges`, keyed by `StockGenerator.trader.defName`), falling back to the generator's XML `countRange` — the shipped default lives only in the XML patches, nothing is written onto defs, so count changes need no restart.

**Per-xenotype filtering (sold ledger):** `XenotypeEligibility` is the only place that decides whether a xenotype is sellable; both the generator and the settings grid read it. The state is an explicit per-xenotype sold ledger on settings (`soldXenotypes` by defName, `soldCustomXenotypes` by `CustomXenotype.name`). A xenotype the ledger has never seen gets its entry from `SeedUnseen`: the majority sold state of its category peers (categories are DISJOINT, first match of player-scenario > archite > inheritable > plain), ties falling back to the shipped default — germline-rewriting xenotypes (`GatesAsInheritable`) unsold, everything else sold. An empty ledger makes every vote tie, which is exactly the shipped defaults; `ResetToDefaults` just clears the ledger and lets the next seed pass do the rest. Seeding runs at startup (`XenotypeLedgerStartup`; once per process is deliberate — entries are name-keyed, so an in-process play-data reload can't invalidate them, see the comment there), from the settings grid and from stock generation (the custom pool is per-game). Entries for xenotypes no longer loaded stay dormant — never consulted, never voting, never pruned — so a temporarily disabled mod keeps its choices. The category rows above the grid are pure derivations of the ledger: a tri-state `Widgets.CheckboxMulti` summary (on = all sold / partial = mixed / off = none) that bulk-writes its whole group on click; grid cells are always interactive, drawing dimmed when unsold. Never read the ledger directly from generation code.

**Non-organic genelines (`GeneExtension`):** vanilla has no "organic humanlike" concept — `canGenerateInGeneSet=false` sits on Core hair-colour endogenes (Impid/Waster/Highmate would vanish) and `selectionWeight=0` on Hemogenic (Sanguophage) — so exclusion is an explicit gene-level marker. Any xenotype, preset or custom, containing a gene whose `GeneExtension.excludeFromXenogermStock` is true fails `IsCandidate` and is hidden from the grid outright (not greyed: no setting can restore it). Gene-level because Vanilla Races Expanded - Android's custom "android projects" land in `Current.Game.customXenotypeDatabase` at game start with no Def to patch. `Patches_VREAndroid.xml` flags VREA's two abstract gene bases (XML inheritance appends child `<li>` lists onto the parent's, so every derived gene inherits it, including other mods' `VREA_HardwareBase` children). Other mods opt out with the same `<modExtensions>` snippet.

### Pricing Formula

```
MarketValue = 20 (base) + basePresetValue + (|metabolism| × valuePerMetabolism) + (complexity × valuePerComplexity) + (archites × valuePerArchite)
SellPriceFactor = 0.05 (base) + (archites × 0.035)
```

Default pricing settings:
- `basePresetValue`: 1300 (range: 0-5000)
- `valuePerMetabolism`: 10 (range: 0-100)
- `valuePerComplexity`: 15 (range: 0-100)
- `valuePerArchite`: 100 (range: 0-1000)

The settings grid previews the *shelf price*, `MarketValue × 1.4` (`XenogermPricing.VanillaBuyMarkup` — vanilla's flat buying markup, hardcoded in `TradeUtility.GetPricePlayerBuy`), because in a trader-stock context players read the preview as what they'll pay; the info card keeps raw MarketValue. The cell tooltip's breakdown closes with market value → +40% (buying, shown as the silver it adds so white rows stay additive and yellow rows are the subtotals) → final price, reusing the vanilla trade-tooltip language keys (no XTS strings). Negotiator/settlement bonuses only ever discount from there; faction goodwill never affects vanilla prices.

## Testing

`Tests/1.6/` holds an xUnit (net472) suite for the pure logic: `XenogermPricing` breakdown/market-value math, settings defaults/`ResetToDefaults`, and the `XenogermCommonality` spawn-weight strategies. Tests are headless — no live game context (`GeneDef`s are built uninitialized via `FormatterServices`); anything needing `DefDatabase`/`Current.Game` is out of scope. Run natively with `dotnet test Tests/1.6/XenogermTraderStock.Tests.csproj` — vstest hosts the net472 suite via mono. If a run fails with `BadImageFormatException`/`TypeLoadException`, a DLL is missing from the test csproj copy target (see the Assembly-CSharp-firstpass comment there): mono resolves field types eagerly where the Windows CLR is lazy. CI builds the Tests project but does not run it.

**Startup smoke test (pre-release):** `python3 Scripts/integration-smoke-test.py` (game closed) boots the *deployed* copy of the mod on its pinned minimal list (Biotech plus VEF core + Vanilla Races Expanded - Android, so `Patches_VREAndroid.xml`'s `FindMod` branch actually executes) — it does not build, and only `-c Release` builds deploy, so run the Release build first or it silently tests the previous DLL. It then classifies Player.log errors by origin and fails on anything attributed to this mod. Run before every release (wired into the release skill); thin shim over the shared engine in `l10n/smoke/` (born from the BetterTradersGuild v1.1.0 CWTL incident).

## Localization

English (`1.6/Languages/English/Keyed/XTS_UI.xml`, `XTS_` prefix) is the source of truth. The pipeline is shared with the sibling mod repos (`../UniqueMeleeWeapons`, `../UniqueWeaponsUnbound`, `../PersonaWeaponsUnbound`, `../BetterTradersGuild`):

- **Shared l10n toolkit (`l10n/` submodule):** the family-wide translation process, per-language mechanics references, cross-language lessons, Workshop conventions, and the checker/refresh script engines live in the `rimworld-l10n` repo, consumed here as the `l10n/` git submodule (canonical working checkout: `~/dev/rimworld-l10n`). `Scripts/check-translations.py` and `Scripts/refresh-translation-expectations.py` are thin per-repo config shims over its engines. If `l10n/` is empty, run `git submodule update --init`. Never edit `l10n/` in place here: mod-independent learnings go upstream in the canonical checkout; mod-specific learnings go in this repo's skill/glossary. Upstream ships as semver release tags (`vMAJOR.MINOR.PATCH`; a major means this repo's shim or flow needs an edit), and the pin here moves only at release (release skill step 2), at the start of a translation pass, or when a new major lands, never per upstream commit, so `git submodule status` names the pinned tag and a stable repo's log stays free of pin bumps.
- `python3 Scripts/check-translations.py [--strict]` — deterministic validator (key/placeholder parity, `<!-- EN: ... -->` staleness comments, DefInjected paths, file hygiene). Run by the `translate` and `release` skills and as a CI release gate.
- `Scripts/expected-injections.json` — checked-in sidecar of every DefInjected key the live game expects for this mod; regenerated by `python3 Scripts/refresh-translation-expectations.py` (boots RimWorld with a pinned mod list via the L10nProbe dev mod — source now lives at `l10n/probe/`; build/deploy it only from the canonical `~/dev/rimworld-l10n` checkout — then restores `ModsConfig.xml`; refuses while the game is open). Currently empty: this mod ships no Defs of its own, only patches.
- **Probe DLC set:** the probe boots with Biotech active (`CANONICAL_ACTIVE_MODS` in the refresh script); the checker's `REQUIRED_DLCS` rejects a sidecar generated without it, since gated defs would drop out of the dump and their shipped translations would turn illegal.
- The `translate` skill's `.claude/skills/translate/glossary/` holds this mod's own coined-term files; per-language grammar/mechanics knowledge now lives upstream in `l10n/languages/<Language>.md`. CONTRIBUTING.md carries the public roster (nine machine-assisted languages plus English) and must move in the same commit as any language change.
- **Workshop title coupling:** each language's `XTS_SettingsCategory` Keyed value is the localized Steam Workshop title and must equal the title line (line 1) of `.steamworkshop/Description/<Language>.txt` — always change the two together (English keeps `Xenogerm Trader Stock` in both).
- **Policy:** translation generation passes run only on explicit request (they are token-expensive). Infra/tooling changes are always fine.

## Linting

Roslynator.Analyzers runs on every build (warnings only, never fails the build; `PrivateAssets=all` so nothing ships). Severities are pinned in `.editorconfig`, which also enforces PascalCase public members (fields excluded — RimWorld's `Scribe_Values.Look` relies on camelCase field names) and the no-XML-doc-comments convention (plain `//` only). Formatting-only sweeps are registered in `.git-blame-ignore-revs`.

## Save Compatibility

- **Adding mid-game**: New xenogerms appear at traders. Existing xenogerms work normally.
- **Removing mid-game**: CompXenotypeSource data ignored on load. Xenogerms remain valid with stored genes. Implantation gives custom xenotype (graceful fallback).
- **XenotypeDef removed**: `Scribe_Defs.Look` returns null. Xenogerm keeps genes, implants as custom xenotype.

## Debugging

Use the `rimworld-logs` skill — it covers Player.log locations (Windows/WSL/Linux), the `[Xenogerm Trader Stock]` log prefix (the mod display name, as vanilla uses for patch/def errors about the mod), and API disassembly (`monodis`/`ilspycmd` against the live install's `Assembly-CSharp.dll`, preferred over the `Krafs.Rimworld.Ref` CI fallback).

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
- `Current.Game.customXenotypeDatabase` — Player-scenario xenotypes (the custom xenotypes the starting pawns carried in; filled by `GameInitData.PrepForMapGen`)
