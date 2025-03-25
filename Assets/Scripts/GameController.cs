using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("Player References")]
    public GameObject player;
    public GameObject aiPlayer;

    [Header("UI References")]
    public GameObject gameOverPanel;       // Your existing GameOverPanel
    public TextMeshProUGUI player1WinsText; // Your existing Player1WinsText
    public TextMeshProUGUI player2WinsText; // Your existing Player2WinsText
    public TextMeshProUGUI tieGameText;    // New UI element for tie notifications

    [Header("Score UI")]
    public TextMeshProUGUI scoreTextPlayer1; // Your existing ScoreTextPlayer1
    public TextMeshProUGUI scoreTextPlayer2; // Your existing ScoreTextPlayer2

    [Header("Scene Navigation")]
    public string mainMenuSceneName = "MainMenuScene"; // Name of your main menu scene

    private PlayerMovement playerMovement;
    private AIController aiController;
    private bool gameOver = false;

    // Score tracking
    private int player1Score = 0;
    private int player2Score = 0;
    private const string PLAYER1_SCORE_KEY = "Player1Score";
    private const string PLAYER2_SCORE_KEY = "Player2Score";

    void Start()
    {
        // Get components
        playerMovement = player.GetComponent<PlayerMovement>();
        aiController = aiPlayer.GetComponent<AIController>();

        // Load scores from PlayerPrefs
        LoadScores();

        // Initialize score displays
        UpdateScoreDisplays();

        // Make sure game over texts are hidden
        if (player1WinsText != null)
        {
            player1WinsText.gameObject.SetActive(false);
        }
        if (player2WinsText != null)
        {
            player2WinsText.gameObject.SetActive(false);
        }
        if (tieGameText != null)
        {
            tieGameText.gameObject.SetActive(false);
        }

        // Hide game over panel at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Add player tag collision detection
        SetupPlayerCollision();
    }

    void SetupPlayerCollision()
    {
        // Make sure both player and AI have the "Player" tag for collision detection
        if (player != null && !player.CompareTag("Player"))
        {
            player.tag = "Player";
        }

        if (aiPlayer != null && !aiPlayer.CompareTag("Player"))
        {
            aiPlayer.tag = "Player";
        }

        // Make sure they have colliders
        if (player != null && player.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }

        if (aiPlayer != null && aiPlayer.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = aiPlayer.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
    }

    // Load scores from PlayerPrefs
    void LoadScores()
    {
        player1Score = PlayerPrefs.GetInt(PLAYER1_SCORE_KEY, 0);
        player2Score = PlayerPrefs.GetInt(PLAYER2_SCORE_KEY, 0);
    }

    // Save scores to PlayerPrefs
    void SaveScores()
    {
        PlayerPrefs.SetInt(PLAYER1_SCORE_KEY, player1Score);
        PlayerPrefs.SetInt(PLAYER2_SCORE_KEY, player2Score);
        PlayerPrefs.Save();
    }

    // Update the score displays
    void UpdateScoreDisplays()
    {
        if (scoreTextPlayer1 != null)
        {
            // Add more space between Player and Score with extra newlines
            scoreTextPlayer1.text = "Player 1\n\n\nScore: " + player1Score;
        }

        if (scoreTextPlayer2 != null)
        {
            // Add more space between Player and Score with extra newlines
            scoreTextPlayer2.text = "Player 2\n\n\nScore: " + player2Score;
        }
    }

    // Called when both players collide (tie)
    public void TieGame()
    {
        if (gameOver) return;
        gameOver = true;

        // Stop both players
        if (playerMovement != null)
        {
            playerMovement.isGameOver = true;
            playerMovement.speed = 0;
        }

        if (aiController != null)
        {
            aiController.isGameOver = true;
            aiController.speed = 0;
        }

        // Show Tie Game text
        if (tieGameText != null)
        {
            tieGameText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Tie Game text is not assigned in the GameController!");
        }

        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Note: we don't change scores in a tie
    }

    // Called when the player crashes
    public void PlayerCrashed()
    {
        if (gameOver) return;
        gameOver = true;

        // Increment AI player score
        player2Score++;
        SaveScores();

        // Stop AI movement too
        if (aiController != null)
        {
            aiController.isGameOver = true;
            aiController.speed = 0;
        }

        // Show Player 2 Wins text
        if (player2WinsText != null)
        {
            player2WinsText.gameObject.SetActive(true);
        }

        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Update the score displays
        UpdateScoreDisplays();
    }

    // Called when the AI crashes
    public void AIPlayerCrashed()
    {
        if (gameOver) return;
        gameOver = true;

        // Increment player score
        player1Score++;
        SaveScores();

        // Stop player movement too
        if (playerMovement != null)
        {
            playerMovement.isGameOver = true;
            playerMovement.speed = 0;
        }

        // Show Player 1 Wins text
        if (player1WinsText != null)
        {
            player1WinsText.gameObject.SetActive(true);
        }

        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Update the score displays
        UpdateScoreDisplays();
    }

    void Update()
    {
        if (gameOver)
        {
            // Detect 'R' key to restart game
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            // Detect 'T' key to return to game mode selection
            if (Input.GetKeyDown(KeyCode.T))
            {
                ReturnToGameModeSelection();
            }
            // Detect 'C' key to go to controls panel
            if (Input.GetKeyDown(KeyCode.C))
            {
                ShowControlsPanel();
            }
        }
    }

    void RestartGame()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    void ReturnToGameModeSelection()
    {
        Debug.Log("Returning to Game Mode Selection Panel...");

        // Reset scores when returning to the main menu
        ResetScores();

        // Set PlayerPrefs flag to indicate we want to open game mode panel
        PlayerPrefs.SetInt("ShowGameModePanel", 1);
        PlayerPrefs.Save();

        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void ShowControlsPanel()
    {
        Debug.Log("Showing Controls Panel...");

        // Reset scores when going to controls panel - just like when returning to menu
        ResetScores();

        // Set a PlayerPref flag to tell MainMenu to open the controls panel
        PlayerPrefs.SetInt("ShowControlsPanel", 1);
        PlayerPrefs.Save();

        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Reset scores to zero
    void ResetScores()
    {
        player1Score = 0;
        player2Score = 0;
        PlayerPrefs.DeleteKey(PLAYER1_SCORE_KEY);
        PlayerPrefs.DeleteKey(PLAYER2_SCORE_KEY);
        PlayerPrefs.Save();
    }
}
