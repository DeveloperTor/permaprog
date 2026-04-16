using MegaCrit.Sts2.Core.Models;
using System.Reflection;
using HarmonyLib;

namespace PermaProg.PermaProgCode.Patches;

[HarmonyPatch]
public static class SetStartingHpPatch
{
    private static void SetHp(CharacterModel __instance, ref int __result)
    {
        if (PP.BalancingEnabled)
            __result = (int)(__result * 0.8);

        __result += (int)PP.MaxHealthValue;
        MF.Log.Info($"Setting starting HP of {__instance} to " + __result);
    }

    public static MethodInfo?[] TargetMethods()
    {
        var baseType = typeof(CharacterModel);

        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return []; }
            })
            .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => t.GetProperty("StartingHp")?.GetGetMethod(true))
            .Where(m => m != null)
            .ToArray();
    }

    public static void Postfix(CharacterModel __instance, ref int __result)
    {
        if(!PP.RunOngoing) SetHp(__instance, ref __result);
    }
}