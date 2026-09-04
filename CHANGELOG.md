# Changelog

All notable changes to Xenogerm Trader Stock will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed

- Xenogerms for a scenario's own germline (inheritable) custom xenotype now implant into the germline like inheritable presets, so the pawn is recognised as that xenotype and its children inherit it. Previously they implanted as xenogenes: a pawn labelled with the xenotype's name that the game, ideoligions and inheritance did not treat as a member. Applies to germs already in saves and to ReSplice copies, since the template is matched by gene set.
- Trader-sold xenogerms for scenario xenotypes are priced like preset xenogerms (the settings grid already showed that price; the item sold for base value).
- Ideoligions with a preferred custom germline xenotype now accept a xenogerm that makes the pawn that xenotype.

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

[1.0.2]: https://github.com/sam-hunt/XenogermTraderStock/releases/tag/v1.0.2
[1.0.1]: https://github.com/sam-hunt/XenogermTraderStock/releases/tag/v1.0.1
[1.0.0]: https://github.com/sam-hunt/XenogermTraderStock/releases/tag/v1.0.0
