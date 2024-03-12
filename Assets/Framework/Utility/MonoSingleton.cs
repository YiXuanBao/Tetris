using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool _isDestroyed = false;
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (!_isDestroyed && _instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<T>();
                    obj.name = typeof(T).ToString();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        _isDestroyed = false;
    }

    protected virtual void OnDestroy()
    {
        _isDestroyed = true;
    }
}

