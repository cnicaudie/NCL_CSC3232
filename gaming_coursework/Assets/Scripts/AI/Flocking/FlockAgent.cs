using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    private Collider m_collider;
    public Collider Collider
    {
        get { return m_collider; }
    }

    private void Start()
    {
        m_collider = GetComponent<Collider>();
    }

    public void Move(Vector2 velocity)
    {
        transform.forward = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}
