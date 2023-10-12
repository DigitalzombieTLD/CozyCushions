using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection; 
using System.Collections;
using Il2Cpp;

namespace CozyCushions
{
    [RegisterTypeInIl2Cpp]
    public class SitOnMe : MonoBehaviour
    {
        public SitOnMe(IntPtr intPtr) : base(intPtr)
        {
        }

        public static bool _playerIsSitting = false;

        private GameObject _centerPoint;
        private string _objectName;

        public static SitOnMe _currentlySittingOn;

        public enum FluffType { None, PillowDZ, PillowHL, Sofa, Chair, ArmChair, Bed, Bedroll, Bench, CushionedChair, CushionedBench, Toilet, BedrollBearskin, Stool, Bunkbed, Moosepelt, Wolfpelt, Stagpelt, Rabbitpelt, Bearpelt };
        public FluffType _fluffType;
        private bool _isSetup = false;
        private bool _isBeingSitOn = false;
        private float _maxAngle = 45.1f;
        private bool _buffActive = false;
        private static object _toiletRoutine;
        private static Vector3 _staticHeightOffset = new Vector3(0, 1f, 0);
        private uint _sitAudioID;
        private Bed _bedComponent;

        private Dictionary<FluffType, Vector3> _centerOffset = new Dictionary<FluffType, Vector3>
        {
            { FluffType.None, new Vector3(0, 0, 0) },
            { FluffType.PillowDZ, new Vector3(0, 0.3f, 0.335f)},
            { FluffType.PillowHL, new Vector3(0, 0.3f, 0)  },
            { FluffType.Sofa, new Vector3(0, 0, 0) },
            { FluffType.Chair, new Vector3(0, 0.58f, 0) },
            { FluffType.CushionedChair, new Vector3(0, 0.40f, 0) },
            { FluffType.CushionedBench, new Vector3(0, 0.10f, 0) },
            { FluffType.ArmChair, new Vector3(0, 0.40f, 0) },
            { FluffType.Bed, new Vector3(0, 0.45f, 0) },
            { FluffType.Bunkbed, new Vector3(0, 0.0f, 0) },
            { FluffType.Bedroll, new Vector3(0, 0.1f, 0) },
            { FluffType.Bench, new Vector3(0, 1f, 0) },
            { FluffType.BedrollBearskin, new Vector3(0, 0.5f, 0) },
            { FluffType.Stool, new Vector3(0, 1f, 0) },
            { FluffType.Bearpelt, new Vector3(0, 0, 0) },
            { FluffType.Wolfpelt, new Vector3(0, 0, 0) },
            { FluffType.Rabbitpelt, new Vector3(0, 0, 0) },
            { FluffType.Stagpelt, new Vector3(0, 0, 0) },
            { FluffType.Moosepelt, new Vector3(0, 0, 0) },
            { FluffType.Toilet, new Vector3(0, 0.50f, 0) }
        };

        private static float m_StartCameraFOV;
        private static Vector2 m_StartPitchLimit;
        private static Vector2 m_StartYawLimit;
        private static Vector3 m_StartPlayerPosition;
        private static float m_StartAngleX;
        private static float m_StartAngleY;

        private static float _originalTempBonus;
        private static float _originalTempClamp;
        private static float _originalTempMinutes;

        private static IEnumerator ToiletRoutine()
        {
            yield return new WaitForSeconds(1.0f);
            GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_DYSENTERY, GameManager.GetPlayerObject());
           
            yield return new WaitForSeconds(3.5f);
            GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_VOBREATHMEDIUMINTENSITYNOLOOP, GameManager.GetPlayerObject());
            yield return new WaitForSeconds(2.5f);           
            
            GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_FISHRELEASE, GameManager.GetPlayerObject());
            GameManager.GetThirstComponent().AddThirst(10f);
            yield return new WaitForSeconds(3.5f);
            GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_VOSTIMPACK, GameManager.GetPlayerObject());    
        }

        private static IEnumerator FlushRoutine()
        {
            yield return new WaitForSeconds(1.0f);
            GameAudioManager.PlaySound(Il2CppAK.EVENTS.PLAY_SNDINVWATERBOTTLE, GameManager.GetPlayerObject());
        }

        private bool IsObjectFlipped()
        {
            if(_fluffType == FluffType.Bedroll || _fluffType == FluffType.BedrollBearskin)
            {
                if(_bedComponent)
                {
                    if(_bedComponent.m_BedRollState == BedRollState.Rolled)
                    {
                        return true;
                    }
                }
            }

            float angleDifference = Vector3.Angle(gameObject.transform.up, Vector3.up);

            if (angleDifference >= _maxAngle)
            {
                return true;
            }



            return false;
        }

        public void Sit()
        {
            if (_playerIsSitting)
            {
                StandUp();
                return;
            }

            if (_fluffType != FluffType.PillowHL && _fluffType != FluffType.PillowDZ && IsObjectFlipped())
            {
                return;
            }

            MelonLogger.Msg("Sitting down ...");

            _currentlySittingOn = this;
            _playerIsSitting = true;
            _isBeingSitOn = true;

            CameraFade.StartAlphaFade(Color.black, true, 1.8f, 0f, null);
            GameManager.GetVpFPSPlayer().EnableCrouch(true);
            m_StartCameraFOV = GameManager.GetMainCamera().fieldOfView;
            m_StartPitchLimit = GameManager.GetVpFPSCamera().RotationPitchLimit;
            m_StartYawLimit = GameManager.GetVpFPSCamera().RotationYawLimit;
            m_StartPlayerPosition = GameManager.GetVpFPSPlayer().transform.position;

            m_StartAngleX = GameManager.GetVpFPSPlayer().transform.rotation.eulerAngles.x;
            m_StartAngleY = GameManager.GetVpFPSPlayer().transform.rotation.eulerAngles.y;

            //GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.InSnowShelter);
            GameManager.GetPlayerManagerComponent().DisableCharacterController();
            GameManager.GetPlayerManagerComponent().TeleportPlayer(GetSitPosition() - GameManager.GetVpFPSCamera().PositionOffset, GameManager.GetVpFPSCamera().transform.rotation);
            GameManager.GetVpFPSCamera().SetAngle(m_StartAngleY + 180f, 0f);
            GameManager.GetVpFPSCamera().SetNearPlaneOverride(0.001f);
            GameManager.GetPlayerManagerComponent().m_NearPlaneOverridden = true;
            ActivateBuff();
            PlaySitAudio();
        }



        public static void StandUp()
        {
            if (!_playerIsSitting)
            {
                return;
            }
            _currentlySittingOn.DeactivateBuff();
            _currentlySittingOn._isBeingSitOn = false;
            _playerIsSitting = false;
            _currentlySittingOn = null;

            MelonLogger.Msg("Standing up ...");
            GameManager.GetVpFPSPlayer().EnableCrouch(false);
            CameraFade.StartAlphaFade(Color.black, true, 1.2f, 0f, null);

            GameManager.GetVpFPSCamera().m_PanViewCamera.m_IsDetachedFromPlayer = false;
            //GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
            GameManager.GetPlayerManagerComponent().EnableCharacterController();
            GameManager.GetVpFPSCamera().UnlockRotationLimit();

            GameManager.GetVpFPSCamera().RotationPitchLimit = m_StartPitchLimit;
            GameManager.GetVpFPSCamera().RotationYawLimit = m_StartYawLimit;
            GameManager.GetVpFPSPlayer().transform.position = m_StartPlayerPosition;
            GameManager.GetVpFPSCamera().transform.localPosition = GameManager.GetVpFPSCamera().PositionOffset;
            GameManager.GetVpFPSCamera().SetAngle(m_StartAngleY, m_StartAngleX);
            GameManager.GetVpFPSCamera().SetFOVFromOptions(m_StartCameraFOV);
            GameManager.GetVpFPSCamera().UpdateCameraRotation();
            GameManager.GetPlayerManagerComponent().m_NearPlaneOverridden = false;
            GameManager.GetVpFPSCamera().StopNearPlaneOverride();

            GameManager.GetPlayerManagerComponent().StickPlayerToGround();
            
        }

        private Vector3 GetSitPosition()
        {            
            if (_fluffType == FluffType.Bench || _fluffType == FluffType.CushionedBench || _fluffType == FluffType.Sofa)
            {
                Ray ray = GameManager.GetMainCamera().ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 10))
                {
                    if (hit.transform.gameObject.name.Contains(_objectName))
                    {
                        return _staticHeightOffset + hit.point + _centerOffset[_fluffType];
                    }
                }
            }

            return _staticHeightOffset + _centerPoint.transform.position;            
        }

        private void CreateCenterpoint()
        {
            if (_centerPoint == null)
            {
                _centerPoint = new GameObject("CenterPoint");
                
                _centerPoint.transform.parent = this.transform;
                _centerPoint.transform.localPosition = _centerOffset[_fluffType];                
            }
        }

        private void UpdateSanitizedName()
        {
            string currentName = this.gameObject.name;

            _objectName = currentName.Replace("(Clone)", "").Trim();
        }

        private void UpdateFluffType()
        {
            if (_objectName.Contains("PillowDZ"))
            {
                _fluffType = FluffType.PillowDZ;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("Stool"))
            {
                _fluffType = FluffType.Stool;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVWOODLIGHT;
                return;
            }

            if (_objectName.Contains("Pillow"))
            {
                _fluffType = FluffType.PillowHL;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("CushionedBench"))
            {
                _fluffType = FluffType.CushionedBench;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("Bench"))
            {
                _fluffType = FluffType.Bench;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVWOODLIGHT;
                return;
            }

            if (_objectName.Contains("CushionedChair"))
            {
                _fluffType = FluffType.CushionedChair;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("ChairA"))
            {
                _fluffType = FluffType.ArmChair;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("Chair"))
            {
                _fluffType = FluffType.Chair;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVWOODLIGHT;
                return;
            }

            if (_objectName.Contains("BearSkinBedRoll"))
            {
                _fluffType = FluffType.BedrollBearskin;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("BedRoll"))
            {
                _fluffType = FluffType.Bedroll;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("BunkBed"))
            {
                _fluffType = FluffType.Bunkbed;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("Bed"))
            {
                _fluffType = FluffType.Bed;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("Sofa"))
            {
                _fluffType = FluffType.Sofa;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("Toilet"))
            {
                _fluffType = FluffType.Toilet;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDMECHVASESETDOWN;
                return;
            }

            if (_objectName.Contains("BearHide"))
            {
                _fluffType = FluffType.Bearpelt;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("MooseHide"))
            {
                _fluffType = FluffType.Moosepelt;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("LeatherHide"))
            {
                _fluffType = FluffType.Stagpelt;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_INVCLOTHLEATHERSTOW;
                return;
            }

            if (_objectName.Contains("WolfPelt"))
            {
                _fluffType = FluffType.Wolfpelt;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHHEAVY;
                return;
            }

            if (_objectName.Contains("RabbitPelt"))
            {
                _fluffType = FluffType.Rabbitpelt;
                _sitAudioID = Il2CppAK.EVENTS.PLAY_SNDINVCLOTHSMALL;
                return;
            }
        }

        public void Awake()
        {
            if (!_isSetup)
            {
                UpdateSanitizedName();
                UpdateFluffType();
                CreateCenterpoint();

                _bedComponent = this.gameObject.GetComponent<Bed>();
                

                _isSetup = true;
            }
        }

        private void PlaySitAudio()
        {
            GameAudioManager.PlaySound(_sitAudioID, GameManager.GetPlayerObject());
        }

        private void ActivateBuff()
        {
            if(!Settings.options.enableBuffs)
            {
                return;
            }

            _buffActive = true;

            _originalTempBonus = GameManager.GetFreezingComponent().m_TemperatureBonusFromRunning;
            _originalTempClamp = GameManager.GetFreezingComponent().m_MaxTemperatureBonusFromRunning;
            _originalTempMinutes = GameManager.GetFreezingComponent().m_HoldRunningTemperatureBonusMinutes;

            if (_fluffType == FluffType.Toilet)
            {
                _toiletRoutine = MelonCoroutines.Start(ToiletRoutine());
            }
        }
        private void DeactivateBuff()
        {
            if (!Settings.options.enableBuffs)
            {
                return;
            }

            GameManager.GetFreezingComponent().m_TemperatureBonusFromRunning = _originalTempBonus;
            GameManager.GetFreezingComponent().m_MaxTemperatureBonusFromRunning = _originalTempClamp;
            GameManager.GetFreezingComponent().m_HoldRunningTemperatureBonusMinutes = _originalTempMinutes;

            if (_fluffType == FluffType.Toilet)
            {
                if(_toiletRoutine != null)
                {
                    MelonCoroutines.Stop(_toiletRoutine);
                }

                MelonCoroutines.Start(FlushRoutine());
            }

            _buffActive = false;
        }

        public void Update()
        {
            if(_isBeingSitOn && Settings.options.enableBuffs)
            {
                if(_fluffType == FluffType.PillowDZ || 
                    _fluffType == FluffType.PillowHL || 
                    _fluffType == FluffType.CushionedBench || 
                    _fluffType == FluffType.CushionedChair || 
                    _fluffType == FluffType.ArmChair || 
                    _fluffType == FluffType.Sofa ||
                    _fluffType == FluffType.Wolfpelt ||
                    _fluffType == FluffType.Stagpelt)
                {
                    GameManager.GetFreezingComponent().m_TemperatureBonusFromRunning = 2f;
                    GameManager.GetFreezingComponent().m_MaxTemperatureBonusFromRunning = 2f;
                    GameManager.GetFreezingComponent().m_HoldRunningTemperatureBonusMinutes = 60f;
                }
                else if (_fluffType == FluffType.Bearpelt || _fluffType == FluffType.Moosepelt)
                {
                    GameManager.GetFreezingComponent().m_TemperatureBonusFromRunning = 3f;
                    GameManager.GetFreezingComponent().m_MaxTemperatureBonusFromRunning = 3f;
                    GameManager.GetFreezingComponent().m_HoldRunningTemperatureBonusMinutes = 60f;
                }
                else if (_fluffType == FluffType.Rabbitpelt)
                {
                    GameManager.GetFreezingComponent().m_TemperatureBonusFromRunning = 1f;
                    GameManager.GetFreezingComponent().m_MaxTemperatureBonusFromRunning = 1f;
                    GameManager.GetFreezingComponent().m_HoldRunningTemperatureBonusMinutes = 60f;
                }
                else if (_fluffType == FluffType.Bed || _fluffType == FluffType.Bedroll || _fluffType == FluffType.Bunkbed)
                {
                    GameManager.GetFreezingComponent().m_TemperatureBonusFromRunning = 4f;
                    GameManager.GetFreezingComponent().m_MaxTemperatureBonusFromRunning = 4f;
                    GameManager.GetFreezingComponent().m_HoldRunningTemperatureBonusMinutes = 60f;
                }
                else if (_fluffType == FluffType.BedrollBearskin)
                {
                    GameManager.GetFreezingComponent().m_TemperatureBonusFromRunning = 5.0f;
                    GameManager.GetFreezingComponent().m_MaxTemperatureBonusFromRunning = 5.0f;
                    GameManager.GetFreezingComponent().m_HoldRunningTemperatureBonusMinutes = 60f;
                }
                else if (_fluffType == FluffType.Toilet)
                {
                    GameManager.GetFreezingComponent().m_TemperatureBonusFromRunning = -2.0f;
                    GameManager.GetFreezingComponent().m_MaxTemperatureBonusFromRunning = -2.0f;
                    GameManager.GetFreezingComponent().m_HoldRunningTemperatureBonusMinutes = 60f;
                }
            }
        }
    }
}