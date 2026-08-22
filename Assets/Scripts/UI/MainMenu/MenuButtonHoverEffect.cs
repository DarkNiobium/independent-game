using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace IndependenceGame.MainMenu
{
    public class MenuButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float transitionSpeed = 12f;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Color normalTextColor = new Color(0.96f, 0.90f, 0.76f, 1f);
        [SerializeField] private Color hoverTextColor = new Color(1f, 0.98f, 0.88f, 1f);

        private Vector3 targetScale = Vector3.one;
        private Color targetColor;

        private void Awake()
        {
            if (buttonText == null)
                buttonText = GetComponentInChildren<TextMeshProUGUI>();
            targetColor = normalTextColor;
            if (buttonText != null)
                buttonText.color = normalTextColor;
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * transitionSpeed);
            if (buttonText != null)
            {
                buttonText.color = Color.Lerp(buttonText.color, targetColor, Time.unscaledDeltaTime * transitionSpeed);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            targetScale = Vector3.one * hoverScale;
            targetColor = hoverTextColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = Vector3.one;
            targetColor = normalTextColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            targetScale = Vector3.one * 0.96f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            targetScale = Vector3.one * hoverScale;
        }
    }
}
