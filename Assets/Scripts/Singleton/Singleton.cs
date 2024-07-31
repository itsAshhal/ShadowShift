using UnityEngine;

namespace ShadowShift
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        private static object _lock = new object();
        private static bool _isApplicationQuitting = false; // To handle edge cases during application quit

        public static T Instance
        {
            get
            {
                if (_isApplicationQuitting)
                {
                    Debug.LogWarning($"Attempt to access {typeof(T).Name} after it has been destroyed.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            Debug.Log($"Creating new instance of {typeof(T).Name}");
                            GameObject singletonGameObject = new GameObject($"{typeof(T).Name} (Singleton)");
                            _instance = singletonGameObject.AddComponent<T>();
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.Log($"Another instance of {typeof(T).Name} was created, so this instance will be destroyed.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null; // Clear the static instance if this was it
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isApplicationQuitting = true; // To prevent access to the singleton during shutdown
        }
    }
}
