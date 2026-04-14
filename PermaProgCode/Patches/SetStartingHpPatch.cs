using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace PermaProg.PermaProgCode.Patches;

[HarmonyPatch]
public static class SetStartingHpPatch
{

    static MethodBase[] TargetMethods()
    {
        var baseType = typeof(CharacterModel);

        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => t.GetProperty("StartingHp")?.GetGetMethod(true))
            .Where(m => m != null)
            .ToArray();
    }

    private static void SetHp(ref int __result)
    {
        if (PP.BalancingEnabled) __result = (int)(__result * 0.8);
        __result += (int)PP.MaxHealthValue;
    }
}