using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownController : MonoBehaviour
{
    [Header("Countdown Settings")]
    public TextMeshProUGUI countdownText;
    public GameObject countdownPanel;
    public float numberDisplayTime = 0.8f; // How long each number displays
    public float transitionTime = 0.2f;    // Fade between numbers

    [Header("Animation Settings")]
    public float fontSize = 36f;        // Exact font size to use
    public Color numberColor = Color.white;
    public Color goColor = Color.green;
    public string goText = "GO!";

    // References to game controllers
    private GameController gameController;
    private PlayerMovement playerMovement;
    private AIController aiController;
    private LevelMusic levelMusic;

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Get required components
        canvasGroup = countdownPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = countdownPanel.AddComponent<CanvasGroup>();
        }

        // Find the game controller
        gameController = FindFirstObjectByType<GameController>();

        // Find LevelMusic controller
        levelMusic = FindFirstObjectByType<LevelMusic>();

        // Find player and AI references if not set directly
        if (gameController != null)
        {
            // Find player movement
            if (gameController.player != null)
            {
                playerMovement = gameController.player.GetComponent<PlayerMovement>();
            }

            // Find AI controller
            if (gameController.aiPlayer != null)
            {
                aiController = gameController.aiPlayer.GetComponent<AIController>();
            }
        }

        // Pause game initially
        PauseGameplay(true);

        // If we have level music, temporarily prevent it from playing
        if (levelMusic != null && levelMusic.audioSource != null)
        {
            // We'll let our countdown trigger the music instead
            if (levelMusic.audioSource.isPlaying)
            {
                levelMusic.audioSource.Stop();
            }
        }

        // Setup the text and panel
        countdownText.fontSize = 36; // Use a specific font size
        countdownText.alignment = TextAlignmentOptions.Center;

        // Start the countdown
        StartCoroutine(StartCountdown());
    }

    void PauseGameplay(bool pause)
    {
        // Disable player and AI movement during countdown
        if (playerMovement != null)
        {
            playerMovement.enabled = !pause;
        }

        if (aiController != null)
        {
            aiController.enabled = !pause;
        }
    }

    IEnumerator StartCountdown()
    {
        // Make sure panel is visible and reset
        countdownPanel.SetActive(true);

        // Display 3
        yield return DisplayNumber("3", numberColor);

        // Display 2
        yield return DisplayNumber("2", numberColor);

        // Display 1
        yield return DisplayNumber("1", numberColor);

        // Display GO!
        yield return DisplayNumber(goText, goColor);

        // Start the game
        PauseGameplay(false);

        // Start the level music with its built-in fade
        if (levelMusic != null)
        {
            levelMusic.PlayMusic();
        }

        // Disable the panel when done
        countdownPanel.SetActive(false);
    }

    IEnumerator DisplayNumber(string number, Color color)
    {
        // Setup the number
        countdownText.text = number;
        countdownText.color = color;

        // Fade in
        float fadeInTime = 0;
        while (fadeInTime < transitionTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, fadeInTime / transitionTime);
            fadeInTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Display for the countdown time
        yield return new WaitForSeconds(numberDisplayTime);

        // Fade out
        float fadeOutTime = 0;
        while (fadeOutTime < transitionTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, fadeOutTime / transitionTime);
            fadeOutTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}


