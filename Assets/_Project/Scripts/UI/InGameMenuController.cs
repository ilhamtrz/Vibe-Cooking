using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VibeCooking
{
    public class InGameMenuController : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button menuToggleButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Panels")]
        [SerializeField] private GameObject pauseModalRoot;
        [SerializeField] private SettingsPanelUI settingsPanel;

        [Header("Scene Navigation")]
        [SerializeField] private string mainMenuSceneName = "MainMenuScene";

        private void Start()
        {
            if (menuToggleButton != null)
                menuToggleButton.onClick.AddListener(OpenPauseMenu);

            if (resumeButton != null)
                resumeButton.onClick.AddListener(ClosePauseMenu);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OpenSettings);

            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);

            if (pauseModalRoot != null)
                pauseModalRoot.SetActive(false);

            if (settingsPanel != null)
                settingsPanel.ClosePanel();
        }

        public void OpenPauseMenu()
        {
            PlayClickSound();
            if (pauseModalRoot != null)
                pauseModalRoot.SetActive(true);
        }

        public void ClosePauseMenu()
        {
            PlayClickSound();
            if (pauseModalRoot != null)
                pauseModalRoot.SetActive(false);
            if (settingsPanel != null)
                settingsPanel.ClosePanel();
        }

        public void OpenSettings()
        {
            PlayClickSound();
            if (settingsPanel != null)
            {
                settingsPanel.OpenPanel();
            }
        }

        public void ReturnToMainMenu()
        {
            PlayClickSound();
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.SaveSettings();

            Debug.Log("[InGameMenuController] Returning to MainMenuScene...");
            SceneManager.LoadScene(mainMenuSceneName);
        }

        private void PlayClickSound()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();
        }

        public void SetReferences(Button toggleBtn, Button resumeBtn, Button settingsBtn, Button menuBtn, GameObject modalRoot, SettingsPanelUI settings)
        {
            menuToggleButton = toggleBtn;
            resumeButton = resumeBtn;
            settingsButton = settingsBtn;
            mainMenuButton = menuBtn;
            pauseModalRoot = modalRoot;
            settingsPanel = settings;
        }
    }
}
