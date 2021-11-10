using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CameraController m_camera;
    private LevelManager m_levelManager;
    private Player m_player;

    public enum GameState
    {
        Menu, Playing
    }
    public static GameState gameState;

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
        Init();
    }

    private void Init()
    {
        // TODO : Change when adding UI
        gameState = GameState.Playing;

        m_camera = FindObjectOfType<CameraController>();

        m_levelManager = FindObjectOfType<LevelManager>();
        m_levelManager.LevelWon += LevelWon;
        m_levelManager.LevelLost += LevelLost;

        m_player = FindObjectOfType<Player>();
        m_player.EntityDie += LevelLost;
    }

    public static bool IsGamePlaying()
    {
        return gameState == GameState.Playing;
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
