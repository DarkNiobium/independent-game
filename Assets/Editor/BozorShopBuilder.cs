using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using BozorShop;

namespace BozorShop.Editor
{
    public static class BozorShopBuilder
    {
        private const string SpritesPath = "Assets/UI/BozorShop/Sprites/";
        private const string DataPath = "Assets/Data/Buildings/";
        private const string PrefabsPath = "Assets/Prefabs/UI/";
        private const string ScenePath = "Assets/Scenes/BozorShopScene.unity";

        [MenuItem("Bozor/Build Everything")]
        public static void BuildEverything()
        {
            SetupDirectories();
            ConfigureTextureImporters();
            AssetDatabase.Refresh();

            TMP_FontAsset fontAsset = TMP_Settings.defaultFontAsset;
            CreateBuildingScriptableObjects();
            GameObject cardPrefab = CreateCardPrefab(fontAsset);
            BuildScene(fontAsset, cardPrefab);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Bozor Shop]</color> Clash of Clans style shop built with new buildings successfully!");
        }

        private static void SetupDirectories()
        {
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
            if (!Directory.Exists(PrefabsPath)) Directory.CreateDirectory(PrefabsPath);
            if (!Directory.Exists("Assets/Scenes")) Directory.CreateDirectory("Assets/Scenes");
        }

