using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // For scene reloading

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 moveDirection = Vector2.up; // Start moving upward
    private bool canTurn = true;
    public bool isGameOver = false; // Made public for TrailGenerator

    // Reference to Game Over UI
    public GameObject gameOverText;

    // Add crash and turn sound references
    public AudioClip crashSound;
    public AudioClip turnSound;

    // Crash animation references
    public Sprite[] crashAnimationFrames;

    private AudioSource audioSource;
    private AudioSource turnAudioSource;

    void Start()
    {
        // Make sure Game Over UI is hidden at start
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        // Set up crash sound audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Set up turn sound audio source
        turnAudioSource = gameObject.AddComponent<AudioSource>();
        turnAudioSource.playOnAwake = false;
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
        // Play turn sound
        if (turnSound != null && turnAudioSource != null)
        {
            turnAudioSource.clip = turnSound;
            turnAudioSource.Play();
        }

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
        // Play turn sound
        if (turnSound != null && turnAudioSource != null)
        {
            turnAudioSource.clip = turnSound;
            turnAudioSource.Play();
        }

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
        Debug.Log(
          $"COLLISION:\n" +
          $"  Player Name: {gameObject.name}, Position: {transform.position}\n" +
          $"  Other Name: {other.gameObject.name}, Position: {other.transform.position}"
        );

        if ((other.CompareTag("Wall") || other.CompareTag("OpponentBorder") || other.CompareTag("Trail")) && !isGameOver)
        {
            GameOver();
        }
    }


    void GameOver()
    {
        // Play crash sound
        if (crashSound != null && audioSource != null)
        {
            audioSource.clip = crashSound;
            audioSource.Play();
        }

        // Trigger crash animation
        if (crashAnimationFrames != null && crashAnimationFrames.Length > 0)
        {
            CrashAnimationController crashAnimation = FindFirstObjectByType<CrashAnimationController>();
            if (crashAnimation != null)
            {
                crashAnimation.StartCrashAnimation(transform.position);
            }
        }

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