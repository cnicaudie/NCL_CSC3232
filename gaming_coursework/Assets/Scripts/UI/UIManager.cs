using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI menus toggle
/// </summary>
public class UIManager : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private static UIManager m_instance; // singleton instance

    [SerializeField] private GameObject m_mainMenuCanvas;
    [SerializeField] private GameObject m_levelMenuCanvas;

    [SerializeField] private GameObject m_endLevelMenuCanvas;
    [SerializeField] private Text m_endLevelText;

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public void ToggleMainMenu()
    {
        m_mainMenuCanvas.SetActive(!m_mainMenuCanvas.activeSelf);
    }

    public void ToggleLevelMenu()
    {
        m_levelMenuCanvas.SetActive(!m_levelMenuCanvas.activeSelf);
    }

    public void ToggleWinLevelMenu()
    {
        m_endLevelText.text = "YOU WON";
        ToggleEndLevelMenu();
    }

    public void ToggleLoseLevelMenu()
    {
        m_endLevelText.text = "YOU LOST";
        ToggleEndLevelMenu();
    }

    public void ToggleEndLevelMenu()
    {
        m_endLevelMenuCanvas.SetActive(!m_endLevelMenuCanvas.activeSelf);
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    /// <summary>
    /// Makes the UIManager a "Don't Destroy On Load" object (singleton)
    /// </summary>
    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
