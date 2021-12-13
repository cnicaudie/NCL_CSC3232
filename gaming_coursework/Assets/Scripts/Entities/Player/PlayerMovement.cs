using UnityEngine;

/// <summary>
/// Handles player movement based on inputs
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private Rigidbody m_rigidbody;
    private Animator m_animator;

    private float m_horizontalInput;
    private float m_verticalInput;
    private bool m_jumpInput;

    private float m_groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask m_groundLayer;
    [SerializeField] private bool m_isGrounded = true;

    private float m_speed;
    [SerializeField] private float m_defaultSpeed = 130.0f;
    [SerializeField] private float m_airSpeed = 80.0f;
    [SerializeField] private bool m_isMoving = false;

    private float m_moveThreshold = 0.5f;
    private float m_slowdownFactor = 0.5f;

    private float m_rotationSmoothTime = 0.1f;
    private float m_rotationVelocity;

    [SerializeField] private bool m_isJumping = false;
    [SerializeField] private float m_jumpForce = 3000.0f;

    // ===================================

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Non physics-based update
    /// </summary>
    private void Update()
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            GetInputs();
            SetSpeed();
        }

        UpdateAnimatorParameters();
    }

    private void GetInputs()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
        m_jumpInput = Input.GetButton("Jump");
    }

    private void SetSpeed()
    {
        m_speed = m_isGrounded ? m_defaultSpeed : m_airSpeed;
    }

    private void UpdateAnimatorParameters()
    {
        m_animator.SetFloat("Speed", m_rigidbody.velocity.magnitude);
        m_animator.SetBool("IsJumping", m_isJumping);
        m_animator.SetBool("IsGrounded", m_isGrounded);
    }

    /// <summary>
    /// Physics-based movements update
    /// </summary>
    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            GroundCheck();

            Move();

            if (m_isGrounded)
            {
                if (m_jumpInput && !m_isJumping)
                {
                    Jump();
                }
                else
                {
                    m_isJumping = false;
                }
            }
        }
    }

    private void GroundCheck()
    {
        m_isGrounded = Physics.CheckSphere(transform.position, m_groundCheckRadius, m_groundLayer);
    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(m_horizontalInput, 0f, m_verticalInput).normalized;

        if (moveDirection.magnitude >= m_moveThreshold)
        {
            if (m_isGrounded)
            {
                SoundManager.Instance.PlaySound("walk");
            }
            else if (!m_isJumping)
            {
                SoundManager.Instance.PauseLoopSound();
            }

            if (!m_isMoving)
            {
                m_isMoving = true;
            }

            ApplyVelocity(moveDirection);
            Rotate(moveDirection);
        }
        else if (m_isGrounded)
        {
            if (m_isMoving)
            {
                m_isMoving = false;
                SoundManager.Instance.PauseLoopSound();
            }

            SlowdownVelocity();
        }
    }

    private void ApplyVelocity(Vector3 moveDirection)
    {
        m_rigidbody.velocity = m_rigidbody.velocity + (moveDirection * m_speed * Time.fixedDeltaTime);
    }

    private void SlowdownVelocity()
    {
        m_rigidbody.velocity *= m_slowdownFactor;
        m_rigidbody.angularVelocity *= m_slowdownFactor;
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
        SoundManager.Instance.PlaySound("jump");
        m_isJumping = true;
        m_rigidbody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Debugging gizmos
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Ground check
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_groundCheckRadius);
    }
}
