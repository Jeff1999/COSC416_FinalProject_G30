using UnityEngine;

public class GameOverMusic : MonoBehaviour
{
    [Range(0f, 2f)]
    public float volume = 1f;

    private AudioSource gameOverAudioSource;

    void Start()
    {
        // Fade out level music if playing
        if (LevelMusic.instance != null)
        {
            LevelMusic.instance.FadeOutAndStop(1.0f); // short fade
        }

        // Load and play pacman_death.wav from Resources folder
        AudioClip gameOverClip = Resources.Load<AudioClip>("pacman_death");

        if (gameOverClip == null)
        {
            Debug.LogError("GameOverMusic: Couldn't find pacman_death.wav in Resources folder!");
            return;
        }

        // Add AudioSource if not already there
        gameOverAudioSource = GetComponent<AudioSource>();
        if (gameOverAudioSource == null)
        {
            gameOverAudioSource = gameObject.AddComponent<AudioSource>();
        }

        gameOverAudioSource.clip = gameOverClip;
        gameOverAudioSource.volume = volume;
        gameOverAudioSource.loop = false;
        gameOverAudioSource.Play();
    }
}
