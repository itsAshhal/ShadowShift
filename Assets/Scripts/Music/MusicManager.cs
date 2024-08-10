using UnityEngine;

namespace ShadowShift.Music
{
    /// <summary>
    /// Mostly used for Music singleton as background music so they stay played and give a good vibe
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        private static MusicManager instance;

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
    }

}