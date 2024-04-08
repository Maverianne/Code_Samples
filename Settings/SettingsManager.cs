using Rewired;
using UnityEngine;

namespace _9YoS.Scripts.Managers {
    public class SettingsObject {
            private int _cameraShake;
            private int _currentLanguage;
            private int _graphicsResolution;
            private int _graphicsWindowMode;
            private int _showDamageNumbers;
            private int _showTips;
            private int _vibration;
            private int _volumeMusic;
            private int _volumeSfx;
            private int _fps;

            public int CurrentLanguage
            {
                get => _currentLanguage;
                set {
                    var previousSetting = _currentLanguage;
                    _currentLanguage = value;

                    if (previousSetting == value) return;
                    MainManager.Instance.SettingsManager.SetGameSetting(ConstantsManager.Settings.GameSettings.SaveValue.Language, _currentLanguage);
                    NotificationManager.TriggerEvent(ConstantsManager.Notification.LanguageChanged);
                }
            }

            public int ShowDamageNumbers
            {
                get => _showDamageNumbers;
                set {
                    var previousSetting = _showDamageNumbers;
                    _showDamageNumbers = value;
                    if (previousSetting == value) return;
                    MainManager.Instance.SettingsManager.SetGameSetting(ConstantsManager.Settings.GameSettings.SaveValue.NumericDamageFeedback, _showDamageNumbers);
                }
            }

            public int CameraShake
            {
                get => _cameraShake;
                set {
                    var previousSetting = _cameraShake;
                    _cameraShake = value;
                    if (previousSetting == value) return;
                    MainManager.Instance.SettingsManager.SetGameSetting(ConstantsManager.Settings.GameSettings.SaveValue.CamShake, _cameraShake);
                    NotificationManager.TriggerEvent(ConstantsManager.Notification.UpdateCameraShake);
                }
            }

            public int Vibration
            {
                get => _vibration;
                set {
                    var previousSetting = _vibration;
                    _vibration = value;

                    if (previousSetting == value) return;
                    MainManager.Instance.SettingsManager.SetGameSetting(ConstantsManager.Settings.GameSettings.SaveValue.Vibration, _vibration);
                }
            }

            public int ShowTips
            {
                get => _showTips;
                set {
                    var previousSetting = _showTips;
                    _showTips = value;

                    if (previousSetting == value) return;
                    MainManager.Instance.SettingsManager.SetGameSetting(ConstantsManager.Settings.GameSettings.SaveValue.ShowTip, _showTips);
                }
            }
            

            public int VolumeSfx {
                get => _volumeSfx;
                set {
                    _volumeSfx = value;
                    MainManager.Instance.SoundManager.UpdateVolume(_volumeSfx);
                    MainManager.Instance.SettingsManager.SetGameSetting(ConstantsManager.Settings.GameSettings.SaveValue.SfxSound, _volumeSfx);
                }
            }

            public int Fps
            {
                get => _fps;
                set
                {
                    var previousSetting = _fps;
                    _fps = value;
                    if (previousSetting == value) return;
                    MainManager.Instance.SettingsManager.SetFps(_fps);
                    MainManager.Instance.SettingsManager.SetGameSetting(ConstantsManager.Settings.GameSettings.SaveValue.Fps, _fps);

                }
            }

            public int VolumeMusic {
                get => _volumeMusic;
                set {
                    _volumeMusic = value;
                    MainManager.Instance.SoundtrackManager.UpdateVolume(_volumeMusic);
                    MainManager.Instance.SettingsManager.SetGameSetting(ConstantsManager.Settings.GameSettings.SaveValue.Music, _volumeMusic);
                }
            }

            public int GraphicsResolution
            {
                get => _graphicsResolution;
                set {
                    var previousSetting = _graphicsResolution;
                    _graphicsResolution = value;

                    if (previousSetting == value) return;
                    MainManager.Instance.SettingsManager.SetGameSetting(ConstantsManager.Settings.GameSettings.SaveValue.Resolution, _graphicsResolution);
                    MainManager.Instance.SettingsManager.SetScreenSizeAndMode();
                }
            }

