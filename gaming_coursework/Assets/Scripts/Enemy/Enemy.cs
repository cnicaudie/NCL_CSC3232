using System.Collections;
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

    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        m_lastPositionPickTime += Time.deltaTime;

        if (m_agent.remainingDistance < m_pickNewPositionThreshold && m_lastPositionPickTime >= m_pickNewPositionCooldown)
        {
            m_lastPositionPickTime = 0f;
            m_agent.destination = PickRandomPosition();
        }

        UpdateAnimatorParameters();
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
        //m_animator.SetBool("IsJumping", m_isJumping);
    }

    private Vector3 PickRandomPosition()
    {
        Debug.Log("Picking a new random position");

        Vector3 destination = transform.position;
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle * m_maxPickNewPositionDistance;
        destination.x += randomDirection.x;
        destination.z += randomDirection.y;

        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, m_maxPickNewPositionDistance, NavMesh.AllAreas);

        return navHit.position;
    }
}