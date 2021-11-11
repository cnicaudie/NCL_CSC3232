using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles AI enemy state-based (using Hierarchical State Machine) behaviours
/// </summary>
public class Enemy : Entity
{
    // Hierarchical State Machine enumerations

    // Note : enums are not the most practical way of doing FSM/HSM
    //        but it was easier to implement with the short time allowed

    public enum EnemySuperState
    {
        Patrol, Offense, Damaged
    }

    public enum EnemyState
    {
        Idle, Walk, Chase, Attack, Hide, Dizzy, Hit, Dead
    }

    // ===================================
    // ATTRIBUTES
    // ===================================

    [SerializeField] private EnemySuperState m_superState;
    [SerializeField] private EnemyState m_state;

    private Animator m_animator;
    private NavMeshAgent m_agent;
    private GameObject[] m_hidespots;
    private Player m_player;

    private float m_distanceThreshold = 0.5f;
    private float m_pickNewPositionCooldown = 3f;
    private float m_lastPositionPickTime = 0f;

    private float m_dizzyCooldown = 3f;

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    /// <summary>
    /// Applies a damage when shot or stunned (based on the hit area)
    /// Computes the next damaged state
    /// </summary>
    /// <param name="damageAmount"></param>
    /// <param name="wasShot"></param>
    public void Damage(float damageAmount, bool wasShot)
    {
        Debug.Log("DAMAGE ENEMY");

        // Compute and apply damage
        float finalDamage = wasShot ? damageAmount : damageAmount / 2f;

        base.Damage(finalDamage);

        // Compute next state
        if (m_isDead)
        {
            SetState(EnemyState.Dead);
        }
        else if (wasShot)
        {
            SetState(EnemyState.Hit);
        }
        else
        {
            SetState(EnemyState.Dizzy);
        }
    }

    public bool IsAttacking()
    {
        return m_state == EnemyState.Attack;
    }

    // ===================================
    // PROTECTED METHODS
    // ===================================

    protected override void Start()
    {
        base.Start();

        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
        m_hidespots = GameObject.FindGameObjectsWithTag("Hidespot");
        m_player = FindObjectOfType<Player>();

        SetState(EnemyState.Idle);
    }

