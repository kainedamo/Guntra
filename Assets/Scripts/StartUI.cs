using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // If text used

public class StartUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel; // Drag StartPanel (self)
    public GameObject controlsPanel; // Drag ControlsPanel
    public GameObject creditsPanel; // Drag CreditsPanel

    void Awake()
    {
        // Activate main at start, hide subs
        if (startPanel != null) startPanel.SetActive(true);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        Time.timeScale = 0f; // Pause game behind UI
    }

    public void ShowMainMenu()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (startPanel != null) startPanel.SetActive(true);
    }

    public void ShowControls()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    public void ShowCredits()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void StartGame()
    {
        Time.timeScale = 1f; // Unfreeze game
        gameObject.SetActive(false); // Hide entire StartUI
    }

    public void QuitGame()
    {
        Application.Quit(); // Editor: Logs "Quit", Build: Closes
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}