using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private static T s_instance; // singleton instance
    public static T Instance
    {
        get { return s_instance; }
    }

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public static bool HasInited()
    {
        return s_instance != null;
    }

    // ===================================
    // PUBLIC METHODS
    // ===================================

    /// <summary>
    /// Makes the instance a "Don't Destroy On Load" object (singleton)
    /// </summary>
    protected virtual void Awake()
    {
        Object[] instances = FindObjectsOfType(typeof(T));

        if (instances.Length > 1)
        {
            Destroy(gameObject);
        }
        else if (s_instance == null && instances.Length > 0)
        {
            s_instance = (T)instances[0];
            DontDestroyOnLoad(gameObject);
        }
    }
}