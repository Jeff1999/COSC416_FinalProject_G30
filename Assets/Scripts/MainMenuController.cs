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
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                difficultyIndex = (difficultyIndex + 1) % 6;
                UpdateDifficultySelection();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
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
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                modeIndex = (modeIndex + 1) % 3;
                UpdateModeSelection();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                ConfirmPlayerMode();
            }

            return;
        }

        // **Controls Page Logic**
        if (isOnControlsPage)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {
                CloseControlsPage();
            }
            return;
        }

        // **Main Menu Navigation**
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + menuLength) % menuLength;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % menuLength;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SelectOption();
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
                Debug.Log("Beginner Mode Selected. Placeholder for game start...");
                break;
            case 1:
                Debug.Log("Easy Mode Selected. Placeholder for game start...");
                break;
            case 2:
                Debug.Log("Medium Mode Selected. Placeholder for game start...");
                break;
            case 3:
                Debug.Log("Hard Mode Selected. Placeholder for game start...");
                break;
            case 4:
                Debug.Log("Expert Mode Selected. Placeholder for game start...");
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





