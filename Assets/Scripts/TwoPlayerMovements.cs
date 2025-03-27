using UnityEngine;
using System.Collections;

public class TwoPlayerMovements : MonoBehaviour
{
    //Bullet Movement
        public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    // -- SPEED / MOVEMENT --
    public float speed = 20f;
    private Vector2 moveDirection;
    private bool canTurn = true;
    public bool isGameOver = false;

    // -- JUMP SETTINGS --
    public float jumpDistance = 5f;
    public float jumpCooldown = 1f;
    private bool canJump = true;
    private bool isJumping = false;
    public AudioClip jumpSound;
    private AudioSource jumpAudioSource;

    // -- AUDIO & SPRITES --
    public GameObject gameOverText;
    public AudioClip crashSound;
    public AudioClip turnSound;
    public Sprite[] crashAnimationFrames;
    public Sprite[] jumpAnimationFrames;
    public float jumpAnimationSpeed = 0.05f;

    private AudioSource audioSource;
    private AudioSource turnAudioSource;
    private TwoPlayerGameController gameController;
    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;

    // -- TRAIL MANAGER --
    private TrailManager trailManager;

    void Start()
    {
        // Set initial movement direction based on rotation
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        moveDirection = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));

        // Hide Game Over UI if assigned
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

        // Find TwoPlayerGameController
        gameController = FindFirstObjectByType<TwoPlayerGameController>();

        // Get the sprite renderer for jump animation
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }

        // Get or add the TrailManager
        trailManager = GetComponent<TrailManager>();
        if (trailManager == null)
        {
            trailManager = gameObject.AddComponent<TrailManager>();
            Debug.Log("TrailManager added to Player 2");
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
                    if (Input.GetKeyDown(KeyCode.J))
                    {
                        TurnLeft();
                    }
                    else if (Input.GetKeyDown(KeyCode.L))
                    {
                        TurnRight();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Semicolon))
                {
                    Shoot();
                }

            // Handle input for jumping
            if (canJump && !isJumping)
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    JumpLeft();
                }
                else if (Input.GetKeyDown(KeyCode.O))
                {
                    JumpRight();
                }
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

            // Directly finish the jump if no animation
            isJumping = false;
            StartCoroutine(JumpCooldown());
        }
    }

    // New coroutine that handles animation and teleport timing - EXACTLY like Player1
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

    // Separated collision check function that returns a bool - EXACTLY like Player1
    bool CheckLandingCollisions(Vector3 landingPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(landingPosition, 0.2f);

        foreach (var collider in colliders)
        {
            // Ignore collisions with our own collider
            if (collider.gameObject == gameObject)
                continue;

            // Check if we landed on something we shouldn't
            if (collider.CompareTag("Wall") || collider.CompareTag("Trail") ||
                (collider.CompareTag("Player") && collider.gameObject != gameObject))
            {
                Debug.Log("Player 2 teleport collision detected with: " + collider.tag);

                // Set game over flag immediately to stop movement
                isGameOver = true;
                speed = 0;

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

                // Notify GameController that player 2 crashed
                if (gameController != null)
                {
                    gameController.AIPlayerCrashed();
                }
                else
                {
                    Debug.LogError("No TwoPlayerGameController found!");
                }

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
        if (other.CompareTag("Player") && other.gameObject != gameObject && !isGameOver)
        {
            Debug.Log("Player 2 collided with Player 1 - Tie");
            HandleTie();
            return;
        }

        // Normal crash conditions
        if ((other.CompareTag("Wall") || other.CompareTag("Trail")) && !isGameOver)
        {
            Debug.Log("Player 2 crashed into wall or trail");
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

        // Set local game over flag immediately to stop movement
        isGameOver = true;
        speed = 0;

        // Notify GameController about the tie
        if (gameController != null)
        {
            gameController.TieGame();
        }
        else
        {
            // Fallback if no game controller
            if (gameOverText != null)
            {
                gameOverText.SetActive(true);
            }
            Debug.LogError("No TwoPlayerGameController found!");
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

        // Set local game over flag immediately to stop movement
        isGameOver = true;
        speed = 0;

        // Notify GameController that player 2 crashed
        if (gameController != null)
        {
            Debug.Log("Player 2 notifying game controller about crash");
            gameController.AIPlayerCrashed();
        }
        else
        {
            // Fallback if no game controller
            if (gameOverText != null)
            {
                gameOverText.SetActive(true);
            }
            Debug.LogError("No TwoPlayerGameController found!");
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = moveDirection.normalized * bulletSpeed;
            }
        }
    }
}






