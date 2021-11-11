using UnityEngine;

/// <summary>
/// Handles health status of an entity
/// </summary>
public class Health : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    [SerializeField] private float m_maxHealth = 100f;
    [SerializeField] private float m_currentHealth;

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    /// <summary>
    /// Decrease health value by amount
    /// </summary>
    /// <param name="amount"></param>
    public void Damage(float amount)
    {
        m_currentHealth -= amount;
    }

    public bool IsDead()
    {
        return m_currentHealth <= 0f;
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_currentHealth = m_maxHealth;
    }
}
