using UnityEngine;

/// <summary>
/// Handles enter/exit of object in planet's orbit
/// </summary>
public class PlanetGravitation : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    [SerializeField] private float m_gravity = -20f;
    public float Gravity
    {
        get { return m_gravity; }
    }

    private static bool s_isSpaceshipInOrbit = false;

    private bool m_isSpaceshipInThisOrbit = false;
    public bool IsSpaceshipInThisOrbit
    {
        get { return m_isSpaceshipInThisOrbit; }
    }

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    /// <summary>
    /// Retrieves the planet's orbit height
    /// </summary>
    /// <returns></returns>
    public int GetOrbitHeight()
    {
        var orbitBound = GetComponent<SphereCollider>().radius * transform.lossyScale.y;
        var planetBound = transform.parent.GetComponent<SphereCollider>().radius * transform.parent.lossyScale.y;
        var orbitSize = orbitBound - planetBound;
        return Mathf.RoundToInt(orbitSize);
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        s_isSpaceshipInOrbit = false;
        m_isSpaceshipInThisOrbit = false;
    }

    /// <summary>
    /// Trigger enter response and feedback
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Spaceship enters this orbit
        if (other.gameObject.CompareTag("Spaceship") && !s_isSpaceshipInOrbit)
        {
            s_isSpaceshipInOrbit = true;
            m_isSpaceshipInThisOrbit = true;

            Debug.Log(transform.name + " orbit entered !");

            // Set up the planet's gravitation model the object has entered
            GravityController gravityController = other.GetComponent<GravityController>();
            Spaceship spaceship = other.GetComponent<Spaceship>();

            gravityController.Planet = this;
            spaceship.EnterNewOrbit(GetOrbitHeight());
        }
    }

    /// <summary>
    /// Trigger exit response and feedback
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        // Spaceship exits this orbit
        if (other.gameObject.CompareTag("Spaceship") && m_isSpaceshipInThisOrbit)
        {
            SoundManager.PlaySound("exitOrbit");

            s_isSpaceshipInOrbit = false;
            m_isSpaceshipInThisOrbit = false;

            Debug.Log(transform.name + " orbit exited !");
        }
    }
}
