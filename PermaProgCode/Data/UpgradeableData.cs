using PermaProg.PermaProgCode.Model;

namespace PermaProg.PermaProgCode.Data;

public class UpgradeableData
{
    public int TotalCurrentLevels;

    public readonly Dictionary<UpgradeableModel, string> All = new();
    public readonly UpgradeableModel CurrencyInterest = new();
    public readonly UpgradeableModel DexterityGain = new();
    public readonly UpgradeableModel UncommonRelic = new();
    public readonly UpgradeableModel CardUpgrades = new();
    public readonly UpgradeableModel CurrencyGain = new();
    public readonly UpgradeableModel StrengthGain = new();
    public readonly UpgradeableModel CommonRelic = new();
    public readonly UpgradeableModel CardRarity = new();
    public readonly UpgradeableModel BlockGain = new();
    public readonly UpgradeableModel MaxHealth = new();
    public readonly UpgradeableModel StartGold = new();
    public readonly UpgradeableModel GoldGain = new();

    public UpgradeableData()
    {
        All.Add(CurrencyInterest, nameof(CurrencyInterest));
        All.Add(DexterityGain, nameof(DexterityGain));
        All.Add(UncommonRelic, nameof(UncommonRelic));
        All.Add(CardUpgrades, nameof(CardUpgrades));
        All.Add(CurrencyGain, nameof(CurrencyGain));
        All.Add(StrengthGain, nameof(StrengthGain));
        All.Add(CommonRelic, nameof(CommonRelic));
        All.Add(CardRarity, nameof(CardRarity));
        All.Add(BlockGain, nameof(BlockGain));
        All.Add(MaxHealth, nameof(MaxHealth));
        All.Add(StartGold, nameof(StartGold));
        All.Add(GoldGain, nameof(GoldGain));

        foreach (var upg in All)
        {
            upg.Key.ValueName = upg.Value + "Value";
            upg.Key.ButtonName = "UpgradeButton" + upg.Value;
            upg.Key.CurrentLevelName = upg.Value + "Level";
        }

        {
            StartGold.MaxLevel = 10;
            StartGold.Vals = [0, 20, 40, 60, 80, 100, 120, 140, 160, 180, 200];
            StartGold.UpgCosts = [0, 400, 800, 1200, 1600, 2400, 3200, 4000, 4800, 6000];
        }

        {
            CurrencyGain.MaxLevel = 10;
            CurrencyGain.Vals = [0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100];
            CurrencyGain.UpgCosts = [300, 600, 900, 1200, 1500, 2100, 2700, 3300, 3900, 4000];
        }

        {
            MaxHealth.MaxLevel = 10;
            MaxHealth.Vals = [0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20];
            MaxHealth.UpgCosts = [500, 950, 1400, 1850, 2300, 3200, 4100, 5000, 5900, 7000];
        }

        {
            CardUpgrades.MaxLevel = 10;
            CardUpgrades.Vals = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            CardUpgrades.UpgCosts = [1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000];
        }

        {
            CurrencyInterest.MaxLevel = 5;
            CurrencyInterest.Vals = [0, 5, 10, 15, 20, 25];
            CurrencyInterest.UpgCosts = [500, 1000, 1500, 2000, 2500];
        }

        {
            GoldGain.MaxLevel = 10;
            GoldGain.Vals = [0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100];
            GoldGain.UpgCosts = [600, 1200, 1800, 2400, 3000, 3600, 4200, 4800, 5400, 6000];
        }

        {
            BlockGain.MaxLevel = 3;
            BlockGain.Vals = [0, 1, 2, 3];
            BlockGain.UpgCosts = [5000, 15000, 45000];
        }

        {
            CardRarity.MaxLevel = 5;
            CardRarity.Vals = [0, 20, 40, 60, 80, 100];
            CardRarity.UpgCosts = [3000, 6000, 9000, 12000, 15000];
        }

        {
            CommonRelic.MaxLevel = 1;
            CommonRelic.Vals = [0, 1];
            CommonRelic.UpgCosts = [10000];
        }

        {
            UncommonRelic.MaxLevel = 1;
            UncommonRelic.Vals = [0, 1];
            UncommonRelic.UpgCosts = [20000];
        }

        {
            StrengthGain.MaxLevel = 3;
            StrengthGain.Vals = [0, 1, 2, 3];
            StrengthGain.UpgCosts = [7000, 21000, 63000];
        }

        {
            DexterityGain.MaxLevel = 3;
            DexterityGain.Vals = [0, 1, 2, 3];
            DexterityGain.UpgCosts = [7000, 21000, 63000];
        }

        MF.Log.Info("Running sanity checks");
        foreach (var upg in All.Keys.Where(upg =>
                     upg.Vals.Count != upg.MaxLevel + 1 || upg.UpgCosts.Count != upg.MaxLevel))
        {
            MF.Log.Info($"This one is invalid -> {upg.CurrentLevelName}");
            MF.Log.Info($"MaxLevel: {upg.MaxLevel} Vals: {upg.Vals.Count} UpgCosts: {upg.UpgCosts.Count}");
            throw new InvalidOperationException();
        }

        MF.Log.Info("Sanity checks OK");
    }
}