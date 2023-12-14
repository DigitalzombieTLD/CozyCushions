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
            if (__instance.gameObject.name.Contains("GEAR_PillowDZ") || __instance.gameObject.name.Contains("HideDried") || __instance.gameObject.name.Contains("PeltDried"))
            {
                SitOnMe sitComponent = __instance.gameObject.GetComponent<SitOnMe>();
                
                if (sitComponent == null)
                {
                    sitComponent = __instance.gameObject.AddComponent<SitOnMe>();
                }
            }
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(WaterSource), "Awake")]
    public class toiletPatcher
    {
        public static void Postfix(ref WaterSource __instance)
        {
            if (__instance.gameObject.name.Contains("Toilet"))
            {
                SitOnMe sitComponent = __instance.gameObject.GetComponent<SitOnMe>();

                if (sitComponent == null)
                {
                    sitComponent = __instance.gameObject.AddComponent<SitOnMe>();
                }
            }
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(BreakDown), "Awake")]
    public class breakdownComponentPatcher
    {
        public static void Postfix(ref BreakDown __instance)
        {
            if (__instance.gameObject.name.Contains("Bench") ||
                __instance.gameObject.name.Contains("Chair") ||
                __instance.gameObject.name.Contains("Stool"))
            {
                SitOnMe sitComponent = __instance.gameObject.GetComponent<SitOnMe>();

                if (sitComponent == null)
                {
                   sitComponent = __instance.gameObject.AddComponent<SitOnMe>();
                }
            }
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Bed), "Awake")]
    public class bedComponentPatcher
    {
        public static void Postfix(ref Bed __instance)
        {
            if (__instance.gameObject.name.Contains("Bed"))
            {
                SitOnMe sitComponent = __instance.gameObject.GetComponent<SitOnMe>();

                if (sitComponent == null)
                {
                    sitComponent = __instance.gameObject.AddComponent<SitOnMe>();
                }
            }
        }
    }



    [HarmonyLib.HarmonyPatch(typeof(Fatigue), "AddFatigue", new Type[] { typeof(float) })]
    public class fatiguePatcher
    {
        public static void Prefix(ref Fatigue __instance, ref float fatigueValue)
        {
            if (SitOnMe._playerIsSitting && Settings.options.enableBuffs)
            {
                if(SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Chair || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Bench || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Stool || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Rabbitpelt)
                {
                    fatigueValue = (fatigueValue / 100) * 80;
                }
                else if(SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.PillowDZ || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.PillowHL || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.CushionedBench || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.CushionedChair || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Wolfpelt || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Stagpelt)
                {
                    fatigueValue = (fatigueValue / 100) * 70;
                }
                else if ( SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.ArmChair || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Sofa || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Bearpelt)
                {
                    fatigueValue = (fatigueValue / 100) * 60;
                }
                else if (SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Bedroll || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.BedrollBearskin || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Bed || SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Bunkbed)
                {
                    fatigueValue = (fatigueValue / 100) * 50;
                }
                else if (SitOnMe._currentlySittingOn._fluffType == SitOnMe.FluffType.Toilet)
                {
                    fatigueValue = (fatigueValue / 100) * 120;
                }
            }            
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(uConsole), "RunCommand")]
    public class uConsolePatch
    {
        public static bool Prefix(ref uConsole __instance, ref string commandWithArgs)
        {
            if(SitOnMe._playerIsSitting)
            {
                if (commandWithArgs.Contains("unsit") || commandWithArgs.Contains("stand"))
                {
                    SitOnMe.StandUp();
                    return false;
                }
            }            

            return true;
        }
    }
}