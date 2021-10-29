using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private Animator m_animator;

    private float m_horizontalInput;
    private float m_verticalInput;
    private bool m_jumpInput;

    private float m_speed;
    [SerializeField] private float m_defaultSpeed = 130.0f;
    [SerializeField] private float m_airSpeed = 80.0f;

    private float m_moveThreshold = 0.5f;
    private float m_slowdownFactor = 0.5f;

    private float m_rotationSmoothTime = 0.1f;
    private float m_rotationVelocity;

    private bool m_isJumping = false;
    [SerializeField] private float m_jumpForce = 3000.0f;

    private bool m_isGrounded = true;

    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GetInputs();

        UpdateAnimatorParameters();

        SetSpeed();
    }

    private void FixedUpdate()
    {
        Move();

        if (m_jumpInput && m_isGrounded)
        {
            Jump();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO : Use a ground check instead ?
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!m_isGrounded)
            {
                m_isGrounded = true;
                m_isJumping = false;
            }
        }
    }

    private void GetInputs()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
        m_jumpInput = Input.GetButtonDown("Jump");
    }

    private void UpdateAnimatorParameters()
    {
        m_animator.SetFloat("Speed", m_rigidbody.velocity.magnitude);
        m_animator.SetBool("IsJumping", m_isJumping);
    }

    private void SetSpeed()
    {
        m_speed = m_isJumping ? m_airSpeed : m_defaultSpeed;
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
        else if (!m_isJumping)
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

    private void Jump()
    {
        m_isJumping = true;
        m_isGrounded = false;
        m_rigidbody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
    }
}
