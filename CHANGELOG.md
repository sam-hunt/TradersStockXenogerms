# Changelog

All notable changes to Xenogerm Trader Stock will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.3] - 2026-09-05

### Added

- Dev-mode tool to rebuild a pawn's genes as a chosen scenario xenotype; repairs pawns affected by the fix below.

### Fixed

- Xenogerms for a scenario's own inheritable xenotype now implant into the germline, so the pawn is recognised as that xenotype and its children inherit it.
- Trader-sold xenogerms for scenario xenotypes are priced like preset xenogerms instead of base value.
- Ideoligions with a preferred custom germline xenotype now accept xenogerms for that xenotype.

## [1.0.2] - 2026-09-04

### Fixed

- Xenogerms copied by ReSplice: Core's duplicator retain their xenotype identity on implantation.

## [1.0.1] - 2026-09-03

### Fixed

- Ideoligions with a preferred xenotype no longer refuse xenogerms for that very xenotype.

## [1.0.0] - 2026-09-02

Initial release.

### Added

- Orbital exotic goods traders sell ready-made xenogerms for preset xenotypes.
- Implanting one assigns the real xenotype, so ideology recognition works.
- Inheritable xenotypes can be implanted into the germline, so children inherit them.
- Baseliner xenogerms, for converting a pawn back to baseliner.
- Deathrest capacity now survives implantation instead of resetting to 1.
- Modded and player-scenario xenotypes are detected and sold automatically.
- Settings for pricing, per-xenotype availability, spawn rarity, and stock counts.
- Compatible with Gene Trader; VRE Android xenotypes are excluded.
- Nine translations: Chinese (Simplified/Traditional), French, German, Japanese, Korean, Brazilian Portuguese, Russian, Spanish.

[1.0.3]: https://github.com/sam-hunt/XenogermTraderStock/releases/tag/v1.0.3
[1.0.2]: https://github.com/sam-hunt/XenogermTraderStock/releases/tag/v1.0.2
[1.0.1]: https://github.com/sam-hunt/XenogermTraderStock/releases/tag/v1.0.1
[1.0.0]: https://github.com/sam-hunt/XenogermTraderStock/releases/tag/v1.0.0
