using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float m_gravityScale;
    [SerializeField] private float m_levelMaxDistance = 10f;

    private Placezone[] m_placezones;
    private int m_storedObjectsCount = 0;

    [SerializeField] private int m_pickableMinCount;
    [SerializeField] private int m_pickableMaxCount;
    [SerializeField] private GameObject m_pickableInstance;
    private List<Pickable> m_pickables;
    private int m_pickablesCount;

    public delegate void OnLevelWon();
    public event OnLevelWon LevelWon;

    public delegate void OnLevelLost();
    public event OnLevelLost LevelLost;

    // ===================================

    private void Start()
    {
        InitGravity();

        m_placezones = FindObjectsOfType<Placezone>();

        for (int i = 0; i < m_placezones.Length; i++)
        {
            m_placezones[i].UsePlacezone += StoreObject;
            m_placezones[i].FreePlacezone += UnstoreObject;
        }

        SpawnPickables();
        
        for (int i = 0; i < m_pickablesCount; i++)
        {
            m_pickables[i].DestroyPickable += DestroyPickable;
        }
    }

    public static bool GetRandomPosition(Vector3 center, float maxDistance, out Vector3 randomPosition)
    {
        Vector2 randomOffset = Random.insideUnitSphere * maxDistance;
        Vector3 randomPoint = center;
        randomPoint.x += randomOffset.x;
        randomPoint.z += randomOffset.y;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, maxDistance, NavMesh.AllAreas))
        {
            randomPosition = hit.position;
            return true;
        }

        randomPosition = Vector3.zero;
        return false;
    }

    public void StoreObject()
    {
        m_storedObjectsCount++;

        if (m_storedObjectsCount == m_placezones.Length)
        {
            if (LevelWon != null)
            {
                LevelWon();
            }
        }
    }

    public void UnstoreObject()
    {
        m_storedObjectsCount--;
    }

    public void DestroyPickable()
    {
        m_pickablesCount--;

        if (m_pickablesCount < m_placezones.Length)
        {
            Debug.Log("Not enough pickables available !");

            if (LevelLost != null)
            {
                LevelLost();
            }
        }
    }

    private void InitGravity()
    {
        Physics.gravity = Vector3.down * m_gravityScale;
    }

    private void SpawnPickables()
    {
        m_pickables = new List<Pickable>();
        m_pickableMinCount = m_placezones.Length;

        if (m_pickableMaxCount < m_pickableMinCount)
        {
            m_pickableMaxCount = m_pickableMinCount;
        }

        m_pickablesCount = Random.Range(m_pickableMinCount, m_pickableMaxCount + 1);

        Debug.Log("Number of pickables : " + m_pickablesCount);

        for (int i = 0; i < m_pickablesCount; i++)
        {
            Vector3 position;

            if (GetRandomPosition(Vector3.zero, m_levelMaxDistance, out position))
            {
                GameObject obj = Instantiate(m_pickableInstance, position, Quaternion.identity, m_pickableInstance.transform.parent);
                Pickable pickable = obj.GetComponent<Pickable>();
                m_pickables.Add(pickable);
            }
        }

        m_pickableInstance.gameObject.SetActive(false);
    }
}
