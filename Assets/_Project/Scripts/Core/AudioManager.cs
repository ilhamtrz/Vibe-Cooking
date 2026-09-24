using UnityEngine;

namespace VibeCooking
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource bgmSource;

        [Header("Sound Clips")]
        [SerializeField] private AudioClip sfxButtonClick;
        [SerializeField] private AudioClip sfxBellDing;
        [SerializeField] private AudioClip sfxCoinCollect;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                LoadAudioClipsIfNull();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ApplySettingsVolume();
        }

        private void InitializeAudioSources()
        {
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.loop = false;
            }

            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.playOnAwake = false;
                bgmSource.loop = true;
            }
        }

        private void LoadAudioClipsIfNull()
        {
#if UNITY_EDITOR
            if (sfxButtonClick == null)
                sfxButtonClick = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/sfx_btn_click.wav");
            if (sfxBellDing == null)
                sfxBellDing = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/sfx_bell_ding.wav");
            if (sfxCoinCollect == null)
                sfxCoinCollect = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Project/Audio/sfx_coin_collect.wav");
#endif
        }

        public void ApplySettingsVolume()
        {
            if (SettingsManager.Instance != null)
            {
                float master = SettingsManager.Instance.MasterVolume;
                if (sfxSource != null)
                    sfxSource.volume = master * SettingsManager.Instance.SfxVolume;
                if (bgmSource != null)
                    bgmSource.volume = master * SettingsManager.Instance.BgmVolume;
            }
            else
            {
                if (sfxSource != null) sfxSource.volume = 0.85f;
                if (bgmSource != null) bgmSource.volume = 0.80f;
            }
        }

        public void PlayButtonClick()
        {
            PlaySFX(sfxButtonClick, 0.9f);
        }

        public void PlayBellDing()
        {
            PlaySFX(sfxBellDing, 1.0f);
        }

        public void PlayCoinCollect()
        {
            PlaySFX(sfxCoinCollect, 0.95f);
        }

        public void PlaySFX(AudioClip clip, float volumeScale = 1.0f)
        {
            if (clip != null && sfxSource != null)
            {
                ApplySettingsVolume();
                sfxSource.PlayOneShot(clip, volumeScale);
            }
        }

        public void SetReferences(AudioClip btnClick, AudioClip bellDing, AudioClip coinCollect)
        {
            sfxButtonClick = btnClick;
            sfxBellDing = bellDing;
            sfxCoinCollect = coinCollect;
        }
    }
}
