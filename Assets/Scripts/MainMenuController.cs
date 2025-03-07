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
    public AudioClip menuScrollSound;  // Assign MenuScrollSound.wav in the Inspector
    public AudioClip menuSelectSound;  // Assign MenuSelectSound.wav in the Inspector
    [Range(0f, 1f)]
    public float scrollSoundVolume = 0.4f;  // Lower volume for scroll sound
    [Range(0f, 2f)]                         // Allow up to 200% volume
    public float selectSoundVolume = 1.2f;  // Boosted volume for select sound (120%)
    private AudioSource audioSource;
    [Tooltip("Enable this to amplify the select sound beyond Unity's normal volume limits")]
    public bool useVolumeBoost = true;

    // **Scene Paths**
    [Header("Scene Paths")]
    public string beginnerScenePath = "Assets/Scenes/BeginnerScene.unity"; // Set this in the Inspector if needed

    // **Selection Tracking Variables**
    private float startY;
    private float stepSize = 136f;
    private int selectedIndex = 0;
    private int menuLength = 3;

    private bool isOnControlsPage = false;
    private bool isSelectingMode = false;
    private bool isSelectingDifficulty = false;

    private int modeIndex = 0; // Game Mode Selection (1P, 2P, Return)
    private float gameModeStartY;
    private float gameModeStepSize = 136f;

    private int difficultyIndex = 0; // Difficulty Selection (Beginner, Easy, Medium, Hard, Expert, Return)
    private float difficultyStartY;
    private float difficultyStepSize = 136f;

    void Start()
    {
        startY = selectionArrow.transform.position.y;
        gameModeStartY = selectionArrowGM.transform.position.y;
        difficultyStartY = selectionArrowDP.transform.position.y;

        // **Hide all non-main menu panels at start**
        controlsPanel.SetActive(false);
        gameModePanel.SetActive(false);
        difficultySelectionPanel.SetActive(false);

        // Initialize audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // Check if we should go directly to difficulty selection panel
        if (PlayerPrefs.GetInt("ShowDifficultyPanel", 0) == 1)
        {
            // Clear the flag
            PlayerPrefs.DeleteKey("ShowDifficultyPanel");
            PlayerPrefs.Save();

            // Open game mode selection first
            OpenPlayerModeSelection();

            // Then open difficulty selection
            OpenDifficultySelection();
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
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {
                PlaySelectSound();
                CloseControlsPage();
            }
            return;
        }

        // **Main Menu Navigation**
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

    // **Audio Methods**
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

            // Boost the volume by playing multiple audio sources simultaneously
            audioSource.Play();

            // Create a temporary additional audio source for a louder effect
            if (selectSoundVolume > 0.5f) // Only create the booster if volume is set high enough
            {
                // Create GameObject with a second audio source for doubled volume
                GameObject audioBooster = new GameObject("SelectSoundBooster");
                audioBooster.transform.position = transform.position;
                AudioSource boosterSource = audioBooster.AddComponent<AudioSource>();

                // Copy settings from main audio source
                boosterSource.clip = menuSelectSound;
                boosterSource.volume = selectSoundVolume * 0.8f; // Slightly lower to avoid distortion
                boosterSource.pitch = audioSource.pitch;
                boosterSource.Play();

                // Destroy the temporary audio source after the sound finishes playing
                Destroy(audioBooster, menuSelectSound.length + 0.1f);
            }
        }
    }

    // **Main Menu Methods**
    void UpdateSelection()
    {
        selectionArrow.transform.position = new Vector3(
            selectionArrow.transform.position.x,
            startY - (selectedIndex * stepSize),
            selectionArrow.transform.position.z);
    }

    void SelectOption()
    {
        switch (selectedIndex)
        {
            case 0:
                Debug.Log("Opening Game Mode Selection...");
                OpenPlayerModeSelection();
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

    // **Game Mode Selection Methods**
    void OpenPlayerModeSelection()
    {
        isSelectingMode = true;
        mainMenuPanel.SetActive(false);
        gameModePanel.SetActive(true);
    }

    void ClosePlayerModeSelection()
    {
        isSelectingMode = false;
        gameModePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    void ConfirmPlayerMode()
    {
        if (modeIndex == 0)
        {
            Debug.Log("1 Player Mode Selected. Moving to Difficulty Selection...");
            OpenDifficultySelection();
        }
        else if (modeIndex == 1)
        {
            Debug.Log("2 Player Mode Selected. Placeholder for 2P match...");
            // **Placeholder for actual 2P scene loading later**
        }
        else if (modeIndex == 2)
        {
            Debug.Log("Returning to Main Menu...");
            ClosePlayerModeSelection();
        }
    }

    void UpdateModeSelection()
    {
        selectionArrowGM.transform.position = new Vector3(
            selectionArrowGM.transform.position.x,
            gameModeStartY - (modeIndex * gameModeStepSize),
            selectionArrowGM.transform.position.z
        );
    }

    // **Difficulty Selection Methods**
    void OpenDifficultySelection()
    {
        isSelectingDifficulty = true;
        gameModePanel.SetActive(false);
        difficultySelectionPanel.SetActive(true);
    }

    void CloseDifficultySelection()
    {
        isSelectingDifficulty = false;
        difficultySelectionPanel.SetActive(false);
        gameModePanel.SetActive(true);
    }

    void ConfirmDifficultySelection()
    {
        switch (difficultyIndex)
        {
            case 0:
                Debug.Log("Beginner Mode Selected. Loading BeginnerScene...");
                // Load the beginner scene by path
                SceneManager.LoadSceneAsync(beginnerScenePath);
                break;
            case 1:
                Debug.Log("Easy Mode Selected. Placeholder for game start...");
                // TODO: Load Easy difficulty scene when ready
                break;
            case 2:
                Debug.Log("Medium Mode Selected. Placeholder for game start...");
                // TODO: Load Medium difficulty scene when ready
                break;
            case 3:
                Debug.Log("Hard Mode Selected. Placeholder for game start...");
                // TODO: Load Hard difficulty scene when ready
                break;
            case 4:
                Debug.Log("Expert Mode Selected. Placeholder for game start...");
                // TODO: Load Expert difficulty scene when ready
                break;
            case 5:
                Debug.Log("Returning to Game Mode Selection...");
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









