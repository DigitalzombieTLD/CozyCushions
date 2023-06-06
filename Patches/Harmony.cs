using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection; 
using System.Collections;
using Il2Cpp;

namespace CozyCushions
{
    [HarmonyLib.HarmonyPatch(typeof(GearItem), "Awake")]
    public class pillowComponentPatcher
    {
        public static void Postfix(ref GearItem __instance)
        {
            if (__instance.gameObject.name.Contains("GEAR_PillowDZ"))
            {                              
                PillowItem pillowComponent = __instance.gameObject.GetComponent<PillowItem>();
                
                if (pillowComponent == null)
                {
                    pillowComponent = __instance.gameObject.AddComponent<PillowItem>();
                }
            }
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Fatigue), "AddFatigue")]
    public class fatiguePatcher
    {
        public static bool Prefix(ref Fatigue __instance, ref float fatigueValue)
        {
            if (!Settings.options.fatigueLoss && fatigueValue < 0)
            {
                return false;
            }

            return true;
        }
    }
}