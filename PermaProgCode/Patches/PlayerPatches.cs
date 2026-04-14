using Godot;
using HarmonyLib;
using BaseLib.Config;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Achievements;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Runs;
using PermaProg.PermaProgCode.Relics;
using PermaProg.PermaProgCode.Extensions;
namespace PermaProg.PermaProgCode.Patches;

[HarmonyPatch]
public static class PlayerPatches
{
    [HarmonyPatch(typeof(Player), "PopulateStartingInventory")]
    [HarmonyPrefix]
    public static void StartNewRun(Player __instance)
    {
        MF.Log.Info("Starting new run");

        PP.TotalCurrencyGainedDuringRun = 0;
        ModConfig.SaveDebounced<PP>();
    }

    [HarmonyPatch(typeof(Player), "PopulateStartingDeck")]
    [HarmonyPostfix]
    public static void UpgradeCards(Player __instance)
    {
        var cards = __instance.Deck.Cards;
        var cardsToUpgrade = RandomlySelectedCards(cards, (int)PP.CardUpgradesValue, cards.Count);
        var cardModels = cardsToUpgrade.ToList();

        MF.Log.Info($"Upgrading {cardModels.Count} cards");
        foreach (var card in cardModels.Where(card => card.IsUpgradable))
        {
            card.UpgradeInternal();
            card.FinalizeUpgradeInternal();
        }
    }

    // Ty Matthew Watson on StackOverflow
    public static IEnumerable<T> RandomlySelectedCards<T>(IEnumerable<T> sequence, int count, int sequenceLength)
    {
        var rng = new Random();
        var available = sequenceLength;
        var remaining = count;

        using var iterator = sequence.GetEnumerator();
        for (var current = 0; current < sequenceLength; ++current)
        {
            iterator.MoveNext();
            if (rng.NextDouble() < remaining / (double)available)
            {
                yield return iterator.Current;
                --remaining;
            }

            --available;
        }
    }

    [HarmonyPatch(typeof(Player), "PopulateStartingRelics")]
    [HarmonyPostfix]
    public static void AddPpRelic(Player __instance)
    {
        MF.Log.Info("Adding Peapod relic");

        var ppRelic = ModelDb.Relic<PpRelic>().ToMutable();
        ppRelic.FloorAddedToDeck = 1;
        __instance.AddRelicInternal(ppRelic, silent: true);
    }
}