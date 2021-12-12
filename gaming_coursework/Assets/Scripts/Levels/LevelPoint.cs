using UnityEngine;

/// <summary>
/// Stores the level name associated to a level point
/// </summary>
public class LevelPoint : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    public string levelName;

    public Color baseColor;
    public Color highlightColor;

    // ===================================

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        SwitchColor(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Spaceship>())
        {
            SwitchColor(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Spaceship>())
        {
            SwitchColor(false);
        }
    }

    private void SwitchColor(bool isHighlight)
    {
        Material material = GetComponent<MeshRenderer>().material;

        material.color = isHighlight ? highlightColor : baseColor;
    }
}
