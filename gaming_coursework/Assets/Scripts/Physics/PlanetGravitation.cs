using UnityEngine;

public class PlanetGravitation : MonoBehaviour
{
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

    private void OnTriggerEnter(Collider other)
    {
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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Spaceship") && m_isSpaceshipInThisOrbit)
        {
            s_isSpaceshipInOrbit = false;
            m_isSpaceshipInThisOrbit = false;

            Debug.Log(transform.name + " orbit exited !");
        }
    }

    public int GetOrbitHeight()
    {
        var orbitBound = GetComponent<SphereCollider>().radius * transform.lossyScale.y;
        var planetBound = transform.parent.GetComponent<SphereCollider>().radius * transform.parent.lossyScale.y;
        var orbitSize = orbitBound - planetBound;

        Debug.Log("Orbit's height : " + orbitSize);

        return Mathf.RoundToInt(orbitSize);
    }
}
