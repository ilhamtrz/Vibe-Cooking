using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace VibeCooking
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        private const string PrefKeyResWidth = "ResolutionWidth";
        private const string PrefKeyResHeight = "ResolutionHeight";
        private const string PrefKeyScreenMode = "ScreenMode";
        private const string PrefKeyMasterVol = "MasterVol";
        private const string PrefKeyBgmVol = "BgmVol";
        private const string PrefKeySfxVol = "SfxVol";

        [Header("Audio Mixer (Optional)")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string masterParam = "MasterVolume";
        [SerializeField] private string bgmParam = "BGMVolume";
        [SerializeField] private string sfxParam = "SFXVolume";

        [Header("Current Runtime Settings")]
        [SerializeField] private float masterVolume = 0.8f;
        [SerializeField] private float bgmVolume = 0.7f;
        [SerializeField] private float sfxVolume = 0.85f;
        [SerializeField] private int resolutionWidth;
        [SerializeField] private int resolutionHeight;
        [SerializeField] private FullScreenMode currentScreenMode = FullScreenMode.FullScreenWindow;

        // Events for UI and systems to subscribe to
        public event Action<float> OnMasterVolumeChanged;
        public event Action<float> OnBgmVolumeChanged;
        public event Action<float> OnSfxVolumeChanged;
        public event Action<int, int, FullScreenMode> OnResolutionChanged;

        public float MasterVolume => masterVolume;
        public float BgmVolume => bgmVolume;
        public float SfxVolume => sfxVolume;
        public int ResolutionWidth => resolutionWidth;
        public int ResolutionHeight => resolutionHeight;
        public FullScreenMode CurrentScreenMode => currentScreenMode;

        private List<Resolution> filteredResolutions = new List<Resolution>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeResolutions();
            LoadSettings();
        }

        private void Start()
        {
            ApplyAllSettings();
        }

        public void InitializeResolutions()
        {
            filteredResolutions.Clear();
            var allResolutions = Screen.resolutions;
            var seen = new HashSet<string>();

            for (int i = 0; i < allResolutions.Length; i++)
            {
                var res = allResolutions[i];
                string key = $"{res.width}x{res.height}";
                if (!seen.Contains(key))
                {
                    seen.Add(key);
                    filteredResolutions.Add(res);
                }
            }

            if (filteredResolutions.Count == 0)
            {
                filteredResolutions.Add(new Resolution { width = Screen.width, height = Screen.height });
            }
        }

        public List<Resolution> GetFilteredResolutions()
        {
            if (filteredResolutions.Count == 0)
            {
                InitializeResolutions();
            }
            return filteredResolutions;
        }

        public int GetCurrentResolutionIndex()
        {
            var resList = GetFilteredResolutions();
            for (int i = 0; i < resList.Count; i++)
            {
                if (resList[i].width == resolutionWidth && resList[i].height == resolutionHeight)
                {
                    return i;
                }
            }

            // Fallback: match current Screen dimensions
            for (int i = 0; i < resList.Count; i++)
            {
                if (resList[i].width == Screen.width && resList[i].height == Screen.height)
                {
                    return i;
                }
            }

            return resList.Count - 1; // Default to highest resolution
        }

        public void SetResolutionIndex(int index, FullScreenMode mode)
        {
            var list = GetFilteredResolutions();
            if (index >= 0 && index < list.Count)
            {
                SetResolution(list[index].width, list[index].height, mode);
            }
        }

        public void SetResolution(int width, int height, FullScreenMode mode)
        {
            resolutionWidth = width;
            resolutionHeight = height;
            currentScreenMode = mode;

            Screen.SetResolution(width, height, mode);
            SaveSettings();
            OnResolutionChanged?.Invoke(width, height, mode);
            Debug.Log($"[SettingsManager] Resolution set to {width}x{height} ({mode})");
        }

        public void SetScreenMode(FullScreenMode mode)
        {
            currentScreenMode = mode;
            Screen.SetResolution(resolutionWidth, resolutionHeight, mode);
            SaveSettings();
            OnResolutionChanged?.Invoke(resolutionWidth, resolutionHeight, mode);
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolume(masterParam, masterVolume);
            AudioListener.volume = masterVolume;
            PlayerPrefs.SetFloat(PrefKeyMasterVol, masterVolume);
            PlayerPrefs.Save();
            OnMasterVolumeChanged?.Invoke(masterVolume);
        }

        public void SetBgmVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            ApplyVolume(bgmParam, bgmVolume);
            PlayerPrefs.SetFloat(PrefKeyBgmVol, bgmVolume);
            PlayerPrefs.Save();
            OnBgmVolumeChanged?.Invoke(bgmVolume);
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolume(sfxParam, sfxVolume);
            PlayerPrefs.SetFloat(PrefKeySfxVol, sfxVolume);
            PlayerPrefs.Save();
            OnSfxVolumeChanged?.Invoke(sfxVolume);
        }

        private void ApplyVolume(string paramName, float linearVol)
        {
            if (audioMixer != null)
            {
                float dB = linearVol > 0.001f ? Mathf.Log10(linearVol) * 20f : -80f;
                audioMixer.SetFloat(paramName, dB);
            }
        }

        public void ApplyAllSettings()
        {
            SetMasterVolume(masterVolume);
            SetBgmVolume(bgmVolume);
            SetSfxVolume(sfxVolume);
            Screen.SetResolution(resolutionWidth, resolutionHeight, currentScreenMode);
        }

        public void LoadSettings()
        {
            masterVolume = PlayerPrefs.GetFloat(PrefKeyMasterVol, 0.8f);
            bgmVolume = PlayerPrefs.GetFloat(PrefKeyBgmVol, 0.7f);
            sfxVolume = PlayerPrefs.GetFloat(PrefKeySfxVol, 0.85f);

            resolutionWidth = PlayerPrefs.GetInt(PrefKeyResWidth, Screen.currentResolution.width);
            resolutionHeight = PlayerPrefs.GetInt(PrefKeyResHeight, Screen.currentResolution.height);
            currentScreenMode = (FullScreenMode)PlayerPrefs.GetInt(PrefKeyScreenMode, (int)FullScreenMode.FullScreenWindow);

            Debug.Log($"[SettingsManager] Loaded Settings - Res: {resolutionWidth}x{resolutionHeight}, Mode: {currentScreenMode}, Master: {masterVolume:F2}, BGM: {bgmVolume:F2}, SFX: {sfxVolume:F2}");
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetInt(PrefKeyResWidth, resolutionWidth);
            PlayerPrefs.SetInt(PrefKeyResHeight, resolutionHeight);
            PlayerPrefs.SetInt(PrefKeyScreenMode, (int)currentScreenMode);
            PlayerPrefs.SetFloat(PrefKeyMasterVol, masterVolume);
            PlayerPrefs.SetFloat(PrefKeyBgmVol, bgmVolume);
            PlayerPrefs.SetFloat(PrefKeySfxVol, sfxVolume);
            PlayerPrefs.Save();
            Debug.Log("[SettingsManager] Settings saved to PlayerPrefs.");
        }

        [ContextMenu("Log Saved Settings")]
        public void LogSavedSettings()
        {
            Debug.Log($"[SettingsManager Saved Prefs]\n" +
                      $"Resolution: {PlayerPrefs.GetInt(PrefKeyResWidth, -1)}x{PlayerPrefs.GetInt(PrefKeyResHeight, -1)}\n" +
                      $"ScreenMode: {(FullScreenMode)PlayerPrefs.GetInt(PrefKeyScreenMode, -1)}\n" +
                      $"MasterVol: {PlayerPrefs.GetFloat(PrefKeyMasterVol, -1f)}\n" +
                      $"BgmVol: {PlayerPrefs.GetFloat(PrefKeyBgmVol, -1f)}\n" +
                      $"SfxVol: {PlayerPrefs.GetFloat(PrefKeySfxVol, -1f)}");
        }

        [ContextMenu("Reset Settings to Default")]
        public void ResetSettingsToDefault()
        {
            PlayerPrefs.DeleteKey(PrefKeyResWidth);
            PlayerPrefs.DeleteKey(PrefKeyResHeight);
            PlayerPrefs.DeleteKey(PrefKeyScreenMode);
            PlayerPrefs.DeleteKey(PrefKeyMasterVol);
            PlayerPrefs.DeleteKey(PrefKeyBgmVol);
            PlayerPrefs.DeleteKey(PrefKeySfxVol);
            PlayerPrefs.Save();
            LoadSettings();
            ApplyAllSettings();
            Debug.Log("[SettingsManager] Reset settings to default values.");
        }
    }
}
