using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public TextMeshProUGUI selectionArrow; // Main menu arrow
    public GameObject mainMenuPanel; // Main menu UI
    public GameObject controlsPanel; // Controls UI
    public TextMeshProUGUI selectionArrowCP; // Arrow in Controls
    public GameObject gameModePanel; // Game Mode selection UI
    public TextMeshProUGUI selectionArrowGM; // Arrow in Game Mode selection

    private float startY; // Main Menu Arrow's original Y position
    private float stepSize = 136f; // Movement step for Main Menu
    private int selectedIndex = 0; // Selected option in Main Menu
    private int menuLength = 3; // Number of Main Menu options
    private bool isOnControlsPage = false; // Tracks if on Controls Page
    private bool isSelectingMode = false; // Tracks if on Game Mode Page
    private int modeIndex = 0; // 0 = 1P, 1 = 2P, 2 = Return
    private float gameModeStartY; // Initial Y for Game Mode Arrow
    private float gameModeStepSize = 136f; // Movement step for Game Mode Selection

    void Start()
    {
        startY = selectionArrow.transform.position.y; // Store Main Menu Arrow position
        gameModeStartY = selectionArrowGM.transform.position.y; // Store Game Mode Arrow position

        controlsPanel.SetActive(false); // Hide Controls Panel initially
        gameModePanel.SetActive(false); // Hide Game Mode Panel initially
    }

    void Update()
    {
        // **Game Mode Selection Logic**
        if (isSelectingMode)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                modeIndex = (modeIndex - 1 + 3) % 3; // Wraps around between 0, 1, 2
                UpdateModeSelection();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                modeIndex = (modeIndex + 1) % 3; // Wraps around between 0, 1, 2
                UpdateModeSelection();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                ConfirmPlayerMode();
            }

            return; // Stop Main Menu input when selecting mode
        }

        // **Controls Page Logic**
        if (isOnControlsPage)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {
                CloseControlsPage();
            }
            return; // Stop Main Menu input when on Controls Page
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
            case 0: // Start Game (Opens Game Mode Selection)
                Debug.Log("Opening Game Mode Selection...");
                OpenPlayerModeSelection();
                break;
            case 1: // Open Controls Page
                Debug.Log("Opening Controls...");
                OpenControlsPage();
                break;
            case 2: // Exit Game
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
            SceneManager.LoadScene("Placeholder1P"); // Replace with actual difficulty selection scene
        }
        else if (modeIndex == 1)
        {
            Debug.Log("2 Player Mode Selected. Loading 2P Match...");
            SceneManager.LoadScene("Placeholder2P"); // Replace with actual 2P game scene
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
}






