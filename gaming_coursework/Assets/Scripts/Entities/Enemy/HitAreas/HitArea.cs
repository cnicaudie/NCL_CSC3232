using UnityEngine;

/// <summary>
/// Handles damage for each specific hit areas of an enemy
/// </summary>
public class HitArea : MonoBehaviour
{
    public enum BodyPart
    {
        Head, Body, Glove
    }

    // ===================================
    // ATTRIBUTES
    // ===================================

    public BodyPart bodyPart;

    [SerializeField] private Enemy m_enemy;

    [SerializeField] private float m_damageAmount;
    [SerializeField] private float m_attackAmount;

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    /// <summary>
    /// Return the attack damage amount
    /// Note : for now, only gloves are considered as a "weapon"
    /// </summary>
    /// <returns></returns>
    public float GetAttackDamage()
    {
        return m_attackAmount;
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        switch (bodyPart)
        {
            case BodyPart.Head:
                m_damageAmount = 20f;
                m_attackAmount = 0f;
                break;

            case BodyPart.Body:
                m_damageAmount = 10f;
                m_attackAmount = 0f;
                break;

            case BodyPart.Glove:
                m_damageAmount = 5f;
                m_attackAmount = 15f;
                break;
        }
    }

    /// <summary>
    /// Collision response and feedback
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (m_enemy.IsDamageable)
        {
            // Collision with a bullet
            if (collision.gameObject.CompareTag("Bullet"))
            {
                Debug.Log("Enemy's " + bodyPart.ToString() + " got hit by a bullet");

                m_enemy.Damage(m_damageAmount, true);   
            }
            // Collision with a pickable object
            else if (collision.gameObject.CompareTag("Pickable"))
            {
                Pickable pickableObject = collision.gameObject.GetComponent<Pickable>();

                // Check is object was thrown
                // (don't want to get damaged by walking against a random object on ground)
                if (pickableObject != null && pickableObject.WasThrown)
                {
                    Debug.Log("Enemy's " + bodyPart.ToString() + " got hit by a pickable object");

                    m_enemy.Damage(m_damageAmount, false);
                }
            }
        }
    }
}