            public int GraphicsWindowMode
            {
                get => _graphicsWindowMode;
                set {
                    var previousSetting = _graphicsWindowMode;
                    _graphicsWindowMode = value;

                    if (previousSetting == value) return;
                    MainManager.Instance.SettingsManager.SetGameSetting(ConstantsManager.Settings.GameSettings.SaveValue.WindowMode, _graphicsWindowMode);
                    MainManager.Instance.SettingsManager.SetScreenSizeAndMode();
                }
            }
        }
    public class ResolutionObject
    {
        private bool _fullHd;
        private bool _qhd;
        private bool _uhd4K;
        private bool _hd;
        private bool _nhd;

        public bool FullHd => MainManager.Instance.SettingsManager.HasResolution(ConstantsManager.Settings.GameSettings.Resolution.FullHD);
        public bool Qhd => MainManager.Instance.SettingsManager.HasResolution(ConstantsManager.Settings.GameSettings.Resolution.QHD);
        public bool Uhd4K  => MainManager.Instance.SettingsManager.HasResolution(ConstantsManager.Settings.GameSettings.Resolution.UHD4K);
        public bool Hd => MainManager.Instance.SettingsManager.HasResolution(ConstantsManager.Settings.GameSettings.Resolution.HD);
        public bool Nhd => MainManager.Instance.SettingsManager.HasResolution(ConstantsManager.Settings.GameSettings.Resolution.NHD);
    }
    public class SettingsManager : MonoBehaviour {
        
        [SerializeField] private ConstantsManager.Settings.GameSettings.Resolution[] resolutionsAvailable;
        public SettingsObject SettingsObject { get; private set; }
        public ResolutionObject ResolutionObject { get; private set; }
        
        private Resolution[] _resolutions;

        private ConstantsManager.Settings.GameSettings.WindowMode CurrentWindowMode
        {
            get
            {
                if (ReferenceEquals(MainManager.Instance.SettingsManager, null)) return ConstantsManager.Settings.GameSettings.WindowMode.FullScreen;
                return ( ConstantsManager.Settings.GameSettings.WindowMode) MainManager.Instance.SettingsManager.SettingsObject.GraphicsWindowMode;
            }
        }
        private ConstantsManager.Settings.GameSettings.Resolution CurrentResolution
        {
            get
            {
                if (ReferenceEquals(MainManager.Instance.SettingsManager, null)) return ConstantsManager.Settings.GameSettings.Resolution.FullHD;
                return ( ConstantsManager.Settings.GameSettings.Resolution) MainManager.Instance.SettingsManager.SettingsObject.GraphicsResolution;
            }
        }
        
        private void Awake()
        {
            SettingsObject = new SettingsObject();
            ResolutionObject = new ResolutionObject();
            _resolutions = Screen.resolutions;
        }
        

        private void LoadSettings()
        {
            SettingsObject.CameraShake  = SaveManager.GetInt(0, ConstantsManager.Settings.GameSettings.SaveValue.CamShake, (int)ConstantsManager.Settings.GameSettings.CameraShake.Full);
            SettingsObject.CurrentLanguage  = SaveManager.GetInt(0, ConstantsManager.Settings.GameSettings.SaveValue.Language, (int)ConstantsManager.Localization.Language.English);
            SettingsObject.GraphicsResolution  = SaveManager.GetInt(0, ConstantsManager.Settings.GameSettings.SaveValue.Resolution, CheckDefaultResolution());
            SettingsObject.GraphicsWindowMode  = SaveManager.GetInt(0, ConstantsManager.Settings.GameSettings.SaveValue.WindowMode);
            SettingsObject.ShowDamageNumbers = SaveManager.GetInt(0, ConstantsManager.Settings.GameSettings.SaveValue.NumericDamageFeedback, (int) ConstantsManager.Settings.GameSettings.EnabledSetting.Enabled);
            SettingsObject.ShowTips  = SaveManager.GetInt(0, ConstantsManager.Settings.GameSettings.SaveValue.ShowTip, (int) ConstantsManager.Settings.GameSettings.EnabledSetting.Enabled);
            SettingsObject.Vibration  = SaveManager.GetInt(0, ConstantsManager.Settings.GameSettings.SaveValue.Vibration, (int) ConstantsManager.Settings.GameSettings.EnabledSetting.Enabled);
            SettingsObject.VolumeMusic  = SaveManager.GetInt(0, ConstantsManager.Settings.GameSettings.SaveValue.Music, 100);
            SettingsObject.VolumeSfx  = SaveManager.GetInt(0, ConstantsManager.Settings.GameSettings.SaveValue.SfxSound, 100);
            var fpsDefault = MainManager.Instance.TargetPlatform != ConstantsManager.GamePlatform.Switch ? (int)ConstantsManager.Settings.GameSettings.Fps.Vsync : (int)ConstantsManager.Settings.GameSettings.Fps.Fps60;  
            SettingsObject.Fps  = SaveManager.GetInt(0, ConstantsManager.Settings.GameSettings.SaveValue.Fps, fpsDefault);
            ReInput.userDataStore.Load();
        }
        
