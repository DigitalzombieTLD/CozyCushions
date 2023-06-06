using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection; 
using System.Collections;
using Il2Cpp;

namespace CozyCushions
{
	public static class PillowManager
	{
        public static bool playerIsSitting = false;

        public static float m_StartCameraFOV;
        public static Vector2 m_StartPitchLimit;
        public static Vector2 m_StartYawLimit;
        public static Vector3 m_StartPlayerPosition;
        public static Quaternion m_BoardRotation;
        public static float m_StartAngleX;
        public static float m_StartAngleY;

        public static Vector3 GetOffsetPosition(PillowItem pillow)
        {
            Vector3 offsetPosition = new Vector3(pillow.centerPoint.transform.position.x, pillow.centerPoint.transform.position.y + Settings.options.headHeight, pillow.centerPoint.transform.position.z);

            return offsetPosition;
        }

        public static void SitDown(PillowItem pillow)
        {
            if (playerIsSitting)
            {
                return;
            }

            MelonLogger.Msg("Sitting down ...");
           
            CameraFade.StartAlphaFade(Color.black, true, 1, 0f, null);
            m_StartCameraFOV = GameManager.GetMainCamera().fieldOfView;
            m_StartPitchLimit = GameManager.GetVpFPSCamera().RotationPitchLimit;
            m_StartYawLimit = GameManager.GetVpFPSCamera().RotationYawLimit;
            m_StartPlayerPosition = GameManager.GetVpFPSPlayer().transform.position;

            m_StartAngleX = GameManager.GetVpFPSPlayer().transform.rotation.eulerAngles.x;
            m_StartAngleY = GameManager.GetVpFPSPlayer().transform.rotation.eulerAngles.y;
      
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.InSnowShelter);
            GameManager.GetPlayerManagerComponent().TeleportPlayer(GetOffsetPosition(pillow) - GameManager.GetVpFPSCamera().PositionOffset, GameManager.GetVpFPSCamera().transform.rotation * Quaternion.Euler(0, 180f, 0));
                       
            playerIsSitting = true;  
        }

        public static void StandUp()
        {
            if(!playerIsSitting)
            {
                return;
            }

            MelonLogger.Msg("Standing up ...");
            CameraFade.StartAlphaFade(Color.black, true, 1, 0f, null);
            
            GameManager.GetVpFPSCamera().m_PanViewCamera.m_IsDetachedFromPlayer = false;
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
            GameManager.GetVpFPSCamera().UnlockRotationLimit();

            GameManager.GetVpFPSCamera().RotationPitchLimit = m_StartPitchLimit;
            GameManager.GetVpFPSCamera().RotationYawLimit = m_StartYawLimit;
            GameManager.GetVpFPSPlayer().transform.position = m_StartPlayerPosition;
            GameManager.GetVpFPSCamera().transform.localPosition = GameManager.GetVpFPSCamera().PositionOffset;
            GameManager.GetVpFPSCamera().SetAngle(m_StartAngleY, m_StartAngleX);
            GameManager.GetVpFPSCamera().SetFOVFromOptions(m_StartCameraFOV);
            GameManager.GetVpFPSCamera().UpdateCameraRotation();
            GameManager.GetPlayerManagerComponent().StickPlayerToGround();

            playerIsSitting = false;
        }
    }
}