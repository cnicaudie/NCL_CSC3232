using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private Animator m_animator;
    private NavMeshAgent m_agent;

    [SerializeField] private Player m_player;

    [SerializeField] private float m_maxPickNewPositionDistance = 15f;
    private float m_pickNewPositionThreshold = 0.5f;
    private float m_pickNewPositionCooldown = 5f;
    private float m_lastPositionPickTime = 0f;

    [SerializeField] private bool m_wasHit = false;
    public bool WasHit
    {
        get { return m_wasHit; }
    }

    [SerializeField] private bool m_isDizzy = false;
    public bool IsDizzy
    {
        get { return m_isDizzy; }
    }

    private float m_hitCooldown = 1.5f;
    private float m_dizzyCooldown = 3f;
    private float m_lastHitTime = 0f;

    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!m_wasHit && !m_isDizzy)
        {
            m_lastPositionPickTime += Time.deltaTime;

            if (m_agent.remainingDistance < m_pickNewPositionThreshold && m_lastPositionPickTime >= m_pickNewPositionCooldown)
            {
                PickRandomPosition();
            }
        }
        else
        {
            m_lastHitTime += Time.deltaTime;

            if (m_isDizzy && m_lastHitTime >= m_dizzyCooldown)
            {
                m_isDizzy = false;
                m_agent.isStopped = false;
            }
            else if (m_wasHit && m_lastHitTime >= m_hitCooldown)
            {
                m_wasHit = false;
                m_agent.isStopped = false;
            }
        }

        UpdateAnimatorParameters();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!m_wasHit && !m_isDizzy)
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                m_wasHit = true;
                m_lastHitTime = 0f;
                m_agent.isStopped = true;
            }
            else if (collision.gameObject.CompareTag("Pickable"))
            {
                Pickable pickableObject = collision.gameObject.GetComponent<Pickable>();

                if (pickableObject != null && pickableObject.WasThrown)
                {
                    m_isDizzy = true;
                    m_lastHitTime = 0f;
                    m_agent.isStopped = true;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_maxPickNewPositionDistance);

        if (m_agent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(m_agent.destination, new Vector3(0.5f, 0.5f, 0.5f));
        }
    }

    private void UpdateAnimatorParameters()
    {
        m_animator.SetFloat("Speed", m_agent.velocity.magnitude);
        m_animator.SetBool("WasHit", m_wasHit);
        m_animator.SetBool("IsDizzy", m_isDizzy);
    }

    private void PickRandomPosition()
    {
        Vector3 destination = transform.position;
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle * m_maxPickNewPositionDistance;

        destination.x += randomDirection.x;
        destination.z += randomDirection.y;

        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, m_maxPickNewPositionDistance, NavMesh.AllAreas);

        m_agent.destination = navHit.position;

        m_lastPositionPickTime = 0f;
    }
}