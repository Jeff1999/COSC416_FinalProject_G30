using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    public float speed = 20f;
    private Vector2 moveDirection;
    private bool canTurn = true;
    public bool isGameOver = false;

    // AI specific variables - significantly enhanced
    public float forwardRayDistance = 60f;   // Extreme forward vision
    public float sideRayDistance = 40f;      // Wide peripheral vision
    public float diagonalRayDistance = 50f;  // Extended diagonal awareness
    public float emergencyRayDistance = 15f; // Dedicated emergency detection

    // Safety buffer - don't turn into walls on spawn
    private bool initialSafetyPeriod = true;
    private float safetyTimer = 1.0f;

    // Direction checking
    private Vector2 lastTurnDirection;
    private int consecutiveTurns = 0;
    private List<Vector2> recentDirections = new List<Vector2>();

    // Emergency handling
    private bool inEmergencyMode = false;

    // Tags to check for obstacles
    public string[] obstacleTags = { "Wall", "Trail" };

    // Reference to player for tracking
    public Transform playerTransform;

    // Game references
    public GameObject gameOverText;
    public AudioClip crashSound;
    public AudioClip turnSound;
    public Sprite[] crashAnimationFrames;

    private AudioSource audioSource;
    private AudioSource turnAudioSource;
    private GameController gameController;

    void Start()
    {
        // Set initial movement direction based on rotation
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        moveDirection = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
        lastTurnDirection = moveDirection;
        recentDirections = new List<Vector2>();

        // Hide Game Over UI
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        // Set up audio sources
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        turnAudioSource = gameObject.AddComponent<AudioSource>();
        turnAudioSource.playOnAwake = false;

        // Find GameController
        gameController = FindFirstObjectByType<GameController>();

        // Find player if not assigned
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // Start both decision-making systems
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        // Pause briefly to ensure everything is initialized
        yield return new WaitForSeconds(0.3f);

        // Start the primary decision system
        StartCoroutine(DecisionMakingCoroutine());

        // Start the safety check system (runs more frequently)
        StartCoroutine(SafetyCheckCoroutine());
    }

    bool IsWallNearby(Vector2 direction, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);
        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            return true;
        }
        return false;
    }

    bool IsTrailNearby(Vector2 direction, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);
        if (hit.collider != null && hit.collider.CompareTag("Trail"))
        {
            return true;
        }
        return false;
    }

    void FixedUpdate()
    {
        if (!isGameOver && canTurn)
        {
            // HIGHEST PRIORITY - Wall avoidance
            if (IsWallNearby(moveDirection, 15f))
            {
                bool wallLeft = IsWallNearby(new Vector2(-moveDirection.y, moveDirection.x), 10f);
                bool wallRight = IsWallNearby(new Vector2(moveDirection.y, -moveDirection.x), 10f);

                inEmergencyMode = true;

                if (!wallLeft)
                {
                    EmergencyTurnLeft();
                }
                else if (!wallRight)
                {
                    EmergencyTurnRight();
                }
            }

            // SECOND PRIORITY - Trail avoidance
            else if (IsTrailNearby(moveDirection, 12f))
            {
                bool trailLeft = IsTrailNearby(new Vector2(-moveDirection.y, moveDirection.x), 8f);
                bool trailRight = IsTrailNearby(new Vector2(moveDirection.y, -moveDirection.x), 8f);

                inEmergencyMode = true;

                if (!trailLeft && !trailRight)
                {
                    // Both directions clear - choose based on open space
                    float leftSpace = MeasureEmergencySpace(new Vector2(-moveDirection.y, moveDirection.x));
                    float rightSpace = MeasureEmergencySpace(new Vector2(moveDirection.y, -moveDirection.x));

                    if (leftSpace > rightSpace)
                        EmergencyTurnLeft();
                    else
                        EmergencyTurnRight();
                }
                else if (!trailLeft)
                {
                    EmergencyTurnLeft();
                }
                else if (!trailRight)
                {
                    EmergencyTurnRight();
                }
            }
            else
            {
                inEmergencyMode = false;
            }
        }
    }

    void Update()
    {
        if (!isGameOver)
        {
            // Regular movement
            transform.position += new Vector3(moveDirection.x, moveDirection.y, 0) * speed * Time.deltaTime;

            // Update safety timer
            if (initialSafetyPeriod)
            {
                safetyTimer -= Time.deltaTime;
                if (safetyTimer <= 0)
                {
                    initialSafetyPeriod = false;
                }
            }

            // Visualize the rays in scene view for debugging
            DrawDebugRays();
        }
    }

    IEnumerator SafetyCheckCoroutine()
    {
        while (!isGameOver)
        {
            // Only do special safety checks if not in regular emergency mode
            if (canTurn && !inEmergencyMode)
            {
                CheckSafety();
            }

            // Run safety checks very frequently
            yield return new WaitForSeconds(0.02f);
        }
    }

    void CheckSafety()
    {
        // Additional safety check for obstacles
        if (CheckForObstacle(moveDirection, 10f))
        {
            bool obstacleLeft = CheckForObstacle(new Vector2(-moveDirection.y, moveDirection.x), 6f);
            bool obstacleRight = CheckForObstacle(new Vector2(moveDirection.y, -moveDirection.x), 6f);

            if (!obstacleLeft || !obstacleRight)
            {
                EmergencyAvoidance(obstacleLeft, obstacleRight);
            }
        }
    }

    void DrawDebugRays()
    {
        Vector2 forwardDir = moveDirection;
        Vector2 leftDir = new Vector2(-moveDirection.y, moveDirection.x);
        Vector2 rightDir = new Vector2(moveDirection.y, -moveDirection.x);

        Debug.DrawRay(transform.position, forwardDir * forwardRayDistance, Color.red);
        Debug.DrawRay(transform.position, leftDir * sideRayDistance, Color.yellow);
        Debug.DrawRay(transform.position, rightDir * sideRayDistance, Color.green);

        Vector2 diagLeftDir = (forwardDir + leftDir).normalized;
        Vector2 diagRightDir = (forwardDir + rightDir).normalized;
        Debug.DrawRay(transform.position, diagLeftDir * diagonalRayDistance, Color.cyan);
        Debug.DrawRay(transform.position, diagRightDir * diagonalRayDistance, Color.magenta);

        // Emergency rays
        Debug.DrawRay(transform.position, forwardDir * emergencyRayDistance, new Color(1f, 0f, 0f, 0.3f));
        Debug.DrawRay(transform.position, leftDir * (emergencyRayDistance * 0.7f), new Color(1f, 1f, 0f, 0.3f));
        Debug.DrawRay(transform.position, rightDir * (emergencyRayDistance * 0.7f), new Color(0f, 1f, 0f, 0.3f));
    }

    IEnumerator DecisionMakingCoroutine()
    {
        // Main decision loop
        while (!isGameOver)
        {
            if (canTurn && !inEmergencyMode)
            {
                MakeAIDecision();
            }

            // Wait between decisions - much faster for better responsiveness
            float decisionTime = Random.Range(0.01f, 0.03f);  // Extremely aggressive timing
            yield return new WaitForSeconds(decisionTime);
        }
    }

    void MakeAIDecision()
    {
        // Get standard directions
        Vector2 forwardDir = moveDirection;
        Vector2 leftDir = new Vector2(-moveDirection.y, moveDirection.x);
        Vector2 rightDir = new Vector2(moveDirection.y, -moveDirection.x);

        // Check for obstacles
        bool obstacleAhead = CheckForObstacle(forwardDir, forwardRayDistance);
        bool obstacleLeft = CheckForObstacle(leftDir, sideRayDistance);
        bool obstacleRight = CheckForObstacle(rightDir, sideRayDistance);

        // Check diagonals
        Vector2 diagLeftDir = (forwardDir + leftDir).normalized;
        Vector2 diagRightDir = (forwardDir + rightDir).normalized;
        bool diagLeftBlocked = CheckForObstacle(diagLeftDir, diagonalRayDistance);
        bool diagRightBlocked = CheckForObstacle(diagRightDir, diagonalRayDistance);

        // Handle obstacles
        if (obstacleAhead)
        {
            // Get distance to obstacle
            float distanceToObstacle = GetDistanceToObstacle(forwardDir, forwardRayDistance);

            // Need to turn - the closer the obstacle, the more urgent
            if (distanceToObstacle < 20f)
            {
                EmergencyAvoidance(obstacleLeft, obstacleRight);
            }
            else
            {
                ChooseTurnDirection(obstacleLeft, obstacleRight);
            }
            return;
        }

        // No obstacle ahead - strategic decisions

        // Skip aggressive moves during safety period
        if (initialSafetyPeriod)
        {
            return;
        }

        // Avoid being boxed in (preventive turn) - higher chance (50%)
        if (IsPotentialTrap() && Random.Range(0, 2) == 0)
        {
            PreventTrapFormation(obstacleLeft, obstacleRight);
            return;
        }

        // Try to intercept player - VERY aggressive (85% chance)
        if (playerTransform != null && Random.Range(0, 100) < 85)
        {
            TryToInterceptPlayer(obstacleLeft, obstacleRight);
            return;
        }

        // Preemptive turns to avoid diagonal obstacles - increased frequency (30%)
        if ((diagLeftBlocked || diagRightBlocked) && Random.Range(0, 10) < 3)
        {
            HandleDiagonalObstacles(diagLeftBlocked, diagRightBlocked, obstacleLeft, obstacleRight);
            return;
        }

        // Occasional random turn - 10% chance for unpredictability
        if (Random.Range(0, 100) < 10)
        {
            MakeRandomTurn(obstacleLeft, obstacleRight);
        }
    }

    void EmergencyTurnLeft()
    {
        if (turnSound != null && turnAudioSource != null)
        {
            turnAudioSource.clip = turnSound;
            turnAudioSource.Play();
        }

        // Rotate sprite
        transform.Rotate(0, 0, 90);

        // Update movement direction
        Vector2 newDir = new Vector2(-moveDirection.y, moveDirection.x);
        moveDirection = newDir;

        // Track turns
        lastTurnDirection = newDir;
        recentDirections.Add(newDir);
        if (recentDirections.Count > 5) recentDirections.RemoveAt(0);

        // Emergency turns have a shorter cooldown
        StartCoroutine(EmergencyTurnCooldown());
    }

    void EmergencyTurnRight()
    {
        if (turnSound != null && turnAudioSource != null)
        {
            turnAudioSource.clip = turnSound;
            turnAudioSource.Play();
        }

        // Rotate sprite
        transform.Rotate(0, 0, -90);

        // Update movement direction
        Vector2 newDir = new Vector2(moveDirection.y, -moveDirection.x);
        moveDirection = newDir;

        // Track turns
        lastTurnDirection = newDir;
        recentDirections.Add(newDir);
        if (recentDirections.Count > 5) recentDirections.RemoveAt(0);

        // Emergency turns have a shorter cooldown
        StartCoroutine(EmergencyTurnCooldown());
    }

    IEnumerator EmergencyTurnCooldown()
    {
        canTurn = false;
        yield return new WaitForSeconds(0.05f);  // Very short cooldown for emergencies
        canTurn = true;
    }

    float MeasureEmergencySpace(Vector2 direction)
    {
        // Quick measurement for emergency decisions
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, emergencyRayDistance);
        if (hit.collider != null && IsObstacle(hit.collider))
        {
            return hit.distance;
        }
        return emergencyRayDistance;
    }

    void EmergencyAvoidance(bool obstacleLeft, bool obstacleRight)
    {
        if (!obstacleLeft && !obstacleRight)
        {
            // Both directions available - choose the one with more space
            float leftSpace = MeasureOpenSpace(new Vector2(-moveDirection.y, moveDirection.x));
            float rightSpace = MeasureOpenSpace(new Vector2(moveDirection.y, -moveDirection.x));

            if (leftSpace > rightSpace)
                TurnLeft();
            else
                TurnRight();
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
            // Both sides blocked - pick best option based on distance
            float leftDist = GetDistanceToObstacle(new Vector2(-moveDirection.y, moveDirection.x), sideRayDistance);
            float rightDist = GetDistanceToObstacle(new Vector2(moveDirection.y, -moveDirection.x), sideRayDistance);

            if (leftDist > rightDist)
                TurnLeft();
            else
                TurnRight();
        }
    }

    void ChooseTurnDirection(bool obstacleLeft, bool obstacleRight)
    {
        if (!obstacleLeft && !obstacleRight)
        {
            // Both directions clear - make strategic choice

            // First check which direction has more open space
            float leftSpace = MeasureOpenSpace(new Vector2(-moveDirection.y, moveDirection.x));
            float rightSpace = MeasureOpenSpace(new Vector2(moveDirection.y, -moveDirection.x));

            // Adjust scores based on player position (if available)
            if (playerTransform != null)
            {
                Vector2 dirToPlayer = (playerTransform.position - transform.position).normalized;
                float leftDot = Vector2.Dot(new Vector2(-moveDirection.y, moveDirection.x), dirToPlayer);
                float rightDot = Vector2.Dot(new Vector2(moveDirection.y, -moveDirection.x), dirToPlayer);

                // Apply large bonus for direction toward player
                if (leftDot > rightDot)
                    leftSpace += 15;  // Increased player-seeking weight
                else if (rightDot > leftDot)
                    rightSpace += 15;  // Increased player-seeking weight
            }

            // Avoid consecutive same turns to prevent loops
            if (IsRepeatingDirection(new Vector2(-moveDirection.y, moveDirection.x)))
            {
                leftSpace *= 0.4f;  // Stronger penalty for repeating patterns
            }
            else if (IsRepeatingDirection(new Vector2(moveDirection.y, -moveDirection.x)))
            {
                rightSpace *= 0.4f;  // Stronger penalty for repeating patterns
            }

            // Choose direction with higher score
            if (leftSpace > rightSpace)
                TurnLeft();
            else
                TurnRight();
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
            // Both sides blocked - emergency random choice
            if (Random.Range(0, 2) == 0)
                TurnLeft();
            else
                TurnRight();
        }
    }

    bool IsRepeatingDirection(Vector2 direction)
    {
        if (recentDirections.Count < 3) return false;

        int matchCount = 0;
        for (int i = recentDirections.Count - 1; i >= recentDirections.Count - 3; i--)
        {
            if (Vector2.Dot(recentDirections[i], direction) > 0.9f)
            {
                matchCount++;
            }
        }

        return matchCount >= 2;  // At least 2 of the last 3 turns were similar
    }

    bool IsPotentialTrap()
    {
        int blockedDirections = 0;
        int criticalBlockCount = 0;

        // Cast rays in multiple directions with longer distance
        for (int i = 0; i < 16; i++) // Even more rays for better detection
        {
            float angle = i * 22.5f * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            float distance = 20f; // Long distance trap detection
            if (CheckForObstacle(direction, distance))
            {
                blockedDirections++;

                // Check if nearby obstacles (critical)
                if (CheckForObstacle(direction, 8f))
                {
                    criticalBlockCount++;
                }
            }
        }

        // If many directions are blocked, it might be a trap
        // More aggressive detection
        return blockedDirections >= 6 || criticalBlockCount >= 3;
    }

    float GetDistanceToObstacle(Vector2 direction, float maxDistance)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance);
        if (hit.collider != null && IsObstacle(hit.collider))
        {
            return hit.distance;
        }
        return maxDistance;
    }

    void PreventTrapFormation(bool obstacleLeft, bool obstacleRight)
    {
        // Find the most open direction with advanced heuristics
        Vector2 leftDir = new Vector2(-moveDirection.y, moveDirection.x);
        Vector2 rightDir = new Vector2(moveDirection.y, -moveDirection.x);

        float forwardSpace = MeasureExtendedOpenSpace(moveDirection);
        float leftSpace = obstacleLeft ? 0 : MeasureExtendedOpenSpace(leftDir);
        float rightSpace = obstacleRight ? 0 : MeasureExtendedOpenSpace(rightDir);

        // Choose direction with most open space
        if (forwardSpace > leftSpace * 1.2f && forwardSpace > rightSpace * 1.2f)
        {
            // Significantly more space forward - continue forward
        }
        else if (leftSpace > rightSpace && !obstacleLeft)
        {
            TurnLeft();
        }
        else if (!obstacleRight)
        {
            TurnRight();
        }
    }

    float MeasureExtendedOpenSpace(Vector2 direction)
    {
        // Enhanced open space measurement for trap avoidance
        float totalSpace = 0;
        int numRays = 7;  // More rays for better detection

        // Cast rays in a wider fan pattern
        for (int i = 0; i < numRays; i++)
        {
            float angle = (i - numRays / 2) * 10f * Mathf.Deg2Rad; // Tighter angles for precision
            Vector2 rayDir = RotateVector(direction, angle);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, forwardRayDistance * 1.2f);
            if (hit.collider != null && IsObstacle(hit.collider))
            {
                totalSpace += hit.distance;
            }
            else
            {
                totalSpace += forwardRayDistance * 1.2f;
            }
        }

        return totalSpace / numRays;
    }

    void TryToInterceptPlayer(bool obstacleLeft, bool obstacleRight)
    {
        if (playerTransform == null) return;

        // Get direction to player
        Vector2 dirToPlayer = (playerTransform.position - transform.position).normalized;
        Vector2 leftDir = new Vector2(-moveDirection.y, moveDirection.x);
        Vector2 rightDir = new Vector2(moveDirection.y, -moveDirection.x);

        // Calculate predicted player path
        Vector2 playerVelocity = dirToPlayer; // Simplistic prediction
        List<Vector2> predictedPlayerPath = new List<Vector2>();
        Vector2 futurePlayerPos = playerTransform.position;

        for (int i = 0; i < 10; i++)
        {
            futurePlayerPos = futurePlayerPos + playerVelocity * 2f;
            predictedPlayerPath.Add(futurePlayerPos);
        }

        // Calculate interception score for each direction
        float forwardScore = CalculateInterceptionScore(moveDirection, predictedPlayerPath);
        float leftScore = obstacleLeft ? 0 : CalculateInterceptionScore(leftDir, predictedPlayerPath);
        float rightScore = obstacleRight ? 0 : CalculateInterceptionScore(rightDir, predictedPlayerPath);

        // Calculate dot product to see which direction aligns better with player
        float dotForward = Vector2.Dot(moveDirection, dirToPlayer);
        float dotLeft = Vector2.Dot(leftDir, dirToPlayer);
        float dotRight = Vector2.Dot(rightDir, dirToPlayer);

        // IMPROVED: Add distance factor to prioritize closer player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        float proximityFactor = Mathf.Clamp(25f / distanceToPlayer, 1f, 15f); // Higher factor when closer

        // Adjust scores with proximity and interception
        forwardScore += dotForward * proximityFactor * 30;
        leftScore += dotLeft * proximityFactor * 30;
        rightScore += dotRight * proximityFactor * 30;

        // Choose direction with highest score
        if (leftScore > forwardScore && leftScore > rightScore && !obstacleLeft)
        {
            TurnLeft();
        }
        else if (rightScore > forwardScore && rightScore > leftScore && !obstacleRight)
        {
            TurnRight();
        }
        // Otherwise continue forward (current direction is best)
    }

    float CalculateInterceptionScore(Vector2 direction, List<Vector2> playerPath)
    {
        float score = 0;
        Vector2 position = transform.position;

        // Project our movement along this direction
        for (int i = 0; i < 12; i++)
        {
            position += direction * 2f;

            // Check if we might intercept the player's path
            foreach (Vector2 playerPos in playerPath)
            {
                float distance = Vector2.Distance(position, playerPos);
                if (distance < 6f) // Close enough to consider an interception
                {
                    // Higher score for earlier interception
                    score += (12f - i) * 5f;

                    // Higher score for closer interception
                    score += (6f - distance) * 3f;
                }
            }
        }

        return score;
    }

    void HandleDiagonalObstacles(bool diagLeftBlocked, bool diagRightBlocked,
                                  bool obstacleLeft, bool obstacleRight)
    {
        // Turn away from blocked diagonal - more aggressive about avoidance
        if (diagLeftBlocked && !diagRightBlocked && !obstacleRight)
        {
            TurnRight();
        }
        else if (!diagLeftBlocked && diagRightBlocked && !obstacleLeft)
        {
            TurnLeft();
        }
        else if (diagLeftBlocked && diagRightBlocked)
        {
            // Both diagonals blocked - evaluate if forward is better
            float forwardSpace = MeasureOpenSpace(moveDirection);
            float leftSpace = obstacleLeft ? 0 : MeasureOpenSpace(new Vector2(-moveDirection.y, moveDirection.x));
            float rightSpace = obstacleRight ? 0 : MeasureOpenSpace(new Vector2(moveDirection.y, -moveDirection.x));

            if ((leftSpace > forwardSpace || rightSpace > forwardSpace) && Random.Range(0, 100) < 70)
            {
                if (leftSpace > rightSpace && !obstacleLeft)
                    TurnLeft();
                else if (!obstacleRight)
                    TurnRight();
            }
        }
    }

    void MakeRandomTurn(bool obstacleLeft, bool obstacleRight)
    {
        // Random turn if possible, weighted by open space
        if (!obstacleLeft && !obstacleRight)
        {
            float leftSpace = MeasureOpenSpace(new Vector2(-moveDirection.y, moveDirection.x));
            float rightSpace = MeasureOpenSpace(new Vector2(moveDirection.y, -moveDirection.x));

            // Weighted random choice - prefer the more open direction
            float totalSpace = leftSpace + rightSpace;
            float randomVal = Random.Range(0, totalSpace);

            if (randomVal < leftSpace)
                TurnLeft();
            else
                TurnRight();
        }
        else if (!obstacleLeft)
        {
            TurnLeft();
        }
        else if (!obstacleRight)
        {
            TurnRight();
        }
        // Both blocked - don't turn
    }

    float MeasureOpenSpace(Vector2 direction)
    {
        float totalSpace = 0;
        int numRays = 5;

        // Cast multiple rays in a fan pattern
        for (int i = 0; i < numRays; i++)
        {
            float angle = (i - numRays / 2) * 15f * Mathf.Deg2Rad;
            Vector2 rayDir = RotateVector(direction, angle);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, forwardRayDistance);
            if (hit.collider != null && IsObstacle(hit.collider))
            {
                totalSpace += hit.distance;
            }
            else
            {
                totalSpace += forwardRayDistance;
            }
        }

        return totalSpace / numRays;
    }

    Vector2 RotateVector(Vector2 vector, float angle)
    {
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
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

        // Rotate sprite
        transform.Rotate(0, 0, 90);

        // Update movement direction
        Vector2 newDir = new Vector2(-moveDirection.y, moveDirection.x);
        moveDirection = newDir;

        // Track consecutive turns
        if (lastTurnDirection == newDir)
        {
            consecutiveTurns++;
        }
        else
        {
            consecutiveTurns = 1;
            lastTurnDirection = newDir;
        }

        // Track turn history
        recentDirections.Add(newDir);
        if (recentDirections.Count > 5) recentDirections.RemoveAt(0);

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

        // Rotate sprite
        transform.Rotate(0, 0, -90);

        // Update movement direction
        Vector2 newDir = new Vector2(moveDirection.y, -moveDirection.x);
        moveDirection = newDir;

        // Track consecutive turns
        if (lastTurnDirection == newDir)
        {
            consecutiveTurns++;
        }
        else
        {
            consecutiveTurns = 1;
            lastTurnDirection = newDir;
        }

        // Track turn history
        recentDirections.Add(newDir);
        if (recentDirections.Count > 5) recentDirections.RemoveAt(0);

        // Prevent multiple turns in the same frame
        StartCoroutine(TurnCooldown());
    }

    IEnumerator TurnCooldown()
    {
        canTurn = false;
        yield return new WaitForSeconds(0.08f);
        canTurn = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.name == "Player" && !isGameOver)
        {
            // Let the player handle the tie condition
            return;
        }

        if ((other.CompareTag("Wall") || other.CompareTag("Trail")) && !isGameOver)
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

        // Trigger animation
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

        // Notify GameController
        if (gameController != null)
        {
            gameController.AIPlayerCrashed();
        }
    }
}

