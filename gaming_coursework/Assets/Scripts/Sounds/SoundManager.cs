using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private static SoundManager m_instance; // singleton instance

    private static AudioSource m_audioSource;
    private static bool m_isLooping = false;

    // Menu
    public GameObject menuAudio;
    public static AudioSource menuBackground; // universe music
    //public static AudioClip buttonClick;

    // Overworld
    public GameObject overworldAudio;
    public static AudioSource overworldBackground;
    public static AudioClip spaceshipEngine;
    public static AudioClip exitOrbit;
    public static AudioClip enterLevel;
    // TODO : add thrust ?

    // Level
    public GameObject levelAudio;
    public static AudioSource levelBackground;
    public static AudioClip walk;
    public static AudioClip jump;
    public static AudioClip punch;
    public static AudioClip shoot;
    public static AudioClip bulletImpact;
    public static AudioClip die;
    public static AudioClip placeObject;
    public static AudioClip pickObject;

    // =================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public static void PlayBackground(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.Menu:
                Debug.Log("PLAY MENU BACKGROUND");
                overworldBackground.Stop();
                levelBackground.Stop();
                menuBackground.Play();
                break;

            case GameManager.GameState.Overworld:
                Debug.Log("PLAY OVERWORLD BACKGROUND");
                menuBackground.Stop();
                levelBackground.Stop();
                overworldBackground.Play();
                break;

            case GameManager.GameState.Level:
                Debug.Log("PLAY LEVEL BACKGROUND");
                menuBackground.Stop();
                overworldBackground.Stop();
                levelBackground.Play();
                break;
        }
    }

    public static void PlaySound(string name)
    {
        switch(GameManager.GetGameState())
        {
            case GameManager.GameState.Menu:
                //PlayMenuSound(name);
                break;

            case GameManager.GameState.Overworld:
                PlayOverworldSound(name);
                break;

            case GameManager.GameState.Level:
                PlayLevelSound(name);
                break;
        }
    }

    public static void PauseSound()
    {
        m_audioSource.Stop();
        m_isLooping = false;
    }

    // ===================================
    // PRIVATE METHODS
    // ===================================

    /// <summary>
    /// Makes the SoundManager a "Don't Destroy On Load" object (singleton)
    /// and init audio sources
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

        InitAudioSources();
    }

    private void Start()
    {
        InitAudioClips();
    }

    private void InitAudioSources()
    {
        m_audioSource = GetComponent<AudioSource>();
        menuBackground = menuAudio.GetComponent<AudioSource>();
        overworldBackground = overworldAudio.GetComponent<AudioSource>();
        levelBackground = levelAudio.GetComponent<AudioSource>();

        m_audioSource.Stop();
        menuBackground.Stop();
        overworldBackground.Stop();
        levelBackground.Stop();
    }

    private void InitAudioClips()
    {
        // TODO : Load audio clips from resources

        // Overworld
        spaceshipEngine = Resources.Load<AudioClip>("Sounds/Spaceship_Engine_2");
        exitOrbit = Resources.Load<AudioClip>("Sounds/Blow_Distance");
        enterLevel = Resources.Load<AudioClip>("Sounds/Teleportation_Jump");

        // Level
        walk = Resources.Load<AudioClip>("Sounds/Bounce_4");
        jump = Resources.Load<AudioClip>("Sounds/Jump_9");
        punch = Resources.Load<AudioClip>("Sounds/Bloody_punch");
        shoot = Resources.Load<AudioClip>("Sounds/Hand_Gun_2");
        bulletImpact = Resources.Load<AudioClip>("Sounds/Bullet_Impact_14");
        die = Resources.Load<AudioClip>("Sounds/Ambient_5");
        placeObject = Resources.Load<AudioClip>("Sounds/Powerup_6");
        pickObject = Resources.Load<AudioClip>("Sounds/Collectibles_3");
    }

    private static void PlayLevelSound(string name)
    {
        switch (name)
        {
            case "jump":
                PlayOneShot(jump);
                break;

            case "punch":
                PlayOneShot(punch);
                break;

            case "shoot":
                PlayOneShot(shoot);
                break;

            case "bulletImpact":
                PlayOneShot(bulletImpact);
                break;

            case "die":
                PlayOneShot(die);
                break;

            case "placeObject":
                PlayOneShot(placeObject);
                break;

            case "pickObject":
                PlayOneShot(pickObject);
                break;

            case "walk":
                PlayLoopOnce(walk, 0.1f, 0.45f);
                break;
        }
    }

    private static void PlayOverworldSound(string name)
    {
        switch (name)
        {
            case "exitOrbit":
                PlayOneShot(exitOrbit);
                break;

            case "enterLevel":
                PlayOneShot(enterLevel);
                break;

            case "spaceshipEngine":
                PlayLoopOnce(spaceshipEngine, 0.3f);
                break;
        }
    }

    private static void PlayLoopOnce(AudioClip clip, float volume = 0.35f, float pitch = 1f)
    {
        if (!m_audioSource.isPlaying)
        {
            if (!m_isLooping || (m_isLooping && m_audioSource.clip != clip))
            {
                m_audioSource.Stop();
                m_isLooping = true;
                m_audioSource.loop = true;
                m_audioSource.volume = volume;
                m_audioSource.pitch = pitch;
                m_audioSource.clip = clip;
                m_audioSource.Play();
            }
        }
    }

    private static void PlayOneShot(AudioClip clip)
    {
        m_audioSource.Stop();
        m_isLooping = false;
        m_audioSource.loop = false;
        m_audioSource.clip = clip;
        m_audioSource.Play();
    }
}
