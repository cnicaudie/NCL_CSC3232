using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 hitPoint;

    private float m_speed = 30f;

    // ===================================

    private void Start()
    {
        GetComponent<Rigidbody>().AddForce((hitPoint - transform.position).normalized * m_speed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Destroyed by : " + collision.gameObject.name);
            Destroy(gameObject, 2f);
        }
        else if (collision.gameObject.CompareTag("Obstacle")
               || collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject, 2f);
        }
    }
}
