using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public Image[] hearts; // Drag 3 heart Images in Inspector (array size 3)
    public Sprite fullHeart; // Your heart sprite
    public Sprite emptyHeart; // Duplicate heart, color gray (#666666) in Photoshop or tint in code

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    public void TakeDamage(int damage = 1)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Clamp to 0

        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
                hearts[i].color = Color.white;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
                hearts[i].color = Color.white;
            }
        }
    }

    void Die()
    {
        // Restart scene (or Game Over screen later)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DeactivateSpreadShot();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestoreHealth(int amount = 1)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth); // Clamp to max
        UpdateHeartsUI();
    }
}