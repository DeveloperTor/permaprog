using Logger = MegaCrit.Sts2.Core.Logging.Logger;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;
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
    }
}