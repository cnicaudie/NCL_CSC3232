using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIManager m_uiManager;
    private CameraController m_camera;

    private LevelManager m_levelManager;
    private string m_nextLevelName;
    
    private Player m_player;
    private Spaceship m_spaceship;

    public enum GameState
    {
        Menu, Overworld, Level
    }
    public static GameState gameState;

    // TODO : Debug tool, remove later
    public enum StartType
    {
        Overworld, Level
    }
    public StartType startType;

    // ===================================

    private void Awake()
    {
        GameManager[] gameManagers = FindObjectsOfType<GameManager>();

        if (gameManagers.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // TODO : Remove later
        switch (startType)
        {
            case StartType.Overworld:
                m_uiManager = FindObjectOfType<UIManager>();
                m_camera = FindObjectOfType<CameraController>();
                gameState = GameState.Menu;
                break;

            case StartType.Level:
                InitLevel();
                break;

            default:
                break;
        }
    }

    private void InitOverworld()
    {
        gameState = GameState.Overworld;

        m_camera = FindObjectOfType<CameraController>();

        m_spaceship = FindObjectOfType<Spaceship>();
        m_spaceship.EnterLevelPoint += EnterLevelPoint;
    }

    private void InitLevel()
    {
        gameState = GameState.Level;

        m_camera = FindObjectOfType<CameraController>();

        m_levelManager = FindObjectOfType<LevelManager>();
        m_levelManager.LevelWon += LevelWon;
        m_levelManager.LevelLost += LevelLost;

        m_player = FindObjectOfType<Player>();
        m_player.EntityDie += LevelLost;
    }

    public static bool IsGamePlaying()
    {
        return gameState == GameState.Level;
    }

    public static bool IsOverworldPlaying()
    {
        return gameState == GameState.Overworld;
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

    public void LoadNextLevel()
    {
        m_uiManager.ToggleLevelMenu();

        StartCoroutine("LoadNextSceneAsync");
    }

    IEnumerator LoadNextSceneAsync()
    {
        Debug.Log("Loading level...");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_nextLevelName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        InitLevel();
        Debug.Log("Level Loaded !");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        InitLevel();
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

        gameState = GameState.Menu;

        m_camera.SetCameraMode(CameraController.CameraMode.Menu);
    }

    private void LevelWon()
    {
        Debug.Log("Level Won !");

        gameState = GameState.Menu;

        m_camera.SetCameraMode(CameraController.CameraMode.Menu);
    }
}
