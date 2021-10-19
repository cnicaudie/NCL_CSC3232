using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody m_rigidbody;

    private float m_horizontalInput;
    private float m_verticalInput;

    [SerializeField] private float m_speed = 100.0f;

    private float m_moveThreshold = 0.5f;
    private float m_slowdownFactor = 0.5f;

    private float m_rotationSmoothTime = 0.1f;
    private float m_rotationVelocity;

    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GetInputs()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
    }

    private void SlowdownVelocity()
    {
        m_rigidbody.velocity *= m_slowdownFactor;
        m_rigidbody.angularVelocity *= m_slowdownFactor;
    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(m_horizontalInput, 0f, m_verticalInput).normalized;

        if (moveDirection.magnitude >= m_moveThreshold)
        {
            ApplyVelocity(moveDirection);
            Rotate(moveDirection);
        }
        else
        {
            SlowdownVelocity();
        }
    }

    private void ApplyVelocity(Vector3 moveDirection)
    {
        m_rigidbody.velocity = m_rigidbody.velocity + (moveDirection * m_speed * Time.fixedDeltaTime);
    }

    private void Rotate(Vector3 moveDirection)
    {
        float facingAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        float smoothFacingAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, facingAngle, ref m_rotationVelocity, m_rotationSmoothTime);

        Quaternion rotation = Quaternion.Euler(0f, smoothFacingAngle, 0f);

        m_rigidbody.MoveRotation(rotation);
    }
}
