using UnityEngine;
using ModSettings;
using MelonLoader;

namespace CozyCushions
{
    internal class CozyCushionsettings : JsonModSettings
    {
        [Section("Hotkeys")]

        [Name("Sit down / stand up")]
        [Description("Aim at pillow and press button to sit down or stand up")]
        public KeyCode interactButton = KeyCode.Mouse2;

        [Section("General")]

        [Name("Head height")]
        [Description("Adjust head height while sitting")]
        [Slider(0.5f, 1.8f)]
        public float headHeight = 1f;


        [Name("Fatigue loss")]
        [Description("When disabled, you won't get tired while sitting")]
        public bool fatigueLoss = true;


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
