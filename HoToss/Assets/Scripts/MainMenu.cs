using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the main gameplay scene
        SceneManager.LoadScene("GameScene");
    }

    public void OpenControls()
    {
        // Load the controls scene
        SceneManager.LoadScene("Controls");
    }

    public void ExitGame()
    {
        // Exit the application
        Application.Quit();

        // This line ensures it also stops Play mode when testing in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
