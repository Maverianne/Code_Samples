using System;
using _9YoS.Scripts.Managers;
using _9YoS.Scripts.UI.GameSettingBase;
using Rewired;
using TMPro;
using UnityEngine;

namespace _9YoS.Scripts.UI.Selections
{
    [Serializable]
    public struct SpinnerRecord
    {
        public int mySavingValue;
        public string localizedStringTag;
    }
    public class SpinnerBase : MonoBehaviour
    {
        [SerializeField] private ConstantsManager.Settings.GameSettings.Setting mySetting;
        [SerializeField] private string settingName;
        [SerializeField] private SpinnerRecord[] mySpinnerRecords;
        [SerializeField] private bool canRestore;
        [SerializeField] private int defaultChoice;
        [SerializeField] private GameSettingsSecondaryScreenBase myTabSetting;
        protected TMP_Text TMPText;
        protected int ChoiceInt;
        private bool _listeningForNotifications;
        
        private float _moveStartTime;
        protected bool IsMoving => Time.unscaledTime <= _moveStartTime + ConstantsManager.Input.UiInpuValues.ConstantMoveDelay;

        protected string SettingName => settingName;
        protected SpinnerRecord[] MySpinnerRecords => mySpinnerRecords;

        protected float MoveStartTime
        {
            get => _moveStartTime;
            set => _moveStartTime = value;
        }

        public ConstantsManager.Settings.GameSettings.Setting MySetting => mySetting;
        protected virtual void Awake()
        {
            TMPText = GetComponentInChildren<TMP_Text>();
        }

        public virtual void StartSpinner()
        {
            SetupInputDelegates(true);
            UpdateSetting();
        }

        public virtual void UpdateSetting()
        {
            var saveValue = GetSaveValue();
            ChoiceInt = 0;
            for (int i = 0; i < MySpinnerRecords.Length; i++)
            {
                var checkValue = MySpinnerRecords[i].mySavingValue;
                if (checkValue != saveValue) continue;
                ChoiceInt = i;
                break;
            }
            TMPText.text = MainManager.Instance.LocalizationManager.GetLocalizedString(MySpinnerRecords[ChoiceInt].localizedStringTag);
        }
        
        public virtual void SetVariable()
        {
            SetupInputDelegates(false);
            SaveSetting();
        }

        protected virtual void Movement(InputActionEventData obj)
        {
            if (!myTabSetting.SecondaryScreenActive)
            {
                SetupInputDelegates(false);
            }
            const float moveThreshold = 0.6f;
            if (obj.GetAxis() > moveThreshold)Move(false) ;
            else if (obj.GetAxis() < -moveThreshold) Move(true);
        }
        

        public virtual void Move(bool movingLeft)
        {
            if (IsMoving) return;
            ChoiceInt = movingLeft ? ChoiceInt - 1 : ChoiceInt + 1;
            ChoiceInt = (ChoiceInt + MySpinnerRecords.Length) % MySpinnerRecords.Length;
            TMPText.text = MainManager.Instance.LocalizationManager.GetLocalizedString(MySpinnerRecords[ChoiceInt].localizedStringTag);
            _moveStartTime = Time.unscaledTime;
        }

        protected virtual void SetupInputDelegates(bool setup)
        {
            if (setup) MainManager.Instance.MenuInputState.SetAction(Movement, UpdateLoopType.Update, InputActionEventType.AxisActive, ConstantsManager.Input.UI.Horizontal);
            else MainManager.Instance.MenuInputState.removeAction(Movement, UpdateLoopType.Update, InputActionEventType.AxisActive, ConstantsManager.Input.UI.Horizontal);

        }

        public virtual void RestoreVariable()
        {
            if (!canRestore) return; 
            ChoiceInt = defaultChoice;
            SaveSetting();
        }
        
