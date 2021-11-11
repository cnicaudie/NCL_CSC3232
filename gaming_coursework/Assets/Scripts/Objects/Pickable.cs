using UnityEngine;

/// <summary>
/// Handles pickable object behaviour
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Pickable : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private Rigidbody m_rigidbody;
    private Placezone m_currentPlacezone;

    private bool m_isPlaced = false;
    public bool IsPlaced
    {
        get { return m_isPlaced; }
    }

    private bool m_wasThrown = false;
    public bool WasThrown
    {
        get { return m_wasThrown; }
    }

    public delegate void OnPickableDestroy();
    public event OnPickableDestroy DestroyPickable;

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public void Place(Placezone placezone)
    {
        m_isPlaced = true;

        m_currentPlacezone = placezone;
        m_currentPlacezone.Use();

        // Place object in placezone
        transform.position = m_currentPlacezone.transform.position;
        transform.rotation = m_currentPlacezone.transform.rotation;

        // Freeze it so that it doesn't get moved around
        m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Unplace()
    {
        m_isPlaced = false;

        m_currentPlacezone.Free();
        m_currentPlacezone = null;

        // Unfreeze the object
        m_rigidbody.constraints = RigidbodyConstraints.None;
    }

    /// <summary>
    /// Physics-based throw
    /// </summary>
    /// <param name="force"></param>
    public void Throw(Vector3 force)
    {
        m_rigidbody.useGravity = true;
        m_rigidbody.AddForce(force, ForceMode.Impulse);
        m_wasThrown = true;
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position.y < -5f)
        {
            if (DestroyPickable != null)
            {
                DestroyPickable();
            }

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Collision response and feedback
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && m_wasThrown)
        {
            m_wasThrown = false;
        }
    }
}
