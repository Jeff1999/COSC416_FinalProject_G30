using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelMusic : MonoBehaviour
{
    [HideInInspector]
    public AudioSource audioSource;

    [Header("Level Music Settings")]
    public AudioClip levelMusic;
    [Range(0f, 2f)]
    public float musicVolume = 1.5f;

    [Header("Fade Settings")]
    public float fadeInDuration = 3.0f;

    public string levelSceneName;

    public static LevelMusic instance;

    private Coroutine fadeCoroutine;
    private bool hasCountdownController = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = false;
            audioSource.loop = true;

            if (string.IsNullOrEmpty(levelSceneName))
                levelSceneName = SceneManager.GetActiveScene().name;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (instance == this && levelMusic != null)
        {
            audioSource.clip = levelMusic;
            hasCountdownController = FindFirstObjectByType<CountdownController>() != null;

            if (SceneManager.GetActiveScene().name == levelSceneName && !hasCountdownController)
            {
                PlayMusic();
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == levelSceneName)
        {
            hasCountdownController = FindFirstObjectByType<CountdownController>() != null;

            if (!hasCountdownController)
            {
                PlayMusic();
            }
        }
        else
        {
            StopMusic();
        }
    }

    public void PlayMusic()
    {
        if (audioSource != null && levelMusic != null)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            if (!audioSource.isPlaying)
            {
                audioSource.volume = 0f;
                audioSource.Play();
                Debug.Log("LevelMusic: Started playing music for " + levelSceneName);
            }

            fadeCoroutine = StartCoroutine(FadeIn());
        }
    }

    public void StopMusic()
    {
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

        audioSource.volume = musicVolume;
        fadeCoroutine = null;
    }

    public void FadeOutAndStop(float fadeDuration = 1.5f)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutCoroutine(fadeDuration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = musicVolume; // reset for next play
        fadeCoroutine = null;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }
}
