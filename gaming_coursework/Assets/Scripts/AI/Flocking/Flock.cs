using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    private List<FlockAgent> m_flockAgents;

    public FlockBehaviour behaviour;

    [Range(5, 15)]
    public int startCount = 5;

    private const float k_agentDensity = 0.8f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;

    [Range(1f, 100f)]
    public float maxSpeed = 10f;

    [Range(1f, 10f)]
    public float neighbourRadius = 2f;

    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    private float m_squareMaxSpeed;
    private float m_squareNeighbourRadius;
    private float m_squareAvoidanceRadius;
    public float SquareAvoidanceRadius
    {
        get { return m_squareAvoidanceRadius; }
    }

    private void Start()
    {
        m_flockAgents = new List<FlockAgent>();

        m_squareMaxSpeed = Mathf.Pow(maxSpeed, 2);
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

            // Debug color
            agent.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.red, 2f * neighbours.Count / startCount);

            /*

            // Compute the move
            Vector3 move = behaviour.CalculateMove(agent, neighbours, this);
            move *= driveFactor;

            if (move.sqrMagnitude > m_squareMaxSpeed)
            {
                move = move.normalized * m_squareMaxSpeed;
            }

            // Apply move to agent
            agent.Move(move);

            */
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
}
