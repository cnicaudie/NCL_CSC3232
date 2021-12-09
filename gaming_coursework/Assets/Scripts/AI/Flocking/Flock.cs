using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    [Header("Flock Agents")]
    public FlockAgent agentPrefab;
    private List<FlockAgent> m_flockAgents;

    private const float k_agentDensity = 0.9f;

    [Range(5, 50)]
    public int startCount = 5;

    [Range(1f, 100f)]
    public float maxSpeed = 10f;

    [Header("Behaviour Weights")]

    [Range(0f, 10f)]
    public float alignmentWeight = 1;
    
    [Range(0f, 10f)]
    public float avoidanceWeight = 1;

    [Range(0f, 10f)]
    public float cohesionWeight = 1;
    
    [Header("Neighbour Detection")]

    [Range(1f, 10f)]
    public float neighbourRadius = 2f; // TODO : add different distance per behaviour ?
    private float m_squareNeighbourRadius;

    [Header("Avoidance Neighbour Detection")]

    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    private float m_squareAvoidanceRadius;
    public float SquareAvoidanceRadius
    {
        get { return m_squareAvoidanceRadius; }
    }

    // ===================================

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_flockAgents = new List<FlockAgent>();

        m_squareNeighbourRadius = Mathf.Pow(neighbourRadius, 2);
        m_squareAvoidanceRadius = m_squareNeighbourRadius * Mathf.Pow(avoidanceRadiusMultiplier, 2);

        SpawnAgents();
    }

    private void Update()
    {
        UpdateFlock();
    }

    private void SpawnAgents()
    {
        for (int i = 0; i < startCount; i++)
        {
            Vector2 randomPosition = k_agentDensity * startCount * Random.insideUnitCircle;
            Vector3 agentPosition = new Vector3(randomPosition.x, 3f, randomPosition.y);
            Quaternion agentRotation = Quaternion.Euler(transform.up * Random.Range(0f, 360f));

            FlockAgent newAgent = Instantiate(agentPrefab, agentPosition, agentRotation, transform);

            newAgent.name = "Agent_" + i;
            m_flockAgents.Add(newAgent);
        }
    }

    private void UpdateFlock()
    {
        foreach (FlockAgent agent in m_flockAgents)
        {
            // Get local neighbours
            List<Transform> neighbours = GetNeighbours(agent);

            // Compute each behaviour move
            Vector3 alignmentMove = ComputeAlignmentMove(agent, neighbours) * alignmentWeight;
            Vector3 avoidanceMove = ComputeAvoidanceMove(agent, neighbours) * avoidanceWeight;
            Vector3 cohesionMove = ComputeCohesionMove(agent, neighbours) * cohesionWeight;

            // TODO : Add obstacle avoidance + random target for agent with no neighbour ?

            // Compute the final move direction
            Vector3 move = alignmentMove + avoidanceMove + cohesionMove;
            //move = Vector3.SmoothDamp(agent.transform.forward, move, ref currentVelocity, smoothDamp);
            move = move.normalized * maxSpeed;

            if (move == Vector3.zero)
            {
                move = transform.forward;
            }

            // Apply move to agent
            agent.Move(move);
        }
    }

    private List<Transform> GetNeighbours(FlockAgent agent)
    {
        List<Transform> neighbours = new List<Transform>();

        Collider[] neighboursColliders = Physics.OverlapSphere(agent.transform.position, neighbourRadius);

        foreach (Collider collider in neighboursColliders)
        {
            if (collider != agent.Collider)
            {
                neighbours.Add(collider.transform);
            }
        }

        return neighbours;
    }

    private Vector3 ComputeAlignmentMove(FlockAgent agent, List<Transform> neighbours)
    {
        if (neighbours.Count == 0)
        {
            return agent.transform.forward;
        }

        Vector3 alignmentMove = agent.transform.forward;

        foreach (Transform neighbour in neighbours)
        {
            alignmentMove += neighbour.forward;
        }

        alignmentMove /= neighbours.Count;
        alignmentMove = alignmentMove.normalized;
        alignmentMove.y = 0f;

        return alignmentMove;
    }

    private Vector3 ComputeAvoidanceMove(FlockAgent agent, List<Transform> neighbours)
    {
        if (neighbours.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 avoidanceMove = Vector3.zero;
        int neighboursToAvoid = 0;

        foreach (Transform neighbour in neighbours)
        {
            if (Vector3.SqrMagnitude(neighbour.position - agent.transform.position) < m_squareAvoidanceRadius)
            {
                neighboursToAvoid++;
                avoidanceMove += agent.transform.position - neighbour.position;
            }
        }

        if (neighboursToAvoid > 0)
        {
            avoidanceMove /= neighboursToAvoid;
        }

        avoidanceMove = avoidanceMove.normalized;
        avoidanceMove.y = 0f;

        return avoidanceMove;
    }

    private Vector3 ComputeCohesionMove(FlockAgent agent, List<Transform> neighbours)
    {
        if (neighbours.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 cohesionMove = Vector3.zero;

        foreach (Transform neighbour in neighbours)
        {
            cohesionMove += neighbour.position;
        }

        cohesionMove /= neighbours.Count;
        cohesionMove -= agent.transform.position;
        cohesionMove = cohesionMove.normalized;
        cohesionMove.y = 0f;

        return cohesionMove;
    }
}
