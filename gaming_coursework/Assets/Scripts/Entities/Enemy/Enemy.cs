using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles AI enemy state-based behaviours
/// </summary>
public class Enemy : Entity
{
    public enum EnemyState
    {
        Idle, Walk, Chase, Attack, Hide, Dizzy, Hit, Dead
    }

    // ===================================
    // ATTRIBUTES
    // ===================================

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

        m_state = EnemyState.Idle;
    }

    protected override void Update()
    {
        if (GameManager.IsGamePlaying())
        {
            base.Update();

            UpdateTimers();
            UpdateStates();
            UpdateAnimatorParameters();
        }
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    #region UPDATE_ENEMY

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
    /// State-based update behaviour
    /// </summary>
    private void UpdateStates()
    {
        switch (m_state)
        {
            case EnemyState.Idle: Idle(); break;

            case EnemyState.Walk: Walk(); break;

            case EnemyState.Chase: Chase(); break;

            case EnemyState.Attack: Attack(); break;

            case EnemyState.Hide: Hide(); break;

            case EnemyState.Dizzy: Dizzy(); break;

            case EnemyState.Hit: Hit(); break;

            default:
                m_agent.isStopped = true;
                break;
        }
    }

    #endregion // UPDATE_ENEMY

    #region ENEMY_STATES

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

    private void Hide()
    {
        if (m_agent.isStopped)
            m_agent.isStopped = false;

        if (m_agent.remainingDistance < m_distanceThreshold)
        {
            m_state = EnemyState.Idle;
        }
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

    #endregion // ENEMY_STATES

    private void PickRandomPosition()
    {
        Vector3 nextDestination;

        if (LevelManager.GetRandomPosition(transform.position, m_actionRange, out nextDestination))
        {
            m_agent.destination = nextDestination;
        }

        m_lastPositionPickTime = 0f;
    }

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
            m_state = EnemyState.Attack;
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
            m_state = EnemyState.Hide;
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