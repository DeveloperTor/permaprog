using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.sts2.Core.Nodes.TopBar;

namespace PermaProg.PermaProgCode.Patches;

[HarmonyPatch]
public class TopBarPatches
{
    [HarmonyPatch(typeof(NTopBarGold), "OnFocus")]
    [HarmonyPrefix]
    public static void AddHoverTip(NTopBarGold __instance)
    {
        var hoverTip = AddCustomHoverTip("PERMAPROG-CURRENCY_LABEL.hover.title", "PERMAPROG-CURRENCY_LABEL.hover.desc");
        if (MF.CurrencyLabel == null) return;
        var andShow = NHoverTipSet.CreateAndShow(MF.CurrencyLabel, hoverTip);
        andShow.GlobalPosition = __instance.GlobalPosition + new Vector2(0.0f, __instance.Size.Y + 165f);

        try
        {
            (andShow.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0) as Control)!.Modulate =
                Colors.GreenYellow;
        }
        catch (Exception e)
        {
            MF.Log.Warn("Could not set CurrencyLabel title to green");
        }
    }

    [HarmonyPatch(typeof(NTopBarGold), "OnUnfocus")]
    [HarmonyPostfix]
    public static void RemoveHoverTip()
    {
        if (MF.CurrencyLabel != null) NHoverTipSet.Remove(MF.CurrencyLabel);
    }

    public static HoverTip AddCustomHoverTip(string? titleEntryKey, string descriptionEntryKey)
    {
        return titleEntryKey != null
            ? new HoverTip(new LocString("settings_ui", titleEntryKey),
                new LocString("settings_ui", descriptionEntryKey))
            : new HoverTip(new LocString("settings_ui", descriptionEntryKey));
    }
}