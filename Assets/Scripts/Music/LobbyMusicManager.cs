using ShadowShift.Music;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowShift
{
    public class LobbyMusicManager : MonoBehaviour
    {
        // first thing we need to do is, search out the MusicManager because we need to turn it off by any means
        [SerializeField] AudioSource m_lobbyMusic;

        // make a public static instance
        public static LobbyMusicManager Instance;

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(this);
            else Instance = this;

            DestroyOldMusic();
            LobbyMusic(false);
        }


        /// <summary>
        /// Since we're using a MusicManager with a singleton pattern, we need a new music for the lobby gameplay and for that
        /// reason we need to destory the old music
        /// </summary>
        void DestroyOldMusic()
        {
            var _oldMusic = FindAnyObjectByType<MusicManager>();
            if (_oldMusic == null) return;

            Destroy(_oldMusic.gameObject);
        }

        public void LobbyMusic(bool isEnabled)
        {
            if (isEnabled) m_lobbyMusic.Play();
            else m_lobbyMusic.Stop();
        }
    }

}
