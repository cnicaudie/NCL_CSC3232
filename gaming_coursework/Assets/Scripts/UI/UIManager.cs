using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenuCanvas;
    [SerializeField] private GameObject m_levelMenuCanvas;

    [SerializeField] private GameObject m_endLevelMenuCanvas;
    [SerializeField] private Text m_endLevelText;

    // ===================================

    private void Awake()
    {
        UIManager[] uiManagers = FindObjectsOfType<UIManager>();

        if (uiManagers.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

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
}
