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
              

        public override void OnUpdate()
        {
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.options.interactButton))
            {
                PlayerManager currentPlayerManager = GameManager.GetPlayerManagerComponent();
                GameObject targetObject = currentPlayerManager.GetInteractiveObjectUnderCrosshairs(2.5f);

                if (targetObject != null)
                {
                    SitOnMe foundSitItem = targetObject.transform.GetComponent<SitOnMe>();

                    if (foundSitItem)
                    {
                        foundSitItem.Sit();                       
                    }
                }
            }
        }
    }
}