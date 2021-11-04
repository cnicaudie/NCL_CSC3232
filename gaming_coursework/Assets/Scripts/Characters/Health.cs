using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float m_maxHealth = 100f;
    [SerializeField] private float m_currentHealth;

    // ===================================

    private void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    public void Damage(float amount)
    {
        Debug.Log("Previous health : " + m_currentHealth);
        m_currentHealth -= amount;
        Debug.Log("New health : " + m_currentHealth);
    }

    public bool IsDead()
    {
        return m_currentHealth <= 0f;
    }
}
