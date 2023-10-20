using UnityEngine;

namespace MissionMap.Util
{
    public class Singleton<T> : MonoBehaviour
        where T : Singleton<T>
    {
        [SerializeField] protected bool IsDontDestroyOnLoad = false;

        private static T _instance = null;

        private bool _alive = true;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var holders = FindObjectsOfType<T>();

                if (holders != null)
                {
                    switch (holders.Length)
                    {
                        case 1:
                        {
                            _instance = holders[0];

                            if (_instance.IsDontDestroyOnLoad)
                                DontDestroyOnLoad(_instance);

                            return _instance;
                        }
                        case > 1:
                        {
                            Debug.LogError($"Have more that one {typeof(T).Name} in scene. " +
                                           "But this is Singleton! Check project.");

                            foreach (var singleton in holders)
                                Destroy(singleton.gameObject);

                            break;
                        }
                    }
                }

                var go = new GameObject(typeof(T).Name, typeof(T));
                _instance = go.GetComponent<T>().Initialize();

                if (_instance.IsDontDestroyOnLoad)
                    DontDestroyOnLoad(_instance.gameObject);

                return _instance;
            }

            set => _instance = value;
        }

        public static bool IsAlive => _instance != null && _instance._alive;

        protected void Awake()
        {
            if (_instance == null)
            {
                _instance = (this as T)!.Initialize();

                if (IsDontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogError($"Have more that one instance of {typeof(T).Name} in scene. ");
                DestroyImmediate(this);
            }
        }

        protected void OnDestroy() => _alive = false;

        protected void OnApplicationQuit() => _alive = false;

        protected virtual T Initialize() => this as T;
    }
}