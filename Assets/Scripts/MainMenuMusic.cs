using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMusic : MonoBehaviour
{
    private AudioSource audioSource;
    [Header("Main Menu Music Settings")]
    public AudioClip menuMusic; // Assign this in Inspector
    [Range(0f, 1f)]
    public float menuMusicVolume = 1.0f;

    // The name of your main menu scene
    public string mainMenuSceneName = "MainMenu";

    void Awake()
    {
        // Don't destroy this object when loading a new scene
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

    void Start()
    {
        // Make sure we have a music clip
        if (menuMusic != null)
        {
            audioSource.clip = menuMusic;
            PlayMusic();
        }
        else
        {
            Debug.LogError("MainMenuMusic: No music file assigned in Inspector!");
        }
    }

    // Called when a scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName)
        {
            // We're in the main menu, play the music
            PlayMusic();
        }
        else
        {
            // We're not in the main menu, stop the music
            StopMusic();
        }
    }

    private void PlayMusic()
    {
        if (audioSource != null && menuMusic != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("MainMenuMusic: Started playing music");
        }
    }

    private void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("MainMenuMusic: Stopped playing music");
        }
    }

    // Clean up on destroy
    void OnDestroy()
    {
        // Unregister from scene change events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

