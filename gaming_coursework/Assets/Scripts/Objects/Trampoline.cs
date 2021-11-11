using UnityEngine;

/// <summary>
/// Handles trampoline behaviour
/// </summary>
public class Trampoline : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private float m_bounciness = 5000f;

    // ===================================

    // ===================================
    // PRIVATE METHODS
    // ===================================

    /// <summary>
    /// Collision feedback and response
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 collisionNormal = collision.GetContact(0).normal.normalized;

            // Physics-based collision response
            if (collisionNormal == Vector3.down)
            {
                collision.rigidbody.AddForce(Vector3.up * m_bounciness, ForceMode.Impulse);
            }
        }
    }
}
