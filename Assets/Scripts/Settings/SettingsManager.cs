using System;
using UnityEngine;

namespace IndependenceGame.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        public const string PrefsKey = "GameSettingsData";

        private static SettingsManager _instance;
        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<SettingsManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("SettingsManager");
                        _instance = go.AddComponent<SettingsManager>();
                    }
                }
                return _instance;
            }
        }

        [SerializeField] private SettingsData currentSettings;
        public SettingsData CurrentSettings => currentSettings;

        public event Action<SettingsData> OnSettingsChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }

            LoadSettings();
            ApplySettings(currentSettings);
        }

        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey(PrefsKey))
            {
                try
                {
                    string json = PlayerPrefs.GetString(PrefsKey);
                    currentSettings = JsonUtility.FromJson<SettingsData>(json);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[SettingsManager] Failed to parse saved settings, resetting: {ex.Message}");
                    currentSettings = SettingsData.CreateDefault();
                }
            }
            else
            {
                currentSettings = SettingsData.CreateDefault();
            }

            if (currentSettings == null)
            {
                currentSettings = SettingsData.CreateDefault();
            }
        }

        public void SaveSettings()
        {
            if (currentSettings == null) return;
            string json = JsonUtility.ToJson(currentSettings, true);
            PlayerPrefs.SetString(PrefsKey, json);
            PlayerPrefs.Save();
            Debug.Log("[SettingsManager] Settings saved to PlayerPrefs.");
        }

        public void ApplySettings(SettingsData data)
        {
            if (data == null) return;
            currentSettings = data;

            // 1. Audio
            AudioListener.volume = data.isMuted ? 0f : Mathf.Clamp01(data.masterVolume);
            AudioListener.pause = data.isMuted;

            // 2. Graphics - Resolution & Mode
            FullScreenMode mode = FullScreenMode.FullScreenWindow;
            switch (data.fullscreenModeIndex)
            {
                case 0: mode = FullScreenMode.FullScreenWindow; break;
                case 1: mode = FullScreenMode.ExclusiveFullScreen; break;
                case 2: mode = FullScreenMode.Windowed; break;
            }

            if (data.resolutionWidth > 0 && data.resolutionHeight > 0)
            {
                RefreshRate rr = new RefreshRate
                {
                    numerator = (uint)Mathf.Max(1, data.resolutionRefreshRate),
                    denominator = 1
                };
                Screen.SetResolution(data.resolutionWidth, data.resolutionHeight, mode, rr);
            }

            // 3. Quality & Performance
            if (QualitySettings.names != null && data.qualityLevel >= 0 && data.qualityLevel < QualitySettings.names.Length)
            {
                QualitySettings.SetQualityLevel(data.qualityLevel, true);
            }

            QualitySettings.vSyncCount = data.vSync ? 1 : 0;
            Application.targetFrameRate = data.targetFps;

            // 4. Fire notification
            OnSettingsChanged?.Invoke(currentSettings);
            Debug.Log($"[SettingsManager] Applied settings: MasterVol={data.masterVolume:P0}, Mute={data.isMuted}, Res={data.resolutionWidth}x{data.resolutionHeight}, Quality={data.qualityLevel}, VSync={data.vSync}, Lang={data.language}");
        }

        public void ResetToDefaults()
        {
            currentSettings = SettingsData.CreateDefault();
            ApplySettings(currentSettings);
            SaveSettings();
            Debug.Log("[SettingsManager] Reset settings to default configuration.");
        }
    }
}
