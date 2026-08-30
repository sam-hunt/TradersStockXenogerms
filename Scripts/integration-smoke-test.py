#!/usr/bin/env python3
# Pre-release integration smoke test: boots the real game once with XTS on
# its pinned minimal list (Core, Harmony, Biotech, XTS itself - no optional
# integration mods), where the baseline is a clean log, then classifies
# every Player.log error/warning by origin and fails on anything attributed
# to XTS. Thin shim over the shared engine in l10n/smoke/startup_smoke.py
# (see its header for mechanics and the BetterTradersGuild v1.1.0 CWTL
# incident this exists to catch).
#
# With no integration mods on the list, there is nothing to attribute an
# error to except this mod or the engine itself - so this shim is a plain
# clean-startup-log gate rather than an integration-seam check.
#
# Run this before every release, with the game closed:
#   python3 Scripts/integration-smoke-test.py              # boot + scan
#   python3 Scripts/integration-smoke-test.py --no-launch  # rescan last log
#   python3 Scripts/integration-smoke-test.py --strict     # any error fails

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "l10n" / "smoke"))
import startup_smoke as engine  # noqa: E402

engine.REPO_ROOT = Path(__file__).resolve().parent.parent

engine.PACKAGE_ID = "shunter.xenogermtraderstock"

# RATIONALE: this is this repo's l10n CANONICAL_ACTIVE_MODS list - Biotech is
# XTS's hard dep (xenogerms are a Biotech concept) and the only DLC the probe
# needs active - plus the one optional integration seam: Vanilla Races Expanded
# - Android (needs VEF core), whose gene bases Patches_VREAndroid.xml patches
# under a PatchOperationFindMod. Without VREA active that patch never runs, so
# a broken xpath would only ever surface in a player's log. Probe last
# (auto-quit).
engine.SMOKE_ACTIVE_MODS = [
    "brrainz.harmony",
    "ludeon.rimworld",
    "ludeon.rimworld.biotech",
    "oskarpotocki.vanillafactionsexpanded.core",
    "vanillaracesexpanded.android",
    "shunter.xenogermtraderstock",
    "shunter.l10nprobe",
]

engine.OWN_PATTERNS = ["XenogermTraderStock", "[Xenogerm Trader Stock]", "XTS_"]

# The other mods' namespaces/prefixes: an error mentioning any of these gates
# the test even when the exception fires inside their code.
engine.INTEGRATION_PATTERNS = {
    "VREA": ["VREAndroids"],
    "VEF": ["VEF."],
}

raise SystemExit(engine.main())
