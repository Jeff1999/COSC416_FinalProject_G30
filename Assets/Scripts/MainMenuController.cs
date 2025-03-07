using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // **Main Menu Elements**
    public TextMeshProUGUI selectionArrow;
    public GameObject mainMenuPanel;

    // **Controls Page Elements**
    public GameObject controlsPanel;
    public TextMeshProUGUI selectionArrowCP;

    // **Game Mode Selection Elements**
    public GameObject gameModePanel;
    public TextMeshProUGUI selectionArrowGM;

    // **Difficulty Selection Elements**
    public GameObject difficultySelectionPanel;
    public TextMeshProUGUI selectionArrowDP;

    // **Audio Elements**
    [Header("Audio Settings")]
    public AudioClip menuScrollSound;  // Assign in the Inspector
    public AudioClip menuSelectSound;  // Assign in the Inspector
    [Range(0f, 1f)]
    public float scrollSoundVolume = 0.4f;
    [Range(0f, 2f)]
    public float selectSoundVolume = 1.2f;
    private AudioSource audioSource;
    public bool useVolumeBoost = true;

    // **Scene Paths**
    [Header("Scene Paths")]
    public string beginnerScenePath = "Assets/Scenes/BeginnerScene.unity";
    public string twoPlayerScenePath = "Assets/Scenes/2PScene.unity";

    // **Selection Tracking Variables**
    private float startY;
    private float stepSize = 136f;
    private int selectedIndex = 0;
    private int menuLength = 3;

    private bool isOnControlsPage = false;
    private bool isSelectingMode = false;
    private bool isSelectingDifficulty = false;

    private int modeIndex = 0;
    private float gameModeStartY;
    private float gameModeStepSize = 136f;

    private int difficultyIndex = 0;
    private float difficultyStartY;
    private float difficultyStepSize = 136f;

    void Start()
    {
        // Record starting positions for your selection arrows
        startY = selectionArrow.transform.position.y;
        gameModeStartY = selectionArrowGM.transform.position.y;
        difficultyStartY = selectionArrowDP.transform.position.y;

        // Hide sub‐panels at the very start
        controlsPanel.SetActive(false);
        gameModePanel.SetActive(false);
        difficultySelectionPanel.SetActive(false);

        // Initialize AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // ---------------------------------------------------------
        // MAIN POINT:
        // Check if we should skip straight to Game Mode or Difficulty.
        // ---------------------------------------------------------
        if (PlayerPrefs.GetInt("ShowGameModePanel", 0) == 1)
        {
            // Clear that flag so it doesn't repeat
            PlayerPrefs.SetInt("ShowGameModePanel", 0);
            PlayerPrefs.Save();

            // Show the "Game Mode" panel
            OpenGameModePanel();
        }
        else if (PlayerPrefs.GetInt("ShowDifficultyPanel", 0) == 1)
        {
            // Clear that flag so it doesn't repeat
            PlayerPrefs.SetInt("ShowDifficultyPanel", 0);
            PlayerPrefs.Save();

            // Show the "Difficulty" panel
            OpenDifficultyPanel();
        }
        else
        {
            // Otherwise, show the normal main menu
            OpenMainMenu();
        }
    }

    void Update()
    {
        // **Difficulty Selection Logic**
        if (isSelectingDifficulty)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                difficultyIndex = (difficultyIndex - 1 + 6) % 6;
                UpdateDifficultySelection();
                PlayScrollSound();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                difficultyIndex = (difficultyIndex + 1) % 6;
                UpdateDifficultySelection();
                PlayScrollSound();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                PlaySelectSound();
                ConfirmDifficultySelection();
            }

            return;
        }

        // **Game Mode Selection Logic**
        if (isSelectingMode)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                modeIndex = (modeIndex - 1 + 3) % 3;
                UpdateModeSelection();
                PlayScrollSound();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                modeIndex = (modeIndex + 1) % 3;
                UpdateModeSelection();
                PlayScrollSound();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                PlaySelectSound();
                ConfirmPlayerMode();
            }

            return;
        }

        // **Controls Page Logic**
        if (isOnControlsPage)
        {
            // Return/Escape to go back
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {
                PlaySelectSound();
                CloseControlsPage();
            }
            return;
        }

        // **Main Menu Navigation** (when not in sub-menus)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + menuLength) % menuLength;
            UpdateSelection();
            PlayScrollSound();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % menuLength;
            UpdateSelection();
            PlayScrollSound();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlaySelectSound();
            SelectOption();
        }
    }

    // ----------------------------------------------------------------------
    //                          AUDIO HELPERS
    // ----------------------------------------------------------------------
    void PlayScrollSound()
    {
        if (menuScrollSound != null && audioSource != null)
        {
            audioSource.volume = scrollSoundVolume;
            audioSource.clip = menuScrollSound;
            audioSource.Play();
        }
    }

    void PlaySelectSound()
    {
        if (menuSelectSound != null && audioSource != null)
        {
            audioSource.volume = selectSoundVolume;
            audioSource.clip = menuSelectSound;
            audioSource.Play();

            // Optional "booster" trick
            if (selectSoundVolume > 0.5f)
            {
                GameObject audioBooster = new GameObject("SelectSoundBooster");
                audioBooster.transform.position = transform.position;
                AudioSource boosterSource = audioBooster.AddComponent<AudioSource>();
                boosterSource.clip = menuSelectSound;
                boosterSource.volume = selectSoundVolume * 0.8f;
                boosterSource.pitch = audioSource.pitch;
                boosterSource.Play();
                Destroy(audioBooster, menuSelectSound.length + 0.1f);
            }
        }
    }

    // ----------------------------------------------------------------------
    //                   MAIN MENU / CONTROLS  METHODS
    // ----------------------------------------------------------------------
    void UpdateSelection()
    {
        // Move the arrow in main menu
        selectionArrow.transform.position = new Vector3(
            selectionArrow.transform.position.x,
            startY - (selectedIndex * stepSize),
            selectionArrow.transform.position.z
        );
    }

    void SelectOption()
    {
        switch (selectedIndex)
        {
            case 0:
                Debug.Log("Opening Game Mode Selection...");
                OpenGameModePanel();
                break;
            case 1:
                Debug.Log("Opening Controls...");
                OpenControlsPage();
                break;
            case 2:
                Debug.Log("Exiting Game...");
                QuitGame();
                break;
        }
    }

    public void OpenMainMenu()
    {
        // Turn ON main menu panel
        mainMenuPanel.SetActive(true);
        // Turn OFF sub‐panels
        controlsPanel.SetActive(false);
        gameModePanel.SetActive(false);
        difficultySelectionPanel.SetActive(false);

        // Reset states
        isSelectingMode = false;
        isSelectingDifficulty = false;
        isOnControlsPage = false;

        // Reset arrow selection index if desired
        selectedIndex = 0;
        UpdateSelection();
    }

    void OpenControlsPage()
    {
        isOnControlsPage = true;
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    void CloseControlsPage()
    {
        isOnControlsPage = false;
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ----------------------------------------------------------------------
    //                  GAME MODE SELECTION METHODS
    // ----------------------------------------------------------------------
    void OpenGameModePanel()
    {
        // Switch into "Game Mode" selection UI
        isSelectingMode = true;
        mainMenuPanel.SetActive(false);
        gameModePanel.SetActive(true);

        // Reset the mode index if you want
        modeIndex = 0;
        UpdateModeSelection();
    }

    void ClosePlayerModeSelection()
    {
        isSelectingMode = false;
        gameModePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    void UpdateModeSelection()
    {
        selectionArrowGM.transform.position = new Vector3(
            selectionArrowGM.transform.position.x,
            gameModeStartY - (modeIndex * gameModeStepSize),
            selectionArrowGM.transform.position.z
        );
    }

    void ConfirmPlayerMode()
    {
        if (modeIndex == 0)
        {
            Debug.Log("1 Player Mode Selected => Opening Difficulty Panel");
            OpenDifficultyPanel();
        }
        else if (modeIndex == 1)
        {
            Debug.Log("2 Player Mode Selected => Load Two‐Player Scene");
            LoadTwoPlayerScene();
        }
        else if (modeIndex == 2)
        {
            Debug.Log("Returning to Main Menu...");
            ClosePlayerModeSelection();
        }
    }

    void LoadTwoPlayerScene()
    {
        try
        {
            // If "2PScene" is in Build Settings by name
            SceneManager.LoadScene("2PScene");
        }
        catch (System.Exception)
        {
            // Otherwise try loading by path
            Debug.LogWarning("Failed to load scene by name. Trying by path...");
            SceneManager.LoadSceneAsync(twoPlayerScenePath);
        }
    }

    // ----------------------------------------------------------------------
    //                 DIFFICULTY SELECTION METHODS
    // ----------------------------------------------------------------------
    void OpenDifficultyPanel()
    {
        // Switch into "Difficulty" selection UI
        isSelectingDifficulty = true;
        gameModePanel.SetActive(false);
        difficultySelectionPanel.SetActive(true);

        // Reset difficulty index if you want
        difficultyIndex = 0;
        UpdateDifficultySelection();
    }

    void CloseDifficultySelection()
    {
        isSelectingDifficulty = false;
        difficultySelectionPanel.SetActive(false);
        // Usually we go back to the Game Mode panel
        gameModePanel.SetActive(true);
        isSelectingMode = true;
    }

    void ConfirmDifficultySelection()
    {
        switch (difficultyIndex)
        {
            case 0:
                Debug.Log("Beginner Mode => Load BeginnerScene");
                SceneManager.LoadSceneAsync(beginnerScenePath);
                break;
            case 1:
                Debug.Log("Easy Mode (TODO: implement)...");
                // ...
                break;
            case 2:
                Debug.Log("Medium Mode (TODO: implement)...");
                // ...
                break;
            case 3:
                Debug.Log("Hard Mode (TODO: implement)...");
                // ...
                break;
            case 4:
                Debug.Log("Expert Mode (TODO: implement)...");
                // ...
                break;
            case 5:
                Debug.Log("Returning to Game Mode Panel");
                CloseDifficultySelection();
                break;
        }
    }

    void UpdateDifficultySelection()
    {
        selectionArrowDP.transform.position = new Vector3(
            selectionArrowDP.transform.position.x,
            difficultyStartY - (difficultyIndex * difficultyStepSize),
            selectionArrowDP.transform.position.z
        );
    }
}









