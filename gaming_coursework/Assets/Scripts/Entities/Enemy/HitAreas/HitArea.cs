using UnityEngine;

public class HitArea : MonoBehaviour
{
    // TODO : Use an entity class
    [SerializeField] private Enemy m_enemy;

    public enum BodyPart
    {
        Head, Body, Glove
    }
    public BodyPart bodyPart;

    [SerializeField] private float m_damageAmount;

    // ===================================

    private void Start()
    {
        switch (bodyPart)
        {
            case BodyPart.Head:
                m_damageAmount = 20f;
                break;

            case BodyPart.Body:
                m_damageAmount = 10f;
                break;

            case BodyPart.Glove:
                m_damageAmount = 5f;
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_enemy.IsDamageable)
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                Debug.Log("Enemy's " + bodyPart.ToString() + " got hit by a bullet");

                m_enemy.Damage(m_damageAmount, true);   
            }
            else if (collision.gameObject.CompareTag("Pickable"))
            {
                Pickable pickableObject = collision.gameObject.GetComponent<Pickable>();

                if (pickableObject != null && pickableObject.WasThrown)
                {
                    Debug.Log("Enemy's " + bodyPart.ToString() + " got hit by a pickable object");

                    m_enemy.Damage(m_damageAmount, false);
                }
            }
        }
    }

    public float GetAttackDamage()
    {
        if (bodyPart == BodyPart.Glove)
        {
            return 15f;
        }
        else
        {
            return 0f;
        }
    }
}
