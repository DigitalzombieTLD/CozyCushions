using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection; 
using System.Collections;


namespace CozyCushions
{
    [RegisterTypeInIl2Cpp]
    public class PillowItem : MonoBehaviour
	{
        public PillowItem(IntPtr intPtr) : base(intPtr)
        {
        }

        public GameObject centerPoint;
        

        public void Awake()
        {     
            if (centerPoint == null)
            {
                centerPoint = gameObject.transform.FindChild("CenterPoint").gameObject;
            }
        }
    }
}