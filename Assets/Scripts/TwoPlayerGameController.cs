using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class TwoPlayerGameController : MonoBehaviour
{
    [Header("Player References")]
    public GameObject player;
    public GameObject aiPlayer;

    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI player1WinsText;
    public TextMeshProUGUI player2WinsText;
    public TextMeshProUGUI tieGameText;

    [Header("Countdown")]
    public TextMeshProUGUI countdownText;

    [Header("Score UI")]
    public TextMeshProUGUI scoreTextPlayer1;
    public TextMeshProUGUI scoreTextPlayer2;

    [Header("Scene Navigation")]
    public string mainMenuSceneName = "MainMenuScene";

    private PlayerMovement playerMovement;
    private AIController aiController;
    private TwoPlayerMovements player2Movement;
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
        player2Movement = aiPlayer.GetComponent<TwoPlayerMovements>();

        // Load scores
        LoadScores();
        UpdateScoreDisplays();

        // Hide game over texts
        if (player1WinsText != null) player1WinsText.gameObject.SetActive(false);
        if (player2WinsText != null) player2WinsText.gameObject.SetActive(false);
        if (tieGameText != null)     tieGameText.gameObject.SetActive(false);

        // Hide gameOverPanel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Set up collision tags/colliders
        SetupPlayerCollision();

        // Start countdown
        StartCoroutine(CountdownAndStart());
    }

    IEnumerator CountdownAndStart()
    {
        // Freeze both players at the start
        if (playerMovement != null) playerMovement.enabled = false;
        if (aiController != null)   aiController.enabled   = false;
        if (player2Movement != null) player2Movement.enabled = false;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            // 3-2-1 countdown
            for (int i = 3; i > 0; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSeconds(0.9f);
            }

            // Show GO! text
            countdownText.text = "GO!";
            yield return new WaitForSeconds(0.4f);

            // Hide countdown
            countdownText.gameObject.SetActive(false);
        }
        else
        {
            // No countdown text? just wait 3 seconds
            yield return new WaitForSeconds(0.1f);
        }

        // Unfreeze players if not game over
        if (!gameOver && playerMovement != null)   playerMovement.enabled = true;
        if (!gameOver && aiController != null)     aiController.enabled   = true;
        if (!gameOver && player2Movement != null)  player2Movement.enabled= true;
    }

    void SetupPlayerCollision()
    {
        if (player != null && !player.CompareTag("Player")) player.tag = "Player";
        if (aiPlayer != null && !aiPlayer.CompareTag("Player")) aiPlayer.tag = "Player";

        // Add colliders if missing
        if (player != null && player.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D col = player.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
        if (aiPlayer != null && aiPlayer.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D col = aiPlayer.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
    }

    void LoadScores()
    {
        player1Score = PlayerPrefs.GetInt(PLAYER1_SCORE_KEY, 0);
        player2Score = PlayerPrefs.GetInt(PLAYER2_SCORE_KEY, 0);
    }

    void SaveScores()
    {
        PlayerPrefs.SetInt(PLAYER1_SCORE_KEY, player1Score);
        PlayerPrefs.SetInt(PLAYER2_SCORE_KEY, player2Score);
        PlayerPrefs.Save();
    }

    void UpdateScoreDisplays()
    {
        if (scoreTextPlayer1 != null)
        {
            scoreTextPlayer1.text = "Player 1\n\n\nScore: " + player1Score;
        }
        if (scoreTextPlayer2 != null)
        {
            scoreTextPlayer2.text = "Player 2\n\n\nScore: " + player2Score;
        }
    }

    public void TieGame()
    {
        if (gameOver) return;
        gameOver = true;

        StopAllPlayers();
        TriggerBothCrashAnimations();

        if (tieGameText != null)
        {
            tieGameText.gameObject.SetActive(true);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void TriggerBothCrashAnimations()
    {
        CrashAnimationController crashAnimation = FindFirstObjectByType<CrashAnimationController>();
        if (crashAnimation != null)
        {
            if (player != null)
                crashAnimation.StartCrashAnimation(player.transform.position);
            if (aiPlayer != null)
                crashAnimation.StartCrashAnimation(aiPlayer.transform.position);
        }

        PlayCrashSoundOnPlayer(player);
        PlayCrashSoundOnPlayer(aiPlayer);
    }

    private void PlayCrashSoundOnPlayer(GameObject playerObj)
    {
        if (playerObj == null) return;

        // Try PlayerMovement first
        PlayerMovement p1 = playerObj.GetComponent<PlayerMovement>();
        if (p1 != null && p1.crashSound != null)
        {
            AudioSource audioSource = playerObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.clip = p1.crashSound;
                audioSource.Play();
            }
            return;
        }

        // Next check TwoPlayerMovements
        TwoPlayerMovements p2 = playerObj.GetComponent<TwoPlayerMovements>();
        if (p2 != null && p2.crashSound != null)
        {
            AudioSource audioSource = playerObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.clip = p2.crashSound;
                audioSource.Play();
            }
        }
    }

    public void PlayerCrashed()
    {
        if (gameOver) return;
        gameOver = true;

        player2Score++;
        SaveScores();
        StopAllPlayers();

        if (player2WinsText != null)
        {
            player2WinsText.gameObject.SetActive(true);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        UpdateScoreDisplays();
    }

    public void AIPlayerCrashed()
    {
        if (gameOver) return;
        gameOver = true;

        player1Score++;
        SaveScores();
        StopAllPlayers();

        if (player1WinsText != null)
        {
            player1WinsText.gameObject.SetActive(true);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        UpdateScoreDisplays();
    }

    private void StopAllPlayers()
    {
        Debug.Log("Stopping all players - Game Over!");

        if (playerMovement != null)
        {
            playerMovement.isGameOver = true;
            playerMovement.speed = 0;
            playerMovement.enabled = false;
        }
        if (aiController != null)
        {
            aiController.isGameOver = true;
            aiController.speed = 0;
            aiController.enabled = false;
        }
        if (player2Movement != null)
        {
            player2Movement.isGameOver = true;
            player2Movement.speed = 0;
            player2Movement.enabled = false;
        }
    }

    void Update()
    {
        if (gameOver)
        {
            // R key => Restart
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            // T key => Return to Game Mode Selection (changed here)
            if (Input.GetKeyDown(KeyCode.T))
            {
                ReturnToGameModeSelection();
            }
        }
    }

    void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // CHANGED: Instead of ReturnToDifficultySelection(), we use ReturnToGameModeSelection().
    void ReturnToGameModeSelection()
    {
        Debug.Log("Returning to Game Mode Panel...");

        // Reset scores if you want
        ResetScores();

        // Here's the important line:
        // We set a different key in PlayerPrefs: "ShowGameModePanel"
        PlayerPrefs.SetInt("ShowGameModePanel", 1);
        PlayerPrefs.Save();

        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void ResetScores()
    {
        player1Score = 0;
        player2Score = 0;
        PlayerPrefs.DeleteKey(PLAYER1_SCORE_KEY);
        PlayerPrefs.DeleteKey(PLAYER2_SCORE_KEY);
        PlayerPrefs.Save();
    }
}


