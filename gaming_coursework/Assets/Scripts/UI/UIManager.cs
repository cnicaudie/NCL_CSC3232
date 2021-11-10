using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenuCanvas;
    [SerializeField] private GameObject m_levelMenuCanvas;

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
}