        public static void ConfigureTextureImporters()
        {
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/UI/BozorShop/Sprites" });
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

                    string filename = Path.GetFileName(path);
                    if (filename == "tab_active_bg.png" || filename == "tab_inactive_bg.png")
                    {
                        importer.spriteBorder = new Vector4(15, 12, 15, 4);
                    }

                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                }
            }
        }

        private static void CreateBuildingScriptableObjects()
        {
            // 1. Ishlab chiqarish (Production)
            CreateSO("SO_YogochKesishxona", "YOG'OCH ZAXIRASI", BuildingCategory.Production, "building_yogoch_kesishxona", "icon_box", 20, "/ soat", 80, 450000);
            CreateSO("SO_QishloqUyi", "QISHLOQ UYI", BuildingCategory.Production, "building_qishloq_uyi", "icon_sheep", 15, "/ soat", 60, 500000);

            // 2. Qishloq xo'jaligi (Agriculture)
            CreateSO("SO_PaxtaOmbori", "PAXTA OMBORI", BuildingCategory.Agriculture, "building_paxta_ombori", "icon_cotton", 25, "/ soat", 100, 550000);
            CreateSO("SO_BugdoyDalasi", "BUG'DOYZOR", BuildingCategory.Agriculture, "building_bugdoy_dalasi", "icon_cotton", 30, "/ soat", 120, 600000);
            CreateSO("SO_SomonOmbori", "SOMONXONA", BuildingCategory.Agriculture, "building_somon_ombori", "icon_box", 18, "/ soat", 75, 400000);

            // 3. Savdo (Trade)
            CreateSO("SO_MatoDokoni", "MATO DO'KONI", BuildingCategory.Trade, "building_mato_dokoni", "icon_robe", 35, "/ soat", 50, 650000);
            CreateSO("SO_OziqOvqat", "OZIQ-OVQAT DO'KONI", BuildingCategory.Trade, "building_oziq_ovqat", "icon_box", 40, "/ soat", 60, 700000);

            // 4. Bezaklar (Decorations)
            CreateSO("SO_QadimiyFavvora", "QADIMIY FAVVORA", BuildingCategory.Decorations, "building_somon_ombori", "icon_gold_coin", 5, " Obro'", 0, 250000);
            CreateSO("SO_SharqHovlisi", "SHARQONA HOVLI", BuildingCategory.Decorations, "building_qishloq_uyi", "icon_gold_coin", 10, " Obro'", 0, 500000);
        }

        private static void CreateSO(string assetName, string displayName, BuildingCategory category, string previewName, string resourceIconName, int rate, string unit, int capacity, int price)
        {
            string path = DataPath + assetName + ".asset";
            BuildingDataSO so = AssetDatabase.LoadAssetAtPath<BuildingDataSO>(path);
            if (so == null)
            {
                so = ScriptableObject.CreateInstance<BuildingDataSO>();
                AssetDatabase.CreateAsset(so, path);
            }

            so.id = assetName;
            so.displayName = displayName;
            so.category = category;
            so.buildingPreview = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + previewName + ".png");
            so.resourceIcon = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + resourceIconName + ".png");
            so.productionRate = rate;
            so.rateUnit = unit;
            so.capacityIcon = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "icon_box.png");
            so.capacity = capacity;
            so.priceGold = price;

            EditorUtility.SetDirty(so);
        }

        private static GameObject CreateCardPrefab(TMP_FontAsset fontAsset)
        {
            string prefabPath = PrefabsPath + "ShopCardPrefab.prefab";
            if (File.Exists(prefabPath))
            {
                AssetDatabase.DeleteAsset(prefabPath);
            }

            GameObject cardGO = new GameObject("ShopCardPrefab", typeof(RectTransform), typeof(Image), typeof(LayoutElement), typeof(ShopCardUI));
            RectTransform rt = cardGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(235, 475);

            LayoutElement le = cardGO.GetComponent<LayoutElement>();
            le.minWidth = 235;
            le.minHeight = 475;
            le.preferredWidth = 235;
            le.preferredHeight = 475;
            le.flexibleWidth = 0;
            le.flexibleHeight = 0;

            Image bgImg = cardGO.GetComponent<Image>();
            bgImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "card_bg_parchment.png");
            bgImg.type = Image.Type.Simple;

            // Title
            GameObject titleGO = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleGO.transform.SetParent(cardGO.transform, false);
            RectTransform titleRt = titleGO.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0, 1);
            titleRt.anchorMax = new Vector2(1, 1);
            titleRt.pivot = new Vector2(0.5f, 1);
            titleRt.anchoredPosition = new Vector2(0, -18);
            titleRt.sizeDelta = new Vector2(-20, 26);

            TextMeshProUGUI titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
            titleTMP.font = fontAsset;
            titleTMP.fontSize = 15;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = new Color(0.24f, 0.16f, 0.10f, 1f);
            titleTMP.text = "BUILDING NAME";

            // Preview Image
            GameObject previewGO = new GameObject("PreviewImage", typeof(RectTransform), typeof(Image));
            previewGO.transform.SetParent(cardGO.transform, false);
            RectTransform prevRt = previewGO.GetComponent<RectTransform>();
            prevRt.anchorMin = new Vector2(0.5f, 0.5f);
            prevRt.anchorMax = new Vector2(0.5f, 0.5f);
            prevRt.pivot = new Vector2(0.5f, 0.5f);
            prevRt.anchoredPosition = new Vector2(0, 52);
            prevRt.sizeDelta = new Vector2(215, 185);

            Image prevImg = previewGO.GetComponent<Image>();
            prevImg.preserveAspect = true;

            // Stats Container
            GameObject statsGO = new GameObject("StatsContainer", typeof(RectTransform));
            statsGO.transform.SetParent(cardGO.transform, false);
            RectTransform statsRt = statsGO.GetComponent<RectTransform>();
            statsRt.anchorMin = new Vector2(0, 0);
            statsRt.anchorMax = new Vector2(1, 0);
            statsRt.pivot = new Vector2(0.5f, 0);
            statsRt.anchoredPosition = new Vector2(0, 70);
            statsRt.sizeDelta = new Vector2(-40, 60);

            // Production Row
            GameObject prodRow = new GameObject("ProductionRow", typeof(RectTransform));
            prodRow.transform.SetParent(statsGO.transform, false);
            RectTransform prodRt = prodRow.GetComponent<RectTransform>();
            prodRt.anchorMin = new Vector2(0, 0.5f);
            prodRt.anchorMax = new Vector2(1, 1);
            prodRt.anchoredPosition = Vector2.zero;
            prodRt.sizeDelta = Vector2.zero;

            GameObject prodIconGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            prodIconGO.transform.SetParent(prodRow.transform, false);
            RectTransform prodIconRt = prodIconGO.GetComponent<RectTransform>();
            prodIconRt.anchorMin = new Vector2(0, 0.5f);
            prodIconRt.anchorMax = new Vector2(0, 0.5f);
            prodIconRt.pivot = new Vector2(0, 0.5f);
            prodIconRt.anchoredPosition = new Vector2(14, 0);
            prodIconRt.sizeDelta = new Vector2(28, 24);
            Image prodIconImg = prodIconGO.GetComponent<Image>();
            prodIconImg.preserveAspect = true;

            GameObject prodTextGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            prodTextGO.transform.SetParent(prodRow.transform, false);
            RectTransform prodTextRt = prodTextGO.GetComponent<RectTransform>();
            prodTextRt.anchorMin = new Vector2(0, 0);
            prodTextRt.anchorMax = new Vector2(1, 1);
            prodTextRt.pivot = new Vector2(0, 0.5f);
            prodTextRt.anchoredPosition = new Vector2(50, 0);
            prodTextRt.sizeDelta = new Vector2(-50, 0);
            TextMeshProUGUI prodTMP = prodTextGO.GetComponent<TextMeshProUGUI>();
            prodTMP.font = fontAsset;
            prodTMP.fontSize = 15;
            prodTMP.fontStyle = FontStyles.Bold;
            prodTMP.alignment = TextAlignmentOptions.Left;
            prodTMP.color = new Color(0.20f, 0.14f, 0.08f, 1f);
            prodTMP.text = "+15 / soat";

            // Capacity Row
            GameObject capRow = new GameObject("CapacityRow", typeof(RectTransform));
            capRow.transform.SetParent(statsGO.transform, false);
            RectTransform capRt = capRow.GetComponent<RectTransform>();
            capRt.anchorMin = new Vector2(0, 0);
            capRt.anchorMax = new Vector2(1, 0.5f);
            capRt.anchoredPosition = Vector2.zero;
            capRt.sizeDelta = Vector2.zero;

            GameObject capIconGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            capIconGO.transform.SetParent(capRow.transform, false);
            RectTransform capIconRt = capIconGO.GetComponent<RectTransform>();
            capIconRt.anchorMin = new Vector2(0, 0.5f);
            capIconRt.anchorMax = new Vector2(0, 0.5f);
            capIconRt.pivot = new Vector2(0, 0.5f);
            capIconRt.anchoredPosition = new Vector2(16, 0);
            capIconRt.sizeDelta = new Vector2(24, 22);
            Image capIconImg = capIconGO.GetComponent<Image>();
            capIconImg.preserveAspect = true;

            GameObject capTextGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            capTextGO.transform.SetParent(capRow.transform, false);
            RectTransform capTextRt = capTextGO.GetComponent<RectTransform>();
            capTextRt.anchorMin = new Vector2(0, 0);
            capTextRt.anchorMax = new Vector2(1, 1);
            capTextRt.pivot = new Vector2(0, 0.5f);
            capTextRt.anchoredPosition = new Vector2(50, 0);
            capTextRt.sizeDelta = new Vector2(-50, 0);
            TextMeshProUGUI capTMP = capTextGO.GetComponent<TextMeshProUGUI>();
            capTMP.font = fontAsset;
            capTMP.fontSize = 15;
            capTMP.fontStyle = FontStyles.Bold;
            capTMP.alignment = TextAlignmentOptions.Left;
            capTMP.color = new Color(0.20f, 0.14f, 0.08f, 1f);
            capTMP.text = "Sig'im: 60";

            // Buy Button
            GameObject buyBtnGO = new GameObject("BuyButton", typeof(RectTransform), typeof(Image), typeof(Button));
            buyBtnGO.transform.SetParent(cardGO.transform, false);
            RectTransform buyRt = buyBtnGO.GetComponent<RectTransform>();
            buyRt.anchorMin = new Vector2(0.5f, 0);
            buyRt.anchorMax = new Vector2(0.5f, 0);
            buyRt.pivot = new Vector2(0.5f, 0);
            buyRt.anchoredPosition = new Vector2(0, 18);
            buyRt.sizeDelta = new Vector2(175, 41);

            Image buyImg = buyBtnGO.GetComponent<Image>();
            buyImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "btn_buy_green_clean.png");
            buyImg.type = Image.Type.Simple;

            Button buyBtn = buyBtnGO.GetComponent<Button>();

            // Price Text
            GameObject priceGO = new GameObject("PriceText", typeof(RectTransform), typeof(TextMeshProUGUI));
            priceGO.transform.SetParent(buyBtnGO.transform, false);
            RectTransform priceRt = priceGO.GetComponent<RectTransform>();
            priceRt.anchorMin = new Vector2(0, 0);
            priceRt.anchorMax = new Vector2(1, 1);
            priceRt.anchoredPosition = new Vector2(18, 0);
            priceRt.sizeDelta = new Vector2(-36, 0);

            TextMeshProUGUI priceTMP = priceGO.GetComponent<TextMeshProUGUI>();
            priceTMP.font = fontAsset;
            priceTMP.fontSize = 16;
            priceTMP.fontStyle = FontStyles.Bold;
            priceTMP.alignment = TextAlignmentOptions.Center;
            priceTMP.color = Color.white;
            priceTMP.text = "500 000";

            // Wire Card Component
            ShopCardUI cardUI = cardGO.GetComponent<ShopCardUI>();
            var serializedObject = new SerializedObject(cardUI);
            serializedObject.FindProperty("titleText").objectReferenceValue = titleTMP;
            serializedObject.FindProperty("previewImage").objectReferenceValue = prevImg;
            serializedObject.FindProperty("resourceIcon").objectReferenceValue = prodIconImg;
            serializedObject.FindProperty("productionText").objectReferenceValue = prodTMP;
            serializedObject.FindProperty("capacityIcon").objectReferenceValue = capIconImg;
            serializedObject.FindProperty("capacityText").objectReferenceValue = capTMP;
            serializedObject.FindProperty("buyButton").objectReferenceValue = buyBtn;
            serializedObject.FindProperty("priceText").objectReferenceValue = priceTMP;
            serializedObject.ApplyModifiedProperties();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cardGO, prefabPath);
            GameObject.DestroyImmediate(cardGO);
            return prefab;
        }

        private static void BuildScene(TMP_FontAsset fontAsset, GameObject cardPrefab)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            GameObject camGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            Camera cam = camGO.GetComponent<Camera>();
            cam.tag = "MainCamera";
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.10f, 0.08f, 0.05f, 1f);
            camGO.transform.position = new Vector3(0, 0, -10);

            // Event System
            GameObject eventSystemGO = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

            // Canvas
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

            // Background Image
            GameObject bgGO = new GameObject("BackgroundImage", typeof(RectTransform), typeof(Image));
            bgGO.transform.SetParent(canvasGO.transform, false);
            RectTransform bgRt = bgGO.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bgGO.GetComponent<Image>();
            bgImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "bg_city_blurred.png");
            bgImg.color = new Color(0.95f, 0.95f, 0.95f, 1f);

            // BozorWindow Root
            GameObject windowRoot = new GameObject("BozorWindow", typeof(RectTransform), typeof(BozorShopController));
            windowRoot.transform.SetParent(canvasGO.transform, false);
            RectTransform windowRt = windowRoot.GetComponent<RectTransform>();
            windowRt.anchorMin = new Vector2(0.5f, 0.5f);
            windowRt.anchorMax = new Vector2(0.5f, 0.5f);
            windowRt.pivot = new Vector2(0.5f, 0.5f);
            windowRt.anchoredPosition = new Vector2(0, -10);
            windowRt.sizeDelta = new Vector2(1004, 610);

            BozorShopController controller = windowRoot.GetComponent<BozorShopController>();

            // Frame Main Wood
            GameObject frameGO = new GameObject("FrameMainWood", typeof(RectTransform), typeof(Image));
            frameGO.transform.SetParent(windowRoot.transform, false);
            RectTransform frameRt = frameGO.GetComponent<RectTransform>();
            frameRt.anchorMin = Vector2.zero;
            frameRt.anchorMax = Vector2.one;
            frameRt.sizeDelta = Vector2.zero;
            Image frameImg = frameGO.GetComponent<Image>();
            frameImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "frame_main_wood.png");
            frameImg.type = Image.Type.Simple;

            // Header - Badge Bozor
            GameObject headerBadge = new GameObject("HeaderBadge_BOZOR", typeof(RectTransform), typeof(Image));
            headerBadge.transform.SetParent(windowRoot.transform, false);
            RectTransform badgeRt = headerBadge.GetComponent<RectTransform>();
            badgeRt.anchorMin = new Vector2(0.5f, 1f);
            badgeRt.anchorMax = new Vector2(0.5f, 1f);
            badgeRt.pivot = new Vector2(0.5f, 0.5f);
            badgeRt.anchoredPosition = new Vector2(0, 16);
            badgeRt.sizeDelta = new Vector2(364, 77);
            Image badgeImg = headerBadge.GetComponent<Image>();
            badgeImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "header_badge_bozor.png");
            badgeImg.type = Image.Type.Simple;

            // Header - Currency Pill
            GameObject currPill = new GameObject("CurrencyPill", typeof(RectTransform), typeof(Image));
            currPill.transform.SetParent(windowRoot.transform, false);
            RectTransform currRt = currPill.GetComponent<RectTransform>();
            currRt.anchorMin = new Vector2(1f, 1f);
            currRt.anchorMax = new Vector2(1f, 1f);
            currRt.pivot = new Vector2(1f, 0.5f);
            currRt.anchoredPosition = new Vector2(-70, 16);
            currRt.sizeDelta = new Vector2(144, 32);
            Image currImg = currPill.GetComponent<Image>();
            currImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "currency_pill_bg.png");
            currImg.type = Image.Type.Simple;

            // Currency Text
            GameObject currTextGO = new GameObject("GoldText", typeof(RectTransform), typeof(TextMeshProUGUI));
            currTextGO.transform.SetParent(currPill.transform, false);
            RectTransform currTextRt = currTextGO.GetComponent<RectTransform>();
            currTextRt.anchorMin = Vector2.zero;
            currTextRt.anchorMax = Vector2.one;
            currTextRt.anchoredPosition = new Vector2(5, 0);
            currTextRt.sizeDelta = new Vector2(-35, 0);
            TextMeshProUGUI currTMP = currTextGO.GetComponent<TextMeshProUGUI>();
            currTMP.font = fontAsset;
            currTMP.fontSize = 15;
            currTMP.fontStyle = FontStyles.Bold;
            currTMP.alignment = TextAlignmentOptions.Center;
            currTMP.color = Color.white;
            currTMP.text = "2 350 000";

            // Currency Add Button
            GameObject addBtnGO = new GameObject("AddGoldButton", typeof(RectTransform), typeof(Button));
            addBtnGO.transform.SetParent(currPill.transform, false);
            RectTransform addBtnRt = addBtnGO.GetComponent<RectTransform>();
            addBtnRt.anchorMin = new Vector2(1f, 0.5f);
            addBtnRt.anchorMax = new Vector2(1f, 0.5f);
            addBtnRt.pivot = new Vector2(1f, 0.5f);
            addBtnRt.anchoredPosition = new Vector2(-2, 0);
            addBtnRt.sizeDelta = new Vector2(26, 26);
            Button addGoldBtn = addBtnGO.GetComponent<Button>();

            // Header - Close Button
            GameObject closeBtnGO = new GameObject("CloseButton", typeof(RectTransform), typeof(Image), typeof(Button));
            closeBtnGO.transform.SetParent(windowRoot.transform, false);
            RectTransform closeRt = closeBtnGO.GetComponent<RectTransform>();
            closeRt.anchorMin = new Vector2(1f, 1f);
            closeRt.anchorMax = new Vector2(1f, 1f);
            closeRt.pivot = new Vector2(1f, 0.5f);
            closeRt.anchoredPosition = new Vector2(-12, 24);
            closeRt.sizeDelta = new Vector2(43, 44);
            Image closeImg = closeBtnGO.GetComponent<Image>();
            closeImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "btn_close_red.png");
            closeImg.type = Image.Type.Simple;
            Button closeBtn = closeBtnGO.GetComponent<Button>();

            // Single Row Tab Bar (Top)
            GameObject tabBarGO = new GameObject("TabBar", typeof(RectTransform));
            tabBarGO.transform.SetParent(windowRoot.transform, false);
            RectTransform tabRt = tabBarGO.GetComponent<RectTransform>();
            tabRt.anchorMin = new Vector2(0.5f, 1f);
            tabRt.anchorMax = new Vector2(0.5f, 1f);
            tabRt.pivot = new Vector2(0.5f, 1f);
            tabRt.anchoredPosition = new Vector2(0, -22);
            tabRt.sizeDelta = new Vector2(960, 43);

            Sprite tabActiveBg = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "tab_active_bg.png");
            Sprite tabInactiveBg = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "tab_inactive_bg.png");

            var tabList = new System.Collections.Generic.List<ShopTabUI>();
            tabList.Add(CreateSolidTab(tabBarGO, "Tab_Production", "ISHLAB CHIQARISH", BuildingCategory.Production, -360f, 236, 43, 0, tabActiveBg, tabInactiveBg, fontAsset, true));
            tabList.Add(CreateSolidTab(tabBarGO, "Tab_Agriculture", "QISHLOQ XO'JALIGI", BuildingCategory.Agriculture, -120f, 236, 38, -3, tabActiveBg, tabInactiveBg, fontAsset, false));
            tabList.Add(CreateSolidTab(tabBarGO, "Tab_Trade", "SAVDO", BuildingCategory.Trade, 120f, 236, 38, -3, tabActiveBg, tabInactiveBg, fontAsset, false));
            tabList.Add(CreateSolidTab(tabBarGO, "Tab_Decorations", "BEZAKLAR", BuildingCategory.Decorations, 360f, 236, 38, -3, tabActiveBg, tabInactiveBg, fontAsset, false));

            // Continuous Horizontal ScrollRect
            GameObject scrollRectGO = new GameObject("CardsScrollView", typeof(RectTransform), typeof(ScrollRect));
            scrollRectGO.transform.SetParent(windowRoot.transform, false);
            RectTransform scrollRt = scrollRectGO.GetComponent<RectTransform>();
            scrollRt.anchorMin = new Vector2(0.5f, 1f);
            scrollRt.anchorMax = new Vector2(0.5f, 1f);
            scrollRt.pivot = new Vector2(0.5f, 1f);
            scrollRt.anchoredPosition = new Vector2(0, -66);
            scrollRt.sizeDelta = new Vector2(964, 495);

            ScrollRect scrollRect = scrollRectGO.GetComponent<ScrollRect>();
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.elasticity = 0.1f;
            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.135f;
            scrollRect.scrollSensitivity = 25f;

            // Viewport
            GameObject viewportGO = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
            viewportGO.transform.SetParent(scrollRectGO.transform, false);
            RectTransform viewRt = viewportGO.GetComponent<RectTransform>();
            viewRt.anchorMin = Vector2.zero;
            viewRt.anchorMax = Vector2.one;
            viewRt.pivot = new Vector2(0, 1);
            viewRt.sizeDelta = Vector2.zero;

            // Content Container
            GameObject contentGO = new GameObject("Content", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
            contentGO.transform.SetParent(viewportGO.transform, false);
            RectTransform contentRt = contentGO.GetComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 0);
            contentRt.anchorMax = new Vector2(0, 1);
            contentRt.pivot = new Vector2(0, 0.5f);
            contentRt.anchoredPosition = Vector2.zero;
            contentRt.sizeDelta = new Vector2(0, 0);

            HorizontalLayoutGroup hlg = contentGO.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 10;
            hlg.padding = new RectOffset(10, 10, 5, 5);
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            ContentSizeFitter csf = contentGO.GetComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            scrollRect.viewport = viewRt;
            scrollRect.content = contentRt;

            // Wire Controller
            var so = new SerializedObject(controller);
            so.FindProperty("goldText").objectReferenceValue = currTMP;
            so.FindProperty("addGoldButton").objectReferenceValue = addGoldBtn;
            so.FindProperty("windowRoot").objectReferenceValue = windowRoot;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.FindProperty("scrollRect").objectReferenceValue = scrollRect;
            so.FindProperty("contentTransform").objectReferenceValue = contentRt;
            so.FindProperty("viewportTransform").objectReferenceValue = viewRt;
            so.FindProperty("cardPrefab").objectReferenceValue = cardPrefab.GetComponent<ShopCardUI>();

            SerializedProperty tabsProp = so.FindProperty("tabs");
            tabsProp.ClearArray();
            for (int i = 0; i < tabList.Count; i++)
            {
                tabsProp.InsertArrayElementAtIndex(i);
                tabsProp.GetArrayElementAtIndex(i).objectReferenceValue = tabList[i];
            }

            SerializedProperty buildingsProp = so.FindProperty("allBuildings");
            buildingsProp.ClearArray();
            string[] buildingPaths = {
                DataPath + "SO_YogochKesishxona.asset",
                DataPath + "SO_QishloqUyi.asset",
                DataPath + "SO_PaxtaOmbori.asset",
                DataPath + "SO_BugdoyDalasi.asset",
                DataPath + "SO_SomonOmbori.asset",
                DataPath + "SO_MatoDokoni.asset",
                DataPath + "SO_OziqOvqat.asset",
                DataPath + "SO_QadimiyFavvora.asset",
                DataPath + "SO_SharqHovlisi.asset"
            };

            for (int i = 0; i < buildingPaths.Length; i++)
            {
                buildingsProp.InsertArrayElementAtIndex(i);
                buildingsProp.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<BuildingDataSO>(buildingPaths[i]);
            }

            so.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        private static ShopTabUI CreateSolidTab(GameObject parent, string name, string label, BuildingCategory cat, float posX, float width, float height, float posY, Sprite activeBg, Sprite inactiveBg, TMP_FontAsset font, bool isInitialActive)
        {
            GameObject tabGO = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(ShopTabUI));
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

            GameObject textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(tabGO.transform, false);
            RectTransform textRt = textGO.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.anchoredPosition = new Vector2(0, isInitialActive ? 2 : 0);
            textRt.sizeDelta = Vector2.zero;

            TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.fontSize = 13;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = isInitialActive ? new Color(0.2f, 0.15f, 0.1f, 1f) : new Color(0.25f, 0.18f, 0.12f, 0.9f);
            tmp.text = label;

            ShopTabUI tabUI = tabGO.GetComponent<ShopTabUI>();
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
    }
}
