using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 moveDirection;
    private bool canTurn = true;
    public bool isGameOver = false;

    // AI specific variables
    public float forwardRayDistance = 8f;   // Longer forward detection
    public float sideRayDistance = 4f;      // Shorter side detection
    public float decisionRate = 0.1f;       // Make decisions more frequently

    // Tags to check for obstacles
    public string[] obstacleTags = { "Wall", "Trail", "OpponentBorder" };

    // Reference to Game Over UI
    public GameObject gameOverText;

    // Add crash and turn sound references
    public AudioClip crashSound;
    public AudioClip turnSound;

    // Crash animation references
    public Sprite[] crashAnimationFrames;

    // Reference to GameController
    private GameController gameController;

    private AudioSource audioSource;
    private AudioSource turnAudioSource;
    private float nextDecisionTime = 0f;

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

        // Set up crash sound audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Set up turn sound audio source
        turnAudioSource = gameObject.AddComponent<AudioSource>();
        turnAudioSource.playOnAwake = false;

        // Find the GameController
        gameController = FindFirstObjectByType<GameController>();
        if (gameController == null)
        {
            Debug.LogWarning("No GameController found in the scene!");
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            // Regular movement
            transform.position += new Vector3(moveDirection.x, moveDirection.y, 0) * speed * Time.deltaTime;

            // AI decision making
            if (canTurn && Time.time >= nextDecisionTime)
            {
                MakeDecision();
                nextDecisionTime = Time.time + decisionRate;
            }
        }
    }

    void MakeDecision()
    {
        // Check forward, left and right for obstacles
        bool obstacleAhead = CheckForObstacle(moveDirection, forwardRayDistance);

        Vector2 leftDir = new Vector2(-moveDirection.y, moveDirection.x);
        Vector2 rightDir = new Vector2(moveDirection.y, -moveDirection.x);

        bool obstacleLeft = CheckForObstacle(leftDir, sideRayDistance);
        bool obstacleRight = CheckForObstacle(rightDir, sideRayDistance);

        // Check diagonal directions too for better navigation
        Vector2 diagonalLeftDir = (moveDirection + leftDir).normalized;
        Vector2 diagonalRightDir = (moveDirection + rightDir).normalized;

        bool obstacleDiagLeft = CheckForObstacle(diagonalLeftDir, sideRayDistance);
        bool obstacleDiagRight = CheckForObstacle(diagonalRightDir, sideRayDistance);

        // Debug visualize the raycasts
        Debug.DrawRay(transform.position, moveDirection * forwardRayDistance, Color.red, decisionRate);
        Debug.DrawRay(transform.position, leftDir * sideRayDistance, Color.yellow, decisionRate);
        Debug.DrawRay(transform.position, rightDir * sideRayDistance, Color.yellow, decisionRate);
        Debug.DrawRay(transform.position, diagonalLeftDir * sideRayDistance, Color.green, decisionRate);
        Debug.DrawRay(transform.position, diagonalRightDir * sideRayDistance, Color.green, decisionRate);

        if (obstacleAhead)
        {
            // If we're very close to an obstacle, emergency turn
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 2f);
            if (hit.collider != null && IsObstacle(hit.collider))
            {
                // Emergency evasion - turn to the most open direction
                if (!obstacleLeft && !obstacleRight)
                {
                    // Both directions clear, choose the one with clearest diagonal
                    if (!obstacleDiagLeft && obstacleDiagRight)
                        TurnLeft();
                    else if (obstacleDiagLeft && !obstacleDiagRight)
                        TurnRight();
                    else
                        // Both diagonals equal, choose randomly
                        if (Random.Range(0, 2) == 0) TurnLeft(); else TurnRight();
                }
                else if (!obstacleLeft)
                {
                    TurnLeft();
                }
                else if (!obstacleRight)
                {
                    TurnRight();
                }
                else
                {
                    // Both sides blocked, try a random direction (probably will crash)
                    if (Random.Range(0, 2) == 0) TurnLeft(); else TurnRight();
                }
            }
            else
            {
                // Obstacle detected ahead but not super close, make a planned turn
                if (!obstacleLeft && !obstacleRight)
                {
                    // Prefer the direction with clearer diagonal path
                    if (!obstacleDiagLeft && obstacleDiagRight)
                        TurnLeft();
                    else if (obstacleDiagLeft && !obstacleDiagRight)
                        TurnRight();
                    else
                        // Choose randomly
                        if (Random.Range(0, 2) == 0) TurnLeft(); else TurnRight();
                }
                else if (!obstacleLeft)
                {
                    TurnLeft();
                }
                else if (!obstacleRight)
                {
                    TurnRight();
                }
            }
        }
        else
        {
            // Nothing directly ahead, but check if we need to make a preemptive turn
            if ((obstacleDiagLeft || obstacleDiagRight) && Random.Range(0, 3) == 0)
            {
                // Occasionally turn away from potential danger
                if (obstacleDiagLeft && !obstacleDiagRight)
                    TurnRight();
                else if (!obstacleDiagLeft && obstacleDiagRight)
                    TurnLeft();
            }
            else
            {
                // Occasionally make random turns (5% chance when nothing ahead)
                if (Random.Range(0, 100) < 5)
                {
                    if (Random.Range(0, 2) == 0)
                        TurnLeft();
                    else
                        TurnRight();
                }
            }
        }
    }

    bool CheckForObstacle(Vector2 direction, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);

        if (hit.collider != null)
        {
            return IsObstacle(hit.collider);
        }

        return false;
    }

    bool IsObstacle(Collider2D collider)
    {
        foreach (string tag in obstacleTags)
        {
            if (collider.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
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

        // Stop the AI
        speed = 0;
        isGameOver = true;

        // Notify GameController of player win
        if (gameController != null)
        {
            gameController.AIPlayerCrashed();
        }
    }
}


