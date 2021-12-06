using UnityEngine;

/// <summary>
/// Utility class to instantiate special effects
/// </summary>
public class EffectsManager : Singleton<EffectsManager>
{
    private static Transform s_defaultParent;

    private void Start()
    {
        s_defaultParent = transform;
    }

    public static void InstantiateEffect(GameObject effect, Vector3 position, Quaternion rotation, Transform parent = null, float duration = 1f)
    {
        if (parent == null)
        {
            parent = s_defaultParent;
        }

        GameObject instantiatedEffect = Instantiate(effect, position, rotation, parent);
        Destroy(instantiatedEffect, duration);
    }
}
