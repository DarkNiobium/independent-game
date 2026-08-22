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
            Debug.Log("<color=green>[Bozor Shop]</color> Build completed successfully!");
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
            CreateSO("SO_ChorvaUyi", "CHORVA UYI", BuildingCategory.Production, "building_chorva_uyi", "icon_sheep", 15, "/ soat", 60, 500000);
            CreateSO("SO_PaxtaDalasi", "PAXTA DALASI", BuildingCategory.Agriculture, "building_paxta_dalasi", "icon_cotton", 20, "/ soat", 80, 500000);
            CreateSO("SO_DoppiDokoni", "DO'PPI DO'KONI", BuildingCategory.Trade, "building_doppi_dokoni", "icon_doppi", 25, "/ soat", 40, 500000);
            CreateSO("SO_OzbekKiyimDokoni", "O'ZBEK KIYIM DO'KONI", BuildingCategory.Trade, "building_ozbek_kiyim", "icon_robe", 30, "/ soat", 50, 500000);
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
            rt.sizeDelta = new Vector2(235, 423);

            LayoutElement le = cardGO.GetComponent<LayoutElement>();
            le.minWidth = 235;
            le.minHeight = 423;
            le.preferredWidth = 235;
            le.preferredHeight = 423;
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
            prevRt.anchoredPosition = new Vector2(0, 48);
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
            statsRt.anchoredPosition = new Vector2(0, 68);
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

            // BozorWindow Root (Buildings Section)
            GameObject windowRoot = new GameObject("BozorWindow", typeof(RectTransform), typeof(BozorShopController));
            windowRoot.transform.SetParent(canvasGO.transform, false);
            RectTransform windowRt = windowRoot.GetComponent<RectTransform>();
            windowRt.anchorMin = new Vector2(0.5f, 0.5f);
            windowRt.anchorMax = new Vector2(0.5f, 0.5f);
            windowRt.pivot = new Vector2(0.5f, 0.5f);
            windowRt.anchoredPosition = new Vector2(0, -22);
            windowRt.sizeDelta = new Vector2(1004, 586);

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
            badgeRt.anchoredPosition = new Vector2(0, 8);
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
            currRt.anchoredPosition = new Vector2(-70, 8);
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
            closeRt.anchoredPosition = new Vector2(-12, 18);
            closeRt.sizeDelta = new Vector2(43, 44);
            Image closeImg = closeBtnGO.GetComponent<Image>();
            closeImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "btn_close_red.png");
            closeImg.type = Image.Type.Simple;
            Button closeBtn = closeBtnGO.GetComponent<Button>();

            // Tab Bar
            GameObject tabBarGO = new GameObject("TabBar", typeof(RectTransform));
            tabBarGO.transform.SetParent(windowRoot.transform, false);
            RectTransform tabRt = tabBarGO.GetComponent<RectTransform>();
            tabRt.anchorMin = new Vector2(0.5f, 1f);
            tabRt.anchorMax = new Vector2(0.5f, 1f);
            tabRt.pivot = new Vector2(0.5f, 1f);
            tabRt.anchoredPosition = new Vector2(0, -27);
            tabRt.sizeDelta = new Vector2(960, 43);

            Sprite tabActiveBg = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "tab_active_bg.png");
            Sprite tabInactiveBg = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "tab_inactive_bg.png");

            var tabList = new System.Collections.Generic.List<ShopTabUI>();
            tabList.Add(CreateSolidTab(tabBarGO, "Tab_All", "BARCHASI", BuildingCategory.All, -390.5f, 177, 43, 0, tabActiveBg, tabInactiveBg, fontAsset, true));
            tabList.Add(CreateSolidTab(tabBarGO, "Tab_Production", "ISHLAB CHIQARISH", BuildingCategory.Production, -218f, 164, 38, -3, tabActiveBg, tabInactiveBg, fontAsset, false));
            tabList.Add(CreateSolidTab(tabBarGO, "Tab_Agriculture", "QISHLOQ XO'JALIGI", BuildingCategory.Agriculture, -48.5f, 167, 38, -3, tabActiveBg, tabInactiveBg, fontAsset, false));
            tabList.Add(CreateSolidTab(tabBarGO, "Tab_Trade", "SAVDO", BuildingCategory.Trade, 121.5f, 163, 38, -3, tabActiveBg, tabInactiveBg, fontAsset, false));
            tabList.Add(CreateSolidTab(tabBarGO, "Tab_Decorations", "BEZAKLAR", BuildingCategory.Decorations, 295.5f, 175, 38, -3, tabActiveBg, tabInactiveBg, fontAsset, false));

            // Cards Container
            GameObject cardsContainerGO = new GameObject("CardsContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            cardsContainerGO.transform.SetParent(windowRoot.transform, false);
            RectTransform cardsRt = cardsContainerGO.GetComponent<RectTransform>();
            cardsRt.anchorMin = new Vector2(0.5f, 1f);
            cardsRt.anchorMax = new Vector2(0.5f, 1f);
            cardsRt.pivot = new Vector2(0.5f, 1f);
            cardsRt.anchoredPosition = new Vector2(0, -70);
            cardsRt.sizeDelta = new Vector2(964, 423);

            HorizontalLayoutGroup hlg = cardsContainerGO.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 8;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            // Create Placeholder Panels for other Sections
            GameObject resPanel = CreateSectionPlaceholderPanel(canvasGO, "ResourcesPanel", "RESURSLAR - OMBOR", "Bu bo'limda resurslar va omborlar boshqariladi.", fontAsset);
            GameObject armyPanel = CreateSectionPlaceholderPanel(canvasGO, "ArmyPanel", "ARMIYA - QO'SHINLAR", "Bu bo'limda qo'shinlar va harbiy binolar boshqariladi.", fontAsset);
            GameObject reschPanel = CreateSectionPlaceholderPanel(canvasGO, "ResearchPanel", "TADQIQOT - FAN", "Bu bo'limda yangi texnologiyalar tadqiq qilinadi.", fontAsset);
            GameObject otherPanel = CreateSectionPlaceholderPanel(canvasGO, "OtherPanel", "BOSHQA - SOZLAMALAR", "O'yin sozlamalari va profil ma'lumotlari.", fontAsset);

            resPanel.SetActive(false);
            armyPanel.SetActive(false);
            reschPanel.SetActive(false);
            otherPanel.SetActive(false);

            // Bottom Navigation Bar
            GameObject navBarGO = new GameObject("BottomNavigationBar", typeof(RectTransform), typeof(Image), typeof(BottomNavUI));
            navBarGO.transform.SetParent(canvasGO.transform, false);
            RectTransform navRt = navBarGO.GetComponent<RectTransform>();
            navRt.anchorMin = new Vector2(0.5f, 0f);
            navRt.anchorMax = new Vector2(0.5f, 0f);
            navRt.pivot = new Vector2(0.5f, 0f);
            navRt.anchoredPosition = new Vector2(0, 15);
            navRt.sizeDelta = new Vector2(984, 62);
            Image navImg = navBarGO.GetComponent<Image>();
            navImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "nav_bar_frame_clean.png");
            navImg.type = Image.Type.Simple;

            BottomNavUI bottomNavUI = navBarGO.GetComponent<BottomNavUI>();
            Sprite navActivePill = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "nav_active_pill.png");

            Sprite iconRes = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "icon_nav_resources.png");
            Sprite iconBldg = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "icon_nav_buildings.png");
            Sprite iconArmy = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "icon_nav_army.png");
            Sprite iconResch = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "icon_nav_research.png");
            Sprite iconOther = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "icon_nav_other.png");

            var navItems = new System.Collections.Generic.List<BottomNavItemUI>();
            navItems.Add(CreateBottomNavItem(navBarGO, "Nav_Resources", "RESURSLAR", BottomNavSection.Resources, -325f, 160f, 52f, iconRes, navActivePill, fontAsset, false));
            navItems.Add(CreateBottomNavItem(navBarGO, "Nav_Buildings", "BINOLAR", BottomNavSection.Buildings, -163f, 160f, 52f, iconBldg, navActivePill, fontAsset, true));
            navItems.Add(CreateBottomNavItem(navBarGO, "Nav_Army", "ARMIYA", BottomNavSection.Army, 0f, 160f, 52f, iconArmy, navActivePill, fontAsset, false));
            navItems.Add(CreateBottomNavItem(navBarGO, "Nav_Research", "TADQIQOT", BottomNavSection.Research, 163f, 160f, 52f, iconResch, navActivePill, fontAsset, false));
            navItems.Add(CreateBottomNavItem(navBarGO, "Nav_Other", "BOSHQA", BottomNavSection.Other, 325f, 160f, 52f, iconOther, navActivePill, fontAsset, false));

            // Wire BottomNavUI
            var navSO = new SerializedObject(bottomNavUI);
            navSO.FindProperty("defaultSection").enumValueIndex = (int)BottomNavSection.Buildings;
            navSO.FindProperty("buildingsPanel").objectReferenceValue = windowRoot;
            navSO.FindProperty("resourcesPanel").objectReferenceValue = resPanel;
            navSO.FindProperty("armyPanel").objectReferenceValue = armyPanel;
            navSO.FindProperty("researchPanel").objectReferenceValue = reschPanel;
            navSO.FindProperty("otherPanel").objectReferenceValue = otherPanel;

            SerializedProperty itemsProp = navSO.FindProperty("navItems");
            itemsProp.ClearArray();
            for (int i = 0; i < navItems.Count; i++)
            {
                itemsProp.InsertArrayElementAtIndex(i);
                itemsProp.GetArrayElementAtIndex(i).objectReferenceValue = navItems[i];
            }
            navSO.ApplyModifiedProperties();

            // Wire Controller
            var so = new SerializedObject(controller);
            so.FindProperty("goldText").objectReferenceValue = currTMP;
            so.FindProperty("addGoldButton").objectReferenceValue = addGoldBtn;
            so.FindProperty("windowRoot").objectReferenceValue = windowRoot;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.FindProperty("cardsContainer").objectReferenceValue = cardsContainerGO.transform;
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
                DataPath + "SO_ChorvaUyi.asset",
                DataPath + "SO_PaxtaDalasi.asset",
                DataPath + "SO_DoppiDokoni.asset",
                DataPath + "SO_OzbekKiyimDokoni.asset"
            };

            for (int i = 0; i < buildingPaths.Length; i++)
            {
                buildingsProp.InsertArrayElementAtIndex(i);
                buildingsProp.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<BuildingDataSO>(buildingPaths[i]);
            }

            so.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        private static GameObject CreateSectionPlaceholderPanel(GameObject parent, string name, string title, string description, TMP_FontAsset font)
        {
            GameObject panelGO = new GameObject(name, typeof(RectTransform), typeof(Image));
            panelGO.transform.SetParent(parent.transform, false);

            RectTransform rt = panelGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0, -22);
            rt.sizeDelta = new Vector2(1004, 586);

            Image img = panelGO.GetComponent<Image>();
            img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "frame_main_wood.png");
            img.type = Image.Type.Simple;

            // Content Card
            GameObject cardGO = new GameObject("Card", typeof(RectTransform), typeof(Image));
            cardGO.transform.SetParent(panelGO.transform, false);
            RectTransform cardRt = cardGO.GetComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f);
            cardRt.anchoredPosition = new Vector2(0, -10);
            cardRt.sizeDelta = new Vector2(940, 470);

            Image cardImg = cardGO.GetComponent<Image>();
            cardImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritesPath + "card_bg_parchment.png");
            cardImg.type = Image.Type.Simple;

            // Title Text
            GameObject titleGO = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleGO.transform.SetParent(cardGO.transform, false);
            RectTransform titleRt = titleGO.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0.5f, 1f);
            titleRt.anchorMax = new Vector2(0.5f, 1f);
            titleRt.pivot = new Vector2(0.5f, 1f);
            titleRt.anchoredPosition = new Vector2(0, -35);
            titleRt.sizeDelta = new Vector2(800, 45);

            TextMeshProUGUI titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
            titleTMP.font = font;
            titleTMP.fontSize = 24;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = new Color(0.24f, 0.16f, 0.10f, 1f);
            titleTMP.text = title;

            // Description Text
            GameObject descGO = new GameObject("DescText", typeof(RectTransform), typeof(TextMeshProUGUI));
            descGO.transform.SetParent(cardGO.transform, false);
            RectTransform descRt = descGO.GetComponent<RectTransform>();
            descRt.anchorMin = new Vector2(0.5f, 0.5f);
            descRt.anchorMax = new Vector2(0.5f, 0.5f);
            descRt.pivot = new Vector2(0.5f, 0.5f);
            descRt.anchoredPosition = new Vector2(0, 0);
            descRt.sizeDelta = new Vector2(700, 100);

            TextMeshProUGUI descTMP = descGO.GetComponent<TextMeshProUGUI>();
            descTMP.font = font;
            descTMP.fontSize = 18;
            descTMP.alignment = TextAlignmentOptions.Center;
            descTMP.color = new Color(0.35f, 0.25f, 0.18f, 1f);
            descTMP.text = description + "\n\n<i><color=#5A402A>(Tez orada yangi imkoniyatlar qo'shiladi)</color></i>";

            return panelGO;
        }

        private static BottomNavItemUI CreateBottomNavItem(GameObject parent, string name, string label, BottomNavSection sec, float posX, float width, float height, Sprite icon, Sprite activePill, TMP_FontAsset font, bool isInitialActive)
        {
            GameObject itemGO = new GameObject(name, typeof(RectTransform), typeof(Button), typeof(BottomNavItemUI));
            itemGO.transform.SetParent(parent.transform, false);

            RectTransform rt = itemGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(posX, 0);
            rt.sizeDelta = new Vector2(width, height);

            // Active Badge
            GameObject badgeGO = new GameObject("ActiveBadge", typeof(RectTransform), typeof(Image));
            badgeGO.transform.SetParent(itemGO.transform, false);
            RectTransform badgeRt = badgeGO.GetComponent<RectTransform>();
            badgeRt.anchorMin = Vector2.zero;
            badgeRt.anchorMax = Vector2.one;
            badgeRt.sizeDelta = Vector2.zero;
            Image badgeImg = badgeGO.GetComponent<Image>();
            badgeImg.sprite = activePill;
            badgeImg.type = Image.Type.Simple;
            badgeGO.SetActive(isInitialActive);

            // Icon
            GameObject iconGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconGO.transform.SetParent(itemGO.transform, false);
            RectTransform iconRt = iconGO.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0, 0.5f);
            iconRt.anchorMax = new Vector2(0, 0.5f);
            iconRt.pivot = new Vector2(0, 0.5f);
            iconRt.anchoredPosition = new Vector2(22, 0);
            iconRt.sizeDelta = new Vector2(36, 34);
            Image iconImg = iconGO.GetComponent<Image>();
            iconImg.sprite = icon;
            iconImg.preserveAspect = true;

            // Text
            GameObject textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(itemGO.transform, false);
            RectTransform textRt = textGO.GetComponent<RectTransform>();
            textRt.anchorMin = new Vector2(0, 0);
            textRt.anchorMax = new Vector2(1, 1);
            textRt.pivot = new Vector2(0, 0.5f);
            textRt.anchoredPosition = new Vector2(46, 0);
            textRt.sizeDelta = new Vector2(-50, 0);

            TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.font = font;
            tmp.fontSize = 13;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Left;
            tmp.color = isInitialActive ? Color.white : new Color(0.75f, 0.65f, 0.52f, 1f);
            tmp.text = label;

            BottomNavItemUI itemUI = itemGO.GetComponent<BottomNavItemUI>();
            var so = new SerializedObject(itemUI);
            so.FindProperty("section").enumValueIndex = (int)sec;
            so.FindProperty("activeBadge").objectReferenceValue = badgeImg;
            so.FindProperty("iconImage").objectReferenceValue = iconImg;
            so.FindProperty("labelText").objectReferenceValue = tmp;
            so.FindProperty("button").objectReferenceValue = itemGO.GetComponent<Button>();
            so.FindProperty("activeTextColor").colorValue = Color.white;
            so.FindProperty("inactiveTextColor").colorValue = new Color(0.75f, 0.65f, 0.52f, 1f);
            so.ApplyModifiedProperties();

            return itemUI;
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
            tmp.fontSize = 12;
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
