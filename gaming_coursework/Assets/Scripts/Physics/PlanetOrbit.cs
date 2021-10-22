using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    private PlanetGravitation m_planet;
    private Rigidbody m_rigidbody;

    [SerializeField] private GameObject m_gravityTarget;

    [SerializeField] private float gravityScale = 20000;
    [SerializeField] private float initialVelocity = 15;

    private bool m_hasObjectInOrbit = false;

    // ===================================

    private void Start()
    {
        m_planet = GetComponentInChildren<PlanetGravitation>();
        // TODO : comment these to leave the moon orbitting
        // TODO : BUT need to figure out how to avoid the spaceship pushing the moon
        m_planet.SpaceshipExitOrbit += InitOrbit;
        m_planet.SpaceshipEnterOrbit += PauseOrbit;

        m_rigidbody = GetComponent<Rigidbody>();
        InitOrbit();
    }

    private void Update()
    {
        if (!m_hasObjectInOrbit)
        {
            transform.LookAt(m_gravityTarget.transform, Vector3.up);
        }
    }

    private void FixedUpdate()
    {
        if (!m_hasObjectInOrbit)
        {
            //m_rigidbody.AddForce(GetForceVector() * initialVelocity, ForceMode.Acceleration);
            m_rigidbody.AddForce(GetForceVector(), ForceMode.Force);
        }
    }

    private void PauseOrbit()
    {
        m_hasObjectInOrbit = true;

        m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void InitOrbit()
    {
        m_hasObjectInOrbit = false;

        m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        transform.LookAt(m_gravityTarget.transform, Vector3.up); 

        m_rigidbody.velocity = transform.right * initialVelocity;
    }

    private Vector3 GetForceVector()
    {
        Vector3 direction = m_gravityTarget.transform.position - transform.position;

        float q = 1.0f / direction.sqrMagnitude;

        return direction.normalized * q * gravityScale;
    }
}
