using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using IndependenceGame.Settings;
using IndependenceGame.MainMenu;

namespace IndependenceGame.MainMenu.Editor
{
    public static class MainMenuSceneBuilder
    {
        private const string MainMenuSpritesPath = "Assets/UI/MainMenu/Sprites/";
        private const string BozorSpritesPath = "Assets/UI/BozorShop/Sprites/";
        private const string SettingsSpritesPath = "Assets/UI/Settings/Sprites/";
        private const string ScenePath = "Assets/Scenes/MainMenuScene.unity";

        [MenuItem("Game/Build Main Menu Scene")]
        public static void BuildEverything()
        {
            SetupDirectories();
            ConfigureTextureImporters();
            AssetDatabase.Refresh();

            TMP_FontAsset fontAsset = TMP_Settings.defaultFontAsset;
            BuildScene(fontAsset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[MainMenu]</color> Left-Aligned Minimalist Main Menu Scene built successfully to " + ScenePath);
        }

        private static void SetupDirectories()
        {
            if (!Directory.Exists("Assets/Scenes")) Directory.CreateDirectory("Assets/Scenes");
            if (!Directory.Exists(MainMenuSpritesPath)) Directory.CreateDirectory(MainMenuSpritesPath);
        }

        public static void ConfigureTextureImporters()
        {
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { MainMenuSpritesPath, SettingsSpritesPath });
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
            cam.backgroundColor = new Color(0.06f, 0.04f, 0.03f, 1f);
            camGO.transform.position = new Vector3(0, 0, -10);

            // 2. Event System
            GameObject eventSystemGO = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

            // 3. Canvas
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(MenuParallaxEffect));
            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 10;

            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1024, 682);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            MenuParallaxEffect parallax = canvasGO.GetComponent<MenuParallaxEffect>();

