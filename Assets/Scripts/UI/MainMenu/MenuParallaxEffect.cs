using UnityEngine;

namespace IndependenceGame.MainMenu
{
    public class MenuParallaxEffect : MonoBehaviour
    {
        [Header("Parallax Targets & Strength")]
        [Tooltip("Background image transform (moves slightly opposite to mouse)")]
        [SerializeField] private RectTransform backgroundTransform;
        [SerializeField] private float backgroundStrength = 18f;

        [Tooltip("Title block transform (subtle forward parallax)")]
        [SerializeField] private RectTransform titleTransform;
        [SerializeField] private float titleStrength = 8f;

        [Tooltip("Buttons stack transform (subtle float)")]
        [SerializeField] private RectTransform buttonsTransform;
        [SerializeField] private float buttonsStrength = 12f;

        [Header("Smoothing")]
        [SerializeField] private float smoothSpeed = 5f;

        private Vector2 bgInitialPos;
        private Vector2 titleInitialPos;
        private Vector2 buttonsInitialPos;

        private Vector2 currentOffset = Vector2.zero;

        private void Start()
        {
            if (backgroundTransform != null) bgInitialPos = backgroundTransform.anchoredPosition;
            if (titleTransform != null) titleInitialPos = titleTransform.anchoredPosition;
            if (buttonsTransform != null) buttonsInitialPos = buttonsTransform.anchoredPosition;
        }

        private void Update()
        {
            // Calculate normalized mouse position from screen center (-0.5 to 0.5)
            float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
            float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;

            Vector2 targetOffset = new Vector2(mouseX, mouseY);
            currentOffset = Vector2.Lerp(currentOffset, targetOffset, 1f - Mathf.Exp(-smoothSpeed * Time.unscaledDeltaTime));

            // Apply parallax shifts
            if (backgroundTransform != null)
            {
                backgroundTransform.anchoredPosition = bgInitialPos - currentOffset * backgroundStrength;
            }

            if (titleTransform != null)
            {
                titleTransform.anchoredPosition = titleInitialPos + currentOffset * titleStrength;
            }

            if (buttonsTransform != null)
            {
                buttonsTransform.anchoredPosition = buttonsInitialPos + currentOffset * buttonsStrength;
            }
        }
    }
}
