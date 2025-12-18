using UnityEngine;

public class EnemyBossNew : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 200;
    private int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.5f; // Stop this close to target X

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    private float nextFireTime;

    [Header("Player Damage")]
    public int damageToPlayer = 1;

    [Header("Arena Bounds")]
    public float arenaMinX = 115f;
    public float arenaMaxX = 155f;

    private Transform player;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindWithTag("Player")?.transform;
        playerController = player?.GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        nextFireTime = Time.time;

        // Ensure boss is on the ground
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // Prevent physics interference
        }

        Debug.Log($"Boss spawned at position: {transform.position}");
    }

    void Update()
    {
        if (player == null) return;

        // Calculate distance to player's X position
        float distanceToPlayerX = Mathf.Abs(player.position.x - transform.position.x);

        // Only move if far enough from player
        if (distanceToPlayerX > stoppingDistance)
        {
            // Determine movement direction
            float direction = Mathf.Sign(player.position.x - transform.position.x);

            // Move towards player
            Vector3 newPosition = transform.position;
            newPosition.x += direction * moveSpeed * Time.deltaTime;

            // Clamp to arena bounds
            newPosition.x = Mathf.Clamp(newPosition.x, arenaMinX, arenaMaxX);

            transform.position = newPosition;
        }

        // Face player (FIXED: removed the < sign, now uses >)
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (player.position.x > transform.position.x);
        }

        // Fire toward player position
        if (Time.time >= nextFireTime && bulletPrefab != null && firePoint != null)
        {
            nextFireTime = Time.time + fireRate;
            Vector2 dir = (player.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // Try both Bullet and EnemyBullet components
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialise(dir);
            }
            else
            {
                EnemyBullet enemyBulletScript = bullet.GetComponent<EnemyBullet>();
                if (enemyBulletScript != null)
                {
                    enemyBulletScript.Initialise(dir);
                }
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Boss took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Boss defeated!");

        if (playerController != null)
        {
            playerController.OnBossDefeated();
        }

        Destroy(gameObject);
    }
}