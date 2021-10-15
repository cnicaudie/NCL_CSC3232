using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody m_rigidbody;

    private float m_horizontalInput;
    private float m_verticalInput;

    [SerializeField] private float m_speed = 100f;

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

    private void Move()
    {
        Vector3 velocity = m_rigidbody.velocity;

        velocity.x += m_horizontalInput * m_speed * Time.fixedDeltaTime;
        velocity.z += m_verticalInput * m_speed * Time.fixedDeltaTime;

        m_rigidbody.velocity = velocity;

        /*
        Vector3 moveDirection = new Vector3(m_horizontalInput, 0f, m_verticalInput).normalized;
        Vector3 positionOffset = moveDirection * m_speed * Time.fixedDeltaTime;

        m_rigidbody.MovePosition(m_rigidbody.position + positionOffset);
        */
    }
}
