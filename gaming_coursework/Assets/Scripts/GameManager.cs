using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manage the game in its globality (states / UI / win / lose)
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Menu, Overworld, Level
    }

    // ===================================
    // ATTRIBUTES
    // ===================================

    private GameState m_gameState;

    private UIManager m_uiManager;
    private CameraController m_camera;

    private LevelManager m_levelManager;
    private string m_nextLevelName;
    
    private Player m_player;
    private Spaceship m_spaceship;

    // TODO : Debug tool, remove later
    public enum StartType
    {
        Overworld, Level
    }
    public StartType startType;

    // ===================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public bool IsGamePlaying()
    {
        return m_gameState == GameState.Level;
    }

    public bool IsOverworldPlaying()
    {
        return m_gameState == GameState.Overworld;
    }

    public GameState GetGameState()
    {
        return m_gameState;
    }

    public void PlayGame()
    {
        m_uiManager.ToggleMainMenu();

        // TODO : Remove later
        switch (startType)
        {
            case StartType.Overworld: InitOverworld(); break;
            case StartType.Level: InitLevel(); break;
            default: break;
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GoToOverworld()
    {
        m_uiManager.ToggleEndLevelMenu();

        StartCoroutine(LoadNextSceneAsync("OverworldScene", false));
    }

    public void LoadNextLevel()
    {
        m_uiManager.ToggleLevelMenu();

        StartCoroutine(LoadNextSceneAsync(m_nextLevelName, true));
    }

    public void RestartLevel()
    {
        m_uiManager.ToggleEndLevelMenu();

        StartCoroutine(LoadNextSceneAsync(m_nextLevelName, true));
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    private void Start()
    {
        m_uiManager = FindObjectOfType<UIManager>();

        // TODO : Remove later
        switch (startType)
        {
            case StartType.Overworld:
                m_camera = FindObjectOfType<CameraController>();
                SetGameState(GameState.Menu);
                break;

            case StartType.Level:
                m_nextLevelName = SceneManager.GetActiveScene().name;
                InitLevel();
                break;

            default:
                break;
        }
    }

    private void InitOverworld()
    {
        Debug.Log("Initializing Overworld...");

        SetGameState(GameState.Overworld);

        m_camera = FindObjectOfType<CameraController>();

        m_spaceship = FindObjectOfType<Spaceship>();
        m_spaceship.EnterLevelPoint += EnterLevelPoint;

        Debug.Log("Overworld initialized !");
    }

    private void InitLevel()
    {
        Debug.Log("Initializing Level...");

        SetGameState(GameState.Level);

        m_camera = FindObjectOfType<CameraController>();

        m_levelManager = FindObjectOfType<LevelManager>();
        m_levelManager.LevelWon += LevelWon;
        m_levelManager.LevelLost += LevelLost;

        m_player = FindObjectOfType<Player>();
        m_player.EntityDie += LevelLost;

        Debug.Log("Level initialized !");
    }

    private void SetGameState(GameState gameState)
    {
        m_gameState = gameState;
        SoundManager.Instance.PlayBackground(gameState);
    }

    private IEnumerator LoadNextSceneAsync(string sceneName, bool isLevel)
    {
        Debug.Log("Loading scene...");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Scene Loaded !");

        if (isLevel)
        {
            InitLevel();
        }
        else
        {
            InitOverworld();
        }
    }

    private void EnterLevelPoint(string levelName)
    {
        Debug.Log("Entered level point : " + levelName + " !");

        m_nextLevelName = levelName;
        m_uiManager.ToggleLevelMenu();
    }

    private void LevelLost()
    {
        Debug.Log("Level Lost !");

        SetGameState(GameState.Menu);
        m_camera.SetCameraMode(CameraController.CameraMode.Menu);
        m_uiManager.ToggleLoseLevelMenu();
    }

    private void LevelWon()
    {
        Debug.Log("Level Won !");

        SetGameState(GameState.Menu);
        m_camera.SetCameraMode(CameraController.CameraMode.Menu);
        m_uiManager.ToggleWinLevelMenu();
    }
}
