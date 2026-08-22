using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using IndependenceGame.Settings;

namespace IndependenceGame.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Main Menu Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;

        [Header("Overlays & Windows")]
        [SerializeField] private SettingsUIController settingsOverlay;

        [Header("Scene Configuration")]
        [SerializeField] private string gameplaySceneName = "SampleScene";

        private void Awake()
        {
            if (startButton != null)
                startButton.onClick.AddListener(OnStartGame);

            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueGame);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnOpenSettings);

            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitGame);
        }

        public void OnStartGame()
        {
            Debug.Log("[MainMenu] Starting new game...");
            if (Application.CanStreamedLevelBeLoaded(gameplaySceneName))
            {
                SceneManager.LoadScene(gameplaySceneName);
            }
            else
            {
                Debug.LogWarning($"[MainMenu] Scene '{gameplaySceneName}' is not in Build Settings or cannot be loaded.");
            }
        }

        public void OnContinueGame()
        {
            Debug.Log("[MainMenu] Continuing game...");
            if (Application.CanStreamedLevelBeLoaded(gameplaySceneName))
            {
                SceneManager.LoadScene(gameplaySceneName);
            }
        }

        public void OnOpenSettings()
        {
            if (settingsOverlay != null)
            {
                settingsOverlay.OpenWindow();
            }
            else
            {
                // Fallback to loading SettingsScene directly
                if (Application.CanStreamedLevelBeLoaded("SettingsScene"))
                {
                    SceneManager.LoadScene("SettingsScene");
                }
            }
        }

        public void OnExitGame()
        {
            Debug.Log("[MainMenu] Exiting game...");
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
