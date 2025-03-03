using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public TextMeshProUGUI selectionArrow;  // Reference to the arrow
    private float startY;  // Store the starting Y position
    private float stepSize = 148f;  // Move exactly 148 pixels per step
    private int selectedIndex = 0;  // Track selection (0 = Start, 1 = Controls, 2 = Exit)
    private int menuLength = 3; // Total number of menu options

    void Start()
    {
        startY = selectionArrow.transform.position.y; // Store where it starts
    }

    void Update()
    {
        // Move Up
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + menuLength) % menuLength;
            UpdateSelection();
        }

        // Move Down
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % menuLength;
            UpdateSelection();
        }

        // Select Option
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SelectOption();
        }
    }

    void UpdateSelection()
    {
        // Moves exactly 148 pixels up/down based on index
        selectionArrow.transform.position = new Vector3(
            selectionArrow.transform.position.x, // Keep X position fixed
            startY - (selectedIndex * stepSize), // Move in steps of 148 pixels
            selectionArrow.transform.position.z);
    }

    void SelectOption()
    {
        switch (selectedIndex)
        {
            case 0:
                Debug.Log("Starting Game...");
                SceneManager.LoadScene("GameScene"); // Change "GameScene" to your actual game scene name
                break;
            case 1:
                Debug.Log("Opening Controls...");
                break;
            case 2:
                Debug.Log("Exiting Game...");
                QuitGame(); // Calls a separate Quit function
                break;
        }
    }

    // Handles quitting the game
    void QuitGame()
    {
        Debug.Log("Quitting Game...");

        // Properly quits when running a built application
        Application.Quit();

        // If running inside the Unity Editor, stop playing instead of quitting
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}





