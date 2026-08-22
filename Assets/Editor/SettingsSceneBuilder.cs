using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using IndependenceGame.Settings;

namespace IndependenceGame.Settings.Editor
{
    public static class SettingsSceneBuilder
    {
        private const string BozorSpritesPath = "Assets/UI/BozorShop/Sprites/";
        private const string SettingsSpritesPath = "Assets/UI/Settings/Sprites/";
        private const string ScenePath = "Assets/Scenes/SettingsScene.unity";

        [MenuItem("Game/Build Settings Scene")]
        public static void BuildEverything()
        {
            SetupDirectories();
            ConfigureTextureImporters();
            AssetDatabase.Refresh();

            TMP_FontAsset fontAsset = TMP_Settings.defaultFontAsset;
            BuildScene(fontAsset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Settings]</color> Settings Scene built and saved successfully to " + ScenePath);
        }

        private static void SetupDirectories()
        {
            if (!Directory.Exists("Assets/Scenes")) Directory.CreateDirectory("Assets/Scenes");
            if (!Directory.Exists(SettingsSpritesPath)) Directory.CreateDirectory(SettingsSpritesPath);
        }

        public static void ConfigureTextureImporters()
        {
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { SettingsSpritesPath });
            foreach (string guid in textureGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.alphaIsTransparency = true;
                    importer.mipmapEnabled = false;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.spriteBorder = Vector4.zero;

                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                }
            }
        }

        private static void BuildScene(TMP_FontAsset fontAsset)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Camera
            GameObject camGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            Camera cam = camGO.GetComponent<Camera>();
            cam.tag = "MainCamera";
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.10f, 0.08f, 0.05f, 1f);
            camGO.transform.position = new Vector3(0, 0, -10);

            // 2. Event System
            GameObject eventSystemGO = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

            // 3. Canvas
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 10;

            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1024, 682);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            // 4. Blurred City Background
            GameObject bgGO = new GameObject("BackgroundImage", typeof(RectTransform), typeof(Image));
            bgGO.transform.SetParent(canvasGO.transform, false);
            RectTransform bgRt = bgGO.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bgGO.GetComponent<Image>();
            bgImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "bg_city_blurred.png");
            bgImg.color = new Color(0.92f, 0.92f, 0.92f, 1f);

            // 5. Settings Manager Host
            GameObject mgrGO = new GameObject("SettingsManagerHost", typeof(SettingsManager));
            mgrGO.transform.SetParent(canvasGO.transform, false);

            // 6. Settings Window Root
            GameObject windowRoot = new GameObject("SettingsWindow", typeof(RectTransform), typeof(SettingsUIController));
            windowRoot.transform.SetParent(canvasGO.transform, false);
            RectTransform windowRt = windowRoot.GetComponent<RectTransform>();
            windowRt.anchorMin = new Vector2(0.5f, 0.5f);
            windowRt.anchorMax = new Vector2(0.5f, 0.5f);
            windowRt.pivot = new Vector2(0.5f, 0.5f);
            windowRt.anchoredPosition = new Vector2(0, -12);
            windowRt.sizeDelta = new Vector2(1004, 586);

            SettingsUIController controller = windowRoot.GetComponent<SettingsUIController>();

            // 7. Wood Frame Background
            GameObject frameGO = new GameObject("FrameMainWood", typeof(RectTransform), typeof(Image));
            frameGO.transform.SetParent(windowRoot.transform, false);
            RectTransform frameRt = frameGO.GetComponent<RectTransform>();
            frameRt.anchorMin = Vector2.zero;
            frameRt.anchorMax = Vector2.one;
            frameRt.sizeDelta = Vector2.zero;
            Image frameImg = frameGO.GetComponent<Image>();
            frameImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "frame_main_wood.png");
            frameImg.type = Image.Type.Simple;

            // 8. Custom Ornate Header Badge
            GameObject headerBadge = new GameObject("HeaderBadge_SETTINGS", typeof(RectTransform), typeof(Image));
            headerBadge.transform.SetParent(windowRoot.transform, false);
            RectTransform badgeRt = headerBadge.GetComponent<RectTransform>();
            badgeRt.anchorMin = new Vector2(0.5f, 1f);
            badgeRt.anchorMax = new Vector2(0.5f, 1f);
            badgeRt.pivot = new Vector2(0.5f, 0.5f);
            badgeRt.anchoredPosition = new Vector2(0, 16);
            badgeRt.sizeDelta = new Vector2(440, 100);
            Image badgeImg = headerBadge.GetComponent<Image>();
            Sprite customHeader = AssetDatabase.LoadAssetAtPath<Sprite>(SettingsSpritesPath + "header_badge_sozlamalar.png");
            badgeImg.sprite = customHeader;
            badgeImg.type = Image.Type.Simple;
            badgeImg.preserveAspect = true;

            // Header Title Text
            GameObject titleGO = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleGO.transform.SetParent(headerBadge.transform, false);
            RectTransform titleRt = titleGO.GetComponent<RectTransform>();
            titleRt.anchorMin = Vector2.zero;
            titleRt.anchorMax = Vector2.one;
            titleRt.anchoredPosition = new Vector2(0, -10);
            titleRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
            titleTMP.font = fontAsset;
            titleTMP.fontSize = 19;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = new Color(0.98f, 0.90f, 0.72f, 1f);
            titleTMP.text = "SOZLAMALAR";

            // 9. Close Button
            GameObject closeBtnGO = new GameObject("CloseButton", typeof(RectTransform), typeof(Image), typeof(Button));
            closeBtnGO.transform.SetParent(windowRoot.transform, false);
            RectTransform closeRt = closeBtnGO.GetComponent<RectTransform>();
            closeRt.anchorMin = new Vector2(1f, 1f);
            closeRt.anchorMax = new Vector2(1f, 1f);
            closeRt.pivot = new Vector2(1f, 0.5f);
            closeRt.anchoredPosition = new Vector2(-12, 18);
            closeRt.sizeDelta = new Vector2(43, 43);
            Image closeImg = closeBtnGO.GetComponent<Image>();
            closeImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "btn_close_red.png");
            closeImg.type = Image.Type.Simple;
            Button closeBtn = closeBtnGO.GetComponent<Button>();

            // 10. Tab Bar
            GameObject tabBarGO = new GameObject("TabBar", typeof(RectTransform));
            tabBarGO.transform.SetParent(windowRoot.transform, false);
            RectTransform tabRt = tabBarGO.GetComponent<RectTransform>();
            tabRt.anchorMin = new Vector2(0.5f, 1f);
            tabRt.anchorMax = new Vector2(0.5f, 1f);
            tabRt.pivot = new Vector2(0.5f, 1f);
            tabRt.anchoredPosition = new Vector2(0, -32);
            tabRt.sizeDelta = new Vector2(960, 43);

            Sprite tabActiveBg = AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "tab_active_bg.png");
            Sprite tabInactiveBg = AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "tab_inactive_bg.png");
            Sprite iconAudio = AssetDatabase.LoadAssetAtPath<Sprite>(SettingsSpritesPath + "icon_tab_audio.png");
            Sprite iconGraphics = AssetDatabase.LoadAssetAtPath<Sprite>(SettingsSpritesPath + "icon_tab_graphics.png");
            Sprite iconGameplay = AssetDatabase.LoadAssetAtPath<Sprite>(SettingsSpritesPath + "icon_tab_gameplay.png");

            var tabList = new List<SettingsTabUI>();
            tabList.Add(CreateSolidTabWithIcon(tabBarGO, "Tab_Audio", "OVOZ (AUDIO)", SettingsTabCategory.Audio, -260f, 240, 43, 0, tabActiveBg, tabInactiveBg, iconAudio, fontAsset, true));
            tabList.Add(CreateSolidTabWithIcon(tabBarGO, "Tab_Graphics", "GRAFIKA (GRAPHICS)", SettingsTabCategory.Graphics, 0f, 240, 43, 0, tabActiveBg, tabInactiveBg, iconGraphics, fontAsset, false));
            tabList.Add(CreateSolidTabWithIcon(tabBarGO, "Tab_Gameplay", "O'YIN (GAMEPLAY)", SettingsTabCategory.Gameplay, 260f, 240, 43, 0, tabActiveBg, tabInactiveBg, iconGameplay, fontAsset, false));

            // 11. Content Area Container (Parchment background)
            GameObject contentContainerGO = new GameObject("ContentContainer", typeof(RectTransform), typeof(Image));
            contentContainerGO.transform.SetParent(windowRoot.transform, false);
            RectTransform contentRt = contentContainerGO.GetComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0.5f, 1f);
            contentRt.anchorMax = new Vector2(0.5f, 1f);
            contentRt.pivot = new Vector2(0.5f, 1f);
            contentRt.anchoredPosition = new Vector2(0, -78);
            contentRt.sizeDelta = new Vector2(940, 416);

            Image contentBg = contentContainerGO.GetComponent<Image>();
            Sprite parchmentSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SettingsSpritesPath + "parchment_clean_box.png");
            contentBg.sprite = parchmentSprite;
            contentBg.type = Image.Type.Simple;
            contentBg.color = Color.white;

            // Panels: Audio, Graphics, Gameplay
            GameObject audioPanel = CreateSectionPanel(contentContainerGO, "AudioPanel");
            GameObject graphicsPanel = CreateSectionPanel(contentContainerGO, "GraphicsPanel");
            GameObject gameplayPanel = CreateSectionPanel(contentContainerGO, "GameplayPanel");

            // --- Populate Audio Panel ---
            var masterSliderRow = CreateSliderRow(audioPanel, "MasterVolumeRow", "ASOSIY OVOZ (Master Volume)", fontAsset, out Slider masterSlider, out TextMeshProUGUI masterValText);
            var musicSliderRow = CreateSliderRow(audioPanel, "MusicVolumeRow", "MUSIQA OVOZI (Music Volume)", fontAsset, out Slider musicSlider, out TextMeshProUGUI musicValText);
            var sfxSliderRow = CreateSliderRow(audioPanel, "SFXVolumeRow", "EFFEKTLAR OVOZI (SFX Volume)", fontAsset, out Slider sfxSlider, out TextMeshProUGUI sfxValText);
            var muteToggleRow = CreateToggleRow(audioPanel, "MuteToggleRow", "BARCHA OVOZNI O'CHIRISH (Mute All)", fontAsset, out Toggle muteToggle, out TextMeshProUGUI muteStatusText);

            // --- Populate Graphics Panel ---
            var resDropdownRow = CreateDropdownRow(graphicsPanel, "ResolutionRow", "EKRAN RUXSATI (Resolution)", fontAsset, out TMP_Dropdown resDropdown);
            var fsDropdownRow = CreateDropdownRow(graphicsPanel, "FullscreenRow", "EKRAN REJIMI (Display Mode)", fontAsset, out TMP_Dropdown fsDropdown);
            var qualDropdownRow = CreateDropdownRow(graphicsPanel, "QualityRow", "GRAFIKA SIFATI (Quality Preset)", fontAsset, out TMP_Dropdown qualDropdown);
            var vsyncToggleRow = CreateToggleRow(graphicsPanel, "VSyncRow", "VERTIKAL SINXRONIZATSIYA (V-Sync)", fontAsset, out Toggle vsyncToggle, out TextMeshProUGUI vsyncStatusText);
            var fpsDropdownRow = CreateDropdownRow(graphicsPanel, "FPSRow", "KADRLAR CHASTOTASI (Target FPS)", fontAsset, out TMP_Dropdown fpsDropdown);

            // --- Populate Gameplay Panel ---
            var langDropdownRow = CreateDropdownRow(gameplayPanel, "LanguageRow", "TIL (Language)", fontAsset, out TMP_Dropdown langDropdown);
            var panSliderRow = CreateSliderRow(gameplayPanel, "PanSpeedRow", "KAMERA TEZLIGI (Camera Pan Speed)", fontAsset, out Slider panSlider, out TextMeshProUGUI panValText, 0.5f, 2.0f);
            var autoSaveDropdownRow = CreateDropdownRow(gameplayPanel, "AutoSaveRow", "AVTO-SAQLASH (Auto-Save)", fontAsset, out TMP_Dropdown autoSaveDropdown);

            graphicsPanel.SetActive(false);
            gameplayPanel.SetActive(false);
            audioPanel.SetActive(true);

            // 12. Bottom Action Buttons Bar
            GameObject bottomBarGO = new GameObject("BottomActionsBar", typeof(RectTransform));
            bottomBarGO.transform.SetParent(windowRoot.transform, false);
            RectTransform bottomRt = bottomBarGO.GetComponent<RectTransform>();
            bottomRt.anchorMin = new Vector2(0.5f, 0f);
            bottomRt.anchorMax = new Vector2(0.5f, 0f);
            bottomRt.pivot = new Vector2(0.5f, 0f);
            bottomRt.anchoredPosition = new Vector2(0, 16);
            bottomRt.sizeDelta = new Vector2(940, 50);

            // Reset Defaults Button
            Button resetBtn = CreateActionButton(bottomBarGO, "ResetDefaultsButton", "ASLIGA QAYTARISH", new Vector2(-180, 0), new Vector2(230, 44), fontAsset,
                AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "tab_inactive_bg.png"), new Color(0.35f, 0.15f, 0.10f, 1f));

            // Save & Apply Button
            Button saveBtn = CreateActionButton(bottomBarGO, "SaveApplyButton", "SAQLASH VA QABUL QILISH", new Vector2(180, 0), new Vector2(250, 44), fontAsset,
                AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "btn_buy_green_clean.png"), Color.white);

            // 13. Serialize Controller Properties
            var so = new SerializedObject(controller);
            so.FindProperty("windowRoot").objectReferenceValue = windowRoot;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.FindProperty("audioPanel").objectReferenceValue = audioPanel;
            so.FindProperty("graphicsPanel").objectReferenceValue = graphicsPanel;
            so.FindProperty("gameplayPanel").objectReferenceValue = gameplayPanel;

            SerializedProperty tabsProp = so.FindProperty("tabs");
            tabsProp.ClearArray();
            for (int i = 0; i < tabList.Count; i++)
            {
                tabsProp.InsertArrayElementAtIndex(i);
                tabsProp.GetArrayElementAtIndex(i).objectReferenceValue = tabList[i];
            }

            // Audio
            so.FindProperty("masterVolumeSlider").objectReferenceValue = masterSlider;
            so.FindProperty("masterVolumeText").objectReferenceValue = masterValText;
            so.FindProperty("musicVolumeSlider").objectReferenceValue = musicSlider;
            so.FindProperty("musicVolumeText").objectReferenceValue = musicValText;
            so.FindProperty("sfxVolumeSlider").objectReferenceValue = sfxSlider;
            so.FindProperty("sfxVolumeText").objectReferenceValue = sfxValText;
            so.FindProperty("muteToggle").objectReferenceValue = muteToggle;
            so.FindProperty("muteToggleStatusText").objectReferenceValue = muteStatusText;

            // Graphics
            so.FindProperty("resolutionDropdown").objectReferenceValue = resDropdown;
            so.FindProperty("fullscreenDropdown").objectReferenceValue = fsDropdown;
            so.FindProperty("qualityDropdown").objectReferenceValue = qualDropdown;
            so.FindProperty("vSyncToggle").objectReferenceValue = vsyncToggle;
            so.FindProperty("vSyncStatusText").objectReferenceValue = vsyncStatusText;
            so.FindProperty("targetFpsDropdown").objectReferenceValue = fpsDropdown;

            // Gameplay
            so.FindProperty("languageDropdown").objectReferenceValue = langDropdown;
            so.FindProperty("cameraPanSpeedSlider").objectReferenceValue = panSlider;
            so.FindProperty("cameraPanSpeedText").objectReferenceValue = panValText;
            so.FindProperty("autoSaveDropdown").objectReferenceValue = autoSaveDropdown;

            // Actions
            so.FindProperty("resetDefaultsButton").objectReferenceValue = resetBtn;
            so.FindProperty("saveApplyButton").objectReferenceValue = saveBtn;

            so.ApplyModifiedProperties();

            // Save Scene
            EditorSceneManager.SaveScene(scene, ScenePath);

            // Register in EditorBuildSettings
            RegisterSceneInBuildSettings(ScenePath);
        }

        private static GameObject CreateSectionPanel(GameObject parent, string name)
        {
            GameObject panelGO = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup));
            panelGO.transform.SetParent(parent.transform, false);
            RectTransform rt = panelGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(70, 24);
            rt.offsetMax = new Vector2(-70, -28);

            VerticalLayoutGroup vlg = panelGO.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 10;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.padding = new RectOffset(50, 50, 28, 16);

            return panelGO;
        }

        private static GameObject CreateSliderRow(GameObject parent, string name, string labelText, TMP_FontAsset font, out Slider slider, out TextMeshProUGUI valueText, float min = 0f, float max = 1f)
        {
            GameObject rowGO = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
            rowGO.transform.SetParent(parent.transform, false);
            LayoutElement le = rowGO.GetComponent<LayoutElement>();
            le.minHeight = 44;
            le.preferredHeight = 44;
            le.flexibleHeight = 0;
            le.flexibleWidth = 1;

            // Label
            GameObject lblGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            lblGO.transform.SetParent(rowGO.transform, false);
            RectTransform lblRt = lblGO.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0, 0);
            lblRt.anchorMax = new Vector2(0.48f, 1);
            lblRt.anchoredPosition = Vector2.zero;
            lblRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI lblTmp = lblGO.GetComponent<TextMeshProUGUI>();
            lblTmp.font = font;
            lblTmp.fontSize = 14;
            lblTmp.fontStyle = FontStyles.Bold;
            lblTmp.color = new Color(0.24f, 0.16f, 0.10f, 1f);
            lblTmp.alignment = TextAlignmentOptions.MidlineLeft;
            lblTmp.text = labelText;

            // Slider Container
            GameObject sliderGO = new GameObject("Slider", typeof(RectTransform), typeof(Slider));
            sliderGO.transform.SetParent(rowGO.transform, false);
            RectTransform sliderRt = sliderGO.GetComponent<RectTransform>();
            sliderRt.anchorMin = new Vector2(0.50f, 0.5f);
            sliderRt.anchorMax = new Vector2(0.86f, 0.5f);
            sliderRt.pivot = new Vector2(0.5f, 0.5f);
            sliderRt.anchoredPosition = Vector2.zero;
            sliderRt.sizeDelta = new Vector2(0, 22);

            slider = sliderGO.GetComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = (min + max) * 0.5f;

            // Background
            GameObject bgGO = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgGO.transform.SetParent(sliderGO.transform, false);
            RectTransform bgRt = bgGO.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0, 0.2f);
            bgRt.anchorMax = new Vector2(1, 0.8f);
            bgRt.anchoredPosition = Vector2.zero;
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bgGO.GetComponent<Image>();
            bgImg.color = new Color(0.25f, 0.18f, 0.12f, 0.85f);

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderGO.transform, false);
            RectTransform fillAreaRt = fillArea.GetComponent<RectTransform>();
            fillAreaRt.anchorMin = new Vector2(0, 0.2f);
            fillAreaRt.anchorMax = new Vector2(1, 0.8f);
            fillAreaRt.anchoredPosition = Vector2.zero;
            fillAreaRt.sizeDelta = new Vector2(-12, 0);

            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRt = fill.GetComponent<RectTransform>();
            fillRt.sizeDelta = Vector2.zero;
            Image fillImg = fill.GetComponent<Image>();
            fillImg.color = new Color(0.88f, 0.68f, 0.25f, 1f); // Warm Gold

            // Handle Area
            GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(sliderGO.transform, false);
            RectTransform handleAreaRt = handleArea.GetComponent<RectTransform>();
            handleAreaRt.anchorMin = Vector2.zero;
            handleAreaRt.anchorMax = Vector2.one;
            handleAreaRt.sizeDelta = new Vector2(-12, 0);

            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(handleArea.transform, false);
            RectTransform handleRt = handle.GetComponent<RectTransform>();
            handleRt.sizeDelta = new Vector2(20, 24);
            Image handleImg = handle.GetComponent<Image>();
            handleImg.color = new Color(0.96f, 0.88f, 0.70f, 1f);

            slider.fillRect = fillRt;
            slider.handleRect = handleRt;
            slider.targetGraphic = handleImg;
            slider.direction = Slider.Direction.LeftToRight;

            // Value Text Label
            GameObject valGO = new GameObject("ValueText", typeof(RectTransform), typeof(TextMeshProUGUI));
            valGO.transform.SetParent(rowGO.transform, false);
            RectTransform valRt = valGO.GetComponent<RectTransform>();
            valRt.anchorMin = new Vector2(0.88f, 0);
            valRt.anchorMax = new Vector2(1f, 1);
            valRt.anchoredPosition = Vector2.zero;
            valRt.sizeDelta = Vector2.zero;
            valueText = valGO.GetComponent<TextMeshProUGUI>();
            valueText.font = font;
            valueText.fontSize = 14;
            valueText.fontStyle = FontStyles.Bold;
            valueText.color = new Color(0.24f, 0.16f, 0.10f, 1f);
            valueText.alignment = TextAlignmentOptions.MidlineRight;
            valueText.text = "100%";

            return rowGO;
        }

        private static GameObject CreateToggleRow(GameObject parent, string name, string labelText, TMP_FontAsset font, out Toggle toggle, out TextMeshProUGUI statusText)
        {
            GameObject rowGO = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
            rowGO.transform.SetParent(parent.transform, false);
            LayoutElement le = rowGO.GetComponent<LayoutElement>();
            le.minHeight = 44;
            le.preferredHeight = 44;
            le.flexibleHeight = 0;
            le.flexibleWidth = 1;

            // Label
            GameObject lblGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            lblGO.transform.SetParent(rowGO.transform, false);
            RectTransform lblRt = lblGO.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0, 0);
            lblRt.anchorMax = new Vector2(0.48f, 1);
            lblRt.anchoredPosition = Vector2.zero;
            lblRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI lblTmp = lblGO.GetComponent<TextMeshProUGUI>();
            lblTmp.font = font;
            lblTmp.fontSize = 14;
            lblTmp.fontStyle = FontStyles.Bold;
            lblTmp.color = new Color(0.24f, 0.16f, 0.10f, 1f);
            lblTmp.alignment = TextAlignmentOptions.MidlineLeft;
            lblTmp.text = labelText;

            // Toggle Switch
            GameObject toggleGO = new GameObject("Toggle", typeof(RectTransform), typeof(Toggle));
            toggleGO.transform.SetParent(rowGO.transform, false);
            RectTransform toggleRt = toggleGO.GetComponent<RectTransform>();
            toggleRt.anchorMin = new Vector2(0.50f, 0.5f);
            toggleRt.anchorMax = new Vector2(0.50f, 0.5f);
            toggleRt.pivot = new Vector2(0, 0.5f);
            toggleRt.anchoredPosition = Vector2.zero;
            toggleRt.sizeDelta = new Vector2(46, 26);

            toggle = toggleGO.GetComponent<Toggle>();

            // Toggle Background Box
            GameObject bgGO = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgGO.transform.SetParent(toggleGO.transform, false);
            RectTransform bgRt = bgGO.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bgGO.GetComponent<Image>();
            bgImg.color = new Color(0.28f, 0.20f, 0.14f, 0.95f);

            // Toggle Checkmark
            GameObject checkGO = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
            checkGO.transform.SetParent(bgGO.transform, false);
            RectTransform checkRt = checkGO.GetComponent<RectTransform>();
            checkRt.anchorMin = new Vector2(0.12f, 0.12f);
            checkRt.anchorMax = new Vector2(0.88f, 0.88f);
            checkRt.sizeDelta = Vector2.zero;
            Image checkImg = checkGO.GetComponent<Image>();
            checkImg.color = new Color(0.35f, 0.85f, 0.35f, 1f);

            toggle.graphic = checkImg;
            toggle.targetGraphic = bgImg;
            toggle.isOn = false;

            // Status Text
            GameObject statGO = new GameObject("StatusText", typeof(RectTransform), typeof(TextMeshProUGUI));
            statGO.transform.SetParent(rowGO.transform, false);
            RectTransform statRt = statGO.GetComponent<RectTransform>();
            statRt.anchorMin = new Vector2(0.58f, 0);
            statRt.anchorMax = new Vector2(1f, 1);
            statRt.anchoredPosition = Vector2.zero;
            statRt.sizeDelta = Vector2.zero;
            statusText = statGO.GetComponent<TextMeshProUGUI>();
            statusText.font = font;
            statusText.fontSize = 13;
            statusText.fontStyle = FontStyles.Bold;
            statusText.color = new Color(0.35f, 0.22f, 0.14f, 1f);
            statusText.alignment = TextAlignmentOptions.MidlineLeft;
            statusText.text = "O'CHIRILGAN (OFF)";

            return rowGO;
        }

        private static GameObject CreateDropdownRow(GameObject parent, string name, string labelText, TMP_FontAsset font, out TMP_Dropdown dropdown)
        {
            GameObject rowGO = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
            rowGO.transform.SetParent(parent.transform, false);
            LayoutElement le = rowGO.GetComponent<LayoutElement>();
            le.minHeight = 44;
            le.preferredHeight = 44;
            le.flexibleHeight = 0;
            le.flexibleWidth = 1;

            // Label
            GameObject lblGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            lblGO.transform.SetParent(rowGO.transform, false);
            RectTransform lblRt = lblGO.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0, 0);
            lblRt.anchorMax = new Vector2(0.48f, 1);
            lblRt.anchoredPosition = Vector2.zero;
            lblRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI lblTmp = lblGO.GetComponent<TextMeshProUGUI>();
            lblTmp.font = font;
            lblTmp.fontSize = 14;
            lblTmp.fontStyle = FontStyles.Bold;
            lblTmp.color = new Color(0.24f, 0.16f, 0.10f, 1f);
            lblTmp.alignment = TextAlignmentOptions.MidlineLeft;
            lblTmp.text = labelText;

            // Dropdown Root
            GameObject ddGO = new GameObject("Dropdown", typeof(RectTransform), typeof(Image), typeof(Outline), typeof(TMP_Dropdown));
            ddGO.transform.SetParent(rowGO.transform, false);
            RectTransform ddRt = ddGO.GetComponent<RectTransform>();
            ddRt.anchorMin = new Vector2(0.50f, 0.5f);
            ddRt.anchorMax = new Vector2(1f, 0.5f);
            ddRt.pivot = new Vector2(0.5f, 0.5f);
            ddRt.anchoredPosition = Vector2.zero;
            ddRt.sizeDelta = new Vector2(0, 36);

            Image ddImg = ddGO.GetComponent<Image>();
            ddImg.color = new Color(0.96f, 0.93f, 0.86f, 1f);

            Outline ddOutline = ddGO.GetComponent<Outline>();
            ddOutline.effectColor = new Color(0.55f, 0.40f, 0.25f, 0.9f);
            ddOutline.effectDistance = new Vector2(1, -1);

            dropdown = ddGO.GetComponent<TMP_Dropdown>();
            dropdown.targetGraphic = ddImg;

            // Caption Text
            GameObject captionGO = new GameObject("CaptionText", typeof(RectTransform), typeof(TextMeshProUGUI));
            captionGO.transform.SetParent(ddGO.transform, false);
            RectTransform capRt = captionGO.GetComponent<RectTransform>();
            capRt.anchorMin = new Vector2(0, 0);
            capRt.anchorMax = new Vector2(1, 1);
            capRt.anchoredPosition = new Vector2(12, 0);
            capRt.sizeDelta = new Vector2(-36, 0);
            TextMeshProUGUI capTmp = captionGO.GetComponent<TextMeshProUGUI>();
            capTmp.font = font;
            capTmp.fontSize = 13;
            capTmp.fontStyle = FontStyles.Bold;
            capTmp.color = new Color(0.20f, 0.14f, 0.08f, 1f);
            capTmp.alignment = TextAlignmentOptions.MidlineLeft;
            capTmp.text = "Option";

            // Arrow
            GameObject arrowGO = new GameObject("Arrow", typeof(RectTransform), typeof(TextMeshProUGUI));
            arrowGO.transform.SetParent(ddGO.transform, false);
            RectTransform arrowRt = arrowGO.GetComponent<RectTransform>();
            arrowRt.anchorMin = new Vector2(1, 0.5f);
            arrowRt.anchorMax = new Vector2(1, 0.5f);
            arrowRt.pivot = new Vector2(1, 0.5f);
            arrowRt.anchoredPosition = new Vector2(-8, 0);
            arrowRt.sizeDelta = new Vector2(20, 20);
            TextMeshProUGUI arrowTmp = arrowGO.GetComponent<TextMeshProUGUI>();
            arrowTmp.font = font;
            arrowTmp.fontSize = 13;
            arrowTmp.alignment = TextAlignmentOptions.Center;
            arrowTmp.color = new Color(0.30f, 0.20f, 0.14f, 1f);
            arrowTmp.text = "▼";

            // Template
            GameObject templateGO = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
            templateGO.transform.SetParent(ddGO.transform, false);
            RectTransform tmplRt = templateGO.GetComponent<RectTransform>();
            tmplRt.anchorMin = new Vector2(0, 0);
            tmplRt.anchorMax = new Vector2(1, 0);
            tmplRt.pivot = new Vector2(0.5f, 1);
            tmplRt.anchoredPosition = new Vector2(0, 2);
            tmplRt.sizeDelta = new Vector2(0, 160);

            Image tmplImg = templateGO.GetComponent<Image>();
            tmplImg.color = new Color(0.95f, 0.92f, 0.85f, 1f);

            ScrollRect scrollRect = templateGO.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            // Viewport
            GameObject viewGO = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            viewGO.transform.SetParent(templateGO.transform, false);
            RectTransform viewRt = viewGO.GetComponent<RectTransform>();
            viewRt.anchorMin = Vector2.zero;
            viewRt.anchorMax = Vector2.one;
            viewRt.sizeDelta = Vector2.zero;
            Mask mask = viewGO.GetComponent<Mask>();
            mask.showMaskGraphic = false;

            // Content
            GameObject cntGO = new GameObject("Content", typeof(RectTransform));
            cntGO.transform.SetParent(viewGO.transform, false);
            RectTransform cntRt = cntGO.GetComponent<RectTransform>();
            cntRt.anchorMin = new Vector2(0, 1);
            cntRt.anchorMax = new Vector2(1, 1);
            cntRt.pivot = new Vector2(0.5f, 1);
            cntRt.anchoredPosition = Vector2.zero;
            cntRt.sizeDelta = new Vector2(0, 32);

            // Item
            GameObject itemGO = new GameObject("Item", typeof(RectTransform), typeof(Toggle));
            itemGO.transform.SetParent(cntGO.transform, false);
            RectTransform itemRt = itemGO.GetComponent<RectTransform>();
            itemRt.anchorMin = new Vector2(0, 0.5f);
            itemRt.anchorMax = new Vector2(1, 0.5f);
            itemRt.sizeDelta = new Vector2(0, 30);

            Toggle itemToggle = itemGO.GetComponent<Toggle>();

            // Item Background
            GameObject itemBgGO = new GameObject("Item Background", typeof(RectTransform), typeof(Image));
            itemBgGO.transform.SetParent(itemGO.transform, false);
            RectTransform itemBgRt = itemBgGO.GetComponent<RectTransform>();
            itemBgRt.anchorMin = Vector2.zero;
            itemBgRt.anchorMax = Vector2.one;
            itemBgRt.sizeDelta = Vector2.zero;
            Image itemBgImg = itemBgGO.GetComponent<Image>();
            itemBgImg.color = new Color(0.88f, 0.84f, 0.76f, 0.8f);

            // Item Label
            GameObject itemLblGO = new GameObject("Item Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            itemLblGO.transform.SetParent(itemGO.transform, false);
            RectTransform itemLblRt = itemLblGO.GetComponent<RectTransform>();
            itemLblRt.anchorMin = Vector2.zero;
            itemLblRt.anchorMax = Vector2.one;
            itemLblRt.anchoredPosition = new Vector2(10, 0);
            itemLblRt.sizeDelta = new Vector2(-20, 0);
            TextMeshProUGUI itemLblTmp = itemLblGO.GetComponent<TextMeshProUGUI>();
            itemLblTmp.font = font;
            itemLblTmp.fontSize = 13;
            itemLblTmp.color = new Color(0.20f, 0.14f, 0.08f, 1f);
            itemLblTmp.alignment = TextAlignmentOptions.MidlineLeft;

            // Wire template
            scrollRect.viewport = viewRt;
            scrollRect.content = cntRt;
            itemToggle.targetGraphic = itemBgImg;

            dropdown.template = tmplRt;
            dropdown.captionText = capTmp;
            dropdown.itemText = itemLblTmp;

            templateGO.SetActive(false);

            return rowGO;
        }

        private static SettingsTabUI CreateSolidTabWithIcon(GameObject parent, string name, string label, SettingsTabCategory cat, float posX, float width, float height, float posY, Sprite activeBg, Sprite inactiveBg, Sprite icon, TMP_FontAsset font, bool isInitialActive)
        {
            GameObject tabGO = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(SettingsTabUI));
            tabGO.transform.SetParent(parent.transform, false);

            RectTransform rt = tabGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(posX, posY);
            rt.sizeDelta = new Vector2(width, height);

            Image img = tabGO.GetComponent<Image>();
            img.sprite = isInitialActive ? activeBg : inactiveBg;
            img.type = Image.Type.Sliced;

            // Tab Icon
            if (icon != null)
            {
                GameObject iconGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
                iconGO.transform.SetParent(tabGO.transform, false);
                RectTransform iconRt = iconGO.GetComponent<RectTransform>();
                iconRt.anchorMin = new Vector2(0, 0.5f);
                iconRt.anchorMax = new Vector2(0, 0.5f);
                iconRt.pivot = new Vector2(0, 0.5f);
                iconRt.anchoredPosition = new Vector2(12, 0);
                iconRt.sizeDelta = new Vector2(24, 24);
                Image iconImg = iconGO.GetComponent<Image>();
                iconImg.sprite = icon;
                iconImg.preserveAspect = true;
            }

            GameObject textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(tabGO.transform, false);
            RectTransform textRt = textGO.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.anchoredPosition = new Vector2(icon != null ? 14 : 0, isInitialActive ? 2 : 0);
            textRt.sizeDelta = Vector2.zero;

            TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.fontSize = 12;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = isInitialActive ? new Color(0.2f, 0.15f, 0.1f, 1f) : new Color(0.25f, 0.18f, 0.12f, 0.9f);
            tmp.text = label;

            SettingsTabUI tabUI = tabGO.GetComponent<SettingsTabUI>();
            var so = new SerializedObject(tabUI);
            so.FindProperty("category").enumValueIndex = (int)cat;
            so.FindProperty("tabBackground").objectReferenceValue = img;
            so.FindProperty("tabLabel").objectReferenceValue = tmp;
            so.FindProperty("button").objectReferenceValue = tabGO.GetComponent<Button>();
            so.FindProperty("activeSprite").objectReferenceValue = activeBg;
            so.FindProperty("inactiveSprite").objectReferenceValue = inactiveBg;
            so.FindProperty("activeTextColor").colorValue = new Color(0.2f, 0.15f, 0.1f, 1f);
            so.FindProperty("inactiveTextColor").colorValue = new Color(0.25f, 0.18f, 0.12f, 0.9f);
            so.ApplyModifiedProperties();

            return tabUI;
        }

        private static Button CreateActionButton(GameObject parent, string name, string label, Vector2 anchoredPos, Vector2 size, TMP_FontAsset font, Sprite bgSprite, Color textColor)
        {
            GameObject btnGO = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btnGO.transform.SetParent(parent.transform, false);
            RectTransform rt = btnGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            Image img = btnGO.GetComponent<Image>();
            img.sprite = bgSprite;
            img.type = Image.Type.Simple;

            GameObject textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(btnGO.transform, false);
            RectTransform textRt = textGO.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.anchoredPosition = Vector2.zero;
            textRt.sizeDelta = Vector2.zero;

            TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.fontSize = 13;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = textColor;
            tmp.text = label;

            return btnGO.GetComponent<Button>();
        }

        private static void RegisterSceneInBuildSettings(string scenePath)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            foreach (var s in scenes)
            {
                if (s.path == scenePath) return;
            }
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
