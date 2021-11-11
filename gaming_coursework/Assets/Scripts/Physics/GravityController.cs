using UnityEngine;

/// <summary>
/// Handles the gravity based on the current planet
/// </summary>
public class GravityController : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    [SerializeField] private PlanetGravitation m_planet;
    public PlanetGravitation Planet
    {
        get { return m_planet; }
        set { m_planet = value; }
    }

    private Rigidbody m_rigidbody;

    private Vector3 m_revGravityDirection;

    private float m_rotationSpeed = 20.0f;

    // ===================================

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.useGravity = false;
        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (m_planet != null)
        {
            ApplyRelativeGravity();
            ApplyRelativeRotation();
        }
    }

    /// <summary>
    /// Computes and applies the relative physics gravity on the current planet
    /// </summary>
    private void ApplyRelativeGravity()
    {
        // Add gravity relatively to the planet it's on
        m_revGravityDirection = transform.position - m_planet.transform.position;
        m_rigidbody.AddForce(m_revGravityDirection * m_planet.Gravity);        
    }

    /// <summary>
    /// Applies a relative rotation to the body to make it "walk on the planet"
    /// </summary>
    private void ApplyRelativeRotation()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, m_revGravityDirection) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_rotationSpeed * Time.fixedDeltaTime);
    }
}
