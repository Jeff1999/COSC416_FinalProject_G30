using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 moveDirection;
    private bool canTurn = true;
    public bool isGameOver = false;

    // Reference to Game Over UI
    public GameObject gameOverText;

    // Sound effects
    public AudioClip crashSound;
    public AudioClip turnSound;
    public AudioClip jumpSound;

    // Animation references
    public Sprite[] crashAnimationFrames;
    public Sprite[] jumpAnimationFrames;
    public float jumpAnimationSpeed = 0.05f;

    // Jump settings
    public float jumpDistance = 5f;
    public float jumpCooldown = 1f;
    private bool canJump = true;
    private bool isJumping = false;

    // Audio sources
    private AudioSource audioSource;
    private AudioSource turnAudioSource;
    private AudioSource jumpAudioSource;

    // Component references
    private GameController gameController;
    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;

    // Reference to the TrailManager
    private TrailManager trailManager;

    void Start()
    {
        // Set initial movement direction based on rotation
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        moveDirection = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));

        // Make sure Game Over UI is hidden at start
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        // Set up audio sources
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        turnAudioSource = gameObject.AddComponent<AudioSource>();
        turnAudioSource.playOnAwake = false;

        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.playOnAwake = false;
        jumpAudioSource.volume = 0.4f;

        // Get references
        gameController = FindFirstObjectByType<GameController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailManager = GetComponent<TrailManager>();

        // Store the original sprite
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            if (!isJumping)
            {
                // Regular movement when not jumping
                transform.position += new Vector3(moveDirection.x, moveDirection.y, 0) * speed * Time.deltaTime;

                // Handle input for turning
                if (canTurn)
                {
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        TurnLeft();
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        TurnRight();
                    }
                }
            }

            // Handle input for jumping
            if (canJump && !isJumping)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    JumpLeft();
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    JumpRight();
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

    void JumpLeft()
    {
        // Calculate the jump vector (perpendicular to current direction, to the left)
        Vector2 jumpDirection = new Vector2(-moveDirection.y, moveDirection.x);
        ExecuteJump(jumpDirection);
    }

    void JumpRight()
    {
        // Calculate the jump vector (perpendicular to current direction, to the right)
        Vector2 jumpDirection = new Vector2(moveDirection.y, -moveDirection.x);
        ExecuteJump(jumpDirection);
    }

    void ExecuteJump(Vector2 jumpDirection)
    {
        // Set jumping state
        isJumping = true;
        canJump = false;

        // Play jump sound
        if (jumpSound != null && jumpAudioSource != null)
        {
            jumpAudioSource.clip = jumpSound;
            jumpAudioSource.Play();
        }

        // 1. Store current position
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(jumpDirection.x, jumpDirection.y, 0) * jumpDistance;

        // 2. Start jump animation BEFORE teleporting
        if (jumpAnimationFrames != null && jumpAnimationFrames.Length > 0 && spriteRenderer != null)
        {
            StartCoroutine(PlayJumpAnimationWithTeleport(startPosition, endPosition));
        }
        else
        {
            // If no animation, just teleport immediately
            // Handle the trail and teleport
            if (trailManager != null)
            {
                trailManager.ResetTrail();
            }
            transform.position = endPosition;
            CheckLandingCollisions(endPosition);
        }
    }

    // New coroutine that handles animation and teleport timing
    IEnumerator PlayJumpAnimationWithTeleport(Vector3 startPos, Vector3 endPos)
    {
        // Save the original sprite
        Sprite savedSprite = spriteRenderer.sprite;

        // Determine halfway point in the animation
        int halfwayFrame = jumpAnimationFrames.Length / 2;

        // Play first half of animation
        for (int i = 0; i < halfwayFrame; i++)
        {
            spriteRenderer.sprite = jumpAnimationFrames[i];
            yield return new WaitForSeconds(jumpAnimationSpeed);
        }

        // Reset trail and teleport at the halfway point
        if (trailManager != null)
        {
            trailManager.ResetTrail();
        }
        transform.position = endPos;

        // Check for collisions at landing position
        bool hitSomething = CheckLandingCollisions(endPos);
        if (hitSomething)
        {
            // If collision detected, stop the animation
            spriteRenderer.sprite = savedSprite;
            yield break;
        }

        // Play second half of animation
        for (int i = halfwayFrame; i < jumpAnimationFrames.Length; i++)
        {
            spriteRenderer.sprite = jumpAnimationFrames[i];
            yield return new WaitForSeconds(jumpAnimationSpeed);
        }

        // Restore original sprite
        spriteRenderer.sprite = savedSprite;

        // Reset jumping state
        isJumping = false;
        StartCoroutine(JumpCooldown());
    }

    // Separated collision check function that returns a bool
    bool CheckLandingCollisions(Vector3 landingPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(landingPosition, 0.2f);

        foreach (var collider in colliders)
        {
            // Ignore collisions with our own collider
            if (collider.gameObject == gameObject)
                continue;

            // Check if we landed on something we shouldn't
            if (collider.CompareTag("Wall") || collider.CompareTag("OpponentBorder") ||
                collider.CompareTag("Trail") || (collider.CompareTag("Player") && collider.gameObject != gameObject))
            {
                // We hit something while landing
                GameOver();
                return true;
            }
        }

        return false; // No collisions detected
    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    IEnumerator TurnCooldown()
    {
        canTurn = false;
        yield return new WaitForSeconds(0.1f);
        canTurn = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collisions while jumping
        if (isJumping) return;

        // Check for player-to-player collision (tie case)
        if (other.CompareTag("Player") && !isGameOver)
        {
            // Only handle tie condition if this is the player (not the AI) to avoid double-processing
            if (gameObject.name == "Player")
            {
                HandleTie();
            }
            return; // Don't process further if it's a tie
        }

        // Normal crash conditions
        if ((other.CompareTag("Wall") || other.CompareTag("OpponentBorder")
             || other.CompareTag("Trail")) && !isGameOver)
        {
            GameOver();
        }
    }

    void HandleTie()
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

        // Notify GameController that it's a tie
        if (gameController != null)
        {
            gameController.TieGame();
        }
        else
        {
            // Fallback if no GameController
            if (gameOverText != null)
            {
                gameOverText.SetActive(true);
            }
            Debug.Log("Tie Game! Press R to restart.");
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

        // Notify GameController that player crashed
        if (gameController != null)
        {
            gameController.PlayerCrashed();
        }
        else
        {
            // Fallback if no GameController
            if (gameOverText != null)
            {
                gameOverText.SetActive(true);
            }
            Debug.Log("Game Over! Press R to restart.");
        }
    }

    void RestartGame()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}

