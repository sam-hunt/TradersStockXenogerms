using System.Collections.Generic;
using Verse;

namespace XenogermTraderStock
{
    public class XenogermTraderStockSettings : ModSettings
    {
        public bool includeArchiteXenotypes = DefaultIncludeArchiteXenotypes;
        public bool includeInheritableXenotypes = DefaultIncludeInheritableXenotypes;
        public bool includePlayerScenarioXenotypes = DefaultIncludePlayerScenarioXenotypes;

        // Per-xenotype opt-outs. The settings UI presents these as a whitelist
        // (checked = sold), but they are stored as a blacklist so the default is
        // "everything on" and a xenotype added or removed by another mod needs no
        // migration: unknown names are simply never matched. Preset xenotypes are
        // keyed by defName, player-scenario ones by CustomXenotype.name (they have
        // no def). Read through XenotypeEligibility rather than directly - these
        // are only one input to the derived sellable state.
        public HashSet<string> excludedXenotypes = new HashSet<string>();
        public HashSet<string> excludedCustomXenotypes = new HashSet<string>();

        // Pricing constants with defaults matching original values
        public float basePresetValue = DefaultBasePresetValue;
        public float valuePerMetabolism = DefaultValuePerMetabolism;
        public float valuePerComplexity = DefaultValuePerComplexity;
        public float valuePerArchite = DefaultValuePerArchite;

        // Default values
        public const bool DefaultIncludeArchiteXenotypes = true;
        public const bool DefaultIncludeInheritableXenotypes = false;
        public const bool DefaultIncludePlayerScenarioXenotypes = true;
        public const float DefaultBasePresetValue = 1300f;
        public const float DefaultValuePerMetabolism = 10f;
        public const float DefaultValuePerComplexity = 15f;
        public const float DefaultValuePerArchite = 100f;

        // Slider ranges and snap steps. Steps are sized to the silver a single
        // notch moves on a typical xenogerm (a few dozen silver against a
        // ~1,500 price), not to the raw unit: a 1-silver notch on a 0-3000 range
        // is unlandable by mouse and meaningless in play. Every default must sit
        // on its step grid so the "(default)" suffix is reachable by dragging.
        public const float MinBasePresetValue = 0f;
        public const float MaxBasePresetValue = 3000f;
        public const float StepBasePresetValue = 50f;
        public const float MinValuePerMetabolism = 0f;
        public const float MaxValuePerMetabolism = 50f;
        public const float StepValuePerMetabolism = 5f;
        public const float MinValuePerComplexity = 0f;
        public const float MaxValuePerComplexity = 75f;
        public const float StepValuePerComplexity = 5f;
        public const float MinValuePerArchite = 0f;
        public const float MaxValuePerArchite = 500f;
        public const float StepValuePerArchite = 25f;

        public void ResetToDefaults()
        {
            includeArchiteXenotypes = DefaultIncludeArchiteXenotypes;
            includeInheritableXenotypes = DefaultIncludeInheritableXenotypes;
            includePlayerScenarioXenotypes = DefaultIncludePlayerScenarioXenotypes;
            basePresetValue = DefaultBasePresetValue;
            valuePerMetabolism = DefaultValuePerMetabolism;
            valuePerComplexity = DefaultValuePerComplexity;
            valuePerArchite = DefaultValuePerArchite;
            excludedXenotypes.Clear();
            excludedCustomXenotypes.Clear();
        }

        public bool IsXenotypeExcluded(string defName)
        {
            return excludedXenotypes.Contains(defName);
        }

        public void SetXenotypeExcluded(string defName, bool excluded)
        {
            SetMembership(excludedXenotypes, defName, excluded);
        }

        public bool IsCustomXenotypeExcluded(string name)
        {
            return excludedCustomXenotypes.Contains(name);
        }

        public void SetCustomXenotypeExcluded(string name, bool excluded)
        {
            SetMembership(excludedCustomXenotypes, name, excluded);
        }

        private static void SetMembership(HashSet<string> set, string key, bool member)
        {
            if (key == null)
            {
                return;
            }
            if (member)
            {
                set.Add(key);
            }
            else
            {
                set.Remove(key);
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref includeArchiteXenotypes, "includeArchiteXenotypes", DefaultIncludeArchiteXenotypes);
            Scribe_Values.Look(ref includeInheritableXenotypes, "includeInheritableXenotypes", DefaultIncludeInheritableXenotypes);
            Scribe_Values.Look(ref includePlayerScenarioXenotypes, "includePlayerScenarioXenotypes", DefaultIncludePlayerScenarioXenotypes);

            Scribe_Values.Look(ref basePresetValue, "basePresetValue", DefaultBasePresetValue);
            Scribe_Values.Look(ref valuePerMetabolism, "valuePerMetabolism", DefaultValuePerMetabolism);
            Scribe_Values.Look(ref valuePerComplexity, "valuePerComplexity", DefaultValuePerComplexity);
            Scribe_Values.Look(ref valuePerArchite, "valuePerArchite", DefaultValuePerArchite);

            Scribe_Collections.Look(ref excludedXenotypes, "excludedXenotypes", LookMode.Value);
            Scribe_Collections.Look(ref excludedCustomXenotypes, "excludedCustomXenotypes", LookMode.Value);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                // Scribe_Collections nulls the target when the node is absent
                // (settings files written before the blacklist existed).
                excludedXenotypes ??= new HashSet<string>();
                excludedCustomXenotypes ??= new HashSet<string>();
            }

            base.ExposeData();
        }
    }
}
