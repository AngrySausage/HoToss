using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;
    public GameObject winPanel;

    [Header("Game UI Elements")]
    public Text canCounterText;      // UI text for "Cans Knocked Down: x/6"
    public GameObject chargeSlider;  // The charge slider object (for enabling/disabling during pause if needed)

    private int cansKnockedDown = 0;
    private int totalCans = 6;  // total number of cans in the level (default 6)

    private bool gameIsActive = false;  // tracks if the gameplay is currently active (not in a menu)

    void Start()
    {
        // Initial state: show main menu, hide other UI panels
        mainMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
        winPanel.SetActive(false);
        // Ensure game UI (counter and slider) is hidden or inactive at start
        if (canCounterText != null)
            canCounterText.gameObject.SetActive(false);
        if (chargeSlider != null)
            chargeSlider.SetActive(false);

        // Count total cans dynamically (optional): find all objects tagged "Can"
        GameObject[] allCans = GameObject.FindGameObjectsWithTag("Can");
        if (allCans.Length > 0)
        {
            totalCans = allCans.Length;
        }
        // Initialize the counter text
        UpdateCanCounterUI();
    }

    // Call this when the Play button is pressed
    public void StartGame()
    {
        // Hide main menu and controls panels, show game UI
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        winPanel.SetActive(false);
        if (canCounterText != null)
        {
            canCounterText.gameObject.SetActive(true);
        }
        if (chargeSlider != null)
        {
            chargeSlider.SetActive(true); // though the ThrowController will control its visibility
        }
        // Reset game state values
        cansKnockedDown = 0;
        UpdateCanCounterUI();
        gameIsActive = true;
        // (If we had paused time, resume it. We did not pause time here, so nothing else needed)
    }

    // Call this when the Controls button is pressed
    public void ShowControls()
    {
        controlsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    // Call this for returning from the Controls screen to Main Menu
    public void HideControls()
    {
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // This is called from DeathZone script when a can falls
    public void OnCanKnockedDown()
    {
        if (!gameIsActive) return;  // only count if game is active
        cansKnockedDown++;
        UpdateCanCounterUI();
        // Check win condition
        if (cansKnockedDown >= totalCans)
        {
            WinGame();
        }
    }

    // Update the UI text for the can counter
    private void UpdateCanCounterUI()
    {
        if (canCounterText != null)
        {
            canCounterText.text = $"Cans Knocked Down: {cansKnockedDown}/{totalCans}";
        }
    }

    // Called when all cans are knocked down
    private void WinGame()
    {
        gameIsActive = false;
        // Show the win screen
        winPanel.SetActive(true);
        // Optionally, pause the game (stop physics and input)
        // Time.timeScale = 0f; // (if you want to freeze everything on win)
    }

    // Call this when "Return to Main Menu" button is pressed on win screen (or Back from controls, if you want to reuse it)
    public void ReturnToMainMenu()
    {
        // Reload the scene to reset everything to initial state
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Call this when Exit button is pressed
    public void ExitGame()
    {
        Application.Quit();
        // Note: This will have no effect in the Unity editor, but will work in a build.
        // You can use UnityEditor.EditorApplication.isPlaying = false; to stop the editor play mode if testing, but it's optional.
    }
}
