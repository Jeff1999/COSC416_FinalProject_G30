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

    [Header("Audio")]
    public AudioClip hitSoundGam;
    private AudioSource audioSource;
    private AudioSource myAudioSource;

    private PlayerMovement playerMovement;
    private AIController aiController;
    private TwoPlayerMovements player2Movement;
    private bool gameOver = false;

    // Score tracking
    private int player1Score = 0;
    private int player2Score = 0;
    private const string PLAYER1_SCORE_KEY = "Player1Score";
    private const string PLAYER2_SCORE_KEY = "Player2Score";

    // Font reference
    public TMP_FontAsset gameFont;  // Add this line to reference the font asset

    //Random Spawn object
    [SerializeField] private Vector2 position; 
    

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f;

        playerMovement = player.GetComponent<PlayerMovement>();
        aiController = aiPlayer.GetComponent<AIController>();
        player2Movement = aiPlayer.GetComponent<TwoPlayerMovements>();

        LoadScores();
        UpdateScoreDisplays();

        // Set the font for all text components
        player1WinsText.font = gameFont;
        player2WinsText.font = gameFont;
        tieGameText.font = gameFont;
        scoreTextPlayer1.font = gameFont;
        scoreTextPlayer2.font = gameFont;

        if (player1WinsText != null) player1WinsText.gameObject.SetActive(false);
        if (player2WinsText != null) player2WinsText.gameObject.SetActive(false);
        if (tieGameText != null) tieGameText.gameObject.SetActive(false);

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        SetupPlayerCollision();

        StartCoroutine(CountdownAndStart());
    }

    IEnumerator CountdownAndStart()
    {
        if (playerMovement != null) playerMovement.enabled = false;
        if (aiController != null) aiController.enabled = false;
        if (player2Movement != null) player2Movement.enabled = false;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            for (int i = 3; i > 0; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSeconds(0.9f);
            }

            countdownText.text = "GO!";
            yield return new WaitForSeconds(0.4f);

            countdownText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        if (!gameOver && playerMovement != null) playerMovement.enabled = true;
        if (!gameOver && aiController != null) aiController.enabled = true;
        if (!gameOver && player2Movement != null) player2Movement.enabled = true;
    }

    void SetupPlayerCollision()
    {
        if (player != null && !player.CompareTag("Player")) player.tag = "Player";
        if (aiPlayer != null && !aiPlayer.CompareTag("Player")) aiPlayer.tag = "Player";

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

        StopAllOtherAudio();
        audioSource.PlayOneShot(hitSoundGam);

        if (tieGameText != null) tieGameText.gameObject.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void PlayerCrashed()
    {
        if (gameOver) return;
        gameOver = true;

        player2Score++;
        SaveScores();
        StopAllPlayers();
        TriggerBothCrashAnimations();

        StopAllOtherAudio();
        audioSource.PlayOneShot(hitSoundGam);

        if (player2WinsText != null) player2WinsText.gameObject.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        UpdateScoreDisplays();
    }

    public void AIPlayerCrashed()
    {
        if (gameOver) return;
        gameOver = true;

        player1Score++;
        SaveScores();
        StopAllPlayers();
        TriggerBothCrashAnimations();

        StopAllOtherAudio();
        audioSource.PlayOneShot(hitSoundGam);

        if (player1WinsText != null) player1WinsText.gameObject.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        UpdateScoreDisplays();
    }

    private void StopAllPlayers()
    {
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

    private void TriggerBothCrashAnimations()
    {
        CrashAnimationController crashAnimation = FindFirstObjectByType<CrashAnimationController>();
        if (crashAnimation != null)
        {
            if (player != null) crashAnimation.StartCrashAnimation(player.transform.position);
            if (aiPlayer != null) crashAnimation.StartCrashAnimation(aiPlayer.transform.position);
        }

        PlayCrashSoundOnPlayer(player);
        PlayCrashSoundOnPlayer(aiPlayer);
    }

    private void PlayCrashSoundOnPlayer(GameObject playerObj)
    {
        if (playerObj == null) return;

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

    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
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

    void ReturnToGameModeSelection()
    {
        Debug.Log("Returning to Level Selection Panel...");

        ResetScores();

        PlayerPrefs.SetInt("ShowLevelSelectPanel", 1);
        PlayerPrefs.Save();

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

    void StopAllOtherAudio()
    {
        AudioSource[] allAudioSources = Object.FindObjectsByType<AudioSource>(
            FindObjectsInactive.Exclude,   // or Include if you want to find disabled objects
            FindObjectsSortMode.None       // how you want them sorted
        );

        foreach (AudioSource audio in allAudioSources)
        {
            if (audio != myAudioSource)
            {
                audio.Stop();
            }
        }
    }
}
