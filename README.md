# Traders Stock Xenogerms

[![RimWorld](https://img.shields.io/badge/RimWorld-1.6-blue.svg)](https://rimworldgame.com/)
[![Biotech DLC](https://img.shields.io/badge/DLC-Biotech%20Required-orange.svg)](https://store.steampowered.com/app/1826140/RimWorld__Biotech/)
[![Version](https://img.shields.io/badge/Version-1.0.0-brightgreen.svg)](https://github.com/sam-hunt/TradersStockXenogerms/releases)
[![Development Status](https://img.shields.io/badge/Status-Stable-brightgreen.svg)](https://github.com/sam-hunt/TradersStockXenogerms/releases)

## About

In vanilla RimWorld with Biotech, the only way to get a specific xenotype is to collect individual genes and assemble a xenogerm yourself — a slow and expensive process. Xenogerms exist as items but traders never sell pre-made ones for preset xenotypes.

This mod adds complete xenogerms to trader inventories, letting you purchase a ready-made xenotype package instead of hunting down each gene individually.

## Features

### Xenogerm Trading

- **Preset xenogerms at traders**: Orbital exotic goods traders stock xenogerms for all preset xenotypes (Hussar, Impid, Sanguophage, etc.)
- **Proper xenotype assignment**: Purchased xenogerms assign the actual xenotype on implantation, enabling ideology recognition and creating "naturalized" members of endogene-only xenotypes
- **Weighted spawn rates**: Cheaper xenogerms appear more frequently, while expensive archite xenotypes like Sanguophage are rarer finds

### Pricing

Xenogerm prices are based on gene complexity, metabolism impact, and archite gene count, e.g:
- Pigskin, Dirtmole: ~1,500 silver
- Hussar, Highmate: ~1,600-1,850 silver
- Sanguophage: ~3,200 silver

Sell prices are intentionally low (5%) to prevent buy-sell exploits, with a bonus for archite xenogerms.

### Mod Settings

- **Include archite xenotypes** — Toggle xenogerms containing archite genes (default: on)
- **Include inheritable xenotypes** — Toggle germline xenotypes like Impid and Yttakin (default: on)
- **Include player-created xenotypes** — Toggle xenotypes from scenario editor (default: off)
- **Xenogerm Pricing** — Customize the price formula by adjusting base preset value and multipliers for metabolism, complexity, and archite genes

## Requirements

- **RimWorld 1.6** or later
- **Biotech DLC** (required)

## Installation

### Steam Workshop (Recommended)

Subscribe on the [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=TODO) and it will auto-download.

### Manual Installation

1. Download the latest release from the [Releases](https://github.com/sam-hunt/TradersStockXenogerms/releases) page
2. Extract the `TradersStockXenogerms` folder to your RimWorld `Mods` directory:
   - **Windows**: `C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\`
   - **Mac**: `~/Library/Application Support/Steam/steamapps/common/RimWorld/RimWorldMac.app/Mods/`
   - **Linux**: `~/.steam/steam/steamapps/common/RimWorld/Mods/`
3. Enable the mod in RimWorld's mod menu
4. Restart RimWorld

## Compatibility

- **Gene Trader mod** (`tac.genetrader`) — Fully supported. If installed, the orbital gene trader will also stock xenogerms.
- **Save-safe** — Can be added or removed mid-game without issues.
- **Xenotype mods** — Automatically includes xenotypes from other mods.

## Contributing

Bug reports and feature requests welcome on [GitHub Issues](https://github.com/sam-hunt/TradersStockXenogerms/issues).
Please attach any relevant logs/stack traces/mod lists etc.

For development setup, see [CLAUDE.md](CLAUDE.md).

## Credits

**Author**: Sam Hunt ([@sam-hunt](https://github.com/sam-hunt))

**Built With**:

- [Harmony](https://github.com/pardeike/Harmony) by Andreas Pardeike — Runtime patching library
- RimWorld modding API, community examples

**Special Thanks**:

- [Ludeon Studios](https://ludeon.com) for RimWorld and modding API
- [The RimWorld modding community](https://steamcommunity.com/app/294100/workshop/) for inspiration and working examples
- [Claude Code](https://claude.com/claude-code) for wading through `monodis` output and breathing C#

## License

MIT
