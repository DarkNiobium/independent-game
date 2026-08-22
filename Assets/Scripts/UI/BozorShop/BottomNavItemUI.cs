using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BozorShop
{
    public enum BottomNavSection
    {
        Resources,
        Buildings,
        Army,
        Research,
        Other
    }

    public class BottomNavItemUI : MonoBehaviour
    {
        [SerializeField] private BottomNavSection section;
        [SerializeField] private Image activeBadge;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private Button button;

        [Header("Colors")]
        [SerializeField] private Color activeTextColor = Color.white;
        [SerializeField] private Color inactiveTextColor = new Color(0.75f, 0.65f, 0.52f, 1f);

        private Action<BottomNavSection> onClickAction;

        public BottomNavSection Section => section;

        public void Initialize(Action<BottomNavSection> callback)
        {
            onClickAction = callback;
            if (button == null)
                button = GetComponent<Button>();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onClickAction?.Invoke(section));
            }
        }

        public void SetActive(bool isActive)
        {
            if (activeBadge != null)
                activeBadge.gameObject.SetActive(isActive);

            if (labelText != null)
                labelText.color = isActive ? activeTextColor : inactiveTextColor;

            if (iconImage != null)
                iconImage.color = isActive ? Color.white : new Color(0.85f, 0.85f, 0.85f, 0.85f);
        }
    }
}
