using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
	// ===================================
	// ATTRIBUTES
	// ===================================

	public float speed = .2f;
	public float strength = 9f;
	public bool isHorizontal = true;

	private Vector3 m_basePosition;
	private float m_offset;

	// ===================================

	// ===================================
	// PRIVATE METHODS
	// ===================================

	private void Start()
	{
		m_basePosition = transform.position;
		m_offset = Random.Range(0f, 2f);
	}

	private void Update()
	{
		float positionOffset = Mathf.Sin(Time.time * speed + m_offset) * strength;
		Vector3 nextPosition = m_basePosition;

		if (isHorizontal)
        {
			nextPosition.x += positionOffset;
        }
		else
        {
			nextPosition.z += positionOffset;
		}

		transform.position = nextPosition;
	}
}
