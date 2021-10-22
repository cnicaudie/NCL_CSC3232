using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private ParticleSystem m_thrustParticles;
    
    [SerializeField] private List<Transform> m_hoverPoints;
    [SerializeField] private float m_hoverHeight;
    private float m_hoverHeightMin = 10f;
    private float m_hoverHeightMax;
    private bool m_isHovering = false;
    private bool m_isAtMaxHeight = false;

    private float m_thrustInput;
    private float m_thrustForce = 500.0f;
    private float m_thrustSlowdownFactor = 0.95f;
    private float m_planetSpeed;

    private float m_rotationInput;
    private float m_rotationSlowdownFactor = 0.5f;

    private float m_slowdownThreshold = 0.1f;

    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        m_thrustParticles = GetComponentInChildren<ParticleSystem>();
        EnableParticles(false);
    }

    private void Update()
    {
        GetInputs();
        SetParticlesSpeed();
    }

    private void FixedUpdate()
    {
        if (m_isHovering)
        {
            Hover();
            ApplyThrust();
            ApplyRotation();

        }

        SlowdownVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LevelPoint") && !m_isHovering)
        {
            Debug.Log("Spaceship landed on a level point !");
        }
    }

    public void EnterNewOrbit(int orbitHeight)
    {
        m_hoverHeight = m_hoverHeightMin;
        m_isAtMaxHeight = false;
        m_hoverHeightMax = m_hoverHeightMin + orbitHeight;
    }

    private void GetInputs()
    {
        m_thrustInput = Input.GetAxisRaw("RocketThrust");
        m_rotationInput = Input.GetAxisRaw("RocketRotation");

        // TODO : make buttons
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartAndStop();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMaxHoverHeight();
        }
    }

    private void EnableParticles(bool enabled)
    {
        var emission = m_thrustParticles.emission;
        emission.enabled = enabled;
    }

    private void SetParticlesSpeed()
    {
        var main = m_thrustParticles.main;
        main.startSpeed = m_thrustInput <= m_slowdownThreshold ? 0.5f : 5.0f;
    }

    private void StartAndStop()
    {
        m_isHovering = m_isHovering ? false : true;
        EnableParticles(m_isHovering);
    }

    private void ToggleMaxHoverHeight()
    {
        m_hoverHeight = m_isAtMaxHeight ? m_hoverHeightMin : m_hoverHeightMax;
        m_isAtMaxHeight = !m_isAtMaxHeight;
    }

    private void SlowdownVelocity()
    {
        if (m_thrustInput <= m_slowdownThreshold)
        {
            m_rigidbody.velocity *= m_thrustSlowdownFactor;   
        }

        if (m_rigidbody.angularVelocity.magnitude > m_slowdownThreshold)
        {
            m_rigidbody.angularVelocity *= m_rotationSlowdownFactor;
        }
    }

    private void Hover()
    {
        int layerMask = ~LayerMask.NameToLayer("Planet");
        RaycastHit hitInfos;

        for (int i = 0; i < m_hoverPoints.Count; i++)
        {
            Transform hoverPoint = m_hoverPoints[i];

            if (Physics.Raycast(hoverPoint.position, -transform.up, out hitInfos, m_hoverHeight, layerMask, QueryTriggerInteraction.Ignore))
            {
                Debug.DrawRay(hoverPoint.position, -transform.up * hitInfos.distance, Color.red);

                m_rigidbody.AddForceAtPosition(transform.up * m_hoverHeight, hoverPoint.position, ForceMode.Acceleration);
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
