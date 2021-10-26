using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    private PlanetGravitation m_planet;
    
    [SerializeField] private GameObject m_gravityTarget;

    public Vector3 orbitAxis = Vector3.up;
    public float orbitRadius;
    public float orbitChangeSpeed;
    public float orbitSpeed;

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

    private Vector3 GetNextOrbitAxis()
    {
        Vector3 randomAxis = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));

        return Vector3.Slerp(orbitAxis, randomAxis.normalized, orbitChangeSpeed * Time.deltaTime);
    }

    private Vector3 GetNextOrbitPosition()
    {
        Vector3 direction = m_gravityTarget.transform.position - transform.position;

        return -direction.normalized * orbitRadius + m_gravityTarget.transform.position;
    }
}
