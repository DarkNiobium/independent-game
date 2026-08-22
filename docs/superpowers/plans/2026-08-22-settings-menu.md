# Settings Menu Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Create a complete, functional, and thematically styled Settings Menu scene (`Assets/Scenes/SettingsScene.unity`) with persistent data management, live engine application for Audio, Graphics, and Gameplay settings, and an automated editor builder.

**Architecture:** A modular MVC-style architecture with `SettingsData` model, persistent `SettingsManager` singleton service (with `PlayerPrefs` serialization and engine API hooks), `SettingsUIController` presentation layer handling tabs and interactive controls, and `SettingsSceneBuilder` editor pipeline tool.

**Tech Stack:** Unity 6000.5.9f1, C#, TextMeshPro, Unity UI (UGUI), Universal Render Pipeline (URP).

## Global Constraints
- Target resolution: `1024x682` reference Canvas scaler (match width/height 0.5).
- UI aesthetic: Oriental / Wood & Parchment matching Bozor Shop UI.
- All code formatted in namespace `IndependenceGame.Settings`.
- Data persistence key: `"GameSettingsData"`.

---

### Task 1: Data Model (`SettingsData.cs`) and Manager (`SettingsManager.cs`)

**Files:**
- Create: `Assets/Scripts/Settings/SettingsData.cs`
- Create: `Assets/Scripts/Settings/SettingsManager.cs`

**Interfaces:**
- Produces:
  - `class SettingsData`: Serializable options state.
  - `class SettingsManager : MonoBehaviour`:
    - `public static SettingsManager Instance { get; }`
    - `public SettingsData CurrentSettings { get; }`
    - `public void SaveSettings()`
    - `public void ApplySettings(SettingsData data)`
    - `public void ResetToDefaults()`
    - `public event System.Action<SettingsData> OnSettingsChanged`

- [ ] **Step 1: Create `SettingsData.cs`**
Create `Assets/Scripts/Settings/SettingsData.cs` containing serializable fields for audio volumes, mute state, resolution, fullscreen mode, quality level, vsync, target fps, language, pan speed, and autosave.

- [ ] **Step 2: Create `SettingsManager.cs`**
Create `Assets/Scripts/Settings/SettingsManager.cs` with singleton lifecycle (`DontDestroyOnLoad`), `PlayerPrefs` JSON serialization/deserialization, live calls to `AudioListener`, `Screen.SetResolution`, `QualitySettings.SetQualityLevel`, `QualitySettings.vSyncCount`, `Application.targetFrameRate`, and `OnSettingsChanged` notification event.

- [ ] **Step 3: Compile & Verify in Unity**
Check for compilation errors using unity mcp or editor compilation check.

- [ ] **Step 4: Commit**
```bash
git add Assets/Scripts/Settings/SettingsData.cs Assets/Scripts/Settings/SettingsManager.cs
git commit -m "feat(settings): create SettingsData model and persistent SettingsManager"
```

---

### Task 2: UI Presentation Layer (`SettingsTabUI.cs` & `SettingsUIController.cs`)

**Files:**
- Create: `Assets/Scripts/Settings/SettingsTabUI.cs`
- Create: `Assets/Scripts/Settings/SettingsUIController.cs`

**Interfaces:**
- Consumes: `SettingsManager`, `SettingsData`
- Produces:
  - `class SettingsTabUI : MonoBehaviour`: Tab toggle visuals, label colors, and click handling.
  - `class SettingsUIController : MonoBehaviour`: Tab switching, dynamic resolution/quality dropdown population, slider percentage formatting, toggle bindings, reset to defaults, and save/close actions.

- [ ] **Step 1: Create `SettingsTabUI.cs`**
Create `Assets/Scripts/Settings/SettingsTabUI.cs` with active/inactive sprite swap, font color switching, and tab selection event callback.

- [ ] **Step 2: Create `SettingsUIController.cs`**
Create `Assets/Scripts/Settings/SettingsUIController.cs` wiring all sliders, dropdowns, and toggles with two-way binding to `SettingsManager.Instance`. Dynamically populate screen resolutions from `Screen.resolutions` and quality levels from `QualitySettings.names`.

- [ ] **Step 3: Compile & Verify in Unity**
Check compilation status to ensure no missing references or syntax errors.

- [ ] **Step 4: Commit**
```bash
git add Assets/Scripts/Settings/SettingsTabUI.cs Assets/Scripts/Settings/SettingsUIController.cs
git commit -m "feat(settings): implement SettingsTabUI and SettingsUIController presentation layer"
```

---

### Task 3: Editor Scene Builder Tool (`SettingsSceneBuilder.cs`)

**Files:**
- Create: `Assets/Editor/SettingsSceneBuilder.cs`

**Interfaces:**
- Consumes: `SettingsUIController`, `SettingsTabUI`, `SettingsManager`, UI Sprites and Fonts.
- Produces:
  - `[MenuItem("Game/Build Settings Scene")]`
  - Generates `Assets/Scenes/SettingsScene.unity` with complete hierarchy, background, wood frames, tabs, content panels, controls, and script wiring.

- [ ] **Step 1: Create `SettingsSceneBuilder.cs`**
Write the procedural builder that constructs:
- Camera & EventSystem
- Scaled Canvas (`1024x682`)
- Background backdrop (`bg_city_blurred.png`)
- Main wood frame (`frame_main_wood.png`)
- Header badge (`header_badge_bozor.png`) with title "SOZLAMALAR"
- Close button (`btn_close_red.png`)
- 3 Category Tabs: OVOZ (Audio), GRAFIKA (Graphics), O'YIN (Gameplay)
- 3 Tab Content Panels with styled Sliders (wooden background, gold fill, knob), styled TMP_Dropdowns, and switch Toggles.
- Bottom Action Buttons: "Asliga qaytarish" (Reset) and "Saqlash" (Apply & Save).
- Wires all serialized fields on `SettingsUIController`.
- Saves scene to `Assets/Scenes/SettingsScene.unity` and registers in `EditorBuildSettings`.

- [ ] **Step 2: Compile & Run Builder**
Execute menu item `Game/Build Settings Scene` to generate the scene.

- [ ] **Step 3: Commit**
```bash
git add Assets/Editor/SettingsSceneBuilder.cs
git commit -m "feat(settings): add automated SettingsSceneBuilder editor tool"
```

---

### Task 4: Scene Verification and Play Mode Testing

**Files:**
- Modify: `Assets/Scenes/SettingsScene.unity`

- [ ] **Step 1: Open Scene in Unity**
Open `Assets/Scenes/SettingsScene.unity` and inspect hierarchy.

- [ ] **Step 2: Enter Play Mode and Test UI**
Test tab switching, slider dragging (volume % updates), toggle toggling, resolution dropdown, reset button, and save button.

- [ ] **Step 3: Verify Persistence**
Change master volume to 40%, change quality, click Save, exit play mode and enter play mode again to verify settings remain at 40%.

- [ ] **Step 4: Commit Scene**
```bash
git add Assets/Scenes/SettingsScene.unity Assets/Scenes/SettingsScene.unity.meta
git commit -m "feat(settings): generate and verify complete Settings Menu scene"
```
