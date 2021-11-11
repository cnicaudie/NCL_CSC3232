using UnityEngine;

public class GravityController : MonoBehaviour
{
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

    private void ApplyRelativeGravity()
    {
        // Add gravity relatively to the planet it's on
        m_revGravityDirection = transform.position - m_planet.transform.position;
        m_rigidbody.AddForce(m_revGravityDirection * m_planet.Gravity);        
    }

    private void ApplyRelativeRotation()
    {
        // Rotate the body so that it always "walks on the ground"
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, m_revGravityDirection) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_rotationSpeed * Time.fixedDeltaTime);
    }
}
