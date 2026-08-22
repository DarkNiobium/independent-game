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

        [Header("Tabs")]
        [SerializeField] private List<ShopTabUI> tabs = new List<ShopTabUI>();

        [Header("Buildings Data")]
        [SerializeField] private List<BuildingDataSO> allBuildings = new List<BuildingDataSO>();
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private ShopCardUI cardPrefab;

        private List<ShopCardUI> activeCardViews = new List<ShopCardUI>();
        private BuildingCategory currentCategory = BuildingCategory.All;

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
                    tab.Initialize(OnTabChanged);
                }
            }
        }

        private void Start()
        {
            UpdateGoldUI();
            SelectTab(BuildingCategory.All);
        }

        public void AddGold(int amount)
        {
            playerGold += amount;
            UpdateGoldUI();
            RefreshAffordability();
        }

        public void OnTabChanged(BuildingCategory category)
        {
            SelectTab(category);
        }

        public void SelectTab(BuildingCategory category)
        {
            currentCategory = category;

            // Update Tab Visuals
            foreach (var tab in tabs)
            {
                if (tab != null)
                {
                    tab.SetActive(tab.Category == category);
                }
            }

            RebuildCards();
        }

        private void RebuildCards()
        {
            // Destroy ALL existing children in cardsContainer
            if (cardsContainer != null)
            {
                for (int i = cardsContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(cardsContainer.GetChild(i).gameObject);
                }
            }
            activeCardViews.Clear();

            // Populate cards
            foreach (var data in allBuildings)
            {
                if (data == null) continue;

                if (currentCategory == BuildingCategory.All || data.category == currentCategory)
                {
                    ShopCardUI cardInstance = Instantiate(cardPrefab, cardsContainer);
                    cardInstance.Setup(data, OnBuyBuilding);
                    cardInstance.UpdateAffordability(playerGold);
                    activeCardViews.Add(cardInstance);
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