        public void SetGameSetting(string settingName, int settingValue, bool autoSave = false) {
            SaveManager.SetInt(0, settingName, settingValue, autoSave);
        }

        public void InitializedSettings()
        { 
            LoadSettings();
            SetScreenSizeAndMode(); 
            SetInitialVolume();
            SaveManager.Save(0);
        }
        public void SetScreenSizeAndMode() {
            //Screen Mode
            var screenMode = FullScreenMode.FullScreenWindow;
            
            switch (CurrentWindowMode) {
                case ConstantsManager.Settings.GameSettings.WindowMode.FullScreen:
                    screenMode = FullScreenMode.FullScreenWindow;
                    break;
                case ConstantsManager.Settings.GameSettings.WindowMode.Windowed:
                    screenMode = FullScreenMode.Windowed;
                    break;
            }
            var width = GetWidthAndHeight(CurrentResolution)[0];
            var height= GetWidthAndHeight(CurrentResolution)[1];
            
            Screen.SetResolution(width, height, screenMode, 60);
    
        }

        public void SetFps(int val)
        {
            switch (val)
            {
                case 0:
                    QualitySettings.vSyncCount = 1;
                    Application.targetFrameRate = 30;
                    break;
                case 1:
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 30;
                    break;
                case 2:
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 60;
                    break;
                case 3:
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 120;
                    break;
            }
        }

        private void SetInitialVolume() {
            MainManager.Instance.SoundtrackManager.UpdateVolume(SettingsObject.VolumeMusic);
            MainManager.Instance.SoundManager.UpdateVolume(SettingsObject.VolumeSfx);
        }

  
        private int CheckDefaultResolution()
        {
            var currentResolution = Screen.currentResolution;
            foreach (var resolution in resolutionsAvailable)
            {
                var width = GetWidthAndHeight(resolution)[0];
                var height = GetWidthAndHeight(resolution)[1];
                if (currentResolution.width != width && currentResolution.height != height) continue;
                return (int)resolution;
            }
            for (var i = 0; i < resolutionsAvailable.Length - 1; i++)
            {
                if (!HasResolution(resolutionsAvailable[i])) continue;
                return (int)resolutionsAvailable[i];
            }
            return 0;
        }
        
        public bool HasResolution(ConstantsManager.Settings.GameSettings.Resolution resolution)
        {
            var width = GetWidthAndHeight(resolution)[0];
            var height= GetWidthAndHeight(resolution)[1];
            foreach (var screenResolutions in _resolutions)
            {
                if(screenResolutions.width != width && screenResolutions.height != height) continue;
                return true;
            }
            return false;
        }

        private int[] GetWidthAndHeight(ConstantsManager.Settings.GameSettings.Resolution resolution)
        {
            switch (resolution)
            {
                case ConstantsManager.Settings.GameSettings.Resolution.FullHD: return new[] { 1920, 1080 };
                case ConstantsManager.Settings.GameSettings.Resolution.QHD: return new[] { 2560, 1440 };
                case ConstantsManager.Settings.GameSettings.Resolution.UHD4K: return new[] { 3840, 2160 };
                case ConstantsManager.Settings.GameSettings.Resolution.HD: return new[] { 1280, 720 };
                case ConstantsManager.Settings.GameSettings.Resolution.NHD: return new[] { 640, 360 };
            }
            return new[] { 1920, 1080 };
        }
    }
}