using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BozorShop
{
    public class BozorShopController : MonoBehaviour
    {
        [Header("Player Economy")]
        [SerializeField] private int playerGold = 2350000;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private Button addGoldButton;

        [Header("Window & Controls")]
        [SerializeField] private GameObject windowRoot;
        [SerializeField] private Button closeButton;

        [Header("Single Row Tabs")]
        [SerializeField] private List<ShopTabUI> tabs = new List<ShopTabUI>();

        [Header("Scroll & Content")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform contentTransform;
        [SerializeField] private RectTransform viewportTransform;
        [SerializeField] private ShopCardUI cardPrefab;

        [Header("Buildings Data (All Categories)")]
        [SerializeField] private List<BuildingDataSO> allBuildings = new List<BuildingDataSO>();

        private List<ShopCardUI> activeCardViews = new List<ShopCardUI>();
        private Dictionary<BuildingCategory, float> categoryNormalizedPositions = new Dictionary<BuildingCategory, float>();
        private Dictionary<BuildingCategory, RectTransform> categoryFirstCard = new Dictionary<BuildingCategory, RectTransform>();

        private BuildingCategory currentActiveCategory = BuildingCategory.Production;
        private Coroutine smoothScrollCoroutine;
        private bool isAutoScrolling = false;

        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(CloseShop);

            if (addGoldButton != null)
                addGoldButton.onClick.AddListener(() => AddGold(500000));

            // Initialize tabs
            foreach (var tab in tabs)
            {
                if (tab != null)
                {
                    tab.Initialize(OnTabClicked);
                }
            }

            if (scrollRect != null)
            {
                scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            }
        }

        private void Start()
        {
            UpdateGoldUI();
            BuildContinuousCardList();
            StartCoroutine(CalculateCategoryPositionsCoroutine());
        }

        public void AddGold(int amount)
        {
            playerGold += amount;
            UpdateGoldUI();
            RefreshAffordability();
        }

        private void BuildContinuousCardList()
        {
            if (contentTransform != null)
            {
                for (int i = contentTransform.childCount - 1; i >= 0; i--)
                {
                    Destroy(contentTransform.GetChild(i).gameObject);
                }
            }
            activeCardViews.Clear();
            categoryFirstCard.Clear();

            // Order categories consecutively
            BuildingCategory[] order = new[] {
                BuildingCategory.Production,
                BuildingCategory.Agriculture,
                BuildingCategory.Trade,
                BuildingCategory.Decorations
            };

            foreach (var cat in order)
            {
                bool isFirstOfCategory = true;
                foreach (var data in allBuildings)
                {
                    if (data == null || data.category != cat) continue;

                    ShopCardUI cardInstance = Instantiate(cardPrefab, contentTransform);
                    cardInstance.Setup(data, OnBuyBuilding);
                    cardInstance.UpdateAffordability(playerGold);
                    activeCardViews.Add(cardInstance);

                    RectTransform cardRt = cardInstance.GetComponent<RectTransform>();
                    if (isFirstOfCategory)
                    {
                        categoryFirstCard[cat] = cardRt;
                        isFirstOfCategory = false;
                    }
                }
            }
        }

        private IEnumerator CalculateCategoryPositionsCoroutine()
        {
            // Wait for end of frame / canvas layout recalculation
            yield return new WaitForEndOfFrame();
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);

            CalculateCategoryPositions();
            SetTabVisuals(BuildingCategory.Production);
        }

        private void CalculateCategoryPositions()
        {
            categoryNormalizedPositions.Clear();
            if (contentTransform == null || viewportTransform == null) return;

            float contentWidth = contentTransform.rect.width;
            float viewportWidth = viewportTransform.rect.width;
            float scrollableRange = contentWidth - viewportWidth;

            if (scrollableRange <= 0.01f)
            {
                foreach (var cat in categoryFirstCard.Keys)
                {
                    categoryNormalizedPositions[cat] = 0f;
                }
                return;
            }

            foreach (var pair in categoryFirstCard)
            {
                BuildingCategory cat = pair.Key;
                RectTransform firstCard = pair.Value;
                if (firstCard != null)
                {
                    // Card position relative to content left edge
                    float cardLeft = firstCard.anchoredPosition.x - (firstCard.rect.width * firstCard.pivot.x);
                    if (cardLeft < 0) cardLeft = 0;

                    float normPos = Mathf.Clamp01(cardLeft / scrollableRange);
                    categoryNormalizedPositions[cat] = normPos;
                }
            }
        }

        public void OnTabClicked(BuildingCategory category)
        {
            if (categoryNormalizedPositions.TryGetValue(category, out float targetNormPos))
            {
                if (smoothScrollCoroutine != null)
                    StopCoroutine(smoothScrollCoroutine);

                smoothScrollCoroutine = StartCoroutine(SmoothScrollTo(targetNormPos, category));
            }
        }

        private IEnumerator SmoothScrollTo(float targetNormPos, BuildingCategory targetCategory)
        {
            isAutoScrolling = true;
            SetTabVisuals(targetCategory);

            float startNormPos = scrollRect.horizontalNormalizedPosition;
            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // Ease out cubic
                float ease = 1f - Mathf.Pow(1f - t, 3f);
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startNormPos, targetNormPos, ease);
                yield return null;
            }

            scrollRect.horizontalNormalizedPosition = targetNormPos;
            isAutoScrolling = false;
            currentActiveCategory = targetCategory;
            SetTabVisuals(targetCategory);
        }

        private void OnScrollValueChanged(Vector2 scrollPos)
        {
            if (isAutoScrolling) return;

            // Determine visible category based on scroll position
            float currentNormPos = scrollRect.horizontalNormalizedPosition;
            BuildingCategory closestCategory = currentActiveCategory;
            float minDistance = float.MaxValue;

            foreach (var pair in categoryNormalizedPositions)
            {
                float dist = Mathf.Abs(currentNormPos - pair.Value);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestCategory = pair.Key;
                }
            }

            if (closestCategory != currentActiveCategory)
            {
                currentActiveCategory = closestCategory;
                SetTabVisuals(closestCategory);
            }
        }

        private void SetTabVisuals(BuildingCategory activeCat)
        {
            foreach (var tab in tabs)
            {
                if (tab != null)
                {
                    tab.SetActive(tab.Category == activeCat);
                }
            }
        }

        private void OnBuyBuilding(BuildingDataSO data)
        {
            if (playerGold >= data.priceGold)
            {
                playerGold -= data.priceGold;
                UpdateGoldUI();
                RefreshAffordability();
                Debug.Log($"<color=green>[Bozor Shop]</color> Purchased: <b>{data.displayName}</b> for {data.priceGold:N0} gold. Remaining: {playerGold:N0}");
            }
            else
            {
                Debug.LogWarning($"<color=red>[Bozor Shop]</color> Not enough gold to buy {data.displayName}!");
            }
        }

        private void RefreshAffordability()
        {
            foreach (var card in activeCardViews)
            {
                if (card != null)
                    card.UpdateAffordability(playerGold);
            }
        }

        private void UpdateGoldUI()
        {
            if (goldText != null)
            {
                goldText.text = $"{playerGold:N0}".Replace(",", " ");
            }
        }

        public void OpenShop()
        {
            if (windowRoot != null) windowRoot.SetActive(true);
        }

        public void CloseShop()
        {
            if (windowRoot != null) windowRoot.SetActive(false);
            Debug.Log("[Bozor Shop] Closed window");
        }
    }
}