        protected virtual void SaveSetting()
        {
            switch (mySetting)
            {
                case ConstantsManager.Settings.GameSettings.Setting.Language:
                    MainManager.Instance.SettingsManager.SettingsObject.CurrentLanguage = MySpinnerRecords[ChoiceInt].mySavingValue;
                    break;
                case ConstantsManager.Settings.GameSettings.Setting.ShowTips:
                    MainManager.Instance.SettingsManager.SettingsObject.ShowTips = MySpinnerRecords[ChoiceInt].mySavingValue;
                    break;
                case ConstantsManager.Settings.GameSettings.Setting.CamShake:
                    MainManager.Instance.SettingsManager.SettingsObject.CameraShake = MySpinnerRecords[ChoiceInt].mySavingValue;
                    break;
                case ConstantsManager.Settings.GameSettings.Setting.Vibration:
                    MainManager.Instance.SettingsManager.SettingsObject.Vibration = MySpinnerRecords[ChoiceInt].mySavingValue;
                    break;
                case ConstantsManager.Settings.GameSettings.Setting.WindowMode:
                    if(MainManager.Instance.TargetPlatform != ConstantsManager.GamePlatform.Switch) MainManager.Instance.SettingsManager.SettingsObject.GraphicsWindowMode = MySpinnerRecords[ChoiceInt].mySavingValue;
                    break;
                case ConstantsManager.Settings.GameSettings.Setting.ShowDamage:
                    MainManager.Instance.SettingsManager.SettingsObject.ShowDamageNumbers = MySpinnerRecords[ChoiceInt].mySavingValue;
                    break;
                case ConstantsManager.Settings.GameSettings.Setting.Fps:
                    if(MainManager.Instance.TargetPlatform != ConstantsManager.GamePlatform.Switch) MainManager.Instance.SettingsManager.SettingsObject.Fps = MySpinnerRecords[ChoiceInt].mySavingValue;
                    break;
            }
            TMPText.text = MainManager.Instance.LocalizationManager.GetLocalizedString(MySpinnerRecords[ChoiceInt].localizedStringTag);
        }

        protected virtual int GetSaveValue()
        {
            switch (mySetting)
            {
                case ConstantsManager.Settings.GameSettings.Setting.Language: return MainManager.Instance.SettingsManager.SettingsObject.CurrentLanguage;
                case ConstantsManager.Settings.GameSettings.Setting.ShowTips: return MainManager.Instance.SettingsManager.SettingsObject.ShowTips;
                case ConstantsManager.Settings.GameSettings.Setting.CamShake: return MainManager.Instance.SettingsManager.SettingsObject.CameraShake;
                case ConstantsManager.Settings.GameSettings.Setting.Vibration: return MainManager.Instance.SettingsManager.SettingsObject.Vibration;
                case ConstantsManager.Settings.GameSettings.Setting.SfxSound: return MainManager.Instance.SettingsManager.SettingsObject.VolumeSfx;
                case ConstantsManager.Settings.GameSettings.Setting.Music: return MainManager.Instance.SettingsManager.SettingsObject.VolumeMusic;
                case ConstantsManager.Settings.GameSettings.Setting.Resolution: return MainManager.Instance.SettingsManager.SettingsObject.GraphicsResolution;
                case ConstantsManager.Settings.GameSettings.Setting.WindowMode: return MainManager.Instance.SettingsManager.SettingsObject.GraphicsWindowMode;
                case ConstantsManager.Settings.GameSettings.Setting.ShowDamage: return MainManager.Instance.SettingsManager.SettingsObject.ShowDamageNumbers;
                case ConstantsManager.Settings.GameSettings.Setting.Fps: return MainManager.Instance.SettingsManager.SettingsObject.Fps;
            }
            return 0;
        }
        private void RegisterNotifications(bool register)
        {
            if (register)
            {
                if(_listeningForNotifications) return;
                _listeningForNotifications = true;
                NotificationManager.StartListening(ConstantsManager.Notification.LanguageChanged, SaveSetting);
            }
            else
            {
                if(!_listeningForNotifications) return;
                _listeningForNotifications = false;
                NotificationManager.StopListening(ConstantsManager.Notification.LanguageChanged, SaveSetting);
            }
        }   
        private void OnEnable()
        {
            RegisterNotifications(true);
        }

        private void OnDisable()
        {
            RegisterNotifications(false);
        }

        private void OnDestroy()
        {
            RegisterNotifications(false);
        }

    }
}