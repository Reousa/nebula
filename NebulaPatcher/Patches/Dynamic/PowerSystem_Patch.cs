﻿#region

using HarmonyLib;
using NebulaWorld;

#endregion

namespace NebulaPatcher.Patches.Dynamic;

[HarmonyPatch(typeof(PowerSystem))]
internal class PowerSystem_Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PowerSystem.GameTick))]
    public static void PowerSystem_GameTick_Prefix(long time, ref bool isActive)
    {
        //Enable signType update on remote planet every 64 tick
        if ((time & 63) == 0 && Multiplayer.IsActive && Multiplayer.Session.LocalPlayer.IsHost)
        {
            isActive = true;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PowerSystem.GameTick))]
    public static void PowerSystem_GameTick_Postfix(PowerSystem __instance)
    {
        if (!Multiplayer.IsActive)
        {
            return;
        }
        for (var i = 1; i < __instance.netCursor; i++)
        {
            var pNet = __instance.netPool[i];
            pNet.energyRequired += Multiplayer.Session.PowerTowers.GetExtraDemand(__instance.planet.id, i);
        }
        Multiplayer.Session.PowerTowers.GivePlayerPower();
        Multiplayer.Session.PowerTowers.UpdateAllAnimations(__instance.planet.id);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PowerSystem.RemoveNodeComponent))]
    public static bool RemoveNodeComponent(PowerSystem __instance, int id)
    {
        if (!Multiplayer.IsActive)
        {
            return true;
        }
        // as the destruct is synced across players this event is too
        // and as such we can safely remove power demand for every player
        var pComp = __instance.nodePool[id];
        Multiplayer.Session.PowerTowers.RemExtraDemand(__instance.planet.id, pComp.networkId, id);

        return true;
    }
}
