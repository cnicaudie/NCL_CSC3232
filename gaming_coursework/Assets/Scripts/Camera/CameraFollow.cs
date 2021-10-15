using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform m_target;

    [SerializeField] private float m_smoothingRate = 0.05f;

    [SerializeField] private float verticalOffset = 35.0f;
    [SerializeField] private float horizontalOffset = 25.0f;
    private Vector3 m_offset;

    // ===================================

    private void FixedUpdate()
    {
        m_offset = new Vector3(m_target.position.x, Mathf.Sign(m_target.position.y) * verticalOffset, Mathf.Sign(m_target.position.z) * horizontalOffset);

        // Apply the camera offset
        Vector3 nextPosition = m_target.position + m_offset;
        transform.position = Vector3.Lerp(transform.position, nextPosition, m_smoothingRate);

        // Look towards the target
        transform.LookAt(m_target);
    }
}
