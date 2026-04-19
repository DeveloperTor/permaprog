using Logger = MegaCrit.Sts2.Core.Logging.Logger;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Debug;
using System.Reflection;
using BaseLib.Config;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.sts2.Core.Nodes.TopBar;

namespace PermaProg.PermaProgCode;

[ModInitializer(nameof(Initialize))]
public partial class MF : Node
{
    public const string ModId = "PermaProg";
    public const string ResPath = $"res://{ModId}";
    public static Logger Log { get; } = new(ModId, LogType.Generic);

    public static SceneTree? _tree;

    public static void Initialize()
    {
        Logger.GlobalLogLevel = LogLevel.VeryDebug; // Not working :(
        Log.WillLog(LogLevel.VeryDebug); // Not working :(
        // Have to update manually each release until I figure out an automatic way to get value from the JSON file
        var gameReleaseInfo = ReleaseInfoManager.Instance.ReleaseInfo;
        var modVersion = Assembly.GetExecutingAssembly().GetName().Version = new Version(0, 5, 3);
        Log.Info($"Game version: {gameReleaseInfo?.Version}, branch: {gameReleaseInfo?.Branch}");
        Log.Info("Mod version: " + modVersion);
        ModConfigRegistry.Register(ModId, new PP());
        new Harmony(ModId).PatchAll();

        _tree = Engine.GetMainLoop() as SceneTree;
        if (_tree is not null)
        {
            _tree.NodeAdded += OnNodeAdded;
        }
    }

    async private static void OnNodeAdded(Node node)
    {
        if (node.Name == "TopBar")
        {
            if (node is Control)
            {
                if (!node.IsNodeReady())
                {
                    await _tree.ToSignal(node, Node.SignalName.Ready);
                }
                Log.Info("TopBar:");
                foreach (var VARIABLE in node.GetChildren())
                {
                    GD.Print(VARIABLE.Name);
                }

                var leftAligned = node.GetNode("LeftAlignedStuff");
                Log.Info("LeftAlignedStuff:");
                foreach (var VARIABLE in leftAligned.GetChildren())
                {
                    GD.Print(VARIABLE.Name);
                }

                var topBarGold = leftAligned.GetNode("TopBarGold");
                Log.Info("TopBarGold:");
                foreach (var VARIABLE in topBarGold.GetChildren())
                {
                    GD.Print(VARIABLE.Name);
                }

                NTopBarGold topBarGoldCopy = (NTopBarGold)leftAligned.GetNode("TopBarGold").Duplicate();
                topBarGoldCopy.Name = "TopBarGoldCopy";
                Log.Info("TopBarGoldCopy:");
                foreach (var VARIABLE in topBarGoldCopy.GetChildren())
                {
                    GD.Print(VARIABLE.Name);
                }

                //topBarGoldCopy.Initialize();

                // foreach (var VARIABLE in leftAligned.GetNode("TopBarGold").GetChildren())
                // {
                //     copy.AddChild(VARIABLE);
                // }


                leftAligned.AddChildSafely(topBarGoldCopy);
            }
        }
    }
}