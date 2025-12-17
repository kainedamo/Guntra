using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 200;
    private int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 2f; // Slow pursuit

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    private float nextFireTime;

    [Header("Player Damage")]
    public int damageToPlayer = 1;

    private Transform player;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindWithTag("Player")?.transform;
        playerController = player?.GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        nextFireTime = Time.time;
    }

    void Update()
    {
        if (player == null) return;

        // Slow horizontal pursuit (match player X, keep boss Y)
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // Face player
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (player.position.x < transform.position.x);
        }

        // Fire toward player position
        if (Time.time >= nextFireTime && bulletPrefab != null && firePoint != null)
        {
            nextFireTime = Time.time + fireRate;
            Vector2 dir = (player.position - (Vector3)firePoint.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialise(dir);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO: Gore particles, score bonus, etc.
        if (playerController != null)
        {
            playerController.OnBossDefeated();
        }
        Destroy(gameObject);
    }
}