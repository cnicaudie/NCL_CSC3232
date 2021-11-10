using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private Transform m_pickUpLocation;

    private Placezone m_currentPlacezone;
    [SerializeField] private Pickable m_currentPickUpObject;
    [SerializeField] private List<Pickable> m_objectsInArea;

    private float m_throwForce = 20f;
    private bool m_hasPickedUpObject;

    // ===================================

    private void Start()
    {
        m_objectsInArea = new List<Pickable>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("PlayerPick"))
        {
            if (m_hasPickedUpObject)
            {
                if (m_currentPlacezone != null)
                {
                    PlaceObject();
                }
                else
                {
                    LetGoObject();
                }
            }
            else
            {
                PickUpObject();
            }
        }

        if (m_hasPickedUpObject)
        {
            m_currentPickUpObject.transform.position = m_pickUpLocation.position;

            if (Input.GetButtonDown("Fire2"))
            {
                ThrowObject();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickable") && !m_hasPickedUpObject)
        {
            m_objectsInArea.Add(other.gameObject.GetComponent<Pickable>());
        }
        else if (other.gameObject.CompareTag("Placezone") && m_hasPickedUpObject)
        {
            Placezone placezone = other.gameObject.GetComponent<Placezone>();

            if (placezone.IsAvailable)
            {
                m_currentPlacezone = placezone;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Pickable"))
        {
            m_objectsInArea.Remove(other.gameObject.GetComponent<Pickable>());
        }
        else if (other.gameObject.CompareTag("Placezone") && m_hasPickedUpObject)
        {
            Placezone placezone = other.gameObject.GetComponent<Placezone>();

            if (m_currentPlacezone == placezone)
            {
                m_currentPlacezone = null;
            }
        }
    }

    private void PickUpObject()
    {
        if (m_objectsInArea.Count > 0)
        {
            // Always pick up the closest object in the list
            m_objectsInArea.Sort((obj1, obj2) => Vector3.Distance(obj1.transform.position, transform.position).CompareTo(Vector3.Distance(obj2.transform.position, transform.position)));

            m_currentPickUpObject = m_objectsInArea[0];
        }

        if (m_currentPickUpObject != null)
        {
            if (m_currentPickUpObject.IsPlaced)
            {
                m_currentPickUpObject.Unplace();
            }

            m_hasPickedUpObject = true;
            m_currentPickUpObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void LetGoObject()
    {
        if (m_currentPickUpObject != null)
        {
            m_hasPickedUpObject = false;
            m_currentPickUpObject.GetComponent<Rigidbody>().useGravity = true;
            m_currentPickUpObject = null;
        }
    }

    private void ThrowObject()
    {
        if (m_currentPickUpObject != null)
        {
            m_hasPickedUpObject = false;
            m_currentPickUpObject.Throw(transform.forward * m_throwForce);
            m_currentPickUpObject = null;
        }
    }

    private void PlaceObject()
    {
        if (m_currentPlacezone.IsAvailable)
        {
            m_hasPickedUpObject = false;
            m_currentPickUpObject.Place(m_currentPlacezone);
            m_currentPickUpObject = null;
        }
        else
        {
            Debug.Log("Placezone is not available");
        }
    }
}
