using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IndependenceGame.Settings
{
    public class SettingsUIController : MonoBehaviour
    {
        [Header("Window & Roots")]
        [SerializeField] private GameObject windowRoot;
        [SerializeField] private Button closeButton;

        [Header("Tabs")]
        [SerializeField] private List<SettingsTabUI> tabs = new List<SettingsTabUI>();
        [SerializeField] private GameObject audioPanel;
        [SerializeField] private GameObject graphicsPanel;
        [SerializeField] private GameObject gameplayPanel;

        [Header("Audio Controls")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private TextMeshProUGUI masterVolumeText;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;
        [SerializeField] private Toggle muteToggle;
        [SerializeField] private TextMeshProUGUI muteToggleStatusText;

        [Header("Graphics Controls")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown fullscreenDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Toggle vSyncToggle;
        [SerializeField] private TextMeshProUGUI vSyncStatusText;
        [SerializeField] private TMP_Dropdown targetFpsDropdown;

        [Header("Gameplay Controls")]
        [SerializeField] private TMP_Dropdown languageDropdown;
        [SerializeField] private Slider cameraPanSpeedSlider;
        [SerializeField] private TextMeshProUGUI cameraPanSpeedText;
        [SerializeField] private TMP_Dropdown autoSaveDropdown;

        [Header("Action Buttons")]
        [SerializeField] private Button resetDefaultsButton;
        [SerializeField] private Button saveApplyButton;

        private SettingsData workingSettings;
        private List<Resolution> availableResolutions = new List<Resolution>();
        private readonly int[] fpsValues = { 30, 60, 120, -1 };
        private readonly int[] autoSaveValues = { 0, 5, 10, 15 };
        private readonly string[] languageCodes = { "uz", "en", "ru" };

        private void Awake()
        {
            // Initialize tab buttons
            foreach (var tab in tabs)
            {
                if (tab != null)
                {
                    tab.Initialize(OnTabSelected);
                }
            }

            // Action buttons
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);

            if (resetDefaultsButton != null)
                resetDefaultsButton.onClick.AddListener(OnResetDefaultsClicked);

            if (saveApplyButton != null)
                saveApplyButton.onClick.AddListener(OnSaveApplyClicked);

            // Wire Audio listeners
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.onValueChanged.AddListener(val =>
                {
                    workingSettings.masterVolume = val;
                    UpdateAudioLabels();
                    LiveApplyAudio();
                });
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.onValueChanged.AddListener(val =>
                {
                    workingSettings.musicVolume = val;
                    UpdateAudioLabels();
                });
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener(val =>
                {
                    workingSettings.sfxVolume = val;
                    UpdateAudioLabels();
                });
            }

            if (muteToggle != null)
            {
                muteToggle.onValueChanged.AddListener(val =>
                {
                    workingSettings.isMuted = val;
                    UpdateAudioLabels();
                    LiveApplyAudio();
                });
            }

            // Wire Graphics listeners
            if (resolutionDropdown != null)
            {
                resolutionDropdown.onValueChanged.AddListener(idx =>
                {
                    if (idx >= 0 && idx < availableResolutions.Count)
                    {
                        var res = availableResolutions[idx];
                        workingSettings.resolutionWidth = res.width;
                        workingSettings.resolutionHeight = res.height;
                        workingSettings.resolutionRefreshRate = (int)Math.Round(res.refreshRateRatio.value > 0 ? res.refreshRateRatio.value : 60);
                    }
                });
            }

            if (fullscreenDropdown != null)
            {
                fullscreenDropdown.onValueChanged.AddListener(idx =>
                {
                    workingSettings.fullscreenModeIndex = idx;
                });
            }

            if (qualityDropdown != null)
            {
                qualityDropdown.onValueChanged.AddListener(idx =>
                {
                    workingSettings.qualityLevel = idx;
                });
            }

            if (vSyncToggle != null)
            {
                vSyncToggle.onValueChanged.AddListener(val =>
                {
                    workingSettings.vSync = val;
                    UpdateGraphicsLabels();
                });
            }

            if (targetFpsDropdown != null)
            {
                targetFpsDropdown.onValueChanged.AddListener(idx =>
                {
                    if (idx >= 0 && idx < fpsValues.Length)
                    {
                        workingSettings.targetFps = fpsValues[idx];
                    }
                });
            }

            // Wire Gameplay listeners
            if (languageDropdown != null)
            {
                languageDropdown.onValueChanged.AddListener(idx =>
                {
                    if (idx >= 0 && idx < languageCodes.Length)
                    {
                        workingSettings.language = languageCodes[idx];
                    }
                });
            }

            if (cameraPanSpeedSlider != null)
            {
                cameraPanSpeedSlider.onValueChanged.AddListener(val =>
                {
                    workingSettings.cameraPanSpeed = val;
                    UpdateGameplayLabels();
                });
            }

            if (autoSaveDropdown != null)
            {
                autoSaveDropdown.onValueChanged.AddListener(idx =>
                {
                    if (idx >= 0 && idx < autoSaveValues.Length)
                    {
                        workingSettings.autoSaveIntervalMinutes = autoSaveValues[idx];
                    }
                });
            }
        }

        private void Start()
        {
            SetupResolutionDropdown();
            SetupQualityDropdown();
            SetupFullscreenDropdown();
            SetupFpsDropdown();
            SetupLanguageDropdown();
            SetupAutoSaveDropdown();

            LoadWorkingSettingsFromManager();
            PopulateUIFromWorkingSettings();
            SelectTab(SettingsTabCategory.Audio);
        }

        private void SetupResolutionDropdown()
        {
            if (resolutionDropdown == null) return;

            resolutionDropdown.ClearOptions();
            availableResolutions.Clear();

            List<string> options = new List<string>();
            var resList = Screen.resolutions;
            var seen = new HashSet<string>();

            for (int i = 0; i < resList.Length; i++)
            {
                string key = $"{resList[i].width}x{resList[i].height}";
                if (!seen.Contains(key))
                {
                    seen.Add(key);
                    availableResolutions.Add(resList[i]);
                    options.Add($"{resList[i].width} x {resList[i].height}");
                }
            }

            if (availableResolutions.Count == 0)
            {
                var fallback = Screen.currentResolution;
                availableResolutions.Add(fallback);
                options.Add($"{fallback.width} x {fallback.height}");
            }

            resolutionDropdown.AddOptions(options);
        }

        private void SetupQualityDropdown()
        {
            if (qualityDropdown == null) return;
            qualityDropdown.ClearOptions();

            List<string> options = new List<string>();
            if (QualitySettings.names != null && QualitySettings.names.Length > 0)
            {
                foreach (var name in QualitySettings.names)
                {
                    options.Add(name);
                }
            }
            else
            {
                options.AddRange(new[] { "Past (Low)", "O'rta (Medium)", "Yuqori (High)", "Ultra" });
            }

            qualityDropdown.AddOptions(options);
        }

        private void SetupFullscreenDropdown()
        {
            if (fullscreenDropdown == null) return;
            fullscreenDropdown.ClearOptions();
            fullscreenDropdown.AddOptions(new List<string>
            {
                "To'liq ekran (Borderless)",
                "To'liq ekran (Eksklyuziv)",
                "Oyna rejimida (Windowed)"
            });
        }

        private void SetupFpsDropdown()
        {
            if (targetFpsDropdown == null) return;
            targetFpsDropdown.ClearOptions();
            targetFpsDropdown.AddOptions(new List<string>
            {
                "30 FPS",
                "60 FPS",
                "120 FPS",
                "Cheksiz (Unlimited)"
            });
        }

        private void SetupLanguageDropdown()
        {
            if (languageDropdown == null) return;
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(new List<string>
            {
                "O'zbekcha",
                "English",
                "Русский"
            });
        }

        private void SetupAutoSaveDropdown()
        {
            if (autoSaveDropdown == null) return;
            autoSaveDropdown.ClearOptions();
            autoSaveDropdown.AddOptions(new List<string>
            {
                "O'chirilgan (Off)",
                "Har 5 daqiqada",
                "Har 10 daqiqada",
                "Har 15 daqiqada"
            });
        }

        private void LoadWorkingSettingsFromManager()
        {
            var source = SettingsManager.Instance.CurrentSettings;
            workingSettings = new SettingsData
            {
                masterVolume = source.masterVolume,
                musicVolume = source.musicVolume,
                sfxVolume = source.sfxVolume,
                isMuted = source.isMuted,
                resolutionWidth = source.resolutionWidth,
                resolutionHeight = source.resolutionHeight,
                resolutionRefreshRate = source.resolutionRefreshRate,
                fullscreenModeIndex = source.fullscreenModeIndex,
                qualityLevel = source.qualityLevel,
                vSync = source.vSync,
                targetFps = source.targetFps,
                language = source.language,
                cameraPanSpeed = source.cameraPanSpeed,
                autoSaveIntervalMinutes = source.autoSaveIntervalMinutes
            };
        }

        private void PopulateUIFromWorkingSettings()
        {
            if (workingSettings == null) return;

            // Audio
            if (masterVolumeSlider != null) masterVolumeSlider.SetValueWithoutNotify(workingSettings.masterVolume);
            if (musicVolumeSlider != null) musicVolumeSlider.SetValueWithoutNotify(workingSettings.musicVolume);
            if (sfxVolumeSlider != null) sfxVolumeSlider.SetValueWithoutNotify(workingSettings.sfxVolume);
            if (muteToggle != null) muteToggle.SetIsOnWithoutNotify(workingSettings.isMuted);
            UpdateAudioLabels();

            // Graphics
            if (resolutionDropdown != null)
            {
                int matchIndex = 0;
                for (int i = 0; i < availableResolutions.Count; i++)
                {
                    if (availableResolutions[i].width == workingSettings.resolutionWidth &&
                        availableResolutions[i].height == workingSettings.resolutionHeight)
                    {
                        matchIndex = i;
                        break;
                    }
                }
                resolutionDropdown.SetValueWithoutNotify(matchIndex);
            }

            if (fullscreenDropdown != null)
                fullscreenDropdown.SetValueWithoutNotify(Mathf.Clamp(workingSettings.fullscreenModeIndex, 0, 2));

            if (qualityDropdown != null && qualityDropdown.options.Count > 0)
                qualityDropdown.SetValueWithoutNotify(Mathf.Clamp(workingSettings.qualityLevel, 0, qualityDropdown.options.Count - 1));

            if (vSyncToggle != null)
                vSyncToggle.SetIsOnWithoutNotify(workingSettings.vSync);

            if (targetFpsDropdown != null)
            {
                int fpsIdx = Array.IndexOf(fpsValues, workingSettings.targetFps);
                if (fpsIdx < 0) fpsIdx = 1; // default 60
                targetFpsDropdown.SetValueWithoutNotify(fpsIdx);
            }
            UpdateGraphicsLabels();

            // Gameplay
            if (languageDropdown != null)
            {
                int langIdx = Array.IndexOf(languageCodes, workingSettings.language);
                if (langIdx < 0) langIdx = 0;
                languageDropdown.SetValueWithoutNotify(langIdx);
            }

            if (cameraPanSpeedSlider != null)
                cameraPanSpeedSlider.SetValueWithoutNotify(workingSettings.cameraPanSpeed);

            if (autoSaveDropdown != null)
            {
                int autoIdx = Array.IndexOf(autoSaveValues, workingSettings.autoSaveIntervalMinutes);
                if (autoIdx < 0) autoIdx = 1; // default 5m
                autoSaveDropdown.SetValueWithoutNotify(autoIdx);
            }
            UpdateGameplayLabels();
        }

        private void UpdateAudioLabels()
        {
            if (masterVolumeText != null) masterVolumeText.text = $"{Mathf.RoundToInt(workingSettings.masterVolume * 100)}%";
            if (musicVolumeText != null) musicVolumeText.text = $"{Mathf.RoundToInt(workingSettings.musicVolume * 100)}%";
            if (sfxVolumeText != null) sfxVolumeText.text = $"{Mathf.RoundToInt(workingSettings.sfxVolume * 100)}%";
            if (muteToggleStatusText != null) muteToggleStatusText.text = workingSettings.isMuted ? "O'CHIRILGAN (MUTED)" : "YOQILGAN (ACTIVE)";
        }

        private void UpdateGraphicsLabels()
        {
            if (vSyncStatusText != null) vSyncStatusText.text = workingSettings.vSync ? "YOQILGAN (ON)" : "O'CHIRILGAN (OFF)";
        }

        private void UpdateGameplayLabels()
        {
            if (cameraPanSpeedText != null) cameraPanSpeedText.text = $"{workingSettings.cameraPanSpeed:F1}x";
        }

        private void LiveApplyAudio()
        {
            AudioListener.volume = workingSettings.isMuted ? 0f : Mathf.Clamp01(workingSettings.masterVolume);
            AudioListener.pause = workingSettings.isMuted;
        }

        public void OnTabSelected(SettingsTabCategory category)
        {
            SelectTab(category);
        }

        public void SelectTab(SettingsTabCategory category)
        {
            foreach (var tab in tabs)
            {
                if (tab != null)
                {
                    tab.SetActive(tab.Category == category);
                }
            }

            if (audioPanel != null) audioPanel.SetActive(category == SettingsTabCategory.Audio);
            if (graphicsPanel != null) graphicsPanel.SetActive(category == SettingsTabCategory.Graphics);
            if (gameplayPanel != null) gameplayPanel.SetActive(category == SettingsTabCategory.Gameplay);
        }

        public void OnResetDefaultsClicked()
        {
            SettingsManager.Instance.ResetToDefaults();
            LoadWorkingSettingsFromManager();
            PopulateUIFromWorkingSettings();
            Debug.Log("[SettingsUI] Reset all UI controls to defaults.");
        }

        public void OnSaveApplyClicked()
        {
            SettingsManager.Instance.ApplySettings(workingSettings);
            SettingsManager.Instance.SaveSettings();
            Debug.Log("[SettingsUI] Settings successfully applied and saved.");
        }

        public void OnCloseClicked()
        {
            if (windowRoot != null)
            {
                windowRoot.SetActive(false);
            }
            Debug.Log("[SettingsUI] Settings window closed.");
        }

        public void OpenWindow()
        {
            if (windowRoot != null)
            {
                windowRoot.SetActive(true);
            }
            LoadWorkingSettingsFromManager();
            PopulateUIFromWorkingSettings();
        }
    }
}
