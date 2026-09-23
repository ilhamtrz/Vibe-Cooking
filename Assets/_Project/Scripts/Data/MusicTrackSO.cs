using UnityEngine;

namespace VibeCooking
{
    [CreateAssetMenu(fileName = "NewMusicTrack", menuName = "Vibe Cooking/Music Track")]
    public class MusicTrackSO : ScriptableObject
    {
        public string trackTitle;
        public string artist;
        public AudioClip audioClip;
        public int bpm;
    }
}
