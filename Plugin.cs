using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System;
using System.Reflection;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Yoga;

namespace PermaBlink
{
    [BepInPlugin("com.astrovoid.permablink", "PermaBlink", "1.0.0")]
    public class PermaBlink : BaseUnityPlugin
    {
        private void Log(string message)
        {
            Logger.LogInfo(message);
        }

        private void Awake()
        {
            // Plugin startup logic
            Log($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            DoPatching();
        }

        private void DoPatching()
        {
            var harmony = new Harmony("com.astrovoid.permablink");

            Patch(harmony, typeof(ShootQuantum), "Awake", "Method1", true);
            Patch(harmony, typeof(ShootQuantum), "Shoot", "Method2", true);
        }

        private void OnDestroy()
        {
            Log($"Bye Bye From {PluginInfo.PLUGIN_GUID}");
        }
        private void Patch(Harmony harmony, Type OriginalClass, string OriginalMethod, string PatchMethod, bool prefix)
        {
            MethodInfo MethodToPatch = AccessTools.Method(OriginalClass, OriginalMethod); // the method to patch
            MethodInfo Patch = AccessTools.Method(typeof(Patches), PatchMethod);
            if (prefix)
            {
                harmony.Patch(MethodToPatch, new HarmonyMethod(Patch));
            }
            else
            {
                harmony.Patch(MethodToPatch, null, new HarmonyMethod(Patch));
            }
            Log($"Patched {OriginalMethod} in {OriginalClass.ToString()}");
        }
    }

    public class Patches
    {
        public static void Method1(ref ShootQuantum __instance)
        {
            __instance.WallDuration = (Fix)1000f;
        }
        public static void Method2(ref ShootQuantum __instance, ref Vec2 firepointFIX, ref Vec2 directionFIX)
        {
            RaycastInformation raycastInformation = DetPhysics.Get().PointCheckAllRoundedRects(firepointFIX);
            if (!raycastInformation)
            {
                raycastInformation = DetPhysics.Get().RaycastToClosest(firepointFIX, directionFIX, __instance.maxDistance, __instance.collisionMask);
            }
            if (raycastInformation.layer == LayerMask.NameToLayer("Player"))
            {
                int hitID = raycastInformation.pp.fixTrans.GetComponent<IPlayerIdHolder>().GetPlayerId();
                System.Diagnostics.Debug.Print(hitID.ToString());
                Player hitPlayer = PlayerHandler.Get().GetPlayer(hitID);
                System.Diagnostics.Debug.Print(hitPlayer.ToString());
                hitPlayer.Kill(__instance.ability.GetPlayerId(),(long)Updater.SimulationTicks,CauseOfDeath.Other);
                    ; ; ; ; ;       ; ; ; ; ; ; ; ;     ; ; ; ; ; ; ;
                    ; ; ; ; ; ;     ; ; ; ; ; ; ; ;     ; ; ; ; ; ; ;
                    ; ; ; ; ; ;           ; ;           ; ;
                    ; ; ;   ; ;           ; ;           ; ;
                    ; ; ;   ; ;           ; ;           ; ; ; ; ; ; ;
                    ; ; ;   ; ;           ; ;           ; ; ; ; ; ; ;
                    ; ; ; ; ; ;           ; ;           ; ;
                    ; ; ; ; ; ;     ; ; ; ; ; ; ; ;     ; ; ; ; ; ; ;
                    ; ; ; ; ;       ; ; ; ; ; ; ; ;     ; ; ; ; ; ; ;
            }
        }
    }
}
