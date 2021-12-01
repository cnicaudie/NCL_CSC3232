using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private static SoundManager m_instance; // singleton instance

    private static AudioSource m_audioSource;

    // Menu
    public GameObject menuAudio;
    public static AudioSource menuBackground; // universe music
    public static AudioClip buttonClick;

    // Overworld
    public GameObject overworldAudio;
    public static AudioSource overworldBackground; // deep in space
    public static AudioClip spaceshipEngine; // spaceship started : ok
    public static AudioClip teleportationJump; // level enter : ok
    public static AudioClip blowDistance; // orbit exited : ok

    // Level
    public GameObject levelAudio;
    public static AudioSource levelBackground; // sci fi loop
    public static AudioClip walk;
    public static AudioClip jump; // ok
    public static AudioClip punch; // ok
    public static AudioClip shoot; // ok
    public static AudioClip bulletImpact; // ok
    public static AudioClip die; // ok : ambient 5
    public static AudioClip placeObject; // ok : powerup
    public static AudioClip pickObject; // ok : collectible 3

    // =================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public static void PlayBackground(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.Menu:
                overworldBackground.Stop();
                levelBackground.Stop();
                menuBackground.Play();
                break;

            case GameManager.GameState.Level:
                menuBackground.Stop();
                overworldBackground.Stop();
                levelBackground.Play();
                break;

            case GameManager.GameState.Overworld:
                menuBackground.Stop();
                levelBackground.Stop();
                overworldBackground.Play();
                break;
        }
    }

    public static void PlaySound(string name)
    {
        // TODO
        /*
        switch (name)
        {
            case "test":
                m_audioSource.PlayOneShot(test);
                break;
        }
        */
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    /// <summary>
    /// Makes the SoundManager a "Don't Destroy On Load" object (singleton)
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

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();

        menuBackground = menuAudio.GetComponent<AudioSource>();
        overworldBackground = overworldAudio.GetComponent<AudioSource>();
        levelBackground = levelAudio.GetComponent<AudioSource>();

        // TODO : Load audio clips from resources
        /*
        pickUpAmmo = Resources.Load<AudioClip>("Sounds/pickUpAmmo");
        fullLayerWin = Resources.Load<AudioClip>("Sounds/fullLayerWin");
        jump = Resources.Load<AudioClip>("Sounds/jump");
        bubbleHit = Resources.Load<AudioClip>("Sounds/bubbleHit");
        bubbleFire = Resources.Load<AudioClip>("Sounds/bubbleFire");
        die = Resources.Load<AudioClip>("Sounds/die");
        bumpInObstacle = Resources.Load<AudioClip>("Sounds/bumpInObstacle");
        */
    }
}