            // 4. Cinematic Background Image
            GameObject bgGO = new GameObject("CinematicBackground", typeof(RectTransform), typeof(Image));
            bgGO.transform.SetParent(canvasGO.transform, false);
            RectTransform bgRt = bgGO.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(-0.05f, -0.05f);
            bgRt.anchorMax = new Vector2(1.05f, 1.05f);
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bgGO.GetComponent<Image>();
            Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MainMenuSpritesPath + "bg_main_menu_cinematic.png");
            if (bgSprite == null)
                bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "bg_city_blurred.png");
            bgImg.sprite = bgSprite;
            bgImg.type = Image.Type.Simple;
            bgImg.preserveAspect = false;

            // Vignette overlay
            GameObject vigGO = new GameObject("VignetteOverlay", typeof(RectTransform), typeof(Image));
            vigGO.transform.SetParent(canvasGO.transform, false);
            RectTransform vigRt = vigGO.GetComponent<RectTransform>();
            vigRt.anchorMin = Vector2.zero;
            vigRt.anchorMax = Vector2.one;
            vigRt.sizeDelta = Vector2.zero;
            Image vigImg = vigGO.GetComponent<Image>();
            vigImg.color = new Color(0.04f, 0.02f, 0.01f, 0.25f);

            // 5. Atmospheric Gold Particles
            CreateAtmosphericParticles(canvasGO);

            // 6. Settings Manager Host
            GameObject mgrGO = new GameObject("SettingsManagerHost", typeof(SettingsManager));
            mgrGO.transform.SetParent(canvasGO.transform, false);

            // 7. Main Menu View Root
            GameObject menuRootGO = new GameObject("MainMenuRoot", typeof(RectTransform), typeof(MainMenuController));
            menuRootGO.transform.SetParent(canvasGO.transform, false);
            RectTransform menuRt = menuRootGO.GetComponent<RectTransform>();
            menuRt.anchorMin = Vector2.zero;
            menuRt.anchorMax = Vector2.one;
            menuRt.sizeDelta = Vector2.zero;

            MainMenuController menuController = menuRootGO.GetComponent<MainMenuController>();

            // 8. Title Header Block (Left-Aligned)
            GameObject titleBlock = new GameObject("TitleBlock", typeof(RectTransform));
            titleBlock.transform.SetParent(menuRootGO.transform, false);
            RectTransform titleBlockRt = titleBlock.GetComponent<RectTransform>();
            titleBlockRt.anchorMin = new Vector2(0, 1f);
            titleBlockRt.anchorMax = new Vector2(0, 1f);
            titleBlockRt.pivot = new Vector2(0, 1f);
            titleBlockRt.anchoredPosition = new Vector2(70, -60);
            titleBlockRt.sizeDelta = new Vector2(450, 120);

            // Main Title: MUSTAQILLIK YO'LI
            GameObject mainTitleGO = new GameObject("MainTitle", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(Outline));
            mainTitleGO.transform.SetParent(titleBlock.transform, false);
            RectTransform mainTitleRt = mainTitleGO.GetComponent<RectTransform>();
            mainTitleRt.anchorMin = new Vector2(0, 0.5f);
            mainTitleRt.anchorMax = new Vector2(1, 1f);
            mainTitleRt.anchoredPosition = Vector2.zero;
            mainTitleRt.sizeDelta = Vector2.zero;

            TextMeshProUGUI mainTitleTMP = mainTitleGO.GetComponent<TextMeshProUGUI>();
            mainTitleTMP.font = fontAsset;
            mainTitleTMP.fontSize = 32;
            mainTitleTMP.fontStyle = FontStyles.Bold;
            mainTitleTMP.characterSpacing = 6;
            mainTitleTMP.enableWordWrapping = false;
            mainTitleTMP.alignment = TextAlignmentOptions.MidlineLeft;
            mainTitleTMP.color = new Color(0.96f, 0.88f, 0.72f, 1f);
            mainTitleTMP.text = "MUSTAQILLIK YO'LI";

            Outline titleOutline = mainTitleGO.GetComponent<Outline>();
            titleOutline.effectColor = new Color(0.08f, 0.05f, 0.02f, 0.8f);
            titleOutline.effectDistance = new Vector2(1, -1);

            // Divider Line
            GameObject lineGO = new GameObject("DividerLine", typeof(RectTransform), typeof(Image));
            lineGO.transform.SetParent(titleBlock.transform, false);
            RectTransform lineRt = lineGO.GetComponent<RectTransform>();
            lineRt.anchorMin = new Vector2(0, 0.45f);
            lineRt.anchorMax = new Vector2(0, 0.45f);
            lineRt.pivot = new Vector2(0, 0.5f);
            lineRt.anchoredPosition = Vector2.zero;
            lineRt.sizeDelta = new Vector2(380, 2);
            Image lineImg = lineGO.GetComponent<Image>();
            lineImg.color = new Color(0.85f, 0.70f, 0.35f, 0.6f);

            // Subtitle: WAY TO INDEPENDENCE
            GameObject subTitleGO = new GameObject("SubTitle", typeof(RectTransform), typeof(TextMeshProUGUI));
            subTitleGO.transform.SetParent(titleBlock.transform, false);
            RectTransform subTitleRt = subTitleGO.GetComponent<RectTransform>();
            subTitleRt.anchorMin = new Vector2(0, 0);
            subTitleRt.anchorMax = new Vector2(1, 0.4f);
            subTitleRt.anchoredPosition = Vector2.zero;
            subTitleRt.sizeDelta = Vector2.zero;

            TextMeshProUGUI subTitleTMP = subTitleGO.GetComponent<TextMeshProUGUI>();
            subTitleTMP.font = fontAsset;
            subTitleTMP.fontSize = 13;
            subTitleTMP.characterSpacing = 14;
            subTitleTMP.enableWordWrapping = false;
            subTitleTMP.alignment = TextAlignmentOptions.MidlineLeft;
            subTitleTMP.color = new Color(0.82f, 0.76f, 0.65f, 0.85f);
            subTitleTMP.text = "WAY TO INDEPENDENCE";

            // 9. Minimalist Menu Buttons Stack (Left-Aligned below Title)
            GameObject buttonStackGO = new GameObject("ButtonStack", typeof(RectTransform), typeof(VerticalLayoutGroup));
            buttonStackGO.transform.SetParent(menuRootGO.transform, false);
            RectTransform stackRt = buttonStackGO.GetComponent<RectTransform>();
            stackRt.anchorMin = new Vector2(0, 1f);
            stackRt.anchorMax = new Vector2(0, 1f);
            stackRt.pivot = new Vector2(0, 1f);
            stackRt.anchoredPosition = new Vector2(70, -200);
            stackRt.sizeDelta = new Vector2(280, 250);

            VerticalLayoutGroup vlg = buttonStackGO.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 14;
            vlg.childAlignment = TextAnchor.MiddleLeft;
            vlg.childControlWidth = false;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = false;
            vlg.childForceExpandHeight = false;

            Sprite pillSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MainMenuSpritesPath + "btn_minimalist_pill.png");

            Button btnStart = CreateJuicyMenuButton(buttonStackGO, "Btn_Start", "BOSHLASH", pillSprite, fontAsset);
            Button btnContinue = CreateJuicyMenuButton(buttonStackGO, "Btn_Continue", "DAVOM ETISH", pillSprite, fontAsset);
            Button btnSettings = CreateJuicyMenuButton(buttonStackGO, "Btn_Settings", "SOZLAMALAR", pillSprite, fontAsset);
            Button btnExit = CreateJuicyMenuButton(buttonStackGO, "Btn_Exit", "CHIQISH", pillSprite, fontAsset);

            // 10. Version Label
            GameObject verGO = new GameObject("VersionLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
            verGO.transform.SetParent(menuRootGO.transform, false);
            RectTransform verRt = verGO.GetComponent<RectTransform>();
            verRt.anchorMin = new Vector2(0, 0);
            verRt.anchorMax = new Vector2(0, 0);
            verRt.pivot = new Vector2(0, 0);
            verRt.anchoredPosition = new Vector2(30, 20);
            verRt.sizeDelta = new Vector2(200, 30);
            TextMeshProUGUI verTMP = verGO.GetComponent<TextMeshProUGUI>();
            verTMP.font = fontAsset;
            verTMP.fontSize = 12;
            verTMP.color = new Color(0.75f, 0.70f, 0.60f, 0.45f);
            verTMP.text = "v0.1.0 Alpha";

            // 11. Embedded Settings Window Overlay
            GameObject settingsOverlay = CreateEmbeddedSettingsOverlay(canvasGO, fontAsset);
            SettingsUIController settingsController = settingsOverlay.GetComponent<SettingsUIController>();

            // Wire Main Menu Controller
            var so = new SerializedObject(menuController);
            so.FindProperty("startButton").objectReferenceValue = btnStart;
            so.FindProperty("continueButton").objectReferenceValue = btnContinue;
            so.FindProperty("settingsButton").objectReferenceValue = btnSettings;
            so.FindProperty("exitButton").objectReferenceValue = btnExit;
            so.FindProperty("settingsOverlay").objectReferenceValue = settingsController;
            so.FindProperty("gameplaySceneName").stringValue = "SampleScene";
            so.ApplyModifiedProperties();

            // Wire Parallax Component
            var soParallax = new SerializedObject(parallax);
            soParallax.FindProperty("backgroundTransform").objectReferenceValue = bgRt;
            soParallax.FindProperty("titleTransform").objectReferenceValue = titleBlockRt;
            soParallax.FindProperty("buttonsTransform").objectReferenceValue = stackRt;
            soParallax.FindProperty("backgroundStrength").floatValue = 18f;
            soParallax.FindProperty("titleStrength").floatValue = 8f;
            soParallax.FindProperty("buttonsStrength").floatValue = 12f;
            soParallax.FindProperty("smoothSpeed").floatValue = 5f;
            soParallax.ApplyModifiedProperties();

            // Save Scene
            EditorSceneManager.SaveScene(scene, ScenePath);

            // Register as Scene 0 in Build Settings
            RegisterMainMenuInBuildSettings(ScenePath);
        }

        private static void CreateAtmosphericParticles(GameObject canvasGO)
        {
            var partGO = new GameObject("AtmosphereParticles", typeof(ParticleSystem));
            partGO.transform.SetParent(canvasGO.transform, false);
            partGO.transform.SetSiblingIndex(2);

            var ps = partGO.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.loop = true;
            main.startLifetime = 9f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(8f, 20f);
            main.startSize = new ParticleSystem.MinMaxCurve(2f, 6f);
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(1f, 0.85f, 0.55f, 0.45f),
                new Color(0.95f, 0.65f, 0.25f, 0.25f)
            );
            main.maxParticles = 50;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;

            var emission = ps.emission;
            emission.rateOverTime = 5f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Rectangle;
            shape.scale = new Vector3(1024f, 682f, 1f);
            shape.position = new Vector3(0, -100f, 0);

            var vel = ps.velocityOverLifetime;
            vel.enabled = true;
            vel.x = new ParticleSystem.MinMaxCurve(3f, 8f);
            vel.y = new ParticleSystem.MinMaxCurve(4f, 14f);

            var col = ps.colorOverLifetime;
            col.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.85f, 0.55f), 0f), new GradientColorKey(new Color(0.95f, 0.65f, 0.25f), 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.45f, 0.3f), new GradientAlphaKey(0.35f, 0.7f), new GradientAlphaKey(0f, 1f) }
            );
            col.color = grad;

            var sz = ps.sizeOverLifetime;
            sz.enabled = true;
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 0.2f);
            curve.AddKey(0.4f, 1f);
            curve.AddKey(1f, 0f);
            sz.size = new ParticleSystem.MinMaxCurve(1f, curve);

            var renderer = partGO.GetComponent<ParticleSystemRenderer>();
            renderer.sortingOrder = 5;
        }

        private static Button CreateJuicyMenuButton(GameObject parent, string name, string label, Sprite pillSprite, TMP_FontAsset font)
        {
            GameObject btnGO = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(JuicyMenuButton), typeof(Outline));
            btnGO.transform.SetParent(parent.transform, false);
            RectTransform rt = btnGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(250, 44);

            Image img = btnGO.GetComponent<Image>();
            if (pillSprite != null)
            {
                img.sprite = pillSprite;
                img.type = Image.Type.Simple;
                img.preserveAspect = true;
            }
            else
            {
                img.color = new Color(0.12f, 0.08f, 0.05f, 0.75f);
            }

            Outline outline = btnGO.GetComponent<Outline>();
            outline.effectColor = new Color(0.85f, 0.70f, 0.35f, 0.35f);
            outline.effectDistance = new Vector2(1, -1);

            Button btn = btnGO.GetComponent<Button>();
            btn.transition = Selectable.Transition.None;

            GameObject textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(btnGO.transform, false);
            RectTransform textRt = textGO.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.anchoredPosition = Vector2.zero;
            textRt.sizeDelta = Vector2.zero;

            TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.fontSize = 14;
            tmp.fontStyle = FontStyles.Bold;
            tmp.characterSpacing = 3;
            tmp.enableWordWrapping = false;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.94f, 0.88f, 0.74f, 0.9f);
            tmp.text = label;

            JuicyMenuButton juicy = btnGO.GetComponent<JuicyMenuButton>();
            var so = new SerializedObject(juicy);
            so.FindProperty("hoverSlideDistance").floatValue = 14f;
            so.FindProperty("slideSpeed").floatValue = 15f;
            so.FindProperty("hoverScale").floatValue = 1.025f;
            so.FindProperty("pressScale").floatValue = 0.94f;
            so.FindProperty("scaleSpeed").floatValue = 18f;
            so.FindProperty("normalTracking").floatValue = 3f;
            so.FindProperty("hoverTracking").floatValue = 7f;
            so.FindProperty("normalTextColor").colorValue = new Color(0.94f, 0.88f, 0.74f, 0.9f);
            so.FindProperty("hoverTextColor").colorValue = new Color(1f, 0.98f, 0.88f, 1f);
            so.FindProperty("normalOutlineColor").colorValue = new Color(0.85f, 0.70f, 0.35f, 0.35f);
            so.FindProperty("hoverOutlineColor").colorValue = new Color(1f, 0.88f, 0.50f, 0.95f);
            so.FindProperty("buttonText").objectReferenceValue = tmp;
            so.ApplyModifiedProperties();

            return btn;
        }

        private static GameObject CreateEmbeddedSettingsOverlay(GameObject canvasGO, TMP_FontAsset fontAsset)
        {
            GameObject modalOverlayGO = new GameObject("SettingsModalOverlay", typeof(RectTransform), typeof(Image));
            modalOverlayGO.transform.SetParent(canvasGO.transform, false);
            RectTransform modalRt = modalOverlayGO.GetComponent<RectTransform>();
            modalRt.anchorMin = Vector2.zero;
            modalRt.anchorMax = Vector2.one;
            modalRt.sizeDelta = Vector2.zero;
            Image modalImg = modalOverlayGO.GetComponent<Image>();
            modalImg.color = new Color(0f, 0f, 0f, 0.7f);

            GameObject windowRoot = new GameObject("SettingsWindow", typeof(RectTransform), typeof(SettingsUIController));
            windowRoot.transform.SetParent(modalOverlayGO.transform, false);
            RectTransform windowRt = windowRoot.GetComponent<RectTransform>();
            windowRt.anchorMin = new Vector2(0.5f, 0.5f);
            windowRt.anchorMax = new Vector2(0.5f, 0.5f);
            windowRt.pivot = new Vector2(0.5f, 0.5f);
            windowRt.anchoredPosition = new Vector2(0, -12);
            windowRt.sizeDelta = new Vector2(1004, 586);

            SettingsUIController controller = windowRoot.GetComponent<SettingsUIController>();

            GameObject frameGO = new GameObject("FrameMainWood", typeof(RectTransform), typeof(Image));
            frameGO.transform.SetParent(windowRoot.transform, false);
            RectTransform frameRt = frameGO.GetComponent<RectTransform>();
            frameRt.anchorMin = Vector2.zero;
            frameRt.anchorMax = Vector2.one;
            frameRt.sizeDelta = Vector2.zero;
            Image frameImg = frameGO.GetComponent<Image>();
            frameImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "frame_main_wood.png");

            GameObject headerBadge = new GameObject("HeaderBadge_SETTINGS", typeof(RectTransform), typeof(Image));
            headerBadge.transform.SetParent(windowRoot.transform, false);
            RectTransform badgeRt = headerBadge.GetComponent<RectTransform>();
            badgeRt.anchorMin = new Vector2(0.5f, 1f);
            badgeRt.anchorMax = new Vector2(0.5f, 1f);
            badgeRt.pivot = new Vector2(0.5f, 0.5f);
            badgeRt.anchoredPosition = new Vector2(0, 16);
            badgeRt.sizeDelta = new Vector2(440, 100);
            Image badgeImg = headerBadge.GetComponent<Image>();
            badgeImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SettingsSpritesPath + "header_badge_sozlamalar.png");
            badgeImg.preserveAspect = true;

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
            Button closeBtn = closeBtnGO.GetComponent<Button>();

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
            tabList.Add(CreateSolidTabWithIcon(tabBarGO, "Tab_Audio", "OVOZ", SettingsTabCategory.Audio, -260f, 240, 43, 0, tabActiveBg, tabInactiveBg, iconAudio, fontAsset, true));
            tabList.Add(CreateSolidTabWithIcon(tabBarGO, "Tab_Graphics", "GRAFIKA", SettingsTabCategory.Graphics, 0f, 240, 43, 0, tabActiveBg, tabInactiveBg, iconGraphics, fontAsset, false));
            tabList.Add(CreateSolidTabWithIcon(tabBarGO, "Tab_Gameplay", "O'YIN", SettingsTabCategory.Gameplay, 260f, 240, 43, 0, tabActiveBg, tabInactiveBg, iconGameplay, fontAsset, false));

            GameObject contentContainerGO = new GameObject("ContentContainer", typeof(RectTransform), typeof(Image));
            contentContainerGO.transform.SetParent(windowRoot.transform, false);
            RectTransform contentRt = contentContainerGO.GetComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0.5f, 1f);
            contentRt.anchorMax = new Vector2(0.5f, 1f);
            contentRt.pivot = new Vector2(0.5f, 1f);
            contentRt.anchoredPosition = new Vector2(0, -78);
            contentRt.sizeDelta = new Vector2(940, 416);

            Image contentBg = contentContainerGO.GetComponent<Image>();
            contentBg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SettingsSpritesPath + "parchment_clean_box.png");
            contentBg.color = Color.white;

            GameObject audioPanel = CreateSectionPanel(contentContainerGO, "AudioPanel");
            GameObject graphicsPanel = CreateSectionPanel(contentContainerGO, "GraphicsPanel");
            GameObject gameplayPanel = CreateSectionPanel(contentContainerGO, "GameplayPanel");

            var masterSliderRow = CreateSliderRow(audioPanel, "MasterVolumeRow", "Asosiy ovoz", fontAsset, out Slider masterSlider, out TextMeshProUGUI masterValText);
            var musicSliderRow = CreateSliderRow(audioPanel, "MusicVolumeRow", "Musiqa", fontAsset, out Slider musicSlider, out TextMeshProUGUI musicValText);
            var sfxSliderRow = CreateSliderRow(audioPanel, "SFXVolumeRow", "Ovoz effektlari", fontAsset, out Slider sfxSlider, out TextMeshProUGUI sfxValText);
            var muteToggleRow = CreateToggleRow(audioPanel, "MuteToggleRow", "Ovozni o'chirish", fontAsset, out Toggle muteToggle, out TextMeshProUGUI muteStatusText);

            var resDropdownRow = CreateDropdownRow(graphicsPanel, "ResolutionRow", "Ekran o'lchami", fontAsset, out TMP_Dropdown resDropdown);
            var fsDropdownRow = CreateDropdownRow(graphicsPanel, "FullscreenRow", "Ekran rejimi", fontAsset, out TMP_Dropdown fsDropdown);
            var qualDropdownRow = CreateDropdownRow(graphicsPanel, "QualityRow", "Grafika sifati", fontAsset, out TMP_Dropdown qualDropdown);
            var vsyncToggleRow = CreateToggleRow(graphicsPanel, "VSyncRow", "V-Sync", fontAsset, out Toggle vsyncToggle, out TextMeshProUGUI vsyncStatusText);
            var fpsDropdownRow = CreateDropdownRow(graphicsPanel, "FPSRow", "FPS cheklovi", fontAsset, out TMP_Dropdown fpsDropdown);

            var langDropdownRow = CreateDropdownRow(gameplayPanel, "LanguageRow", "Til", fontAsset, out TMP_Dropdown langDropdown);
            var panSliderRow = CreateSliderRow(gameplayPanel, "PanSpeedRow", "Kamera tezligi", fontAsset, out Slider panSlider, out TextMeshProUGUI panValText, 0.5f, 2.0f);
            var autoSaveDropdownRow = CreateDropdownRow(gameplayPanel, "AutoSaveRow", "Avto-saqlash", fontAsset, out TMP_Dropdown autoSaveDropdown);

            graphicsPanel.SetActive(false);
            gameplayPanel.SetActive(false);
            audioPanel.SetActive(true);

            GameObject bottomBarGO = new GameObject("BottomActionsBar", typeof(RectTransform));
            bottomBarGO.transform.SetParent(windowRoot.transform, false);
            RectTransform bottomRt = bottomBarGO.GetComponent<RectTransform>();
            bottomRt.anchorMin = new Vector2(0.5f, 0f);
            bottomRt.anchorMax = new Vector2(0.5f, 0f);
            bottomRt.pivot = new Vector2(0.5f, 0f);
            bottomRt.anchoredPosition = new Vector2(0, 16);
            bottomRt.sizeDelta = new Vector2(940, 50);

            Button resetBtn = CreateActionButton(bottomBarGO, "ResetDefaultsButton", "Standart", new Vector2(-150, 0), new Vector2(180, 44), fontAsset,
                AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "tab_inactive_bg.png"), new Color(0.35f, 0.15f, 0.10f, 1f));

            Button saveBtn = CreateActionButton(bottomBarGO, "SaveApplyButton", "Saqlash", new Vector2(150, 0), new Vector2(180, 44), fontAsset,
                AssetDatabase.LoadAssetAtPath<Sprite>(BozorSpritesPath + "btn_buy_green_clean.png"), Color.white);

            var so = new SerializedObject(controller);
            so.FindProperty("windowRoot").objectReferenceValue = modalOverlayGO;
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

            so.FindProperty("masterVolumeSlider").objectReferenceValue = masterSlider;
            so.FindProperty("masterVolumeText").objectReferenceValue = masterValText;
            so.FindProperty("musicVolumeSlider").objectReferenceValue = musicSlider;
            so.FindProperty("musicVolumeText").objectReferenceValue = musicValText;
            so.FindProperty("sfxVolumeSlider").objectReferenceValue = sfxSlider;
            so.FindProperty("sfxVolumeText").objectReferenceValue = sfxValText;
            so.FindProperty("muteToggle").objectReferenceValue = muteToggle;
            so.FindProperty("muteToggleStatusText").objectReferenceValue = muteStatusText;

            so.FindProperty("resolutionDropdown").objectReferenceValue = resDropdown;
            so.FindProperty("fullscreenDropdown").objectReferenceValue = fsDropdown;
            so.FindProperty("qualityDropdown").objectReferenceValue = qualDropdown;
            so.FindProperty("vSyncToggle").objectReferenceValue = vsyncToggle;
            so.FindProperty("vSyncStatusText").objectReferenceValue = vsyncStatusText;
            so.FindProperty("targetFpsDropdown").objectReferenceValue = fpsDropdown;

            so.FindProperty("languageDropdown").objectReferenceValue = langDropdown;
            so.FindProperty("cameraPanSpeedSlider").objectReferenceValue = panSlider;
            so.FindProperty("cameraPanSpeedText").objectReferenceValue = panValText;
            so.FindProperty("autoSaveDropdown").objectReferenceValue = autoSaveDropdown;

            so.FindProperty("resetDefaultsButton").objectReferenceValue = resetBtn;
            so.FindProperty("saveApplyButton").objectReferenceValue = saveBtn;
            so.ApplyModifiedProperties();

            modalOverlayGO.SetActive(false);

            return windowRoot;
        }

        private static GameObject CreateSectionPanel(GameObject parent, string name)
        {
            GameObject panelGO = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup));
            panelGO.transform.SetParent(parent.transform, false);
            RectTransform rt = panelGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(40, 24);
            rt.offsetMax = new Vector2(-40, -28);

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

            GameObject lblGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            lblGO.transform.SetParent(rowGO.transform, false);
            RectTransform lblRt = lblGO.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0, 0);
            lblRt.anchorMax = new Vector2(0.48f, 1);
            lblRt.anchoredPosition = Vector2.zero;
            lblRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI lblTmp = lblGO.GetComponent<TextMeshProUGUI>();
            lblTmp.font = font;
            lblTmp.fontSize = 15;
            lblTmp.fontStyle = FontStyles.Bold;
            lblTmp.color = new Color(0.24f, 0.16f, 0.10f, 1f);
            lblTmp.alignment = TextAlignmentOptions.MidlineLeft;
            lblTmp.text = labelText;

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

            GameObject bgGO = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgGO.transform.SetParent(sliderGO.transform, false);
            RectTransform bgRt = bgGO.GetComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0, 0.2f);
            bgRt.anchorMax = new Vector2(1, 0.8f);
            bgRt.anchoredPosition = Vector2.zero;
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bgGO.GetComponent<Image>();
            bgImg.color = new Color(0.25f, 0.18f, 0.12f, 0.85f);

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
            fillImg.color = new Color(0.88f, 0.68f, 0.25f, 1f);

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

            GameObject valGO = new GameObject("ValueText", typeof(RectTransform), typeof(TextMeshProUGUI));
            valGO.transform.SetParent(rowGO.transform, false);
            RectTransform valRt = valGO.GetComponent<RectTransform>();
            valRt.anchorMin = new Vector2(0.88f, 0);
            valRt.anchorMax = new Vector2(1f, 1);
            valRt.anchoredPosition = Vector2.zero;
            valRt.sizeDelta = Vector2.zero;
            valueText = valGO.GetComponent<TextMeshProUGUI>();
            valueText.font = font;
            valueText.fontSize = 15;
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

            GameObject lblGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            lblGO.transform.SetParent(rowGO.transform, false);
            RectTransform lblRt = lblGO.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0, 0);
            lblRt.anchorMax = new Vector2(0.48f, 1);
            lblRt.anchoredPosition = Vector2.zero;
            lblRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI lblTmp = lblGO.GetComponent<TextMeshProUGUI>();
            lblTmp.font = font;
            lblTmp.fontSize = 15;
            lblTmp.fontStyle = FontStyles.Bold;
            lblTmp.color = new Color(0.24f, 0.16f, 0.10f, 1f);
            lblTmp.alignment = TextAlignmentOptions.MidlineLeft;
            lblTmp.text = labelText;

            GameObject toggleGO = new GameObject("Toggle", typeof(RectTransform), typeof(Toggle));
            toggleGO.transform.SetParent(rowGO.transform, false);
            RectTransform toggleRt = toggleGO.GetComponent<RectTransform>();
            toggleRt.anchorMin = new Vector2(0.50f, 0.5f);
            toggleRt.anchorMax = new Vector2(0.50f, 0.5f);
            toggleRt.pivot = new Vector2(0, 0.5f);
            toggleRt.anchoredPosition = Vector2.zero;
            toggleRt.sizeDelta = new Vector2(46, 26);

            toggle = toggleGO.GetComponent<Toggle>();

            GameObject bgGO = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgGO.transform.SetParent(toggleGO.transform, false);
            RectTransform bgRt = bgGO.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bgGO.GetComponent<Image>();
            bgImg.color = new Color(0.28f, 0.20f, 0.14f, 0.95f);

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

            GameObject statGO = new GameObject("StatusText", typeof(RectTransform), typeof(TextMeshProUGUI));
            statGO.transform.SetParent(rowGO.transform, false);
            RectTransform statRt = statGO.GetComponent<RectTransform>();
            statRt.anchorMin = new Vector2(0.58f, 0);
            statRt.anchorMax = new Vector2(1f, 1);
            statRt.anchoredPosition = Vector2.zero;
            statRt.sizeDelta = Vector2.zero;
            statusText = statGO.GetComponent<TextMeshProUGUI>();
            statusText.font = font;
            statusText.fontSize = 14;
            statusText.fontStyle = FontStyles.Bold;
            statusText.color = new Color(0.35f, 0.22f, 0.14f, 1f);
            statusText.alignment = TextAlignmentOptions.MidlineLeft;
            statusText.text = "O'chirilgan";

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

            GameObject lblGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            lblGO.transform.SetParent(rowGO.transform, false);
            RectTransform lblRt = lblGO.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0, 0);
            lblRt.anchorMax = new Vector2(0.48f, 1);
            lblRt.anchoredPosition = Vector2.zero;
            lblRt.sizeDelta = Vector2.zero;
            TextMeshProUGUI lblTmp = lblGO.GetComponent<TextMeshProUGUI>();
            lblTmp.font = font;
            lblTmp.fontSize = 15;
            lblTmp.fontStyle = FontStyles.Bold;
            lblTmp.color = new Color(0.24f, 0.16f, 0.10f, 1f);
            lblTmp.alignment = TextAlignmentOptions.MidlineLeft;
            lblTmp.text = labelText;

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

            GameObject captionGO = new GameObject("CaptionText", typeof(RectTransform), typeof(TextMeshProUGUI));
            captionGO.transform.SetParent(ddGO.transform, false);
            RectTransform capRt = captionGO.GetComponent<RectTransform>();
            capRt.anchorMin = new Vector2(0, 0);
            capRt.anchorMax = new Vector2(1, 1);
            capRt.anchoredPosition = new Vector2(12, 0);
            capRt.sizeDelta = new Vector2(-36, 0);
            TextMeshProUGUI capTmp = captionGO.GetComponent<TextMeshProUGUI>();
            capTmp.font = font;
            capTmp.fontSize = 14;
            capTmp.fontStyle = FontStyles.Bold;
            capTmp.color = new Color(0.20f, 0.14f, 0.08f, 1f);
            capTmp.alignment = TextAlignmentOptions.MidlineLeft;
            capTmp.text = "Tanlang";

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
            arrowTmp.fontSize = 14;
            arrowTmp.alignment = TextAlignmentOptions.Center;
            arrowTmp.color = new Color(0.30f, 0.20f, 0.14f, 1f);
            arrowTmp.text = "▼";

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

            GameObject viewGO = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            viewGO.transform.SetParent(templateGO.transform, false);
            RectTransform viewRt = viewGO.GetComponent<RectTransform>();
            viewRt.anchorMin = Vector2.zero;
            viewRt.anchorMax = Vector2.one;
            viewRt.sizeDelta = Vector2.zero;
            Mask mask = viewGO.GetComponent<Mask>();
            mask.showMaskGraphic = false;

            GameObject cntGO = new GameObject("Content", typeof(RectTransform));
            cntGO.transform.SetParent(viewGO.transform, false);
            RectTransform cntRt = cntGO.GetComponent<RectTransform>();
            cntRt.anchorMin = new Vector2(0, 1);
            cntRt.anchorMax = new Vector2(1, 1);
            cntRt.pivot = new Vector2(0.5f, 1);
            cntRt.anchoredPosition = Vector2.zero;
            cntRt.sizeDelta = new Vector2(0, 32);

            GameObject itemGO = new GameObject("Item", typeof(RectTransform), typeof(Toggle));
            itemGO.transform.SetParent(cntGO.transform, false);
            RectTransform itemRt = itemGO.GetComponent<RectTransform>();
            itemRt.anchorMin = new Vector2(0, 0.5f);
            itemRt.anchorMax = new Vector2(1, 0.5f);
            itemRt.sizeDelta = new Vector2(0, 30);

            Toggle itemToggle = itemGO.GetComponent<Toggle>();

            GameObject itemBgGO = new GameObject("Item Background", typeof(RectTransform), typeof(Image));
            itemBgGO.transform.SetParent(itemGO.transform, false);
            RectTransform itemBgRt = itemBgGO.GetComponent<RectTransform>();
            itemBgRt.anchorMin = Vector2.zero;
            itemBgRt.anchorMax = Vector2.one;
            itemBgRt.sizeDelta = Vector2.zero;
            Image itemBgImg = itemBgGO.GetComponent<Image>();
            itemBgImg.color = new Color(0.88f, 0.84f, 0.76f, 0.8f);

            GameObject itemLblGO = new GameObject("Item Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            itemLblGO.transform.SetParent(itemGO.transform, false);
            RectTransform itemLblRt = itemLblGO.GetComponent<RectTransform>();
            itemLblRt.anchorMin = Vector2.zero;
            itemLblRt.anchorMax = Vector2.one;
            itemLblRt.anchoredPosition = new Vector2(10, 0);
            itemLblRt.sizeDelta = new Vector2(-20, 0);
            TextMeshProUGUI itemLblTmp = itemLblGO.GetComponent<TextMeshProUGUI>();
            itemLblTmp.font = font;
            itemLblTmp.fontSize = 14;
            itemLblTmp.color = new Color(0.20f, 0.14f, 0.08f, 1f);
            itemLblTmp.alignment = TextAlignmentOptions.MidlineLeft;

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
            tmp.fontSize = 13;
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

        private static void RegisterMainMenuInBuildSettings(string scenePath)
        {
            var scenes = new List<EditorBuildSettingsScene>();
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));

            foreach (var s in EditorBuildSettings.scenes)
            {
                if (s.path != scenePath)
                {
                    scenes.Add(s);
                }
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
