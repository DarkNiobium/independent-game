using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BozorShop
{
    public class ShopCardUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image previewImage;
        [SerializeField] private Image resourceIcon;
        [SerializeField] private TextMeshProUGUI productionText;
        [SerializeField] private Image capacityIcon;
        [SerializeField] private TextMeshProUGUI capacityText;
        [SerializeField] private Button buyButton;
        [SerializeField] private TextMeshProUGUI priceText;

        public BuildingDataSO CurrentData { get; private set; }
        private Action<BuildingDataSO> onBuyClicked;

        public void Setup(BuildingDataSO data, Action<BuildingDataSO> buyCallback)
        {
            CurrentData = data;
            onBuyClicked = buyCallback;

            if (titleText != null)
                titleText.text = data.displayName;

            if (previewImage != null && data.buildingPreview != null)
                previewImage.sprite = data.buildingPreview;

            if (resourceIcon != null && data.resourceIcon != null)
                resourceIcon.sprite = data.resourceIcon;

            if (productionText != null)
                productionText.text = $"+{data.productionRate} {data.rateUnit}";

            if (capacityIcon != null && data.capacityIcon != null)
                capacityIcon.sprite = data.capacityIcon;

            if (capacityText != null)
                capacityText.text = $"Sig'im: {data.capacity}";

            if (priceText != null)
                priceText.text = $"{data.priceGold:N0}".Replace(",", " ");

            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(HandleBuyClicked);
            }
        }

        public void UpdateAffordability(int currentGold)
        {
            if (buyButton != null && CurrentData != null)
            {
                bool canAfford = currentGold >= CurrentData.priceGold;
                buyButton.interactable = canAfford;
            }
        }

        private void HandleBuyClicked()
        {
            // Trigger purchase callback
            onBuyClicked?.Invoke(CurrentData);
        }
    }
}
