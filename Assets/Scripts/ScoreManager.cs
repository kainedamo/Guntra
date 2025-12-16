using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText; // Drag ScoreText
    public int score = 0;

    public static ScoreManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddScore(int amount)
    {
        if (instance == null) return; // Null-safe

        instance.score += amount;
        if (instance.scoreText != null)
        {
            instance.scoreText.text = "Score: " + instance.score;
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("HighScore", score);
    }
}