using UnityEngine;

public class SpreadShotPickup : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerController != null)
        {
            playerController.ActivateSpreadShot(); // Trigger spread
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.powerupClip);
            Destroy(gameObject);
        }
    }
}