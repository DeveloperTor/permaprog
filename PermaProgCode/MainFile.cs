using Logger = MegaCrit.Sts2.Core.Logging.Logger;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;
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
        // Have to update manually each release until I figure out an automatic way to get value from the JSON file
        var version = Assembly.GetExecutingAssembly().GetName().Version = new Version(0, 5, 1);
        Log.Info("Mod version: " + version);
        ModConfigRegistry.Register(ModId, new PP());
        new Harmony(ModId).PatchAll();
    }
}