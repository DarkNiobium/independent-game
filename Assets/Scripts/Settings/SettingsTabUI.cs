using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IndependenceGame.Settings
{
    public enum SettingsTabCategory
    {
        Audio,
        Graphics,
        Gameplay
    }

    public class SettingsTabUI : MonoBehaviour
    {
        [SerializeField] private SettingsTabCategory category;
        [SerializeField] private Image tabBackground;
        [SerializeField] private TextMeshProUGUI tabLabel;
        [SerializeField] private Button button;

        [Header("Visual States")]
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;
        [SerializeField] private Color activeTextColor = new Color(0.20f, 0.15f, 0.10f, 1f);
        [SerializeField] private Color inactiveTextColor = new Color(0.25f, 0.18f, 0.12f, 0.9f);

        public SettingsTabCategory Category => category;

        public void Initialize(Action<SettingsTabCategory> onSelected)
        {
            if (button == null) button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onSelected?.Invoke(category));
            }
        }

        public void SetActive(bool isActive)
        {
            if (tabBackground != null)
            {
                tabBackground.sprite = isActive ? activeSprite : inactiveSprite;
            }

            if (tabLabel != null)
            {
                tabLabel.color = isActive ? activeTextColor : inactiveTextColor;
                RectTransform rt = tabLabel.rectTransform;
                if (rt != null)
                {
                    rt.anchoredPosition = new Vector2(0, isActive ? 2 : 0);
                }
            }
        }
    }
}
