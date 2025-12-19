using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(true); // Force on (safety)
        Time.timeScale = 0f; // Freeze everything
    }

    public void OnStartButton()
    {
        Time.timeScale = 1f;
        PlayerController pc = FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            pc.canControl = true; // <--- Directly set (no method needed)
        }
        gameObject.SetActive(false);
    }
}