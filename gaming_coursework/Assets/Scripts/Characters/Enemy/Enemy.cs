using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Animator m_animator;
    private NavMeshAgent m_agent;
    private Health m_health;

    private Player m_player;

    public enum EnemyState
    {
        Idle, Walk, ChasePlayer, Dizzy, Hit, Dead
    }
    [SerializeField] private EnemyState m_state;

    [SerializeField] private float m_actionRadius = 7f;

    private float m_distanceThreshold = 0.5f;
    private float m_pickNewPositionCooldown = 3f;
    private float m_lastPositionPickTime = 0f;

    private float m_hitCooldown = 1.5f;
    private float m_dizzyCooldown = 3f;
    private float m_lastHitTime = 0f;

    // ===================================

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        m_health = GetComponent<Health>();

        m_player = FindObjectOfType<Player>();

        m_state = EnemyState.Idle;
    }

    private void Update()
    {
        UpdateTimers();
        UpdateStates();
        UpdateAnimatorParameters();
    }

    private void UpdateTimers()
    {
        m_lastPositionPickTime += Time.deltaTime;
        m_lastHitTime += Time.deltaTime;
    }

    private void UpdateStates()
    {
        switch (m_state)
        {
            case EnemyState.Idle: UpdateIdle(); break;

            case EnemyState.Walk: UpdateWalk(); break;

            case EnemyState.ChasePlayer: UpdateChasePlayer(); break;

            case EnemyState.Dizzy: UpdateDizzy(); break;

            case EnemyState.Hit: UpdateHit(); break;

            default:
                m_agent.isStopped = true;
                break;
        }
    }

    private void UpdateIdle()
    {
        if (!m_agent.isStopped)
            m_agent.isStopped = true;

        if (m_lastPositionPickTime >= m_pickNewPositionCooldown)
        {
            m_state = EnemyState.Walk;
        }
    }

    private void UpdateWalk()
    {
        if (m_agent.isStopped)
            m_agent.isStopped = false;

        if (Vector3.Distance(transform.position, m_player.transform.position) < m_actionRadius)
        {
            m_state = EnemyState.ChasePlayer;
        }
        else if (m_agent.remainingDistance < m_distanceThreshold)
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

    private void UpdateChasePlayer()
    {
        if (Vector3.Distance(transform.position, m_player.transform.position) > m_actionRadius)
        {
            m_state = EnemyState.Idle;
        }
        else
        {
            m_agent.destination = m_player.transform.position;
        }
    }

    private void UpdateDizzy()
    {
        if (!m_agent.isStopped)
            m_agent.isStopped = true;

        if (m_lastHitTime >= m_dizzyCooldown)
        {
            m_state = EnemyState.Idle;
        }
    }

    private void UpdateHit()
    {
        if (!m_agent.isStopped)
            m_agent.isStopped = true;

        if (m_lastHitTime >= m_hitCooldown)
        {
            m_state = EnemyState.Idle;
        }
    }

    private void UpdateAnimatorParameters()
    {
        m_animator.SetFloat("Speed", m_agent.velocity.magnitude);
        m_animator.SetBool("WasHit", WasHit());
        m_animator.SetBool("IsDizzy", IsDizzy());
        m_animator.SetBool("IsDead", m_health.IsDead);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!WasHit())
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                m_lastHitTime = 0f;

                // TODO : get the damage amount from the bullet / hit area
                m_health.Damage(10f);

                m_state = m_health.IsDead ? EnemyState.Dead : EnemyState.Hit;
            }
            else if (collision.gameObject.CompareTag("Pickable") && !IsDizzy())
            {
                Pickable pickableObject = collision.gameObject.GetComponent<Pickable>();

                if (pickableObject != null && pickableObject.WasThrown)
                {
                    m_lastHitTime = 0f;
                    m_state = EnemyState.Dizzy;

                    // TODO : Damage a little ?
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_actionRadius);

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

    private void PickRandomPosition()
    {
        Vector3 destination = transform.position;
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle * m_actionRadius;

        destination.x += randomDirection.x;
        destination.z += randomDirection.y;

        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, m_actionRadius, NavMesh.AllAreas);

        m_agent.destination = navHit.position;

        m_lastPositionPickTime = 0f;
    }
}