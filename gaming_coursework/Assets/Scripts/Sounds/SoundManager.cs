using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    // ===================================
    // ATTRIBUTES
    // ===================================

    private static SoundManager m_instance; // singleton instance
    public static SoundManager Instance
    {
        get { return m_instance; }
    }

    private AudioSource m_audioSource;
    private bool m_isLooping = false;

    // Menu
    public GameObject menuAudio;
    private AudioSource m_menuBackground;
    
    // Overworld
    public GameObject overworldAudio;
    private AudioSource m_overworldBackground;
    private AudioClip m_spaceshipEngine;
    private AudioClip m_exitOrbit;
    private AudioClip m_enterLevel;

    // Level
    public GameObject levelAudio;
    private AudioSource m_levelBackground;
    private AudioClip m_walk;
    private AudioClip m_jump;
    private AudioClip m_punch;
    private AudioClip m_shoot;
    private AudioClip m_bulletImpact;
    private AudioClip m_die;
    private AudioClip m_placeObject;
    private AudioClip m_pickObject;

    // =================================

    // ===================================
    // PUBLIC METHODS
    // ===================================

    public void PlayBackground(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.Menu:
                Debug.Log("PLAY MENU BACKGROUND");
                m_overworldBackground.Stop();
                m_levelBackground.Stop();
                m_menuBackground.Play();
                break;

            case GameManager.GameState.Overworld:
                Debug.Log("PLAY OVERWORLD BACKGROUND");
                m_menuBackground.Stop();
                m_levelBackground.Stop();
                m_overworldBackground.Play();
                break;

            case GameManager.GameState.Level:
                Debug.Log("PLAY LEVEL BACKGROUND");
                m_menuBackground.Stop();
                m_overworldBackground.Stop();
                m_levelBackground.Play();
                break;
        }
    }

    public void PlaySound(string name)
    {
        switch(GameManager.GetGameState())
        {
            case GameManager.GameState.Overworld:
                PlayOverworldSound(name);
                break;

            case GameManager.GameState.Level:
                PlayLevelSound(name);
                break;

            default:
                break;
        }
    }

    public void PauseSound()
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
            InitAudioSources();
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitAudioClips();
    }

    private void InitAudioSources()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_menuBackground = menuAudio.GetComponent<AudioSource>();
        m_overworldBackground = overworldAudio.GetComponent<AudioSource>();
        m_levelBackground = levelAudio.GetComponent<AudioSource>();

        m_audioSource.Stop();
        m_menuBackground.Stop();
        m_overworldBackground.Stop();
        m_levelBackground.Stop();
    }

    private void InitAudioClips()
    {
        // Overworld
        m_spaceshipEngine = Resources.Load<AudioClip>("Sounds/Spaceship_Engine_2");
        m_exitOrbit = Resources.Load<AudioClip>("Sounds/Blow_Distance");
        m_enterLevel = Resources.Load<AudioClip>("Sounds/Teleportation_Jump");

        // Level
        m_walk = Resources.Load<AudioClip>("Sounds/Bounce_4");
        m_jump = Resources.Load<AudioClip>("Sounds/Jump_9");
        m_punch = Resources.Load<AudioClip>("Sounds/Bloody_punch");
        m_shoot = Resources.Load<AudioClip>("Sounds/Hand_Gun_2");
        m_bulletImpact = Resources.Load<AudioClip>("Sounds/Bullet_Impact_14");
        m_die = Resources.Load<AudioClip>("Sounds/Ambient_5");
        m_placeObject = Resources.Load<AudioClip>("Sounds/Powerup_6");
        m_pickObject = Resources.Load<AudioClip>("Sounds/Collectibles_3");
    }

    private void PlayLevelSound(string name)
    {
        switch (name)
        {
            case "jump":
                PlayOneShot(m_jump);
                break;

            case "punch":
                PlayOneShot(m_punch);
                break;

            case "shoot":
                PlayOneShot(m_shoot);
                break;

            case "bulletImpact":
                PlayOneShot(m_bulletImpact);
                break;

            case "die":
                PlayOneShot(m_die);
                break;

            case "placeObject":
                PlayOneShot(m_placeObject);
                break;

            case "pickObject":
                PlayOneShot(m_pickObject);
                break;

            case "walk":
                PlayLoopOnce(m_walk, 0.1f, 0.45f);
                break;
        }
    }

    private void PlayOverworldSound(string name)
    {
        switch (name)
        {
            case "exitOrbit":
                PlayOneShot(m_exitOrbit);
                break;

            case "enterLevel":
                PlayOneShot(m_enterLevel);
                break;

            case "spaceshipEngine":
                PlayLoopOnce(m_spaceshipEngine, 0.3f);
                break;
        }
    }

    private void PlayLoopOnce(AudioClip clip, float volume = 0.35f, float pitch = 1f)
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

    private void PlayOneShot(AudioClip clip)
    {
        m_audioSource.Stop();
        m_isLooping = false;
        m_audioSource.loop = false;
        m_audioSource.clip = clip;
        m_audioSource.Play();
    }
}
