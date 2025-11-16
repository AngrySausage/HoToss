using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // or TMP if you use TextMeshPro

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject winPanel;          // Your existing "You Win!" panel in GameScene
    [SerializeField] private Text canCounterText;          // If using TextMeshPro, change type and update code accordingly

    [Header("Cans")]
    [SerializeField] private string canTag = "Can";
    [SerializeField] private int totalCansExpected = 6;    // Fallback; will be auto-counted at runtime

    private int totalCans;
    private int knockedDown;
    private readonly HashSet<int> countedInstanceIds = new HashSet<int>();

    private void Awake()
    {
        // Auto-detect total cans present at runtime by tag
        var cans = GameObject.FindGameObjectsWithTag(canTag);
        totalCans = cans.Length > 0 ? cans.Length : totalCansExpected;

        knockedDown = 0;
        countedInstanceIds.Clear();

        if (winPanel != null) winPanel.SetActive(false);
        UpdateCounterUI();
    }

    public void OnCanEnteredDeathZone(GameObject can)
    {
        if (can == null) return;

        int id = can.GetInstanceID();
        if (countedInstanceIds.Contains(id)) return; // already counted this specific instance

        countedInstanceIds.Add(id);
        knockedDown++;
        UpdateCounterUI();

        // Optional: despawn the can so it cannot be re-counted by other triggers
        Destroy(can);

        if (knockedDown >= totalCans)
        {
            HandleWin();
        }
    }

    private void UpdateCounterUI()
    {
        if (canCounterText != null)
        {
            canCounterText.text = $"Cans Knocked Down: {knockedDown}/{totalCans}";
        }
    }

    private void HandleWin()
    {
        if (winPanel != null) winPanel.SetActive(true);
        // Optionally pause physics:
        // Time.timeScale = 0f;
    }

    // If your Win panel has "Return to Main Menu" button wired to this:
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
