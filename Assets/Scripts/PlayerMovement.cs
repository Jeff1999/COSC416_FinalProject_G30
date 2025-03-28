using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float speed = 5f;
    private Vector2 moveDirection;
    private bool canTurn = true;
    public bool isGameOver = false;

    public GameObject gameOverText;

    public AudioClip crashSound;
    public AudioClip turnSound;
    public AudioClip jumpSound;

    public Sprite[] crashAnimationFrames;
    public Sprite[] jumpAnimationFrames;
    public float jumpAnimationSpeed = 0.05f;

    public float jumpDistance = 5f;
    public float jumpCooldown = 1f;
    private bool canJump = true;
    private bool isJumping = false;

    private AudioSource audioSource;
    private AudioSource turnAudioSource;
    private AudioSource jumpAudioSource;

    private GameController gameController;
    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;

    private TrailManager trailManager;

    void Start()
    {
        // ✅ Set initial rotation to face RIGHT (not UP)
        transform.eulerAngles = new Vector3(0, 0, -90);

        // Calculate movement direction
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        moveDirection = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));

        Debug.Log("Initial moveDirection: " + moveDirection);

        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        turnAudioSource = gameObject.AddComponent<AudioSource>();
        turnAudioSource.playOnAwake = false;

        jumpAudioSource = gameObject.AddComponent<AudioSource>();
        jumpAudioSource.playOnAwake = false;
        jumpAudioSource.volume = 0.4f;

        gameController = FindFirstObjectByType<GameController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailManager = GetComponent<TrailManager>();

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
                transform.position += new Vector3(moveDirection.x, moveDirection.y, 0) * speed * Time.deltaTime;

                if (canTurn)
                {
                    if (Input.GetKeyDown(KeyCode.A))
                        TurnLeft();
                    else if (Input.GetKeyDown(KeyCode.D))
                        TurnRight();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }

            if (canJump && !isJumping)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                    JumpLeft();
                else if (Input.GetKeyDown(KeyCode.E))
                    JumpRight();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }

    void TurnLeft()
    {
        if (turnSound != null && turnAudioSource != null)
        {
            turnAudioSource.clip = turnSound;
            turnAudioSource.Play();
        }

        transform.Rotate(0, 0, 90);
        moveDirection = new Vector2(-moveDirection.y, moveDirection.x);
        StartCoroutine(TurnCooldown());
    }

    void TurnRight()
    {
        if (turnSound != null && turnAudioSource != null)
        {
            turnAudioSource.clip = turnSound;
            turnAudioSource.Play();
        }

        transform.Rotate(0, 0, -90);
        moveDirection = new Vector2(moveDirection.y, -moveDirection.x);
        StartCoroutine(TurnCooldown());
    }

    void JumpLeft()
    {
        Vector2 jumpDirection = new Vector2(-moveDirection.y, moveDirection.x);
        ExecuteJump(jumpDirection);
    }

    void JumpRight()
    {
        Vector2 jumpDirection = new Vector2(moveDirection.y, -moveDirection.x);
        ExecuteJump(jumpDirection);
    }

    void ExecuteJump(Vector2 jumpDirection)
    {
        isJumping = true;
        canJump = false;

        if (jumpSound != null && jumpAudioSource != null)
        {
            jumpAudioSource.clip = jumpSound;
            jumpAudioSource.Play();
        }

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(jumpDirection.x, jumpDirection.y, 0) * jumpDistance;

        if (jumpAnimationFrames != null && jumpAnimationFrames.Length > 0 && spriteRenderer != null)
        {
            StartCoroutine(PlayJumpAnimationWithTeleport(startPosition, endPosition));
        }
        else
        {
            if (trailManager != null)
                trailManager.ResetTrail();

            transform.position = endPosition;
            CheckLandingCollisions(endPosition);
        }
    }

    IEnumerator PlayJumpAnimationWithTeleport(Vector3 startPos, Vector3 endPos)
    {
        Sprite savedSprite = spriteRenderer.sprite;
        int halfwayFrame = jumpAnimationFrames.Length / 2;

        for (int i = 0; i < halfwayFrame; i++)
        {
            spriteRenderer.sprite = jumpAnimationFrames[i];
            yield return new WaitForSeconds(jumpAnimationSpeed);
        }

        if (trailManager != null)
            trailManager.ResetTrail();

        transform.position = endPos;

        bool hitSomething = CheckLandingCollisions(endPos);
        if (hitSomething)
        {
            spriteRenderer.sprite = savedSprite;
            yield break;
        }

        for (int i = halfwayFrame; i < jumpAnimationFrames.Length; i++)
        {
            spriteRenderer.sprite = jumpAnimationFrames[i];
            yield return new WaitForSeconds(jumpAnimationSpeed);
        }

        spriteRenderer.sprite = savedSprite;
        isJumping = false;
        StartCoroutine(JumpCooldown());
    }

    bool CheckLandingCollisions(Vector3 landingPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(landingPosition, 0.2f);

        foreach (var collider in colliders)
        {
            if (collider.gameObject == gameObject)
                continue;

            if (collider.CompareTag("Wall") || collider.CompareTag("OpponentBorder") || collider.CompareTag("Trail") ||
                (collider.CompareTag("Player") && collider.gameObject != gameObject))
            {
                Debug.Log("Player 1 teleport collision detected with: " + collider.tag);

                TwoPlayerGameController twoPlayerController = FindFirstObjectByType<TwoPlayerGameController>();

                if (twoPlayerController != null)
                {
                    isGameOver = true;
                    speed = 0;
                    if (crashSound != null && audioSource != null)
                    {
                        audioSource.clip = crashSound;
                        audioSource.Play();
                    }

                    if (crashAnimationFrames != null && crashAnimationFrames.Length > 0)
                    {
                        CrashAnimationController crashAnimation = FindFirstObjectByType<CrashAnimationController>();
                        if (crashAnimation != null)
                        {
                            crashAnimation.StartCrashAnimation(transform.position);
                        }
                    }

                    twoPlayerController.PlayerCrashed();
                }
                else
                {
                    GameOver();
                }

                return true;
            }
        }

        return false;
    }
    public void CrashByBullet()
{
    if (isGameOver || isJumping) return;

    TwoPlayerGameController twoPlayerController = FindFirstObjectByType<TwoPlayerGameController>();

    isGameOver = true;
    speed = 0;

    if (crashSound != null && audioSource != null)
    {
        audioSource.clip = crashSound;
        audioSource.Play();
    }

    if (crashAnimationFrames != null && crashAnimationFrames.Length > 0)
    {
        CrashAnimationController crashAnimation = FindFirstObjectByType<CrashAnimationController>();
        if (crashAnimation != null)
        {
            crashAnimation.StartCrashAnimation(transform.position);
        }
    }

    if (twoPlayerController != null)
    {
        twoPlayerController.PlayerCrashed();
    }
    else
    {
        GameOver();
    }
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
        TwoPlayerGameController twoPlayerController = FindFirstObjectByType<TwoPlayerGameController>();

        if (isJumping) return;

        if (other.CompareTag("Player") && other.gameObject != gameObject && !isGameOver)
        {
            if (twoPlayerController != null)
            {
                isGameOver = true;
                speed = 0;

                if (crashSound != null && audioSource != null)
                {
                    audioSource.clip = crashSound;
                    audioSource.Play();
                }

                if (crashAnimationFrames != null && crashAnimationFrames.Length > 0)
                {
                    CrashAnimationController crashAnimation = FindFirstObjectByType<CrashAnimationController>();
                    if (crashAnimation != null)
                    {
                        crashAnimation.StartCrashAnimation(transform.position);
                    }
                }

                twoPlayerController.TieGame();
                return;
            }

            HandleTie();
            return;
        }

        if ((other.CompareTag("Wall") || other.CompareTag("OpponentBorder") || other.CompareTag("Trail")) && !isGameOver)
        {
            if (twoPlayerController != null)
            {
                isGameOver = true;
                speed = 0;

                if (crashSound != null && audioSource != null)
                {
                    audioSource.clip = crashSound;
                    audioSource.Play();
                }

                if (crashAnimationFrames != null && crashAnimationFrames.Length > 0)
                {
                    CrashAnimationController crashAnimation = FindFirstObjectByType<CrashAnimationController>();
                    if (crashAnimation != null)
                    {
                        crashAnimation.StartCrashAnimation(transform.position);
                    }
                }

                twoPlayerController.PlayerCrashed();
                return;
            }

            GameOver();
        }
    }

   

    void HandleTie()
    {
        if (crashSound != null && audioSource != null)
        {
            audioSource.clip = crashSound;
            audioSource.Play();
        }

        if (crashAnimationFrames != null && crashAnimationFrames.Length > 0)
        {
            CrashAnimationController crashAnimation = FindFirstObjectByType<CrashAnimationController>();
            if (crashAnimation != null)
            {
                crashAnimation.StartCrashAnimation(transform.position);
            }
        }

        speed = 0;
        isGameOver = true;

        if (gameController != null)
        {
            gameController.TieGame();
        }
        else
        {
            if (gameOverText != null)
            {
                gameOverText.SetActive(true);
            }
            Debug.Log("Tie Game! Press R to restart.");
        }
    }

    void GameOver()
    {
        if (crashSound != null && audioSource != null)
        {
            audioSource.clip = crashSound;
            audioSource.Play();
        }

        if (crashAnimationFrames != null && crashAnimationFrames.Length > 0)
        {
            CrashAnimationController crashAnimation = FindFirstObjectByType<CrashAnimationController>();
            if (crashAnimation != null)
            {
                crashAnimation.StartCrashAnimation(transform.position);
            }
        }

        speed = 0;
        isGameOver = true;

        if (gameController != null)
        {
            gameController.PlayerCrashed();
        }
        else
        {
            if (gameOverText != null)
            {
                gameOverText.SetActive(true);
            }
            Debug.Log("Game Over! Press R to restart.");
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

    void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
