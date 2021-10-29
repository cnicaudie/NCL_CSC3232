using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickable : MonoBehaviour
{
    private Placezone m_currentPlacezone;

    private bool m_isPlaced = false;
    public bool IsPlaced
    {
        get { return m_isPlaced; }
    }

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
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        Debug.Log("Placed object");
    }

    public void Unplace()
    {
        m_isPlaced = false;

        m_currentPlacezone.Free();
        m_currentPlacezone = null;

        // Unfreeze the object
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;

        Debug.Log("Unplaced object");
    }
    
}
