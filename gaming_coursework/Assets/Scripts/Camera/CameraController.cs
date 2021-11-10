using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        Menu,
        Overworld,
        EncounterLevel
    }
    [SerializeField] private CameraMode m_cameraMode;

    [SerializeField] private Transform m_target;

    [SerializeField] private float m_smoothingRate = 0.05f;

    [SerializeField] private float verticalOffset = 35.0f;
    [SerializeField] private float horizontalOffset = 25.0f;
    private Vector3 m_offset;

    // ===================================

    private void Update()
    {
        switch (m_cameraMode)
        {
            case CameraMode.Overworld:
                m_offset = new Vector3(0f, Mathf.Sign(m_target.position.y) * verticalOffset, Mathf.Sign(m_target.position.z) * horizontalOffset);
                FollowTarget();
                break;

            case CameraMode.EncounterLevel:
                m_offset = new Vector3(0f, verticalOffset, -horizontalOffset);
                FollowTarget();
                break;

            default:
                break;
        }
    }

    private void FollowTarget()
    {
        // Apply the camera offset
        Vector3 nextPosition = m_target.position + m_offset;
        transform.position = Vector3.Lerp(transform.position, nextPosition, m_smoothingRate);

        // Look towards the target
        transform.LookAt(m_target);
    }

    public void SetCameraMode(CameraMode cameraMode)
    {
        m_cameraMode = cameraMode;
    }
}
