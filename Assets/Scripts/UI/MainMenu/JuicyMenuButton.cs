using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace IndependenceGame.MainMenu
{
    [RequireComponent(typeof(Button))]
    public class JuicyMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Slide & Motion")]
        [SerializeField] private float hoverSlideDistance = 14f;
        [SerializeField] private float slideSpeed = 15f;
        [SerializeField] private float hoverScale = 1.03f;
        [SerializeField] private float pressScale = 0.94f;
        [SerializeField] private float scaleSpeed = 18f;

        [Header("Typography & Glow")]
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private float normalTracking = 3f;
        [SerializeField] private float hoverTracking = 7f;
        [SerializeField] private Color normalTextColor = new Color(0.94f, 0.88f, 0.74f, 0.9f);
        [SerializeField] private Color hoverTextColor = new Color(1f, 0.98f, 0.88f, 1f);
        [SerializeField] private Color normalOutlineColor = new Color(0.85f, 0.70f, 0.35f, 0.35f);
        [SerializeField] private Color hoverOutlineColor = new Color(1f, 0.88f, 0.50f, 0.95f);

        private RectTransform rectTransform;
        private Outline buttonOutline;
        private Image buttonImage;
        private Vector3 baseLocalPosition;
        private Vector3 targetLocalPosition;
        private Vector3 targetScale = Vector3.one;
        private Color targetTextColor;
        private Color targetOutlineColor;
        private float targetTracking;
        private bool isHovered = false;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            buttonOutline = GetComponent<Outline>();
            buttonImage = GetComponent<Image>();
            if (buttonText == null)
                buttonText = GetComponentInChildren<TextMeshProUGUI>();

            baseLocalPosition = transform.localPosition;
            targetLocalPosition = baseLocalPosition;
            targetTextColor = normalTextColor;
            targetOutlineColor = normalOutlineColor;
            targetTracking = normalTracking;

            if (buttonText != null)
            {
                buttonText.color = normalTextColor;
                buttonText.characterSpacing = normalTracking;
            }

            if (buttonOutline != null)
            {
                buttonOutline.effectColor = normalOutlineColor;
            }
        }

        private void OnEnable()
        {
            baseLocalPosition = transform.localPosition;
            targetLocalPosition = baseLocalPosition;
            targetScale = Vector3.one;
        }

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;

            // Smooth position dampening on localPosition (independent of VerticalLayoutGroup)
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPosition, 1f - Mathf.Exp(-slideSpeed * dt));

            // Smooth scale spring
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 1f - Mathf.Exp(-scaleSpeed * dt));

            // Smooth typography & colors
            if (buttonText != null)
            {
                buttonText.color = Color.Lerp(buttonText.color, targetTextColor, 1f - Mathf.Exp(-12f * dt));
                buttonText.characterSpacing = Mathf.Lerp(buttonText.characterSpacing, targetTracking, 1f - Mathf.Exp(-12f * dt));
            }

            if (buttonOutline != null)
            {
                buttonOutline.effectColor = Color.Lerp(buttonOutline.effectColor, targetOutlineColor, 1f - Mathf.Exp(-12f * dt));
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            targetLocalPosition = baseLocalPosition + new Vector3(hoverSlideDistance, 0, 0);
            targetScale = Vector3.one * hoverScale;
            targetTextColor = hoverTextColor;
            targetOutlineColor = hoverOutlineColor;
            targetTracking = hoverTracking;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            targetLocalPosition = baseLocalPosition;
            targetScale = Vector3.one;
            targetTextColor = normalTextColor;
            targetOutlineColor = normalOutlineColor;
            targetTracking = normalTracking;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            targetScale = Vector3.one * pressScale;
            targetLocalPosition = baseLocalPosition + new Vector3(hoverSlideDistance * 0.5f, 0, 0);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isHovered)
            {
                targetScale = Vector3.one * hoverScale;
                targetLocalPosition = baseLocalPosition + new Vector3(hoverSlideDistance, 0, 0);
            }
            else
            {
                targetScale = Vector3.one;
                targetLocalPosition = baseLocalPosition;
            }
        }
    }
}
