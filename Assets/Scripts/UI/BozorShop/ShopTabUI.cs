using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BozorShop
{
    public class ShopTabUI : MonoBehaviour
    {
        [SerializeField] private BuildingCategory category;
        [SerializeField] private Image tabBackground;
        [SerializeField] private TextMeshProUGUI tabLabel;
        [SerializeField] private Button button;

        [Header("Visual States")]
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;
        [SerializeField] private Color activeTextColor = new Color(0.18f, 0.12f, 0.08f, 1f);
        [SerializeField] private Color inactiveTextColor = new Color(0.24f, 0.17f, 0.10f, 0.9f);

        private Action<BuildingCategory> onTabSelected;

        public BuildingCategory Category => category;

        public void Initialize(Action<BuildingCategory> callback)
        {
            onTabSelected = callback;
            if (button == null)
                button = GetComponent<Button>();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onTabSelected?.Invoke(category));
            }
        }

        public void SetActive(bool isActive)
        {
            if (tabBackground != null)
            {
                if (activeSprite != null && inactiveSprite != null)
                    tabBackground.sprite = isActive ? activeSprite : inactiveSprite;

                RectTransform rt = tabBackground.rectTransform;
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, isActive ? 43 : 38);
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, isActive ? 0 : -3);
            }

            if (tabLabel != null)
            {
                tabLabel.color = isActive ? activeTextColor : inactiveTextColor;
            }
        }
    }
}
