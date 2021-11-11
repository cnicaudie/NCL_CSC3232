using UnityEngine;

/// <summary>
/// Handles the camera movements in each camera modes
/// </summary>
public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        Menu,
        Overworld,
        EncounterLevel
    }

    // ===================================
    // ATTRIBUTES
    // ===================================

    [SerializeField] private CameraMode m_cameraMode;

    [SerializeField] private Transform m_target;

    [SerializeField] private float m_smoothingRate = 0.05f;

    [SerializeField] private float verticalOffset = 25.0f;
    [SerializeField] private float horizontalOffset = 15.0f;
    private Vector3 m_offset;

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    /// <summary>
    /// Sets the new camera mode
    /// </summary>
    /// <param name="cameraMode"></param>
    public void SetCameraMode(CameraMode cameraMode)
    {
        m_cameraMode = cameraMode;
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    /// <summary>
    /// Camera follow is done in FixedUpdate to avoid lag/flickering
    /// </summary>
    private void FixedUpdate()
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

    /// <summary>
    /// Follows the target with a given offset
    /// </summary>
    private void FollowTarget()
    {
        // Apply the camera offset
        Vector3 nextPosition = m_target.position + m_offset;
        transform.position = Vector3.Lerp(transform.position, nextPosition, m_smoothingRate);

        // Look towards the target
        transform.LookAt(m_target);
    }
}
