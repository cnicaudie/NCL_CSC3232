using UnityEngine;

public class PlanetGravitation : MonoBehaviour
{
    [SerializeField] private float m_gravity = -20f;
    public float Gravity
    {
        get { return m_gravity; }
    }

    // ===================================

    private void OnTriggerEnter(Collider other)
    {
        GravityController gravityController = other.GetComponent<GravityController>();

        if (gravityController)
        {
            // Set up the planet's gravitation model the object has entered
            gravityController.Planet = this.GetComponent<PlanetGravitation>();
        }
    }
}
