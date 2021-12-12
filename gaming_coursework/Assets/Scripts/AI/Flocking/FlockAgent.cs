using UnityEngine;

/// <summary>
/// Handles the final movement of the flock agent
/// </summary>
[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private Collider m_collider;
    public Collider Collider
    {
        get { return m_collider; }
    }

    private bool m_hasFallen = false;
    public bool HasFallen
    {
        get { return m_hasFallen; }
    }

    private bool m_wasKilled = false;
    public bool WasKilled
    {
        get { return m_wasKilled; }
    }

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public void Move(Vector3 velocity)
    {
        transform.forward = velocity;
        transform.position += velocity * Time.deltaTime;
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (transform.position.y < -5f && !m_hasFallen && !m_wasKilled)
        {
            m_hasFallen = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !m_hasFallen && !m_wasKilled)
        {
            // Instantiate hit effect
            Vector3 impactPoint = collision.GetContact(0).point;
            Quaternion impactAngle = Quaternion.Euler(collision.GetContact(0).normal);
            EffectsManager.InstantiateEffect("blood", impactPoint, impactAngle);

            SoundManager.Instance.PlaySound("punch");

            m_wasKilled = true;
        }
    }
}
