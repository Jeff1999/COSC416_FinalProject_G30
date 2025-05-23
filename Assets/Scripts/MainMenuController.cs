﻿using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // **Main Menu Elements**
    public TextMeshProUGUI selectionArrow;
    public GameObject mainMenuPanel;

    // Parent objects for menu items
    public GameObject startItem;
    public GameObject controlsItem;
    public GameObject exitItem;

    // **Controls Page Elements**
    public GameObject controlsPanel;
    public TextMeshProUGUI selectionArrowCP;

    // **Game Mode Selection Elements**
    public GameObject gameModePanel;
    public TextMeshProUGUI selectionArrowGM;

    // Parent objects for game mode items
    public GameObject tutorialModeItem;
    public GameObject twoPlayerModeItem;
    public GameObject backToMenuItem;

    // **Level Selection Elements**
    public GameObject levelSelectPanel;
    public TextMeshProUGUI selectionArrowLS;

    // Parent objects for level selection items
    public GameObject level1Item;
    public GameObject level2Item;
    public GameObject level3Item;
    public GameObject level4Item; 
    public GameObject level5Item;
    public GameObject backToGameModeItem;

    // Level paths
    public string level1ScenePath = "Assets/Scenes/2PSceneLevel1.unity";
    public string level2ScenePath = "Assets/Scenes/2PSceneLevel2.unity";
    public string level3ScenePath = "Assets/Scenes/2PSceneLevel3.unity";
    public string level4ScenePath = "Assets/Scenes/2PSceneLevel4.unity";
    public string level5ScenePath = "Assets/Scenes/2PSceneLevel5.unity";

    // **Audio Elements**
    [Header("Audio Settings")]
    public AudioClip menuScrollSound;
    public AudioClip menuSelectSound;
    [Range(0f, 1f)]
    public float scrollSoundVolume = 0.4f;
    [Range(0f, 2f)]
    public float selectSoundVolume = 1.2f;
    private AudioSource audioSource;
    public bool useVolumeBoost = true;

    // Fixed positions for arrows (modify these to match your UI)
    [Header("Arrow Positioning")]
    public float arrowOffsetX = -20f;  // How far from the menu item the arrow should be
    public float arrowOffsetY = 0f;    // Vertical adjustment for the arrow position

    // **Scene Paths**
    [Header("Scene Paths")]
    public string tutorialScenePath = "Assets/Scenes/TutorialScene.unity";
    public string twoPlayerScenePath = "Assets/Scenes/2PScene.unity";

    // **Selection Tracking Variables**
    private int selectedIndex = 0;
    private int menuLength = 3;

    private bool isOnControlsPage = false;
    private bool isSelectingMode = false;
    private bool isSelectingLevel = false;

    private int modeIndex = 0;
    private int levelIndex = 0;

    void Start()
    {
        // Hide sub-panels at the very start
        controlsPanel.SetActive(false);
        gameModePanel.SetActive(false);
        levelSelectPanel.SetActive(false);

        // Initialize AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // Check if we should skip straight to Level Select Panel
        if (PlayerPrefs.GetInt("ShowLevelSelectPanel", 0) == 1)
        {
            // Clear that flag so it doesn't repeat
            PlayerPrefs.SetInt("ShowLevelSelectPanel", 0);
            PlayerPrefs.Save();

            // Show the Level Select panel
            OpenLevelSelectPanel();
        }
        // Check if we should skip straight to Controls Panel
        else if (PlayerPrefs.GetInt("ShowControlsPanel", 0) == 1)
        {
            // Clear that flag so it doesn't repeat
            PlayerPrefs.SetInt("ShowControlsPanel", 0);
            PlayerPrefs.Save();

            // Show the Controls panel
            OpenControlsPage();
        }
        // Check if we should skip straight to Game Mode
        else if (PlayerPrefs.GetInt("ShowGameModePanel", 0) == 1)
        {
            // Clear that flag so it doesn't repeat
            PlayerPrefs.SetInt("ShowGameModePanel", 0);
            PlayerPrefs.Save();

            // Show the "Game Mode" panel
            OpenGameModePanel();
        }
        else
        {
            // Otherwise, show the normal main menu
            OpenMainMenu();
        }

        // Initialize the arrow position on startup
        UpdateSelection();
    }

    void Update()
    {
        // **Level Selection Logic**
        if (isSelectingLevel)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Return → Level 1 → Level 2 → Level 3 → Level 4 → Level 5 → Return
                if (levelIndex == 5)      // From Return
                    levelIndex = 0;       // Go to Level 1
                else if (levelIndex == 0) // From Level 1
                    levelIndex = 1;       // Go to Level 2
                else if (levelIndex == 1) // From Level 2
                    levelIndex = 2;       // Go to Level 3
                else if (levelIndex == 2) // From Level 3
                    levelIndex = 3;       // Go to Level 4
                else if (levelIndex == 3) // From Level 4
                    levelIndex = 4;       // Go to Level 5
                else if (levelIndex == 4) // From Level 5
                    levelIndex = 5;       // Go to Return

                UpdateLevelSelection();
                PlayScrollSound();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                // Return → Level 3 → Level 2 → Level 1 → Return
                if (levelIndex == 5)      // From Return
                    levelIndex = 4;       // Go to Level 5
                else if (levelIndex == 4) // From Level 5
                    levelIndex = 3;       // Go to Level 4
                else if (levelIndex == 3) // From Level 4
                    levelIndex = 2;  // Go to Level 3
                else if (levelIndex == 2) // From Level 3
                    levelIndex = 1;       // Go to Level 2
                else if (levelIndex == 1) // From Level 2
                    levelIndex = 0;        // Go to Level 1
                else if (levelIndex == 0) // From Level 1
                    levelIndex = 5;       // Go to Return

                UpdateLevelSelection();
                PlayScrollSound();
            }

            // For horizontal movement in the top row
            if (levelIndex < 3)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    levelIndex = (levelIndex - 1 + 3) % 3;
                    UpdateLevelSelection();
                    PlayScrollSound();
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    levelIndex = (levelIndex + 1) % 3;
                    UpdateLevelSelection();
                    PlayScrollSound();
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                PlaySelectSound();
                ConfirmLevelSelection();
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
        // Position the arrow next to the selected menu item using absolute positioning
        // This is better for WebGL as it doesn't rely on dynamic calculations
        RectTransform targetRectTransform;

        switch (selectedIndex)
        {
            case 0: // Start Game
                targetRectTransform = startItem.GetComponent<RectTransform>();
                break;

            case 1: // Controls
                targetRectTransform = controlsItem.GetComponent<RectTransform>();
                break;

            case 2: // Exit Game
                targetRectTransform = exitItem.GetComponent<RectTransform>();
                break;

            default:
                targetRectTransform = startItem.GetComponent<RectTransform>();
                break;
        }

        // Fixed position for the arrow - use absolute coordinates instead of relative positioning
        selectionArrow.rectTransform.anchoredPosition = new Vector2(
            arrowOffsetX,
            targetRectTransform.anchoredPosition.y + arrowOffsetY
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
        levelSelectPanel.SetActive(false);

        // Reset states
        isSelectingMode = false;
        isOnControlsPage = false;
        isSelectingLevel = false;

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

#if UNITY_WEBGL && !UNITY_EDITOR
    // For WebGL builds, try to close the browser tab directly
    Application.ExternalEval("window.close();");
#else
        // For standalone builds, actually quit the application
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
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
        // Position arrow using fixed positions rather than calculations
        RectTransform targetRectTransform;

        switch (modeIndex)
        {
            case 0: // Tutorial Mode
                targetRectTransform = tutorialModeItem.GetComponent<RectTransform>();
                break;

            case 1: // 2 Player Mode
                targetRectTransform = twoPlayerModeItem.GetComponent<RectTransform>();
                break;

            case 2: // Back
                targetRectTransform = backToMenuItem.GetComponent<RectTransform>();
                break;

            default:
                targetRectTransform = tutorialModeItem.GetComponent<RectTransform>();
                break;
        }

        // Set fixed position for the game mode arrow
        selectionArrowGM.rectTransform.anchoredPosition = new Vector2(
            arrowOffsetX,
            targetRectTransform.anchoredPosition.y + arrowOffsetY
        );
    }

    void ConfirmPlayerMode()
    {
        if (modeIndex == 0)
        {
            Debug.Log("Tutorial Mode Selected => Loading Tutorial Scene");
            LoadTutorialScene();
        }
        else if (modeIndex == 1)
        {
            Debug.Log("2 Player Mode Selected => Opening Level Selection");
            OpenLevelSelectPanel();
        }
        else if (modeIndex == 2)
        {
            Debug.Log("Returning to Main Menu...");
            ClosePlayerModeSelection();
        }
    }

    void LoadTutorialScene()
    {
        try
        {
            // If "TutorialScene" is in Build Settings by name
            SceneManager.LoadScene("TutorialScene");
        }
        catch (System.Exception)
        {
            // Otherwise try loading by path
            Debug.LogWarning("Failed to load scene by name. Trying by path...");
            SceneManager.LoadSceneAsync(tutorialScenePath);
        }
    }

    // ----------------------------------------------------------------------
    //                  LEVEL SELECTION METHODS
    // ----------------------------------------------------------------------
    void OpenLevelSelectPanel()
    {
        // Switch into "Level Selection" UI
        isSelectingLevel = true;
        isSelectingMode = false;
        gameModePanel.SetActive(false);
        levelSelectPanel.SetActive(true);

        // Reset the level index to 3 (RETURN button)
        levelIndex = 5;
        UpdateLevelSelection();
    }

    void CloseLevelSelectPanel()
    {
        isSelectingLevel = false;
        isSelectingMode = true;
        levelSelectPanel.SetActive(false);
        gameModePanel.SetActive(true);

        // Reset game mode selection
        UpdateModeSelection();
    }

    void UpdateLevelSelection()
    {
        switch (levelIndex)
        {
            case 0: // Level 1
                    // same as before
                selectionArrowLS.rectTransform.anchoredPosition = new Vector2(-260, 230);
                break;

            case 1: // Level 2
                    // now using the old Level 3 coords
                selectionArrowLS.rectTransform.anchoredPosition = new Vector2(165, 230);
                break;

            case 2: // Level 3
                    // now using the old Level 4 coords
                selectionArrowLS.rectTransform.anchoredPosition = new Vector2(590, 230);
                break;

            case 3: // Level 4
                    // now using the old Level 2 coords
                selectionArrowLS.rectTransform.anchoredPosition = new Vector2(-260, -180);
                break;

            case 4: // Level 5
                    // same as before
                selectionArrowLS.rectTransform.anchoredPosition = new Vector2(590, -180);
                break;

            default:
                // Return position (unchanged)
                selectionArrowLS.rectTransform.anchoredPosition = new Vector2(200, -362);
                break;
        }
    }


    void ConfirmLevelSelection()
    {
        switch (levelIndex)
        {
            case 0:
                Debug.Log("Level 1 Selected => Loading 2PSceneLevel1");
                try
                {
                    // Load the 2PSceneLevel1 by name
                    SceneManager.LoadScene("2PSceneLevel1");
                }
                catch (System.Exception)
                {
                    // Fallback to path loading if needed
                    Debug.LogWarning("Failed to load scene by name. Trying by path...");
                    SceneManager.LoadSceneAsync(level1ScenePath);
                }
                break;

            case 1:
                Debug.Log("Level 2 Selected => Loading 2PSceneLevel2");
                try
                {
                    // Load the 2PSceneLevel2 by name
                    SceneManager.LoadScene("2PSceneLevel2");
                }
                catch (System.Exception)
                {
                    // Fallback to path loading if needed
                    Debug.LogWarning("Failed to load scene by name. Trying by path...");
                    SceneManager.LoadSceneAsync(level2ScenePath);
                }
                break;

            case 2:
                Debug.Log("Level 3 Selected => Loading 2PSceneLevel3");
                try
                {
                    // Load the 2PSceneLevel3 by name
                    SceneManager.LoadScene("2PSceneLevel3");
                }
                catch (System.Exception)
                {
                    // Fallback to path loading if needed
                    Debug.LogWarning("Failed to load scene by name. Trying by path...");
                    SceneManager.LoadSceneAsync(level3ScenePath);
                }
                break;

            case 3:
                Debug.Log("Level 3 Selected => Loading 2PSceneLevel4");
                try
                {
                    // Load the 2PSceneLevel3 by name
                    SceneManager.LoadScene("2PSceneLevel4");
                }
                catch (System.Exception)
                {
                    // Fallback to path loading if needed
                    Debug.LogWarning("Failed to load scene by name. Trying by path...");
                    SceneManager.LoadSceneAsync(level3ScenePath);
                }
                break;

            case 4:
                Debug.Log("Level 3 Selected => Loading 2PSceneLevel5");
                try
                {
                    // Load the 2PSceneLevel3 by name
                    SceneManager.LoadScene("2PSceneLevel5");
                }
                catch (System.Exception)
                {
                    // Fallback to path loading if needed
                    Debug.LogWarning("Failed to load scene by name. Trying by path...");
                    SceneManager.LoadSceneAsync(level3ScenePath);
                }
                break;

            case 5:
                Debug.Log("Returning to Game Mode Selection...");
                CloseLevelSelectPanel();
                break;
        }
    }

    // Add these methods for when you're ready to implement the actual level loading
    void LoadLevel(int levelNumber)
    {
        string scenePath = "";
        string sceneName = "";

        switch (levelNumber)
        {
            case 1:
                scenePath = level1ScenePath;
                sceneName = "Level1";
                break;
            case 2:
                scenePath = level2ScenePath;
                sceneName = "Level2";
                break;
            case 3:
                scenePath = level3ScenePath;
                sceneName = "Level3";
                break;
            case 4: 
                scenePath = level4ScenePath;
                sceneName = "Level4";
                break;
            case 5:
                scenePath = level5ScenePath;
                sceneName = "Level5";
                break;
        }

        try
        {
            // First try loading by name
            SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception)
        {
            // Otherwise try loading by path
            Debug.LogWarning("Failed to load scene by name. Trying by path...");
            SceneManager.LoadSceneAsync(scenePath);
        }
    }
}








