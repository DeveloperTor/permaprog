using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Achievements;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Odds;
using MegaCrit.Sts2.Core.Runs;
using BaseLib.Config;
using HarmonyLib;
using Godot;

namespace PermaProg.PermaProgCode.Patches;

[HarmonyPatch]
public static class PermaProgPatches
{
    [HarmonyPatch(typeof(RunState), "CreateForNewRun")]
    [HarmonyPostfix]
    public static void CreateForNewRun(RunState __result)
    {
        PP.CurrentRunAscensionLevel = __result.AscensionLevel;
    }

    [HarmonyPatch(typeof(GoldReward), MethodType.Constructor, [typeof(int), typeof(int), typeof(Player), typeof(bool)])]
    [HarmonyPrefix]
    public static void IncreaseGoldRewardDuringRun(ref int min, ref int max, Player player)
    {
        var balancingMultiplier = PP.BalancingEnabled ? 0.9 : 1.0;
        min = (int)Math.Round(min * balancingMultiplier * (1.0 + PP.GoldGainValue / 100.0));
        max = (int)Math.Round(max * balancingMultiplier * (1.0 + PP.GoldGainValue / 100.0));
    }

    [HarmonyPatch(typeof(CardRarityOdds), "GetBaseOdds")]
    [HarmonyPostfix]
    public static void IncreaseCardRarityOdds(ref float __result, CardRarityOddsType type, CardRarity rarity)
    {
        if (PP.CardRarityValue <= 0.1) return;
        MF.Log.Info($"Boosting card rarity odds by {(int)PP.CardRarityValue}%");
        __result *= 1.0f + (float)PP.CardRarityValue / 100.0f;
    }

    [HarmonyPatch(typeof(PlayerCmd), "GainGold")]
    [HarmonyPrefix]
    public static void GainCurrencyDuringRun(decimal amount, Player player, bool wasStolenBack)
    {
        var multiplier = 1.0 + PP.CurrencyGainValue / 100.0;
        multiplier += PP.CurrentRunAscensionLevel * PP.AscensionCurrencyValue / 100.0;
        MF.Log.Info($"Currency to gain: {(int)((double)amount * multiplier)} from {amount} gold " +
                    $"with multiplier {multiplier.ToString()[..4]}");
        PP.CurrencyToGain += (int)((double)amount * multiplier);
        MF.CurrencyLabel?.SetTextAutoSize((PP.CurrencyAvailable + PP.CurrencyToGain).ToString().PadLeft(7));
    }

    [HarmonyPatch(typeof(SaveManager), "SaveRun")]
    [HarmonyPostfix]
    public static void IncrementTotalCurrencyGained(AbstractRoom? preFinishedRoom, bool saveProgress)
    {
        if (PP.CurrencyToGain <= 0) return;
        MF.Log.Info("Adding via 'SaveRun'");
        SaveCurrency();
    }

    [HarmonyPatch(typeof(AbstractRoom), "Enter")]
    [HarmonyPostfix]
    public static void OnRoomEnter(AbstractRoom __instance)
    {
        MF.Log.Info("Adding via 'OnRoomEnter'");
        PP.CurrencyToGain += (int)PP.TravelCurrencyValue;
        SaveCurrency();
    }

    [HarmonyPatch(typeof(AchievementsHelper), "AfterRunEnded")]
    [HarmonyPrefix]
    public static void SaveDataAtEndOfRun(RunState state, Player player, bool isVictory)
    {
        MF.Log.Info("Run ended.");
        ApplyInterest(state);
        AddLastCurrencyRewardToAvailableCurrency();

        MF.Log.Info("Setting RunOngoing to false");
        PP.RunOngoing = false;
    }

    [HarmonyPatch(typeof(NGameOverScreen), "AddScoreLine")]
    [HarmonyPrefix]
    public static void ChangeGoldToCurrency(string locEntryKey, string? locAmountKey, ref int amount, string? iconPath)
    {
        if (locEntryKey != "SCORE_LINE.goldGained") return;
        MF.Log.Info($"Exchange end-of-run gold ({amount}) to currency gained ({PP.TotalCurrencyGainedDuringRun})");
        amount = PP.TotalCurrencyGainedDuringRun;
    }

    [HarmonyPatch(typeof(NScoreLine), "Create")]
    [HarmonyPrefix]
    public static void ChangeGoldScoreTextToCurrency(ref string label, Texture2D? icon)
    {
        if (!label.Contains("Gold")) return;
        MF.Log.Info("Exchange end-of-run 'Gold gained' text to 'Currency Gained'");
        label = label.Replace("Gold", "Currency");
    }

    [HarmonyPatch(typeof(NMainMenu), "OnContinueButtonPressed")]
    [HarmonyPostfix]
    public static void ContinueRunFromMainMenu(NButton _)
    {
        MF.Log.Info("Run continued from main menu. Setting RunOngoing to true");
        PP.CurrencyToGain = 0;
        PP.RunOngoing = true;

        foreach (var upg in PP.Upgrades.All.Keys.Where(upg => upg.CurrentLevel > 0))
        {
            MF.Log.Info($"{upg.CurrentLevelName} is level {upg.CurrentLevel}");
        }
    }

    [HarmonyPatch(typeof(NMainMenu), nameof(NMainMenu.AbandonRun))]
    [HarmonyPostfix]
    public static void AbandonRunFromMainMenu(NMainMenu __instance)
    {
        MF.Log.Info("Run abandoned from main menu. Setting RunOngoing to false");
        PP.CurrencyToGain = 0;
        PP.RunOngoing = false;
    }

    // Helpers
    private static void ApplyInterest(RunState state)
    {
        if (state.CurrentActIndex < 3 || PP.CurrencyInterestValue <= 0.1) return;

        var interest = (PP.CurrencyAvailable - PP.TotalCurrencyGainedDuringRun) * PP.CurrencyInterestValue / 100.0;
        interest = Math.Clamp(interest, 0.0, 3000.0);
        MF.Log.Info($"Gain {(int)interest} in interest");
        PP.TotalCurrencyGainedDuringRun += (int)interest;
        PP.CurrencyAvailable += (int)interest;
    }

    private static void AddLastCurrencyRewardToAvailableCurrency()
    {
        if (PP.CurrencyToGain <= 0) return;
        MF.Log.Info("Adding via 'AfterRunEnded'");
        SaveCurrency();
    }

    private static void SaveCurrency()
    {
        PP.TotalCurrencyGainedDuringRun += PP.CurrencyToGain;
        MF.Log.Info($"Add currency ({PP.CurrencyToGain}) to available and " +
                    $"total gained ({PP.TotalCurrencyGainedDuringRun})");
        PP.CurrencyGainedLastRunText = PP.TotalCurrencyGainedDuringRun.ToString();
        PP.CurrencyAvailable += PP.CurrencyToGain;
        PP.CurrencyToGain = 0;
        MF.CurrencyLabel?.SetTextAutoSize(PP.CurrencyAvailable.ToString().PadLeft(7));
        ModConfig.SaveDebounced<PP>();
    }
}