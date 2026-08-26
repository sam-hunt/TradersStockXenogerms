#!/usr/bin/env python3
# XenogermTraderStock's config shim over the shared sidecar-refresh engine
# (l10n/refresh/refresh_expectations.py — the rimworld-l10n submodule),
# which drives the L10nProbe dev mod (source at l10n/probe/; build/deploy it
# only from the canonical ~/dev/rimworld-l10n checkout). The engine holds all
# logic; this file holds only this repo's config and the rationale behind it.
# Usage is unchanged (game must be closed):
#   python3 Scripts/refresh-translation-expectations.py [--no-launch]
# If l10n/ is empty, run: git submodule update --init

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "l10n" / "refresh"))
import refresh_expectations as engine  # noqa: E402  (import after sys.path edit)

engine.REPO_ROOT = Path(__file__).resolve().parent.parent

engine.PACKAGE_ID = "shunter.xenogermtraderstock"

# RATIONALE: Biotech is a hard dependency (About.xml's modDependencies); no
# other DLC gates any content here. Unlike the UniqueMeleeWeapons family
# (three independent mods riding along in one boot for convenience), this
# mod is not part of a probed family, so the list has no siblings to add.
# Gene Trader (tac.genetrader) is soft-patched (Patches_GeneTrader.xml,
# PatchOperationFindMod) but adds no label/description — no translatable
# keys depend on it being active, so it is left out. See the engine's header
# for the general membership rule, the lowercase-id warning, and the
# pinning rationale; order is load order, the probe last.
engine.CANONICAL_ACTIVE_MODS = [
    "brrainz.harmony",
    "ludeon.rimworld",
    "ludeon.rimworld.biotech",
    "shunter.xenogermtraderstock",
    "shunter.l10nprobe",
]

raise SystemExit(engine.main())