    protected override void Update()
    {
        if (GameManager.IsGamePlaying())
        {
            base.Update();

            UpdateTimers();
            UpdateCurrentState();
            UpdateAnimatorParameters();
            ComputeNextState();
        }
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void UpdateTimers()
    {
        m_lastPositionPickTime += Time.deltaTime;
    }

    private void UpdateAnimatorParameters()
    {
        m_animator.SetFloat("Speed", m_agent.velocity.magnitude);
        m_animator.SetBool("IsAttacking", m_state == EnemyState.Attack);
        m_animator.SetBool("WasHit", m_state == EnemyState.Hit);
        m_animator.SetBool("IsDizzy", m_state == EnemyState.Dizzy);
        m_animator.SetBool("IsDead", m_isDead);
    }

    /// <summary>
    /// Updates enemy's behaviour based on its state
    /// </summary>
    private void UpdateCurrentState()
    {
        switch (m_superState)
        {
            case EnemySuperState.Patrol:
            {
                switch (m_state)
                {
                    case EnemyState.Idle: StopAgent(true); break;

                    case EnemyState.Walk: Walk(); break;

                    default:
                        StopAgent(true);
                        break;
                }
                break;
            }

            case EnemySuperState.Offense:
            {
                switch (m_state)
                {
                    case EnemyState.Chase: Chase(); break;

                    case EnemyState.Attack: Attack(); break;

                    case EnemyState.Hide: StopAgent(false); break;

                    default:
                        StopAgent(true);
                        break;
                }
                break;
            }

            case EnemySuperState.Damaged:
            {
                StopAgent(true);
                break;
            }
        }
    }

    /// <summary>
    /// Computes next state based on current state and environment
    /// </summary>
    private void ComputeNextState()
    {
        switch (m_superState)
        {
            case EnemySuperState.Patrol:
            {
                if (Vector3.Distance(transform.position, m_player.transform.position) < m_actionRange)
                {
                    SetState(EnemyState.Chase);
                    break;
                }

                switch (m_state)
                {
                    case EnemyState.Idle:
                        if (m_lastPositionPickTime >= m_pickNewPositionCooldown)
                        {
                            SetState(EnemyState.Walk);
                        }
                        break;

                    case EnemyState.Walk:
                        if (m_agent.remainingDistance < m_distanceThreshold && m_lastPositionPickTime < m_pickNewPositionCooldown)
                        {
                            SetState(EnemyState.Idle);
                        }
                        break;

                    default:
                        break;
                }

                break;
            }

            case EnemySuperState.Offense:
            {
                if (m_state == EnemyState.Hide)
                {
                    if (m_agent.remainingDistance < m_distanceThreshold)
                    {
                        SetState(EnemyState.Idle);
                    }
                }
                else if (Vector3.Distance(transform.position, m_player.transform.position) > m_actionRange)
                {
                    SetState(EnemyState.Idle);
                }
                break;
            }

            case EnemySuperState.Damaged:
            {
                switch (m_state)
                {
                    case EnemyState.Dizzy:
                        if (m_lastDamageTime >= m_dizzyCooldown)
                        {
                            SetState(EnemyState.Idle);
                        }
                        break;

                    case EnemyState.Hit:
                        if (m_lastDamageTime >= m_damageCooldown)
                        {
                            SetState(EnemyState.Idle);
                        }
                        break;

                    default:
                        break;
                }
                break;
            }

            default:
                break;
        }
    }

    /// <summary>
    /// Sets new state and superstate
    /// </summary>
    /// <param name="state"></param>
    private void SetState(EnemyState state)
    {
        m_state = state;

        if (state == EnemyState.Idle || state == EnemyState.Walk)
        {
            m_superState = EnemySuperState.Patrol;
        }
        else if (state == EnemyState.Chase || state == EnemyState.Attack || state == EnemyState.Hide)
        {
            m_superState = EnemySuperState.Offense;
        }
        else if (state == EnemyState.Dizzy || state == EnemyState.Hit || state == EnemyState.Dead)
        {
            m_superState = EnemySuperState.Damaged;
        }
    }

    /// <summary>
    /// Start/Stop the NavMeshAgent
    /// </summary>
    /// <param name="isStopped"></param>
    private void StopAgent(bool isStopped)
    {
        if (m_agent.isStopped != isStopped)
        {
            m_agent.isStopped = isStopped;
        }
    }

    private void Walk()
    {
        StopAgent(false);

        if (m_agent.remainingDistance < m_distanceThreshold && m_lastPositionPickTime >= m_pickNewPositionCooldown)
        {
            PickRandomPosition();
        }
    }

    private void Chase()
    {
        StopAgent(false);

        m_agent.destination = m_player.transform.position;
    }

    private void Attack()
    {
        StopAgent(true);

        transform.LookAt(m_player.transform);
    }

    /// <summary>
    /// Picks a random position from the level map as a new destination
    /// </summary>
    private void PickRandomPosition()
    {
        Vector3 nextDestination;

        if (LevelManager.GetRandomPosition(transform.position, m_actionRange, out nextDestination))
        {
            m_agent.destination = nextDestination;
        }

        m_lastPositionPickTime = 0f;
    }

    /// <summary>
    /// Picks a random hidespot to hide from the player
    /// </summary>
    private void PickRandomHidespot()
    {
        if (m_hidespots.Length > 0)
        {
            int randomIndex = Random.Range(0, m_hidespots.Length);
            m_agent.destination = m_hidespots[randomIndex].transform.position;
        }
        else
        {
            Debug.LogError("No hidespots are defined !");
        }
    }

    /// <summary>
    /// Trigger enter response and feedback
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && m_state == EnemyState.Chase)
        {
            SetState(EnemyState.Attack);
        }
    }

    /// <summary>
    /// Trigger exit response and feedback
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && m_state == EnemyState.Attack)
        {
            PickRandomHidespot();
            SetState(EnemyState.Hide);
        }
    }

    /// <summary>
    /// Debugging gizmos
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Action range sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_actionRange);

        // Agent's next destination
        if (m_agent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(m_agent.destination, new Vector3(0.5f, 0.5f, 0.5f));
        }
    }
}