using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public TextMeshProUGUI selectionArrow; // Reference to the arrow
    public GameObject mainMenuPanel; // Main menu UI panel
    public GameObject controlsPanel; // Controls page UI panel
    public TextMeshProUGUI selectionArrowCP; // Selection arrow on controls page

    private float startY; // Stores the original Y position of the selection arrow
    private float stepSize = 148f; // Moves exactly 148 pixels per step
    private int selectedIndex = 0; // Tracks which menu option is selected
    private int menuLength = 3; // Number of options in the main menu
    private bool isOnControlsPage = false; // Tracks whether we're in the Controls Page

    void Start()
    {
        startY = selectionArrow.transform.position.y; // Store starting Y position
        controlsPanel.SetActive(false); // Hide controls panel at the start
    }

    void Update()
    {
        if (isOnControlsPage)
        {
            // Controls Page Navigation: Press Enter or Escape to return to Main Menu
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {
                CloseControlsPage();
            }
            return; // Prevents main menu input from interfering while in Controls Page
        }

        // Menu Navigation
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
            case 0: // Start Game
                Debug.Log("Starting Game...");
                SceneManager.LoadScene("GameScene"); // Change "GameScene" to your actual game scene
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
        mainMenuPanel.SetActive(false); // Hide Main Menu
        controlsPanel.SetActive(true); // Show Controls Page
    }

    void CloseControlsPage()
    {
        isOnControlsPage = false;
        controlsPanel.SetActive(false); // Hide Controls Page
        mainMenuPanel.SetActive(true); // Show Main Menu
    }

    void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}





