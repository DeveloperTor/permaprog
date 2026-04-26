using PermaProg.PermaProgCode.Model;
using PermaProg.PermaProgCode.Data;
using System.Reflection;
using Godot.Collections;
using BaseLib.Config.UI;
using BaseLib.Config;
using Godot;

namespace PermaProg.PermaProgCode;

internal class PP : SimpleModConfig
{
    private static Control? _optionContainer;
    [ConfigIgnore] public static UpgradeableData Upgrades { get; } = new();
    [ConfigIgnore] public static int CurrencyToGain { get; set; }
    [ConfigIgnore] public static bool RunOngoing { get; set; }
    [ConfigHideInUI] public static int TotalCurrencyGainedDuringRun { get; set; }
    [ConfigHideInUI] public static int CurrencyAvailable { get; set; }
    public static bool DebugMenuEnabled { get; set; }
    public static string CurrencyText { get; set; } = "0";
    public static string CurrencyGainedLastRunText { get; set; } = "0";
    public static bool BalancingEnabled { get; set; } = true;

    public override void SetupConfigUI(Control optionContainer)
    {
        MF.Log.Info("Shop menu entered");
        _optionContainer = optionContainer;
        _optionContainer.AddChild(CreateToggleOption(GetPropertyInfo(nameof(DebugMenuEnabled))));
        if (DebugMenuEnabled)
        {
            MF.Log.Info("Showing debug menu");
            AddRestoreDefaultsButton(_optionContainer);
            _optionContainer.AddChild(CreateButton("Add gold (debug)", "+500", AddGold500));
            _optionContainer.AddChild(CreateDividerControl());
        }

        _optionContainer.AddChild(CreateToggleOption(GetPropertyInfo(nameof(BalancingEnabled))));
        CreateLineEdit(nameof(CurrencyGainedLastRunText), 20);
        CreateLineEdit(nameof(CurrencyText), 50, true);
        _optionContainer.AddChild(CreateDividerControl());

        _optionContainer.AddChild(CreateSectionHeader("Tier 1 upgrades"));
        CreateUpgradeableUi(Upgrades.StartGold, UpgradeButtonStartGold);
        CreateUpgradeableUi(Upgrades.CurrencyGain, UpgradeButtonCurrencyGain);
        CreateUpgradeableUi(Upgrades.MaxHealth, UpgradeButtonMaxHealth);
        CreateUpgradeableUi(Upgrades.TravelCurrency, UpgradeButtonTravelCurrency);

        UpdateCurrentValues();
        Tier2Upgrades(optionContainer);
        UpdateCurrentValues();
        Tier3Upgrades(optionContainer);
        UpdateCurrentValues();
        Tier4Upgrades(optionContainer);
        UpdateCurrentValues();
        Tier5Upgrades(optionContainer);
        UpdateUi();
    }

    private void Tier2Upgrades(Control optionContainer)
    {
        if (Upgrades.TotalCurrentLevels < 5)
        {
            optionContainer.AddChild(CreateSectionHeader("..some beings... ..are yet to... ..be revealed..."));
            optionContainer.AddChild(CreateSectionHeader("???"));

            Upgrades.CurrencyInterest.Unlocked = false;
            Upgrades.DexterityGain.Unlocked = false;
            Upgrades.UncommonRelic.Unlocked = false;
            Upgrades.CardUpgrades.Unlocked = false;
            Upgrades.StrengthGain.Unlocked = false;
            Upgrades.CommonRelic.Unlocked = false;
            Upgrades.CardRarity.Unlocked = false;
            Upgrades.BlockGain.Unlocked = false;
            Upgrades.GoldGain.Unlocked = false;
        }
        else
        {
            optionContainer.AddChild(CreateSectionHeader("Tier 2 upgrades"));
            CreateUpgradeableUi(Upgrades.CurrencyInterest, UpgradeButtonCurrencyInterest, true);
            CreateUpgradeableUi(Upgrades.GoldGain, UpgradeButtonGoldGain, true);
            CreateUpgradeableUi(Upgrades.CardUpgrades, UpgradeButtonCardUpgrades);
        }
    }

