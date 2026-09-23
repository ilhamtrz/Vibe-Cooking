using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VibeCooking
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Scene Navigation")]
        [SerializeField] private string gameSceneName = "GameScene";

        [Header("UI References")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private SettingsPanelUI settingsPanel;

        [Header("Audio (Optional)")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip buttonClickClip;

        private void Start()
        {
            if (startButton != null)
                startButton.onClick.AddListener(OnStartClicked);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);

            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitClicked);

            if (settingsPanel != null)
                settingsPanel.ClosePanel();
        }

        public void OnStartClicked()
        {
            PlayButtonClickSound();
            Debug.Log("[MainMenuController] Loading GameScene...");
            SceneManager.LoadScene(gameSceneName);
        }

        public void OnSettingsClicked()
        {
            PlayButtonClickSound();
            if (settingsPanel != null)
            {
                settingsPanel.OpenPanel();
            }
        }

        public void OnExitClicked()
        {
            PlayButtonClickSound();
            Debug.Log("[MainMenuController] Exiting game...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void PlayButtonClickSound()
        {
            if (audioSource != null && buttonClickClip != null)
            {
                audioSource.PlayOneShot(buttonClickClip);
            }
        }
    }
}
