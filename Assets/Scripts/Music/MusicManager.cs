using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowShift.Music
{
    /// <summary>
    /// Mostly used for Music singleton as background music so they stay played and give a good vibe
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        private static MusicManager instance;
        public AudioSource MainAudioSource;
        public AudioClip[] AudioClips;

        void Awake()
        {


            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Debug.Log($"Start function is called on MusicManager");
            // searching for the multiplayer lobby scene so we can chnage the music
            Scene scene = SceneManager.GetSceneByName("Lobby");
            int referenceSceneIndex = scene.buildIndex;

            if (SceneManager.GetActiveScene().buildIndex == referenceSceneIndex)
            {
                // it means we're at the lobby scene and we don't need the old music
                Destroy(this.gameObject);
            }

            // randomly change the audio source
            int index = Random.Range(0, AudioClips.Length);
            MainAudioSource.clip = AudioClips[index];
            MainAudioSource.Stop();
            MainAudioSource.Play();
        }
    }

}