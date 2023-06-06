using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection; 
using System.Collections;
using Il2Cpp;

namespace CozyCushions
{
	public class CozyCushionsMain : MelonMod
	{
        public override void OnInitializeMelon()
		{    
            Settings.OnLoad();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
       


        }

        public override void OnUpdate()
        {
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.interactButton))
            {
                PlayerManager currentPlayerManager = GameManager.GetPlayerManagerComponent();
                GameObject targetObject = currentPlayerManager.GetInteractiveObjectUnderCrosshairs(2.5f);

                if (targetObject != null && targetObject.name.Contains("PillowDZ"))
                {
                    if (PillowManager.playerIsSitting)
                    {
                        PillowManager.StandUp();
                        return;
                    }


                    PillowItem foundPillow = targetObject.transform.GetComponent<PillowItem>();

                    if(foundPillow != null)
                    {
                        PillowManager.SitDown(foundPillow);
                    }                   
                }
            }
        }
    }
}