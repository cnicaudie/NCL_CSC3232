using UnityEngine;

/// <summary>
/// Handles bullet behaviour
/// </summary>
public class Bullet : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    public Vector3 hitPoint;

    private Rigidbody m_rigidbody;
    private float m_speed = 30f;
    private float m_lifetime = 5f;

    private float m_explosionForce = 7000f;
    private float m_explosionRadius = 30f;

    [SerializeField] private GameObject m_hitExplosion;
    [SerializeField] private GameObject m_hitImpact;

    // ===================================

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.useGravity = false;
        m_rigidbody.AddForce((hitPoint - transform.position).normalized * m_speed, ForceMode.VelocityChange);
        Destroy(gameObject, m_lifetime);
    }

    /// <summary>
    /// Collision response and feedback
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        SoundManager.Instance.PlaySound("bulletImpact");

        m_rigidbody.useGravity = true;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Explode(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Obstacle")
            || collision.gameObject.CompareTag("Ground"))
        {
            Vector3 impactPoint = collision.GetContact(0).point;
            Quaternion impactAngle = Quaternion.Euler(collision.GetContact(0).normal);
            GameObject impact = Instantiate(m_hitImpact, impactPoint, impactAngle, transform.parent);
            Destroy(impact, 1f);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Adds a feedback force to the enemy's body and destroy the bullet
    /// </summary>
    /// <param name="collisionObject"></param>
    private void Explode(GameObject collisionObject)
    {
        // Instantiate explosion effect
        GameObject explosion = Instantiate(m_hitExplosion, transform.position, transform.rotation, transform.parent);
        Destroy(explosion, 1f);

        // Apply explosion force
        Enemy enemy = collisionObject.GetComponent<Enemy>();
        Rigidbody rb = collisionObject.GetComponent<Rigidbody>();

        if (enemy != null && enemy.IsDamageable && rb != null)
        {
            rb.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 0f, ForceMode.Force);
        }
    }
}
