using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Animator m_animator;
    private NavMeshAgent m_agent;
    private Player m_player;

    public enum EnemyState
    {
        Idle, Walk, Dizzy, Hit
    }
    [SerializeField] private EnemyState m_state;

    [SerializeField] private float m_maxPickNewPositionDistance = 15f;
    private float m_pickNewPositionThreshold = 0.5f;
    private float m_pickNewPositionCooldown = 5f;
    private float m_lastPositionPickTime = 0f;

    private float m_hitCooldown = 1.5f;
    private float m_dizzyCooldown = 3f;
    private float m_lastHitTime = 0f;

    // ===================================

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        m_player = FindObjectOfType<Player>();

        m_state = EnemyState.Idle;
    }

    private void Update()
    {
        UpdateStates();
        UpdateAnimatorParameters();
    }

    private void UpdateStates()
    {
        switch (m_state)
        {
            case EnemyState.Idle: UpdateIdle(); break;

            case EnemyState.Walk: UpdateWalk(); break;

            case EnemyState.Dizzy: UpdateDizzy(); break;

            case EnemyState.Hit: UpdateHit(); break;

            default: break;
        }
    }

    private void UpdateIdle()
    {
        m_lastPositionPickTime += Time.deltaTime;

        if (!m_agent.isStopped)
            m_agent.isStopped = true;

        if (m_lastPositionPickTime >= m_pickNewPositionCooldown)
        {
            m_state = EnemyState.Walk;
        }
    }

    private void UpdateWalk()
    {
        m_lastPositionPickTime += Time.deltaTime;

        if (m_agent.isStopped)
            m_agent.isStopped = false;

        if (m_agent.remainingDistance < m_pickNewPositionThreshold)
        {
            if (m_lastPositionPickTime < m_pickNewPositionCooldown)
            {
                m_state = EnemyState.Idle;
            }
            else
            {
                PickRandomPosition();
            }
        }
    }

    private void UpdateDizzy()
    {
        m_lastHitTime += Time.deltaTime;

        if (!m_agent.isStopped)
            m_agent.isStopped = true;

        if (m_lastHitTime >= m_dizzyCooldown)
        {
            m_state = EnemyState.Idle;
        }
    }

    private void UpdateHit()
    {
        m_lastHitTime += Time.deltaTime;

        if (!m_agent.isStopped)
            m_agent.isStopped = true;

        if (m_lastHitTime >= m_hitCooldown)
        {
            m_state = EnemyState.Idle;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!WasHit())
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                m_lastHitTime = 0f;
                m_state = EnemyState.Hit;
            }
            else if (collision.gameObject.CompareTag("Pickable") && !IsDizzy())
            {
                Pickable pickableObject = collision.gameObject.GetComponent<Pickable>();

                if (pickableObject != null && pickableObject.WasThrown)
                {
                    m_lastHitTime = 0f;
                    m_state = EnemyState.Dizzy;
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

    public bool IsDizzy()
    {
        return m_state == EnemyState.Dizzy;
    }

    public bool WasHit()
    {
        return m_state == EnemyState.Hit;
    }

    private void UpdateAnimatorParameters()
    {
        m_animator.SetFloat("Speed", m_agent.velocity.magnitude);
        m_animator.SetBool("WasHit", WasHit());
        m_animator.SetBool("IsDizzy", IsDizzy());
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