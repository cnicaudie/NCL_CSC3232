using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    private Animator m_animator;
    private NavMeshAgent m_agent;
    
    private Player m_player;

    public enum EnemyState
    {
        Idle, Walk, Chase, Attack, Dizzy, Hit, Dead
    }
    [SerializeField] private EnemyState m_state;

    private float m_distanceThreshold = 0.5f;
    private float m_pickNewPositionCooldown = 3f;
    private float m_lastPositionPickTime = 0f;

    private float m_dizzyCooldown = 3f;
    
    // ===================================

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        m_health = GetComponent<Health>();

        m_player = FindObjectOfType<Player>();

        m_state = EnemyState.Idle;
    }

    protected override void Update()
    {
        base.Update();
        UpdateTimers();
        UpdateStates();
        UpdateAnimatorParameters();
    }

    private void UpdateTimers()
    {
        m_lastPositionPickTime += Time.deltaTime;
    }

    private void UpdateStates()
    {
        switch (m_state)
        {
            case EnemyState.Idle: Idle(); break;

            case EnemyState.Walk: Walk(); break;

            case EnemyState.Chase: Chase(); break;

            case EnemyState.Attack: Attack(); break;

            case EnemyState.Dizzy: Dizzy(); break;

            case EnemyState.Hit: Hit(); break;

            default:
                m_agent.isStopped = true;
                break;
        }
    }

    private void Idle()
    {
        if (!m_agent.isStopped)
            m_agent.isStopped = true;

        if (m_lastPositionPickTime >= m_pickNewPositionCooldown)
        {
            m_state = EnemyState.Walk;
        }
    }

    private void Walk()
    {
        if (m_agent.isStopped)
            m_agent.isStopped = false;

        if (Vector3.Distance(transform.position, m_player.transform.position) < m_actionRange)
        {
            m_state = EnemyState.Chase;
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

    private void Chase()
    {
        if (Vector3.Distance(transform.position, m_player.transform.position) > m_actionRange)
        {
            m_state = EnemyState.Idle;
        }
        else
        {
            m_agent.destination = m_player.transform.position;
        }
    }

    private void Attack()
    {
        if (!m_agent.isStopped)
            m_agent.isStopped = true;

        transform.LookAt(m_player.transform);
    }

    private void Dizzy()
    {
        if (!m_agent.isStopped)
            m_agent.isStopped = true;

        if (m_lastDamageTime >= m_dizzyCooldown)
        {
            m_state = EnemyState.Idle;
        }
    }

    private void Hit()
    {
        if (!m_agent.isStopped)
            m_agent.isStopped = true;

        if (m_lastDamageTime >= m_damageCooldown)
        {
            m_state = EnemyState.Idle;
        }
    }

    private void UpdateAnimatorParameters()
    {
        m_animator.SetFloat("Speed", m_agent.velocity.magnitude);
        m_animator.SetBool("IsAttacking", IsAttacking());
        m_animator.SetBool("WasHit", WasHit());
        m_animator.SetBool("IsDizzy", IsDizzy());
        m_animator.SetBool("IsDead", m_isDead);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && m_state == EnemyState.Chase)
        {
            m_state = EnemyState.Attack;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && m_state == EnemyState.Attack)
        {
            // TODO : Hide state
            m_state = EnemyState.Idle;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_actionRange);

        if (m_agent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(m_agent.destination, new Vector3(0.5f, 0.5f, 0.5f));
        }
    }

    public void Damage(float damageAmount, bool wasShot)
    {
        Debug.Log("DAMAGE ENEMY");

        // Compute and apply damage
        float finalDamage = wasShot ? damageAmount : damageAmount / 2f;

        base.Damage(finalDamage);

        // Compute next state
        if (m_isDead)
        {
            m_state = EnemyState.Dead;
        }
        else if (wasShot)
        {
            m_state = EnemyState.Hit;
        }
        else
        {
            m_state = EnemyState.Dizzy;
        }
    }

    public bool IsAttacking()
    {
        return m_state == EnemyState.Attack;
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
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle * m_actionRange;

        destination.x += randomDirection.x;
        destination.z += randomDirection.y;

        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, m_actionRange, NavMesh.AllAreas);

        m_agent.destination = navHit.position;

        m_lastPositionPickTime = 0f;
    }
}