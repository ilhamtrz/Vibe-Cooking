using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VibeCooking
{
    public class SettingsPanelUI : MonoBehaviour
    {
        [Header("Panel Root")]
        [SerializeField] private GameObject panelRoot;

        [Header("Display Controls")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown screenModeDropdown;

        [Header("Audio Sliders")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Value Labels (Optional)")]
        [SerializeField] private TextMeshProUGUI masterValueText;
        [SerializeField] private TextMeshProUGUI bgmValueText;
        [SerializeField] private TextMeshProUGUI sfxValueText;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        private List<Resolution> resolutions = new List<Resolution>();
        private bool isInitializing = false;

        private void Awake()
        {
            if (panelRoot == null)
                panelRoot = gameObject;

            if (closeButton != null)
                closeButton.onClick.AddListener(ClosePanel);
        }

        public void OpenPanel()
        {
            if (panelRoot != null)
                panelRoot.SetActive(true);

            PopulateSettings();
        }

        public void ClosePanel()
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.SaveSettings();

            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        private void PopulateSettings()
        {
            if (SettingsManager.Instance == null) return;

            isInitializing = true;

            // 1. Setup Resolutions
            resolutions = SettingsManager.Instance.GetFilteredResolutions();
            if (resolutionDropdown != null)
            {
                resolutionDropdown.ClearOptions();
                var options = new List<string>();
                for (int i = 0; i < resolutions.Count; i++)
                {
                    options.Add($"{resolutions[i].width} x {resolutions[i].height}");
                }
                resolutionDropdown.AddOptions(options);
                resolutionDropdown.value = SettingsManager.Instance.GetCurrentResolutionIndex();
                resolutionDropdown.RefreshShownValue();

                resolutionDropdown.onValueChanged.RemoveAllListeners();
                resolutionDropdown.onValueChanged.AddListener(OnResolutionSelected);
            }

            // 2. Setup Screen Mode
            if (screenModeDropdown != null)
            {
                screenModeDropdown.ClearOptions();
                screenModeDropdown.AddOptions(new List<string> { "Fullscreen", "Borderless Window", "Windowed" });

                int currentModeIndex = 0;
                switch (SettingsManager.Instance.CurrentScreenMode)
                {
                    case FullScreenMode.ExclusiveFullScreen: currentModeIndex = 0; break;
                    case FullScreenMode.FullScreenWindow: currentModeIndex = 1; break;
                    case FullScreenMode.Windowed: currentModeIndex = 2; break;
                }
                screenModeDropdown.value = currentModeIndex;
                screenModeDropdown.RefreshShownValue();

                screenModeDropdown.onValueChanged.RemoveAllListeners();
                screenModeDropdown.onValueChanged.AddListener(OnScreenModeSelected);
            }

            // 3. Setup Audio Sliders
            if (masterSlider != null)
            {
                masterSlider.value = SettingsManager.Instance.MasterVolume;
                UpdateMasterLabel(masterSlider.value);
                masterSlider.onValueChanged.RemoveAllListeners();
                masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            }

            if (bgmSlider != null)
            {
                bgmSlider.value = SettingsManager.Instance.BgmVolume;
                UpdateBgmLabel(bgmSlider.value);
                bgmSlider.onValueChanged.RemoveAllListeners();
                bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
            }

            if (sfxSlider != null)
            {
                sfxSlider.value = SettingsManager.Instance.SfxVolume;
                UpdateSfxLabel(sfxSlider.value);
                sfxSlider.onValueChanged.RemoveAllListeners();
                sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
            }

            isInitializing = false;
        }

        private void OnResolutionSelected(int index)
        {
            if (isInitializing || SettingsManager.Instance == null) return;
            SettingsManager.Instance.SetResolutionIndex(index, SettingsManager.Instance.CurrentScreenMode);
        }

        private void OnScreenModeSelected(int index)
        {
            if (isInitializing || SettingsManager.Instance == null) return;
            FullScreenMode mode = FullScreenMode.FullScreenWindow;
            switch (index)
            {
                case 0: mode = FullScreenMode.ExclusiveFullScreen; break;
                case 1: mode = FullScreenMode.FullScreenWindow; break;
                case 2: mode = FullScreenMode.Windowed; break;
            }
            SettingsManager.Instance.SetScreenMode(mode);
        }

        private void OnMasterSliderChanged(float val)
        {
            if (isInitializing || SettingsManager.Instance == null) return;
            SettingsManager.Instance.SetMasterVolume(val);
            UpdateMasterLabel(val);
        }

        private void OnBgmSliderChanged(float val)
        {
            if (isInitializing || SettingsManager.Instance == null) return;
            SettingsManager.Instance.SetBgmVolume(val);
            UpdateBgmLabel(val);
        }

        private void OnSfxSliderChanged(float val)
        {
            if (isInitializing || SettingsManager.Instance == null) return;
            SettingsManager.Instance.SetSfxVolume(val);
            UpdateSfxLabel(val);
        }

        private void UpdateMasterLabel(float val)
        {
            if (masterValueText != null)
                masterValueText.text = $"{Mathf.RoundToInt(val * 100f)}%";
        }

        private void UpdateBgmLabel(float val)
        {
            if (bgmValueText != null)
                bgmValueText.text = $"{Mathf.RoundToInt(val * 100f)}%";
        }

        private void UpdateSfxLabel(float val)
        {
            if (sfxValueText != null)
                sfxValueText.text = $"{Mathf.RoundToInt(val * 100f)}%";
        }
    }
}
