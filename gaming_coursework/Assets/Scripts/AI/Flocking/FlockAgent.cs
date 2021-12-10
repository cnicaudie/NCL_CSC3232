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
        // Check if agent falls off the ground
        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }
}
