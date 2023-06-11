using UnityEngine;
using ModSettings;
using MelonLoader;

namespace CozyCushions
{
    internal class CozyCushionsettings : JsonModSettings
    {
        [Section("Hotkeys")]

        [Name("Sit down / stand up")]
        [Description("Aim at pillows, beds, bedrolls, benches, chairs, stools, cured hides/pelts, toilets")]
        public KeyCode interactButton = KeyCode.Mouse2;

        
        [Name("Buffs")]
        [Description("Enable different buffs while sitting")]
        public bool enableBuffs = true;


        protected override void OnConfirm()
        {
            base.OnConfirm();
        }
    }

    internal static class Settings
    {
        public static CozyCushionsettings options;

        public static void OnLoad()
        {
            options = new CozyCushionsettings();
            options.AddToModSettings("CozyCushions");
        }
    }
}
