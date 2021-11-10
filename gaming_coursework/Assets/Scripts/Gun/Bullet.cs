using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 hitPoint;

    private Rigidbody m_rigidbody;
    private float m_speed = 30f;

    [SerializeField] private float m_explosionForce = 7000f;
    [SerializeField] private float m_explosionRadius = 30f;

    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.useGravity = false;
        m_rigidbody.AddForce((hitPoint - transform.position).normalized * m_speed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_rigidbody.useGravity = true;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Explode(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Obstacle")
            || collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject, 0.5f);
        }
        else
        {
            Destroy(gameObject, 2f);
        }
    }

    private void Explode(GameObject collisionObject)
    {
        Enemy enemy = collisionObject.GetComponent<Enemy>();
        Rigidbody rb = collisionObject.GetComponent<Rigidbody>();

        if (enemy != null && enemy.IsDamageable && rb != null)
        {
            rb.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 0f, ForceMode.Force);
        }

        Destroy(gameObject, 0.5f);
    }
}
