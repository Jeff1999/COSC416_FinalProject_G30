using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("Player References")]
    public GameObject player;
    public GameObject aiPlayer;

    [Header("UI References")]
    public GameObject gameOverPanel;    // Optional - if you have a panel
    public GameObject player1WinsText;  // Your existing Player1WinsText
    public GameObject player2WinsText;  // Your existing Player2WinsText

    private PlayerMovement playerMovement;
    private AIController aiController;
    private bool gameOver = false;

    void Start()
    {
        // Get components
        playerMovement = player.GetComponent<PlayerMovement>();
        aiController = aiPlayer.GetComponent<AIController>();

        // Make sure game over texts are hidden
        if (player1WinsText != null)
        {
            player1WinsText.SetActive(false);
        }

        if (player2WinsText != null)
        {
            player2WinsText.SetActive(false);
        }

        // If you have a panel, hide it too
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Called when the player crashes
    public void PlayerCrashed()
    {
        if (gameOver) return;

        gameOver = true;

        // Stop AI movement too
        if (aiController != null)
        {
            aiController.isGameOver = true;
            aiController.speed = 0;
        }

        // Show Player 2 Wins text
        if (player2WinsText != null)
        {
            player2WinsText.SetActive(true);
        }

        // Show game over panel if you have one
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    // Called when the AI crashes
    public void AIPlayerCrashed()
    {
        if (gameOver) return;

        gameOver = true;

        // Stop player movement too
        if (playerMovement != null)
        {
            playerMovement.isGameOver = true;
            playerMovement.speed = 0;
        }

        // Show Player 1 Wins text
        if (player1WinsText != null)
        {
            player1WinsText.SetActive(true);
        }

        // Show game over panel if you have one
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    void Update()
    {
        // Detect 'R' key to restart game
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}