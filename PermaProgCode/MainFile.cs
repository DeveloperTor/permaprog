using Logger = MegaCrit.Sts2.Core.Logging.Logger;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;
using PermaProg.PermaProgCode.UI;
using MegaCrit.Sts2.Core.Debug;
using System.Reflection;
using BaseLib.Config;
using HarmonyLib;
using Godot;

namespace PermaProg.PermaProgCode;

[ModInitializer(nameof(Initialize))]
public partial class MF : Node
{
    public const string ModId = "PermaProg";
    public const string ResPath = $"res://{ModId}";
    public static Logger Log { get; } = new(ModId, LogType.Generic);

    private static SceneTree? _tree;

    public static MegaLabel? CurrencyLabel;
    public static MegaLabel? HpRefLabel;
    public static MegaLabel? GoldRefLabel;

    public static void Initialize()
    {
        // Have to update manually each release until I figure out an automatic way to get value from the JSON file
        var gameReleaseInfo = ReleaseInfoManager.Instance.ReleaseInfo;
        var modVersion = Assembly.GetExecutingAssembly().GetName().Version = new Version(0, 6, 0);
        Log.Info($"Game version: {gameReleaseInfo?.Version}, branch: {gameReleaseInfo?.Branch}");
        Log.Info("Mod version: " + modVersion);

        var pp = new PP();
        ModConfigRegistry.Register(ModId, pp);
        pp.InitUpgradeablesCurrentLevel();
        pp.ConfigChanged += PP.UpdateCharacterSelectHpGold;

        new Harmony(ModId).PatchAll();

        _tree = Engine.GetMainLoop() as SceneTree;
        if (_tree is not null)
        {
            _tree.NodeAdded += OnNodeAdded;
        }
    }

    async private static void OnNodeAdded(Node node)
    {
        try
        {
            if (node.Name != "TopBar" && node.Name != "CharacterSelectScreen") return;
            if (node is not Control) return;
            if (!node.IsNodeReady())
            {
                await _tree?.ToSignal(node, Node.SignalName.Ready)!;
            }

            if (node.Name == "TopBar")
            {
                AddCurrencyLabel(node);
            }

            if (node.Name == "CharacterSelectScreen")
            {
                HandleCharacterSelectScreen(node);
            }
        }
        catch (Exception e)
        {
            Log.Warn("Error finding node tree: " + e);
        }
    }

    private static void AddCurrencyLabel(Node node)
    {
        var leftAligned = node.GetNode("LeftAlignedStuff");
        var topBarGold = leftAligned.GetNode("TopBarGold");
        var labelCopy = (MegaLabel)topBarGold.GetNode("GoldLabel").Duplicate();
        labelCopy.Name = "CurrencyLabel";
        labelCopy.SetTextAutoSize(PP.CurrencyAvailable.ToString().PadLeft(7));
        labelCopy.Modulate = Colors.GreenYellow;
        CurrencyLabel = labelCopy;
        topBarGold.AddChildSafely(labelCopy);
        topBarGold.MoveChild(labelCopy, 0);
    }

    private static void HandleCharacterSelectScreen(Node node)
    {
        node.AddChildSafely(new EnterShopButtonUi());

        var hpGold = node.GetNode("InfoPanel").GetChild(1).GetNode("HpGoldSpacer").GetChild(0);
        HpRefLabel = (MegaLabel)hpGold.GetNode("Hp").GetNode("Label");
        GoldRefLabel = (MegaLabel)hpGold.GetNode("Gold").GetNode("Label");
    }
}