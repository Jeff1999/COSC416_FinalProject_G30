using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // For scene reloading

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 moveDirection = Vector2.up; // Start moving upward
    private bool canTurn = true;
    private bool isGameOver = false;

    // Reference to Game Over UI
    public GameObject gameOverText;

    void Start()
    {
        // Make sure Game Over UI is hidden at start
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            // Regular movement
            transform.position += new Vector3(moveDirection.x, moveDirection.y, 0) * speed * Time.deltaTime;

            // Handle input for turning
            if (canTurn)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    TurnLeft();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    TurnRight();
                }
            }
        }
        else
        {
            // Check for R key press to restart when game is over
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }

    void TurnLeft()
    {
        // Rotate the sprite
        transform.Rotate(0, 0, 90);

        // Update movement direction
        Vector2 newDir = new Vector2(-moveDirection.y, moveDirection.x);
        moveDirection = newDir;

        // Prevent multiple turns in the same frame
        StartCoroutine(TurnCooldown());
    }

    void TurnRight()
    {
        // Rotate the sprite
        transform.Rotate(0, 0, -90);

        // Update movement direction
        Vector2 newDir = new Vector2(moveDirection.y, -moveDirection.x);
        moveDirection = newDir;

        // Prevent multiple turns in the same frame
        StartCoroutine(TurnCooldown());
    }

    IEnumerator TurnCooldown()
    {
        canTurn = false;
        yield return new WaitForSeconds(0.1f);
        canTurn = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") && !isGameOver)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        // Stop the player
        speed = 0;
        isGameOver = true;

        // Show Game Over UI
        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        Debug.Log("Game Over! Press R to restart.");
    }

    void RestartGame()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}

