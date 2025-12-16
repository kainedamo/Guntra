using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerHealth != null)
        {
            playerHealth.RestoreHealth(1);
            Destroy(gameObject);
        }
    }
}