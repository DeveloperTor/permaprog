using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using PermaProg.PermaProgCode.Relics;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Config;
using HarmonyLib;

namespace PermaProg.PermaProgCode.Patches;

[HarmonyPatch]
public static class PlayerPatches
{
    [HarmonyPatch(typeof(Player), "CreateForNewRun", [typeof(CharacterModel), typeof(UnlockState), typeof(ulong)])]
    [HarmonyPrefix]
    public static void StartNewRun(Player __instance)
    {
        MF.Log.Info("Starting new run");
        PP.RunOngoing = false;

        foreach (var upg in PP.Upgrades.All.Keys.Where(upg => upg.CurrentLevel > 0))
        {
            MF.Log.Info($"{upg.CurrentLevelName} is level {upg.CurrentLevel}");
        }

        PP.TotalCurrencyGainedDuringRun = 0;
        ModConfig.SaveDebounced<PP>();
    }

    [HarmonyPatch(typeof(Player), "CreateForNewRun", [typeof(CharacterModel), typeof(UnlockState), typeof(ulong)])]
    [HarmonyPostfix]
    public static void SetRunOngoing(Player __instance)
    {
        MF.Log.Info("Setting RunOngoing to true");
        PP.RunOngoing = true;
    }

    [HarmonyPatch(typeof(Player), "PopulateStartingRelics")]
    [HarmonyPostfix]
    public static void AddRelics(Player __instance)
    {
        MF.Log.Info("Adding Peapod relic");

        var ppRelic = ModelDb.Relic<PpRelic>().ToMutable();
        ppRelic.FloorAddedToDeck = 1;
        __instance.AddRelicInternal(ppRelic, silent: true);

        if (PP.CommonRelicValue)
        {
            var allCommonRelics = ModelDb.RelicPool<SharedRelicPool>().GetUnlockedRelics(__instance.UnlockState)
                .Where(relic => relic.Rarity == RelicRarity.Common).ToList();
            var randomCommonRelicToAdd = allCommonRelics[new Random().Next(allCommonRelics.Count)].ToMutable();
            MF.Log.Info($"Adding random common relic ({randomCommonRelicToAdd})");
            randomCommonRelicToAdd.FloorAddedToDeck = 1;
            __instance.AddRelicInternal(randomCommonRelicToAdd, silent: true);
        }

        if (PP.UncommonRelicValue)
        {
            var allUncommonRelics = ModelDb.RelicPool<SharedRelicPool>().GetUnlockedRelics(__instance.UnlockState)
                .Where(relic => relic.Rarity == RelicRarity.Uncommon).ToList();
            var randomUncommonRelicToAdd = allUncommonRelics[new Random().Next(allUncommonRelics.Count)].ToMutable();
            MF.Log.Info($"Adding random uncommon relic ({randomUncommonRelicToAdd})");
            randomUncommonRelicToAdd.FloorAddedToDeck = 1;
            __instance.AddRelicInternal(randomUncommonRelicToAdd, silent: true);
        }

        if (PP.RareRelicValue)
        {
            var allRareRelics = ModelDb.RelicPool<SharedRelicPool>().GetUnlockedRelics(__instance.UnlockState)
                .Where(relic => relic.Rarity == RelicRarity.Rare).ToList();
            var randomRareRelicToAdd = allRareRelics[new Random().Next(allRareRelics.Count)].ToMutable();
            MF.Log.Info($"Adding random rare relic ({randomRareRelicToAdd})");
            randomRareRelicToAdd.FloorAddedToDeck = 1;
            __instance.AddRelicInternal(randomRareRelicToAdd, silent: true);
        }
    }
}