using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMusic : MonoBehaviour
{
    // Singleton instance
    private static MainMenuMusic instance;

    private AudioSource audioSource;

    [Header("Main Menu Music Settings")]
    public AudioClip menuMusic;
    [Range(0f, 1f)]
    public float menuMusicVolume = 1.0f;
    public string mainMenuSceneName = "MainMenu";

    void Awake()
    {
        // Implement singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Get or add the AudioSource component
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Configure the AudioSource
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.volume = menuMusicVolume;

            // Register for scene change events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // Another instance exists, destroy this one
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Only execute for the singleton instance
        if (instance == this && menuMusic != null)
        {
            audioSource.clip = menuMusic;
            // Check if we're in the main menu scene
            if (SceneManager.GetActiveScene().name == mainMenuSceneName)
            {
                PlayMusic();
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName)
        {
            PlayMusic();
        }
        else
        {
            StopMusic();
        }
    }

    private void PlayMusic()
    {
        if (audioSource != null && menuMusic != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}

