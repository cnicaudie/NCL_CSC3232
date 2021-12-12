using UnityEngine;

/// <summary>
/// Utility class to instantiate special effects
/// </summary>
public class EffectsManager : Singleton<EffectsManager>
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    // Parent Transform
    private static Transform s_defaultParent;

    // Effects
    private static GameObject m_hitBlood;
    private static GameObject m_hitImpact;
    private static GameObject m_hitExplosion;

    // =================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public static void InstantiateEffect(string effectName, Vector3 position, Quaternion rotation, Transform parent = null, float duration = 1f)
    {
        GameObject effect = GetEffectByName(effectName);

        if (effect == null)
        {
            Debug.LogWarning("Couldn't find the desired effect !");
            return;
        }

        if (parent == null)
        {
            parent = s_defaultParent;
        }

        GameObject instantiatedEffect = Instantiate(effect, position, rotation, parent);
        Destroy(instantiatedEffect, duration);
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        s_defaultParent = transform;

        m_hitBlood = Resources.Load<GameObject>("Effects/HitBlood");
        m_hitImpact = Resources.Load<GameObject>("Effects/HitTree");
        m_hitExplosion = Resources.Load<GameObject>("Effects/Explosion01");
    }

    private static GameObject GetEffectByName(string effectName)
    {
        switch (effectName)
        {
            case "blood":
                return m_hitBlood;

            case "impact":
                return m_hitImpact;

            case "explosion":
                return m_hitExplosion;

            default:
                return null;
        }
    }
}