    private void Tier3Upgrades(Control optionContainer)
    {
        switch (Upgrades.TotalCurrentLevels)
        {
            case < 5:
                break;
            case < 10:
                optionContainer.AddChild(CreateSectionHeader("..you have... ..done well... ..so far..."));
                optionContainer.AddChild(CreateSectionHeader("???"));
                break;
            default:
                optionContainer.AddChild(CreateSectionHeader("Tier 3 upgrades"));
                CreateUpgradeableUi(Upgrades.BlockGain, UpgradeButtonBlockGain);
                CreateUpgradeableUi(Upgrades.CardRarity, UpgradeButtonCardRarity, true);
                CreateUpgradeableUi(Upgrades.CommonRelic, UpgradeButtonCommonRelic, false, true);
                break;
        }
    }

    private void Tier4Upgrades(Control optionContainer)
    {
        switch (Upgrades.TotalCurrentLevels)
        {
            case < 10:
                break;
            case < 15:
                optionContainer.AddChild(CreateSectionHeader("..there is ... ..yet more... ..to be revealed..."));
                optionContainer.AddChild(CreateSectionHeader("???"));
                break;
            default:
                optionContainer.AddChild(CreateSectionHeader("Tier 4 upgrades"));
                CreateUpgradeableUi(Upgrades.StrengthGain, UpgradeButtonStrengthGain);
                CreateUpgradeableUi(Upgrades.DexterityGain, UpgradeButtonDexterityGain);
                CreateUpgradeableUi(Upgrades.UncommonRelic, UpgradeButtonUncommonRelic, false, true);
                break;
        }
    }

    private void Tier5Upgrades(Control optionContainer)
    {
        switch (Upgrades.TotalCurrentLevels)
        {
            case < 15:
                break;
            case < 20:
                optionContainer.AddChild(CreateSectionHeader("..the journey... ..continues... .. ever onward..."));
                optionContainer.AddChild(CreateSectionHeader("???"));
                break;
            default:
                optionContainer.AddChild(CreateSectionHeader("Tier 5 upgrades"));
                optionContainer.AddChild(CreateSectionHeader("..some beings... ..are yet to... ..be created..."));
                optionContainer.AddChild(CreateSectionHeader("(end of beta content)"));
                break;
        }
    }

    // Checkboxes
    public static bool CommonRelicValue { get; set; }
    public static int CommonRelicLevel { get; set; }

    public static bool UncommonRelicValue { get; set; }
    public static int UncommonRelicLevel { get; set; }

