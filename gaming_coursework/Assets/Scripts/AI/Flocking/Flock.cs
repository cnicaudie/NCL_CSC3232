using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles every agents of the flock and their behaviour
/// </summary>
public class Flock : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    [Header("Flock Agents")]
    public FlockAgent agentPrefab;
    private List<FlockAgent> m_flockAgents;

    private const float k_agentDensity = 0.3f;

    [Range(5, 50)]
    public int startCount = 5;

    [Range(1f, 100f)]
    public float maxSpeed = 10f;

    [Header("Behaviour Weights")]

    [Range(0f, 10f)]
    public float alignmentWeight = 1f;
    
    [Range(0f, 10f)]
    public float avoidanceWeight = 1f;

    [Range(0f, 10f)]
    public float cohesionWeight = 1f;

    [Header("Obstacle Collision avoidance")]

    [Range(0f, 10f)]
    public float obstacleAvoidanceWeight = 1;

    private float m_obstacleAvoidanceRadius = 0.7f;
    private float m_maxAvoidanceDistance = 6f;

    private const int m_numViewDirections = 300;
    private Vector3[] m_avoidanceDirections;

    [SerializeField] private LayerMask m_obstacleMask;

    [Header("Target")]

    [SerializeField] private Vector3 m_targetPosition;

    [Range(0f, 10f)]
    public float targetWeight = 1f;

    private float m_lookForTargetThreshold = 15f;

    private float m_newTargetRange = 30f;
    private float m_pickNewTargetCooldown = 4f;
    private float m_lastTargetPickTime = 0f;

    [Header("Neighbour Detection")]

    [Range(1f, 10f)]
    public float neighbourRadius = 2f;
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

        if (m_targetPosition == null)
        {
            m_targetPosition = transform.position;
        }

        m_squareNeighbourRadius = Mathf.Pow(neighbourRadius, 2);
        m_squareAvoidanceRadius = m_squareNeighbourRadius * Mathf.Pow(avoidanceRadiusMultiplier, 2);

        GenerateAvoidanceDirections();

        SpawnAgents();
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            UpdateTargetPosition();
            UpdateFlock();
        }
    }

    private void SpawnAgents()
    {
        const float baseHeight = 3f;

        for (int i = 0; i < startCount; i++)
        {
            Vector2 randomPosition = k_agentDensity * startCount * Random.insideUnitCircle;
            Vector3 agentPosition = new Vector3(randomPosition.x, baseHeight, randomPosition.y);
            Quaternion agentRotation = Quaternion.Euler(transform.up * Random.Range(0f, 360f));

            // Avoid spawning agents on the void
            LevelManager.CorrectPosition(ref agentPosition);

            FlockAgent newAgent = Instantiate(agentPrefab, agentPosition, agentRotation, transform);

            newAgent.name = "Agent_" + i;
            m_flockAgents.Add(newAgent);
        }
    }

    /// <summary>
    /// Generates directions to check when avoiding a collision obstacle
    /// From : https://github.com/SebLague/Boids/blob/master/Assets/Scripts/BoidHelper.cs
    /// </summary>
    private void GenerateAvoidanceDirections()
    {
        m_avoidanceDirections = new Vector3[m_numViewDirections];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < m_numViewDirections; i++)
        {
            float t = (float)i / m_numViewDirections;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);

            m_avoidanceDirections[i] = new Vector3(x, y, z);
        }
    }

    private void UpdateTargetPosition()
    {
        m_lastTargetPickTime += Time.deltaTime;

        if (m_lastTargetPickTime >= m_pickNewTargetCooldown)
        {
            Vector3 nextTargetPosition;

            if (LevelManager.GetRandomPosition(transform.position, m_newTargetRange, out nextTargetPosition))
            {
                m_targetPosition = nextTargetPosition;
                m_lastTargetPickTime = 0f;
            }
        }
    }

    /// <summary>
    /// Updates the flock agents behaviours
    /// </summary>
    private void UpdateFlock()
    {
        // Note : the .toArray() in the foreach loop avoids getting an error 
        //        when removing an element from the list while iterating on it

        foreach (FlockAgent agent in m_flockAgents.ToArray())
        {
            // Check if agent falls off the ground
            if (agent.transform.position.y < -5f)
            {
                Debug.Log("Spider " + agent.name + " died!");
                m_flockAgents.Remove(agent);
                Destroy(agent.gameObject);
                continue;
            }

            // Get local neighbours
            List<Transform> neighbours = GetNeighbours(agent);

            // Compute each behaviour move
            Vector3 alignmentMove = ComputeAlignmentMove(agent, neighbours) * alignmentWeight;
            Vector3 avoidanceMove = ComputeAvoidanceMove(agent, neighbours) * avoidanceWeight;
            Vector3 cohesionMove = ComputeCohesionMove(agent, neighbours) * cohesionWeight;

            // Compute movement towards target
            Vector3 targetMove = ComputeTargetMove(agent) * targetWeight;

            // Compute obstacle avoidance move
            Vector3 obstacleAvoidanceMove = ComputeObstacleAvoidanceMove(agent) * obstacleAvoidanceWeight;

            // Compute the final move direction
            Vector3 move = alignmentMove + avoidanceMove + cohesionMove + targetMove + obstacleAvoidanceMove;

            if (move == Vector3.zero)
            {
                move = agent.transform.forward;
            }

            // Apply velocity to agent
            agent.Move(move.normalized * maxSpeed);
        }
    }

    /// <summary>
    /// Retrives a list of the agent's neighbours in a given radius
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
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

        // Compute the average of the flock group forward direction
        Vector3 alignmentMove = agent.transform.forward;

        foreach (Transform neighbour in neighbours)
        {
            alignmentMove += neighbour.forward;
        }

        alignmentMove /= neighbours.Count;

        return NormalizeMoveVector(alignmentMove);
    }

    private Vector3 ComputeAvoidanceMove(FlockAgent agent, List<Transform> neighbours)
    {
        if (neighbours.Count == 0)
        {
            return Vector3.zero;
        }

        // Compute the average offset to avoid surrounding flock agents 
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

        return NormalizeMoveVector(avoidanceMove);
    }

    private Vector3 ComputeCohesionMove(FlockAgent agent, List<Transform> neighbours)
    {
        if (neighbours.Count == 0)
        {
            return PickRandomAgentPosition();
        }

        // Compute average center position of the flock group
        Vector3 cohesionMove = Vector3.zero;

        foreach (Transform neighbour in neighbours)
        {
            cohesionMove += neighbour.position;
        }

        cohesionMove /= neighbours.Count;
        cohesionMove -= agent.transform.position;
        
        return NormalizeMoveVector(cohesionMove);
    }

    private Vector3 ComputeTargetMove(FlockAgent agent)
    {
        Vector3 targetOffset = m_targetPosition - agent.transform.position;

        if (targetOffset.magnitude > m_lookForTargetThreshold)
        {
            return NormalizeMoveVector(targetOffset);
        }

        return Vector3.zero;
    }

    private Vector3 ComputeObstacleAvoidanceMove(FlockAgent agent)
    {
        Vector3 agentPosition = agent.transform.position;
        Vector3 agentDirection = agent.transform.forward;

        RaycastHit hit;

        // Check if a collision is going to happen
        if (Physics.SphereCast(agentPosition, m_obstacleAvoidanceRadius, agentDirection,
            out hit, m_maxAvoidanceDistance, m_obstacleMask, QueryTriggerInteraction.Ignore))
        {
            // Look fpr the best direction to avoid the obstacle
            for (int i = 0; i < m_avoidanceDirections.Length; i++)
            {
                Vector3 newDirection = agent.transform.TransformDirection(m_avoidanceDirections[i]);

                Ray ray = new Ray(agent.transform.position, newDirection);

                if (!Physics.SphereCast(ray, m_obstacleAvoidanceRadius, m_maxAvoidanceDistance, m_obstacleMask))
                {
                    return NormalizeMoveVector(newDirection);
                }
            }
        }

        return agentDirection;
    }

    private Vector3 PickRandomAgentPosition()
    {
        int randomIndex = Random.Range(0, m_flockAgents.Count);
        return NormalizeMoveVector(m_flockAgents[randomIndex].transform.position);
    }

    private Vector3 NormalizeMoveVector(Vector3 move)
    {
        move.y = 0f; // flock agent is only moving in x and z direction
        return move.normalized;
    }

    /// <summary>
    /// Debugging gizmos
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(m_targetPosition, new Vector3(0.5f, 0.5f, 0.5f));
    }
}
