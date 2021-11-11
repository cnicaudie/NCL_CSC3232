using UnityEngine;

/// <summary>
/// Orbit movement simulator (moon orbit around earth)
/// This is not physics based to allow the orbit to pause
/// when the spaceship enters this orbit
/// </summary>
public class PlanetOrbit : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private PlanetGravitation m_planet;
    
    [SerializeField] private GameObject m_gravityTarget;

    public Vector3 orbitAxis = Vector3.up;
    public float orbitRadius;
    public float orbitChangeSpeed;
    public float orbitSpeed;

    // ===================================

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_planet = GetComponentInChildren<PlanetGravitation>();

        transform.position = GetNextOrbitPosition();
    }

    private void Update()
    {
        if (!m_planet.IsSpaceshipInThisOrbit)
        {
            // Rotate around gravity target with a slow orbit axis change
            orbitAxis = GetNextOrbitAxis();
            transform.RotateAround(m_gravityTarget.transform.position, orbitAxis, orbitSpeed * Time.deltaTime);

            // Adjust distance to target
            // -> Constant distance makes it easier to reach the moon and avoid collision with earth
            transform.position = Vector3.MoveTowards(transform.position, GetNextOrbitPosition(), orbitChangeSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Computes a slight random change in orbit axis
    /// </summary>
    /// <returns></returns>
    private Vector3 GetNextOrbitAxis()
    {
        Vector3 randomAxis = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));

        return Vector3.Slerp(orbitAxis, randomAxis.normalized, orbitChangeSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Get adjusted distance to the gravity target
    /// </summary>
    /// <returns></returns>
    private Vector3 GetNextOrbitPosition()
    {
        Vector3 direction = m_gravityTarget.transform.position - transform.position;

        return -direction.normalized * orbitRadius + m_gravityTarget.transform.position;
    }
}
