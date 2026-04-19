using MegaCrit.Sts2.Core.Nodes.CommonUi;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.sts2.Core.Nodes.TopBar;
using MegaCrit.Sts2.Core.Runs;

namespace PermaProg.PermaProgCode.Patches;

[HarmonyPatch]
public class TopBarPatches
{
    // [HarmonyPatch(typeof(NTopBar), "_Ready")]
    // [HarmonyPostfix]
    // public static void DoStuff1(ref NTopBar __instance)
    // {
    //     GD.Print(__instance.GetThemeFont(ThemeConstants.RichTextLabel.NormalFont));
    //     _font = __instance.GetThemeFont(ThemeConstants.RichTextLabel.NormalFont);
    //     GD.Print("In NTopBar ready");
    //     foreach (var VARIABLE in __instance.GetChildren())
    //     {
    //         GD.Print(VARIABLE.Name);
    //     }
    //
    //     var topBarGold = new NTopBarGold();
    //     topBarGold.Name = "PermaProgTopBarGold";
    //
    //     var goldMegaLabel = new MegaLabel();
    //     goldMegaLabel.Name = "GoldLabel";
    //     goldMegaLabel.AddThemeFontOverride(ThemeConstants.RichTextLabel.NormalFont, _font);
    //     var goldPopupMegaLabel = new MegaLabel();
    //     goldPopupMegaLabel.Name = "GoldPopup";
    //     goldPopupMegaLabel.Modulate = Colors.Transparent;
    //     goldPopupMegaLabel.AddThemeFontOverride(ThemeConstants.RichTextLabel.NormalFont, _font);
    //     topBarGold.AddChild(goldMegaLabel);
    //     topBarGold.AddChild(goldPopupMegaLabel);
    //
    //
    //     __instance.GetNode("LeftAlignedStuff").AddChild(topBarGold);
    //     //__instance.AddChild(topBarGold);
    //     //
    //     // var goldMegaLabel = new MegaLabel();
    //     // goldMegaLabel.Name = "GoldLabel";
    //     // goldMegaLabel.AddThemeFontOverride(ThemeConstants.Label.font, font);
    //     // var goldPopupMegaLabel = new MegaLabel();
    //     // goldPopupMegaLabel.Name = "GoldPopup";
    //     // goldPopupMegaLabel.AddThemeFontOverride(ThemeConstants.Label.font, font);
    //     //
    //     // topBarGold.AddChild(goldMegaLabel);
    //     // topBarGold.AddChild(goldPopupMegaLabel);
    //     //
    //     // GD.Print("HERE");
    //     // GD.Print(topBarGold.GetNode<MegaLabel>((NodePath) "%GoldLabel"));
    // }
    //
     private static Font _font = new FontFile();

    [HarmonyPatch(typeof(NTopBarGold), "_Ready")]
    [HarmonyPrefix]
    public static bool DoStuff2(ref NTopBarGold __instance, ref MegaLabel ____goldLabel, ref MegaLabel ____goldPopupLabel)
    {
        GD.Print(__instance.GetThemeFont(ThemeConstants.RichTextLabel.NormalFont));
        _font = __instance.GetThemeFont(ThemeConstants.RichTextLabel.NormalFont);

        if (__instance.Name != "TopBarGoldCopy") return true;
        GD.Print("In TopBarGoldCopy _Ready");


         ____goldLabel = __instance.GetNode<MegaLabel>("GoldLabel");
         ____goldPopupLabel = __instance.GetParent().GetParent().GetNode<MegaLabel>("GoldPopup");
         //__instance.Initialize(LocalContext.GetMe());

        // var goldMegaLabel = new MegaLabel();
        // goldMegaLabel.Name = "GoldLabel";
        // goldMegaLabel.AddThemeFontOverride(ThemeConstants.RichTextLabel.NormalFont, _font);
        // var goldPopupMegaLabel = new MegaLabel();
        // goldPopupMegaLabel.Name = "GoldPopup";
        // goldPopupMegaLabel.Modulate = Colors.Transparent;
        // goldPopupMegaLabel.AddThemeFontOverride(ThemeConstants.RichTextLabel.NormalFont, _font);
        // ____goldLabel = goldMegaLabel;
        // ____goldPopupLabel = goldPopupMegaLabel;
        return false;
        //
        // __instance.AddChild(goldMegaLabel);
        // __instance.AddChild(goldPopupMegaLabel);
        //
        // foreach (var VARIABLE in __instance.GetChildren())
        // {
        //     GD.Print(VARIABLE.Name);
        //     GD.Print(VARIABLE.GetPath());
        // }
    }

    // [HarmonyPatch(typeof(NTopBarGold), "_Ready")]
    // [HarmonyPostfix]
    // public static void DoStuff3(ref NTopBarGold __instance, ref MegaLabel ____goldLabel, ref MegaLabel ____goldPopupLabel)
    // {
    //     GD.Print("In PermaProgtopBarGold POSTFIX ready");
    //     var goldMegaLabel = new MegaLabel();
    //     goldMegaLabel.Name = "GoldLabel";
    //     goldMegaLabel.AddThemeFontOverride(ThemeConstants.Label.font, font);
    //     var goldPopupMegaLabel = new MegaLabel();
    //     goldPopupMegaLabel.Name = "GoldPopup";
    //     goldPopupMegaLabel.AddThemeFontOverride(ThemeConstants.Label.font, font);
    //     ____goldLabel = goldMegaLabel;
    //     ____goldPopupLabel = goldPopupMegaLabel;
    //
    //     if (__instance.Name != "PermaProgtopBarGold") return;
    //
    // }

    [HarmonyPatch(typeof(NTopBar), "Initialize")]
    [HarmonyPostfix]
    public static void DoStuff(ref NTopBar __instance, IRunState runState)
    {
        GD.Print("In NTopBar Initialize");
        foreach (var VARIABLE in __instance.GetChildren())
        {
            GD.Print(VARIABLE.Name);
            GD.Print(VARIABLE.GetChildren());
            GD.Print(" ");
        }

        var test = (NTopBarGold)__instance.GetNode("LeftAlignedStuff").GetNode("TopBarGoldCopy");
        GD.Print(test.Name);
        var player = LocalContext.GetMe(runState);
        // test._Ready();
        // test.Initialize(player);

        // var topBarGold = new NTopBarGold();
        // topBarGold._Ready();
        // var player = LocalContext.GetMe(runState);
        // topBarGold.Initialize(player);
        //__instance.AddChild(topBarGold);
    }
}