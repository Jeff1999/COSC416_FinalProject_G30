using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelMusic : MonoBehaviour
{
    private AudioSource audioSource;
    [Header("Level Music Settings")]
    public AudioClip levelMusic; // Assign this in Inspector
    [Range(0f, 2f)]
    public float musicVolume = 1.5f;

    [Header("Fade Settings")]
    public float fadeInDuration = 3.0f;

    // The name of this level scene
    public string levelSceneName;

    // Static reference to ensure only one instance exists
    private static LevelMusic instance;

    // Coroutine reference to manage fading
    private Coroutine fadeCoroutine;

    void Awake()
    {
        // Singleton pattern - ensure only one music manager exists
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

            // If no level name was set, use the current scene name
            if (string.IsNullOrEmpty(levelSceneName))
            {
                levelSceneName = SceneManager.GetActiveScene().name;
            }

            // Register for scene change events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            // Destroy any duplicate instances
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // This only runs for the valid instance
        if (instance == this && levelMusic != null)
        {
            audioSource.clip = levelMusic;
            if (SceneManager.GetActiveScene().name == levelSceneName)
            {
                PlayMusic();
            }
        }
    }

    // Called when a scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == levelSceneName)
        {
            // We're in this level, play the music
            PlayMusic();
        }
        else
        {
            // We're not in this level, stop the music
            StopMusic();
        }
    }

    private void PlayMusic()
    {
        if (audioSource != null && levelMusic != null)
        {
            // Stop any ongoing fade
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            // Start the music at zero volume if it's not already playing
            if (!audioSource.isPlaying)
            {
                audioSource.volume = 0f;
                audioSource.Play();
                Debug.Log("LevelMusic: Started playing music for " + levelSceneName);
            }

            // Start fading in
            fadeCoroutine = StartCoroutine(FadeIn());
        }
    }

    private void StopMusic()
    {
        // Stop any ongoing fade
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("LevelMusic: Stopped playing music for " + levelSceneName);
        }
    }

    private IEnumerator FadeIn()
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, musicVolume, timer / fadeInDuration);
            yield return null;
        }

        // Ensure we end at the target volume
        audioSource.volume = musicVolume;
        fadeCoroutine = null;
    }

    // Clean up on destroy
    void OnDestroy()
    {
        // Only unregister if this is the active instance
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }
}





