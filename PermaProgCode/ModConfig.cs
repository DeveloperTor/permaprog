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
    [ConfigIgnore] public static int CurrentRunAscensionLevel { get; set; }
    [ConfigIgnore] public static bool RunOngoing { get; set; }
    [ConfigHideInUI] public static int TotalCurrencyGainedDuringRun { get; set; }
    [ConfigHideInUI] public static int CurrencyAvailable { get; set; }
    [ConfigHideInUI] public static CharEnum SelectedCharacter { get; set; } = CharEnum.Ironclad;
    public static bool DebugMenuEnabled { get; set; }
    public static string CurrencyText { get; set; } = "0";
    public static string CurrencyGainedLastRunText { get; set; } = "0";
    public static bool BalancingEnabled { get; set; } = false;
    public static bool PerCharacterEnabled { get; set; } = true;

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
            _optionContainer.AddChild(CreateSliderOption(GetPropertyInfo(nameof(GlobalCostMultiplier))));
            _optionContainer.AddChild(CreateSliderOption(GetPropertyInfo(nameof(GlobalValueMultiplier))));
            _optionContainer.AddChild(CreateDividerControl());
            _optionContainer.AddChild(CreateToggleOption(GetPropertyInfo(nameof(BalancingEnabled))));
            _optionContainer.AddChild(CreateToggleOption(GetPropertyInfo(nameof(PerCharacterEnabled))));
        }

        _optionContainer.AddChild(CreateDividerControl());

        CreateLineEdit(nameof(CurrencyGainedLastRunText), 20);
        CreateLineEdit(nameof(CurrencyText), 50, true);

        _optionContainer.AddChild(CreateDividerControl());
        _optionContainer.AddChild(CreateDividerControl());
        AddCharacterTitle(_optionContainer);
        _optionContainer.AddChild(CreateDividerControl());
        _optionContainer.AddChild(CreateDividerControl());

        _optionContainer.AddChild(CreateSectionHeader("Tier 1 upgrades"));
        CreateUpgradeableUi(Upgrades.StartGold, UpgradeButtonStartGold);
        CreateUpgradeableUi(Upgrades.CurrencyGain, UpgradeButtonCurrencyGain, true);
        CreateUpgradeableUi(Upgrades.MaxHealth, UpgradeButtonMaxHealth);
        CreateUpgradeableUi(Upgrades.TravelCurrency, UpgradeButtonTravelCurrency);

        SetLocked();
        UpdateCurrentValues();
        Tier2Upgrades(_optionContainer);
        UpdateCurrentValues();
        Tier3Upgrades(_optionContainer);
        UpdateCurrentValues();
        Tier4Upgrades(_optionContainer);
        UpdateCurrentValues();
        Tier5Upgrades(_optionContainer);
        UpdateUi();
    }

    private void AddCharacterTitle(Control optionContainer)
    {
        switch (SelectedCharacter)
        {
            case CharEnum.Ironclad:
                optionContainer.AddChild(CreateSectionHeader("The Ironclad"));
                break;
            case CharEnum.Silent:
                optionContainer.AddChild(CreateSectionHeader("The Silent"));
                break;
            case CharEnum.Regent:
                optionContainer.AddChild(CreateSectionHeader("The Regent"));
                break;
            case CharEnum.Necrobinder:
                optionContainer.AddChild(CreateSectionHeader("The Necrobinder"));
                break;
            case CharEnum.Defect:
                optionContainer.AddChild(CreateSectionHeader("The Defect"));
                break;
            case CharEnum.ModdedCharacter:
            default:
                optionContainer.AddChild(CreateSectionHeader("The Unknown"));
                break;
        }
    }

    private static void SetLocked()
    {
        Upgrades.AscensionCurrency.Unlocked = false;
        Upgrades.CurrencyInterest.Unlocked = false;
        Upgrades.DexterityGain.Unlocked = false;
        Upgrades.UncommonRelic.Unlocked = false;
        Upgrades.CardUpgrades.Unlocked = false;
        Upgrades.StrengthGain.Unlocked = false;
        Upgrades.CommonRelic.Unlocked = false;
        Upgrades.CardRarity.Unlocked = false;
        Upgrades.BlockGain.Unlocked = false;
        Upgrades.RareRelic.Unlocked = false;
        Upgrades.GoldGain.Unlocked = false;
    }

    private void Tier2Upgrades(Control optionContainer)
    {
        if (Upgrades.TotalCurrentLevels < 5)
        {
            optionContainer.AddChild(CreateSectionHeader("..some beings... ..are yet to... ..be revealed..."));
            optionContainer.AddChild(CreateSectionHeader("???"));
        }
        else
        {
            optionContainer.AddChild(CreateSectionHeader("Tier 2 upgrades"));
            CreateUpgradeableUi(Upgrades.GoldGain, UpgradeButtonGoldGain, true);
            CreateUpgradeableUi(Upgrades.AscensionCurrency, UpgradeButtonAscensionCurrency);
            CreateUpgradeableUi(Upgrades.CardUpgrades, UpgradeButtonCardUpgrades);
            CreateUpgradeableUi(Upgrades.CurrencyInterest, UpgradeButtonCurrencyInterest, true);
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
                CreateUpgradeableUi(Upgrades.RareRelic, UpgradeButtonRareRelic, false, true);
                optionContainer.AddChild(CreateSectionHeader("..some beings... ..are yet to... ..be created..."));
                optionContainer.AddChild(CreateSectionHeader("(end of beta content)"));
                break;
        }
    }

    // Enums
    public enum CharEnum
    {
        Ironclad,
        Silent,
        Regent,
        Necrobinder,
        Defect,
        ModdedCharacter
    }

    // Checkboxes
    public static bool CommonRelicValue { get; set; }
    public static ulong CommonRelicValueSaved { get; set; }
    public static int CommonRelicLevel { get; set; }

    public static bool UncommonRelicValue { get; set; }
    public static ulong UncommonRelicValueSaved { get; set; }
    public static int UncommonRelicLevel { get; set; }

    public static bool RareRelicValue { get; set; }
    public static ulong RareRelicValueSaved { get; set; }
    public static int RareRelicLevel { get; set; }

    // Sliders
    [ConfigSlider(75.0, Format = "{0:0}%")]
    public static double GlobalCostMultiplier { get; set; } = 100.0;

    [ConfigSlider(100.0, 200.0, Format = "{0:0}%")]
    public static double GlobalValueMultiplier { get; set; } = 100.0;

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} gold")]
    public static double StartGoldValue { get; set; } // Currently applied value accessible in-game in mod menu
    public static ulong StartGoldValueSaved { get; set; } // Saved values (10 bits per character)
    public static int StartGoldLevel { get; set; } // Unlocked max level (5 bits per character)

    [ConfigSlider(0.0, 1000.0, Format = "{0:0}%")]
    public static double CurrencyGainValue { get; set; }
    public static ulong CurrencyGainValueSaved { get; set; }
    public static int CurrencyGainLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} hp")]
    public static double MaxHealthValue { get; set; }
    public static ulong MaxHealthValueSaved { get; set; }
    public static int MaxHealthLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} card(s)")]
    public static double CardUpgradesValue { get; set; }
    public static ulong CardUpgradesValueSaved { get; set; }
    public static int CardUpgradesLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0}%")]
    public static double CurrencyInterestValue { get; set; }
    public static ulong CurrencyInterestValueSaved { get; set; }
    public static int CurrencyInterestLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0}%")]
    public static double GoldGainValue { get; set; }
    public static ulong GoldGainValueSaved { get; set; }
    public static int GoldGainLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} block")]
    public static double BlockGainValue { get; set; }
    public static ulong BlockGainValueSaved { get; set; }
    public static int BlockGainLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0}%")]
    public static double CardRarityValue { get; set; }
    public static ulong CardRarityValueSaved { get; set; }
    public static int CardRarityLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} str")]
    public static double StrengthGainValue { get; set; }
    public static ulong StrengthGainValueSaved { get; set; }
    public static int StrengthGainLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} dex")]
    public static double DexterityGainValue { get; set; }
    public static ulong DexterityGainValueSaved { get; set; }
    public static int DexterityGainLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} ₵")]
    public static double TravelCurrencyValue { get; set; }
    public static ulong TravelCurrencyValueSaved { get; set; }
    public static int TravelCurrencyLevel { get; set; }

    [ConfigSlider(0.0, 1000.0, Format = "{0:0} %")]
    public static double AscensionCurrencyValue { get; set; }
    public static ulong AscensionCurrencyValueSaved { get; set; }
    public static int AscensionCurrencyLevel { get; set; }

    // Buttons
    public void UpgradeButtonAscensionCurrency()
    {
        if (IsLevelUpSuccessful(Upgrades.AscensionCurrency)) AscensionCurrencyLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonTravelCurrency()
    {
        if (IsLevelUpSuccessful(Upgrades.TravelCurrency)) TravelCurrencyLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonRareRelic()
    {
        if (IsLevelUpSuccessful(Upgrades.RareRelic)) RareRelicLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonUncommonRelic()
    {
        if (IsLevelUpSuccessful(Upgrades.UncommonRelic)) UncommonRelicLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonCommonRelic()
    {
        if (IsLevelUpSuccessful(Upgrades.CommonRelic)) CommonRelicLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonDexterityGain()
    {
        if (IsLevelUpSuccessful(Upgrades.DexterityGain)) DexterityGainLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonStrengthGain()
    {
        if (IsLevelUpSuccessful(Upgrades.StrengthGain)) StrengthGainLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonCardRarity()
    {
        if (IsLevelUpSuccessful(Upgrades.CardRarity)) CardRarityLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonBlockGain()
    {
        if (IsLevelUpSuccessful(Upgrades.BlockGain)) BlockGainLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonGoldGain()
    {
        if (IsLevelUpSuccessful(Upgrades.GoldGain)) GoldGainLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonCurrencyInterest()
    {
        if (IsLevelUpSuccessful(Upgrades.CurrencyInterest)) CurrencyInterestLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonCardUpgrades()
    {
        if (IsLevelUpSuccessful(Upgrades.CardUpgrades)) CardUpgradesLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonMaxHealth()
    {
        if (IsLevelUpSuccessful(Upgrades.MaxHealth)) MaxHealthLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonCurrencyGain()
    {
        if (IsLevelUpSuccessful(Upgrades.CurrencyGain)) CurrencyGainLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void UpgradeButtonStartGold()
    {
        if (IsLevelUpSuccessful(Upgrades.StartGold)) StartGoldLevel += 1 << GetShift(5);
        UpdateUi();
    }

    public void AddGold500()
    {
        AddCurrencyToAvailable(500);
        UpdateUi();
    }

    // Helpers
    public static void AddCurrencyToAvailable(int currency)
    {
        CurrencyAvailable += Math.Clamp(currency, 0, 9999);
        CurrencyAvailable = Math.Clamp(CurrencyAvailable, 0, 999999);
    }

    public static void RemoveCurrencyFromAvailable(int currency)
    {
        CurrencyAvailable += Math.Clamp(-currency, -9999, 0);
        CurrencyAvailable = Math.Clamp(CurrencyAvailable, 0, 999999);
    }

    public static int GetUpgLevel(int currentLevel)
    {
        return currentLevel >> GetShift(5) & 0b11111;
    }

    private static void SetUpgLevel(ref int currentLevel, int newLevel)
    {
        var bitMask = 0b11111 << GetShift(5);
        currentLevel = currentLevel & ~bitMask | newLevel << GetShift(5);
    }

    public static double GetUpgValue(ulong upgValue)
    {
        return upgValue >> GetShift(10) & 0b1111111111ul;
    }

    private static ulong SetUpgValue(ulong upgValue, ulong newValue)
    {
        var bitMask = 0b1111111111ul << GetShift(10);
        return upgValue & ~bitMask | newValue << GetShift(10);
    }

    private static int GetShift(int bits)
    {
        return PerCharacterEnabled ? (int)SelectedCharacter * bits : (int)CharEnum.ModdedCharacter * bits;
    }

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
            totalCurrentLevels += GetUpgLevel(upg.CurrentLevel);
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
        var maxSliderValue = upg.Vals[GetUpgLevel(upg.CurrentLevel)] * (GlobalValueMultiplier / 100.0);
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
        checkbox.Visible = GetUpgLevel(upg.CurrentLevel) >= upg.MaxLevel;
    }

    private void UpdateButtons(UpgradeableModel upg)
    {
        var buttonRow = _optionContainer?.GetNode<NConfigOptionRow>(upg.ButtonName);
        if (buttonRow?.SettingControl is not NConfigButton button) return;

        if (GetUpgLevel(upg.CurrentLevel) >= upg.MaxLevel)
        {
            (button.GetChild(1) as Label)!.Text = "Maxed out!";
            return;
        }

        IsArraySafe(upg, upg.UpgCosts);
        (button.GetChild(1) as Label)!.Text =
            upg.UpgCosts[GetUpgLevel(upg.CurrentLevel)] <= 0
                ? "Free!"
                : ((int)(upg.UpgCosts[GetUpgLevel(upg.CurrentLevel)] * (GlobalCostMultiplier / 100))).ToString();
    }

    private bool IsLevelUpSuccessful(UpgradeableModel upg)
    {
        if (GetUpgLevel(upg.CurrentLevel) >= upg.MaxLevel) return false;
        if (!IsArraySafe(upg, upg.UpgCosts)) return false;
        if (upg.UpgCosts[GetUpgLevel(upg.CurrentLevel)] * (GlobalCostMultiplier / 100) > CurrencyAvailable)
            return false;

        RemoveCurrencyFromAvailable((int)(upg.UpgCosts[GetUpgLevel(upg.CurrentLevel)] * (GlobalCostMultiplier / 100)));
        SetUpgLevel(ref upg.CurrentLevel, GetUpgLevel(upg.CurrentLevel) + 1);
        MF.Log.Info($"Upgraded {upg.ValueName[..^"Value".Length]} to level {GetUpgLevel(upg.CurrentLevel)}");
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
        if (GetUpgLevel(upg.CurrentLevel) <= upgArray.Count - 1 || GetUpgLevel(upg.CurrentLevel) == 0) return true;

        MF.Log.Error($"{upg.CurrentLevelName}: Current level ({GetUpgLevel(upg.CurrentLevel)}) is higher than values " +
                     $"available ({upgArray.Count - 1}). Lowering value to max level available");
        SetUpgLevel(ref upg.CurrentLevel, upgArray.Count - 1);
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
        if (BaseHp == 9999 || BaseGold == 9999) return; // Naive fix to not update text when random is selected

        SaveValues();

        var hp = BaseHp;
        if (BalancingEnabled) hp = (int)(hp * 0.9);
        var hpText = (hp + (int)MaxHealthValue).ToString();
        MF.HpRefLabel?.SetTextAutoSize(hpText + "/" + hpText);

        var gold = BaseGold;
        if (BalancingEnabled) gold = 0;
        var goldText = (gold + (int)StartGoldValue).ToString();
        MF.GoldRefLabel?.SetTextAutoSize(goldText);
    }

    private static void SaveValues()
    {
        CommonRelicValueSaved = SetUpgValue(CommonRelicValueSaved, CommonRelicValue ? (ulong)1 : 0);
        UncommonRelicValueSaved = SetUpgValue(UncommonRelicValueSaved, UncommonRelicValue ? (ulong)1 : 0);
        RareRelicValueSaved = SetUpgValue(RareRelicValueSaved, RareRelicValue ? (ulong)1 : 0);

        StartGoldValueSaved = SetUpgValue(StartGoldValueSaved, (ulong)StartGoldValue);
        CurrencyGainValueSaved = SetUpgValue(CurrencyGainValueSaved, (ulong)CurrencyGainValue);
        MaxHealthValueSaved = SetUpgValue(MaxHealthValueSaved, (ulong)MaxHealthValue);
        CardUpgradesValueSaved = SetUpgValue(CardUpgradesValueSaved, (ulong)CardUpgradesValue);
        CurrencyInterestValueSaved = SetUpgValue(CurrencyInterestValueSaved, (ulong)CurrencyInterestValue);
        GoldGainValueSaved = SetUpgValue(GoldGainValueSaved, (ulong)GoldGainValue);
        BlockGainValueSaved = SetUpgValue(BlockGainValueSaved, (ulong)BlockGainValue);
        CardRarityValueSaved = SetUpgValue(CardRarityValueSaved, (ulong)CardRarityValue);
        StrengthGainValueSaved = SetUpgValue(StrengthGainValueSaved, (ulong)StrengthGainValue);
        DexterityGainValueSaved = SetUpgValue(DexterityGainValueSaved, (ulong)DexterityGainValue);
        TravelCurrencyValueSaved = SetUpgValue(TravelCurrencyValueSaved, (ulong)TravelCurrencyValue);
        AscensionCurrencyValueSaved = SetUpgValue(AscensionCurrencyValueSaved, (ulong)AscensionCurrencyValue);
    }

    public static void LoadValues()
    {
        CommonRelicValue = GetUpgValue(CommonRelicValueSaved) > 0.5;
        UncommonRelicValue = GetUpgValue(UncommonRelicValueSaved) > 0.5;
        RareRelicValue = GetUpgValue(RareRelicValueSaved) > 0.5;

        StartGoldValue = GetUpgValue(StartGoldValueSaved);
        CurrencyGainValue = GetUpgValue(CurrencyGainValueSaved);
        MaxHealthValue = GetUpgValue(MaxHealthValueSaved);
        CardUpgradesValue = GetUpgValue(CardUpgradesValueSaved);
        CurrencyInterestValue = GetUpgValue(CurrencyInterestValueSaved);
        GoldGainValue = GetUpgValue(GoldGainValueSaved);
        BlockGainValue = GetUpgValue(BlockGainValueSaved);
        CardRarityValue = GetUpgValue(CardRarityValueSaved);
        StrengthGainValue = GetUpgValue(StrengthGainValueSaved);
        DexterityGainValue = GetUpgValue(DexterityGainValueSaved);
        TravelCurrencyValue = GetUpgValue(TravelCurrencyValueSaved);
        AscensionCurrencyValue = GetUpgValue(AscensionCurrencyValueSaved);
    }
}