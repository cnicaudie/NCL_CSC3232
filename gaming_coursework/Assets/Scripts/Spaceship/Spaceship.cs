using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    private Rigidbody m_rigidbody;

    [SerializeField] private List<Transform> m_hoverPoints;
    private float m_hoverHeight = 4f;
    private float m_hoverForce = 20.0f;
    private bool m_isHovering = false;

    private float m_thrustInput;
    private float m_thrustForce = 500.0f;
    private float m_thrustSlowdownFactor = 0.95f;

    private float m_rotationInput;
    private float m_rotationSlowdownFactor = 0.5f;

    // TODO : define 1 "startRocket" button

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
        if (m_isHovering)
        {
            Hover();
            ApplyThrust();
            ApplyRotation();

            if (m_thrustInput == 0.0f) // TODO : use threshold
            {
                m_rigidbody.velocity *= m_thrustSlowdownFactor;
            }

            m_rigidbody.angularVelocity *= m_rotationSlowdownFactor;
        }
    }

    private void GetInputs()
    {
        m_thrustInput = Input.GetAxisRaw("RocketThrust");
        m_rotationInput = Input.GetAxisRaw("RocketRotation");

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartAndStop();
        }
    }

    private void StartAndStop()
    {
        m_isHovering = m_isHovering ? false : true;
    }

    private void Hover()
    {
        RaycastHit hit;

        for (int i = 0; i < m_hoverPoints.Count; i++)
        {
            Transform hoverPoint = m_hoverPoints[i];

            if (Physics.Raycast(hoverPoint.position, -transform.up, out hit, m_hoverHeight))
            {
                Debug.DrawRay(hoverPoint.position, -transform.up * m_hoverHeight, Color.red);

                m_rigidbody.AddForceAtPosition(transform.up * m_hoverForce * (1.0f - (hit.distance / m_hoverHeight)), hoverPoint.position, ForceMode.Acceleration);
            }
        }
    }

    private void ApplyThrust()
    {
        Vector3 thrust = transform.forward * m_thrustForce * m_thrustInput;
        m_rigidbody.AddForce(thrust * Time.fixedDeltaTime, ForceMode.Impulse);
    }

    private void ApplyRotation()
    {
        m_rigidbody.AddRelativeTorque(0f, m_rotationInput, 0f, ForceMode.VelocityChange);
    }
}
