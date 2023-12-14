using MelonLoader;
using UnityEngine;
using System.Collections;
using Il2Cpp;

namespace CozyCushions
{
	public class CozyCushionsMain : MelonMod
	{
        public static int layerMask = 0;
        public override void OnInitializeMelon()
		{    
            Settings.OnLoad();
            layerMask |= 1 << 17; // gear layer
            layerMask |= 1 << 19; // InteractiveProp layer
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