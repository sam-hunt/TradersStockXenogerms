using HarmonyLib;
using UnityEngine;
using Verse;

namespace TradersStockXenogerms
{
    public class TradersStockXenogermsMod : Mod
    {
        public static TradersStockXenogermsSettings Settings { get; private set; }

        public TradersStockXenogermsMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<TradersStockXenogermsSettings>();

            var harmony = new Harmony("shunter.tradersstockxenogerms");
            harmony.PatchAll();
            Log.Message("[TradersStockXenogerms] Mod loaded.");
        }

        public override string SettingsCategory()
        {
            return "Traders Stock Xenogerms";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled(
                "Include archite xenotypes",
                ref Settings.includeArchiteXenotypes,
                "Allow traders to sell xenogerms for xenotypes containing archite genes (e.g., Sanguophage). These are typically more expensive.");

            listing.CheckboxLabeled(
                "Include inheritable xenotypes",
                ref Settings.includeInheritableXenotypes,
                "Allow traders to sell xenogerms for germline xenotypes (e.g., Impid, Yttakin). Implanting these creates 'naturalized' members recognized by ideology.");

            listing.CheckboxLabeled(
                "Include player-created xenotypes",
                ref Settings.includePlayerCreatedXenotypes,
                "Allow traders to sell xenogerms for xenotypes you created in the scenario editor. Disabled by default since traders wouldn't logically have access to your custom designs.");

            listing.GapLine(16f);
            listing.Label("Xenogerm Pricing");
            Text.Font = GameFont.Tiny;
            listing.Label("Adjust the market value formula for trader-sold xenogerms. Changes affect new xenogerms only.");
            Text.Font = GameFont.Small;
            listing.Gap(6f);

            Settings.basePresetValue = (float)System.Math.Round(listing.SliderLabeled(
                $"Base preset value: {Settings.basePresetValue:F0}",
                Settings.basePresetValue,
                TradersStockXenogermsSettings.MinBasePresetValue,
                TradersStockXenogermsSettings.MaxBasePresetValue,
                tooltip: $"Base silver value added to all preset xenotype xenogerms. Default: {TradersStockXenogermsSettings.DefaultBasePresetValue}"));

            Settings.valuePerMetabolism = (float)System.Math.Round(listing.SliderLabeled(
                $"Value per metabolism: {Settings.valuePerMetabolism:F0}",
                Settings.valuePerMetabolism,
                TradersStockXenogermsSettings.MinValuePerMetabolism,
                TradersStockXenogermsSettings.MaxValuePerMetabolism,
                tooltip: $"Silver value added per point of absolute metabolism. Default: {TradersStockXenogermsSettings.DefaultValuePerMetabolism}"));

            Settings.valuePerComplexity = (float)System.Math.Round(listing.SliderLabeled(
                $"Value per complexity: {Settings.valuePerComplexity:F0}",
                Settings.valuePerComplexity,
                TradersStockXenogermsSettings.MinValuePerComplexity,
                TradersStockXenogermsSettings.MaxValuePerComplexity,
                tooltip: $"Silver value added per point of genetic complexity. Default: {TradersStockXenogermsSettings.DefaultValuePerComplexity}"));

            Settings.valuePerArchite = (float)System.Math.Round(listing.SliderLabeled(
                $"Value per archite gene: {Settings.valuePerArchite:F0}",
                Settings.valuePerArchite,
                TradersStockXenogermsSettings.MinValuePerArchite,
                TradersStockXenogermsSettings.MaxValuePerArchite,
                tooltip: $"Silver value added per archite gene. Default: {TradersStockXenogermsSettings.DefaultValuePerArchite}"));

            listing.End();
        }
    }
}
