using UnityEngine;

public class Trampoline : MonoBehaviour
{
    private float m_bounciness = 5000f;

    // ===================================

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 collisionNormal = collision.GetContact(0).normal.normalized;

            if (collisionNormal == Vector3.down)
            {
                collision.rigidbody.AddForce(Vector3.up * m_bounciness, ForceMode.Impulse);
            }
        }
    }
}
