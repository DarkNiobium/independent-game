using System;
using UnityEngine;

namespace IndependenceGame.Settings
{
    [Serializable]
    public class SettingsData
    {
        [Header("Audio")]
        [Range(0f, 1f)] public float masterVolume = 0.8f;
        [Range(0f, 1f)] public float musicVolume = 0.7f;
        [Range(0f, 1f)] public float sfxVolume = 0.8f;
        public bool isMuted = false;

        [Header("Graphics")]
        public int resolutionWidth = 1920;
        public int resolutionHeight = 1080;
        public int resolutionRefreshRate = 60;
        public int fullscreenModeIndex = 0; // 0: FullScreenWindow, 1: ExclusiveFullScreen, 2: Windowed
        public int qualityLevel = 2;
        public bool vSync = true;
        public int targetFps = 60; // 30, 60, 120, -1 (Unlimited)

        [Header("Gameplay")]
        public string language = "uz"; // "uz", "en", "ru"
        public float cameraPanSpeed = 1.0f; // 0.5 to 2.0
        public int autoSaveIntervalMinutes = 5; // 0, 5, 10, 15

        public static SettingsData CreateDefault()
        {
            var data = new SettingsData();
            if (Screen.currentResolution.width > 0 && Screen.currentResolution.height > 0)
            {
                data.resolutionWidth = Screen.currentResolution.width;
                data.resolutionHeight = Screen.currentResolution.height;
                data.resolutionRefreshRate = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value > 0 
                    ? Screen.currentResolution.refreshRateRatio.value 
                    : 60);
            }
            if (QualitySettings.names != null && QualitySettings.names.Length > 0)
            {
                data.qualityLevel = Mathf.Clamp(QualitySettings.GetQualityLevel(), 0, QualitySettings.names.Length - 1);
            }
            data.vSync = QualitySettings.vSyncCount > 0;
            return data;
        }
    }
}