    // Sliders
    [ConfigSlider(0.0, 1000.0, Format = "{0:0} gold")]
    public static double StartGoldValue { get; set; }
    public static int StartGoldLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0}%")]
    public static double CurrencyGainValue { get; set; }
    public static int CurrencyGainLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} hp")]
    public static double MaxHealthValue { get; set; }
    public static int MaxHealthLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} card(s)")]
    public static double CardUpgradesValue { get; set; }
    public static int CardUpgradesLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0}%")]
    public static double CurrencyInterestValue { get; set; }
    public static int CurrencyInterestLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0}%")]
    public static double GoldGainValue { get; set; }
    public static int GoldGainLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} block")]
    public static double BlockGainValue { get; set; }
    public static int BlockGainLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0}%")]
    public static double CardRarityValue { get; set; }
    public static int CardRarityLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} str")]
    public static double StrengthGainValue { get; set; }
    public static int StrengthGainLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} dex")]
    public static double DexterityGainValue { get; set; }
    public static int DexterityGainLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} ₵")]
    public static double TravelCurrencyValue { get; set; }
    public static int TravelCurrencyLevel { get; set; }

    // Buttons
    public void UpgradeButtonTravelCurrency()
    {
        if (IsLevelUpSuccessful(Upgrades.TravelCurrency)) TravelCurrencyLevel++;
        UpdateUi();
    }

    public void UpgradeButtonUncommonRelic()
    {
        if (IsLevelUpSuccessful(Upgrades.UncommonRelic)) UncommonRelicLevel++;
        UpdateUi();
    }

    public void UpgradeButtonDexterityGain()
    {
        if (IsLevelUpSuccessful(Upgrades.DexterityGain)) DexterityGainLevel++;
        UpdateUi();
    }

    public void UpgradeButtonStrengthGain()
    {
        if (IsLevelUpSuccessful(Upgrades.StrengthGain)) StrengthGainLevel++;
        UpdateUi();
    }

    public void UpgradeButtonCommonRelic()
    {
        if (IsLevelUpSuccessful(Upgrades.CommonRelic)) CommonRelicLevel++;
        UpdateUi();
    }

    public void UpgradeButtonCardRarity()
    {
        if (IsLevelUpSuccessful(Upgrades.CardRarity)) CardRarityLevel++;
        UpdateUi();
    }

    public void UpgradeButtonBlockGain()
    {
        if (IsLevelUpSuccessful(Upgrades.BlockGain)) BlockGainLevel++;
        UpdateUi();
    }

    public void UpgradeButtonGoldGain()
    {
        if (IsLevelUpSuccessful(Upgrades.GoldGain)) GoldGainLevel++;
        UpdateUi();
    }

    public void UpgradeButtonCurrencyInterest()
    {
        if (IsLevelUpSuccessful(Upgrades.CurrencyInterest)) CurrencyInterestLevel++;
        UpdateUi();
    }

    public void UpgradeButtonCardUpgrades()
    {
        if (IsLevelUpSuccessful(Upgrades.CardUpgrades)) CardUpgradesLevel++;
        UpdateUi();
    }

    public void UpgradeButtonMaxHealth()
    {
        if (IsLevelUpSuccessful(Upgrades.MaxHealth)) MaxHealthLevel++;
        UpdateUi();
    }

    public void UpgradeButtonCurrencyGain()
    {
        if (IsLevelUpSuccessful(Upgrades.CurrencyGain)) CurrencyGainLevel++;
        UpdateUi();
    }

    public void UpgradeButtonStartGold()
    {
        if (IsLevelUpSuccessful(Upgrades.StartGold)) StartGoldLevel++;
        UpdateUi();
    }

    public void AddGold500()
    {
        CurrencyAvailable += 500;
        UpdateUi();
    }

    // Helpers
    private void UpdateUi()
    {
        UpdateCurrentValues();
        UpdateLineEdits();

        foreach (var upg in Upgrades.All.Keys.Where(upg => upg.Unlocked))
        {
            UpdateSliders(upg);
            UpdateCheckboxes(upg);
            UpdateButtons(upg);
        }
    }

    private void UpdateCurrentValues()
    {
        var totalCurrentLevels = 0;
        foreach (var upg in Upgrades.All.Keys.Where(upg => upg.Unlocked))
        {
            var propertyInfo = GetPropertyInfo(upg.CurrentLevelName);
            upg.CurrentLevel = (int)(propertyInfo.GetValue(Upgrades) ?? throw new InvalidOperationException());
            totalCurrentLevels += upg.CurrentLevel;
        }

        Upgrades.TotalCurrentLevels = totalCurrentLevels;
    }

    private static void UpdateLineEdits()
    {
        CurrencyText = CurrencyAvailable.ToString();
        var headerRow = _optionContainer?.GetNode<NConfigOptionRow>("CurrencyText");
        if (headerRow?.SettingControl is NConfigLineEdit header) header.Text = CurrencyText;

        var headerRow2 = _optionContainer?.GetNode<NConfigOptionRow>("CurrencyGainedLastRunText");
        if (headerRow2?.SettingControl is NConfigLineEdit header2) header2.Text = CurrencyGainedLastRunText;
    }

    private void UpdateSliders(UpgradeableModel upg)
    {
        var sliderRow = _optionContainer?.GetNode<NConfigOptionRow>(upg.ValueName);
        if (sliderRow?.SettingControl is not NConfigSlider slider) return;

        IsArraySafe(upg, upg.Vals);
        var maxSliderValue = upg.Vals[upg.CurrentLevel];
        if (maxSliderValue <= 0)
        {
            slider.Visible = false;
        }
        else
        {
            slider.SetRange(0.0, maxSliderValue);
            slider.Visible = true;
        }
    }

    private static void UpdateCheckboxes(UpgradeableModel upg)
    {
        var checkboxRow = _optionContainer?.GetNode<NConfigOptionRow>(upg.ValueName);
        if (checkboxRow?.SettingControl is not NConfigTickbox checkbox) return;
        checkbox.Visible = upg.CurrentLevel >= upg.MaxLevel;
    }

    private void UpdateButtons(UpgradeableModel upg)
    {
        var buttonRow = _optionContainer?.GetNode<NConfigOptionRow>(upg.ButtonName);
        if (buttonRow?.SettingControl is not NConfigButton button) return;

        if (upg.CurrentLevel >= upg.MaxLevel)
        {
            (button.GetChild(1) as Label)!.Text = "Maxed out!";
            return;
        }

        IsArraySafe(upg, upg.UpgCosts);
        (button.GetChild(1) as Label)!.Text =
            upg.UpgCosts[upg.CurrentLevel] <= 0 ? "Free!" : upg.UpgCosts[upg.CurrentLevel].ToString();
    }

    private bool IsLevelUpSuccessful(UpgradeableModel upg)
    {
        if (upg.CurrentLevel >= upg.MaxLevel) return false;
        if (!IsArraySafe(upg, upg.UpgCosts)) return false;
        if (upg.UpgCosts[upg.CurrentLevel] > CurrencyAvailable) return false;

        CurrencyAvailable -= upg.UpgCosts[upg.CurrentLevel];
        upg.CurrentLevel++;
        MF.Log.Info($"Upgraded {upg.ValueName[..^"Value".Length]} to level {upg.CurrentLevel}");
        return true;
    }

    private void CreateLineEdit(string name, int fontSize, bool addHoverTip = false, bool isEditable = false)
    {
        var propertyInfo = GetPropertyInfo(name);
        var headerRow = CreateLineEditOption(propertyInfo);
        if (headerRow.SettingControl is NConfigLineEdit header)
        {
            header.AddThemeFontSizeOverride("font_size", fontSize);
            header.Editable = isEditable;
        }

        if (addHoverTip) headerRow.AddHoverTip();

        _optionContainer?.AddChild(headerRow);
    }

    private void CreateUpgradeableUi(UpgradeableModel upg, Action onPressed, bool addHoverTip = false,
        bool isCheckbox = false
    )
    {
        var optionRow = isCheckbox
            ? CreateToggleOption(GetPropertyInfo(upg.ValueName))
            : CreateSliderOption(GetPropertyInfo(upg.ValueName));

        if (addHoverTip) optionRow.AddHoverTip();

        _optionContainer?.AddChild(optionRow);
        _optionContainer?.AddChild(CreateButton(upg.ButtonName, "Default text", onPressed));
        _optionContainer?.AddChild(CreateDividerControl());
        upg.Unlocked = true;
    }

    private PropertyInfo GetPropertyInfo(string name)
    {
        var propertyInfo = ConfigProperties.Find(x => x.Name == name);
        return propertyInfo ?? throw new InvalidOperationException();
    }

    private bool IsArraySafe(UpgradeableModel upg, Array<int> upgArray)
    {
        if (upg.CurrentLevel <= upgArray.Count - 1 || upg.CurrentLevel == 0) return true;

        MF.Log.Error($"{upg.CurrentLevelName}: Current level ({upg.CurrentLevel}) is higher than values " +
                     $"available ({upgArray.Count - 1}). Lowering value to max level available. Bugs may occur");
        upg.CurrentLevel = upgArray.Count - 1;
        GetPropertyInfo(upg.CurrentLevelName).SetValue(Upgrades, upg.CurrentLevel);
        return false;
    }

    public void InitUpgradeablesCurrentLevel()
    {
        foreach (var upg in Upgrades.All.Keys)
        {
            var propertyInfo = GetPropertyInfo(upg.CurrentLevelName);
            upg.CurrentLevel = (int)(propertyInfo.GetValue(Upgrades) ?? throw new InvalidOperationException());
        }
    }

    public static int BaseHp;
    public static int BaseGold;

    public static void UpdateCharacterSelectHpGold(object? sender, EventArgs e)
    {
        var hp = BaseHp;
        if (BalancingEnabled) hp = (int)(hp * 0.9);
        var hpText = (hp + (int)MaxHealthValue).ToString();
        MF.HpRefLabel?.SetTextAutoSize(hpText + "/" + hpText);

        var gold = BaseGold;
        if (BalancingEnabled) gold = 0;
        var goldText = (gold + (int)StartGoldValue).ToString();
        MF.GoldRefLabel?.SetTextAutoSize(goldText);
    }
}