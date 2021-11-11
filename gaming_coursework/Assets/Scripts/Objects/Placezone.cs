using UnityEngine;

/// <summary>
/// Handles placezone behaviour
/// </summary>
public class Placezone : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    [SerializeField] private Color m_availableColor;
    [SerializeField] private Color m_unavailableColor;

    private bool m_isAvailable = true;
    public bool IsAvailable
    {
        get { return m_isAvailable; }
    }

    public delegate void OnPlacezoneUse();
    public event OnPlacezoneUse UsePlacezone;

    public delegate void OnPlacezoneFree();
    public event OnPlacezoneFree FreePlacezone;

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public void Use()
    {
        m_isAvailable = false;
        SetColor();

        if (UsePlacezone != null)
        {
            UsePlacezone();
        }
    }

    public void Free()
    {
        m_isAvailable = true;
        SetColor();

        if (FreePlacezone != null)
        {
            FreePlacezone();
        }
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        SetColor();
    }

    private void SetColor()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = m_isAvailable ? m_availableColor : m_unavailableColor;
    }
}
