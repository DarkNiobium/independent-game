# Settings Menu System Design Specification

## Overview
The Settings Menu provides players with a comprehensive, thematic configuration interface matching the game's Oriental / Wood & Parchment visual aesthetic (as established in the Bozor Shop system). It supports real-time adjustments and persistence across game sessions for Audio, Graphics, and Gameplay settings.

## Goals & Key Features
- **Visual Cohesion:** Carved wood framing, decorative header badge, parchment panels, and Cinzel typography.
- **Persistent State:** Saves and loads configuration from `PlayerPrefs` using a centralized `SettingsManager`.
- **Live Engine Integration:** Directly updates Unity's `AudioListener`, `QualitySettings`, `Screen` resolution/display mode, and target framerate.
- **Categorized Tabs:**
  1. **Audio (Ovoz):** Master Volume, Music Volume, SFX Volume, Mute All toggle.
  2. **Graphics (Grafika):** Resolution selector (dynamic monitor resolution list), Display Mode (Fullscreen / Windowed / Borderless), Quality Presets (Low to Ultra), VSync toggle, Target Framerate (30 / 60 / 120 / Unlimited).
  3. **Gameplay (O'yin):** Language selection (O'zbekcha / English / Русский), Camera Pan/Scroll Sensitivity multiplier, Auto-save interval selector.
- **Action Controls:** Reset to Defaults button, Save & Apply button, and Back/Close button.
- **Automated Scene Builder:** An Editor utility (`SettingsSceneBuilder.cs`) that constructs `Assets/Scenes/SettingsScene.unity` with all components, hierarchies, styles, and serialized references wired automatically.

## Architecture

### 1. Data Layer (`SettingsData.cs`)
Serializable data model storing player preferences:
- `masterVolume` (float, range [0, 1], default 0.8)
- `musicVolume` (float, range [0, 1], default 0.7)
- `sfxVolume` (float, range [0, 1], default 0.8)
- `isMuted` (bool, default false)
- `resolutionWidth` (int, default native width)
- `resolutionHeight` (int, default native height)
- `resolutionRefreshRate` (int, default native refresh rate)
- `fullscreenMode` (FullScreenMode, default FullScreenWindow)
- `qualityLevel` (int, default QualitySettings.GetQualityLevel())
- `vSync` (bool, default true)
- `targetFps` (int, default 60)
- `language` (string: "uz", "en", "ru", default "uz")
- `cameraPanSpeed` (float, range [0.5, 2.0], default 1.0)
- `autoSaveIntervalMinutes` (int: 0, 5, 10, 15, default 5)

### 2. Service Layer (`SettingsManager.cs`)
- Persistent singleton (`DontDestroyOnLoad`).
- Methods:
  - `LoadSettings()`: Reads JSON payload from `PlayerPrefs` key `"GameSettingsData"`, falling back to device-appropriate defaults.
  - `SaveSettings()`: Writes JSON payload to `PlayerPrefs` and calls `PlayerPrefs.Save()`.
  - `ApplySettings(SettingsData data)`: Executes engine API calls:
    - Audio: `AudioListener.volume = data.isMuted ? 0f : data.masterVolume;`
    - Graphics: `Screen.SetResolution(data.resolutionWidth, data.resolutionHeight, data.fullscreenMode, data.resolutionRefreshRate);`
    - Quality: `QualitySettings.SetQualityLevel(data.qualityLevel, true); QualitySettings.vSyncCount = data.vSync ? 1 : 0; Application.targetFrameRate = data.targetFps;`
    - Events: Fires `OnSettingsChanged` action for listeners (e.g. CameraController, AudioMixer, Localization).
  - `ResetToDefaults()`: Resets `SettingsData` to default values and invokes `ApplySettings`.

### 3. Presentation Layer (`SettingsUIController.cs`)
Attached to the settings window root in `SettingsScene.unity`:
- Tab management with visual active/inactive tab state switching.
- Two-way binding between UI elements and `SettingsData`:
  - Sliders update label percentages in real time (`"80%"`, `"1.2x"`).
  - Dropdowns are dynamically populated from `Screen.resolutions` (filtered for unique aspect/sizes) and `QualitySettings.names`.
  - Action buttons: Reset Defaults, Save & Apply, Close/Back.

### 4. Automated Builder (`Editor/SettingsSceneBuilder.cs`)
- Provides `[MenuItem("Game/Build Settings Scene")]`.
- Creates or refreshes `Assets/Scenes/SettingsScene.unity`.
- Configures Canvas with `1024x682` reference resolution, Main Camera, EventSystem, wood frames, tab buttons, sliders, toggles, dropdowns, and wires all script references.
- Adds scene to `EditorBuildSettings`.

## Verification Strategy
- **Scene Creation:** Run builder from Editor menu and verify `SettingsScene.unity` generates without warnings/errors.
- **UI Responsiveness:** In Play Mode, test slider interactions, dropdown changes, toggle clicks, and tab switching.
- **Persistence Verification:** Change values, save, restart play mode, and verify that custom settings persist.
- **Reset Functionality:** Click "Asliga qaytarish" (Reset) and ensure all sliders, toggles, and dropdowns snap back to initial defaults.
- **Engine State:** Confirm `AudioListener.volume`, `QualitySettings.GetQualityLevel()`, and `Screen` dimensions update accordingly.
