using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float m_maxHealth = 100f;

    private bool m_isDead = false;
    public bool IsDead
    {
        get { return m_isDead; }
    }

    // ===================================

    public void Damage(float amount)
    {
        m_maxHealth -= amount;

        if (m_maxHealth <= 0f)
        {
            m_isDead = true;

            Destroy(gameObject, 5f);
        }
    }
}
