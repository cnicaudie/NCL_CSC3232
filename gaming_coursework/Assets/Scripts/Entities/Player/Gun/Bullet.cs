using UnityEngine;

/// <summary>
/// Handles bullet behaviour
/// </summary>
public class Bullet : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private Vector3 m_hitPoint;

    private Rigidbody m_rigidbody;

    private float m_speed = 30f;

    private float m_lifetime = 5f;
    private float m_lifetimeTimer = 0f;

    private float m_explosionForce = 7000f;
    private float m_explosionRadius = 30f;

    [SerializeField] private GameObject m_hitExplosion;
    [SerializeField] private GameObject m_hitImpact;

    public delegate void OnBulletImpact(GameObject bulletGameObject);
    public event OnBulletImpact DestroyBullet;

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public void Reset(Vector3 hitPoint, bool isNewBullet)
    {
        m_lifetimeTimer = 0f;
        m_hitPoint = hitPoint;

        if (!isNewBullet)
        {
            // Reset event listeners
            DestroyBullet = null;

            // Reset rigidbody attributes
            m_rigidbody.useGravity = false;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;

            m_rigidbody.AddForce((m_hitPoint - transform.position).normalized * m_speed, ForceMode.VelocityChange);
        }
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.useGravity = false;
        m_rigidbody.AddForce((m_hitPoint - transform.position).normalized * m_speed, ForceMode.VelocityChange);
    }

    private void Update()
    {
        m_lifetimeTimer += Time.deltaTime;

        if (m_lifetimeTimer >= m_lifetime)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        if (gameObject.activeSelf) // safe check
        {
            if (DestroyBullet != null)
            {
                DestroyBullet(gameObject);
            }
        }
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
            // Instantiate hit impact effect
            Vector3 impactPoint = collision.GetContact(0).point;
            Quaternion impactAngle = Quaternion.Euler(collision.GetContact(0).normal);
            EffectsManager.InstantiateEffect(m_hitImpact, impactPoint, impactAngle, transform.parent);
        }

        Destroy();
    }

    /// <summary>
    /// Adds a feedback force to the enemy's body and destroy the bullet
    /// </summary>
    /// <param name="collisionObject"></param>
    private void Explode(GameObject collisionObject)
    {
        // Instantiate explosion effect
        EffectsManager.InstantiateEffect(m_hitExplosion, transform.position, transform.rotation);

        // Apply explosion force
        Enemy enemy = collisionObject.GetComponent<Enemy>();
        Rigidbody rb = collisionObject.GetComponent<Rigidbody>();

        if (enemy != null && enemy.IsDamageable && rb != null)
        {
            rb.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 0f, ForceMode.Force);
        }
    }
}
