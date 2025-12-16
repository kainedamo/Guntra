using UnityEngine;
using System.Collections; // For IEnumerator

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public int maxHealth = 3;
    public float edgeCheckDistance = 0.6f;
    public LayerMask groundLayer;

    [Header("Shooting")]
    public GameObject enemyBulletPrefab;
    public Transform enemyFirePoint;
    public float enemyFireRate = 2.5f;
    private float nextEnemyFireTime;

    private int currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int direction = -1;
    private Transform player;

    [Header("Pickups")]
    public GameObject healthPickupPrefab; // Drag HealthPickup prefab
    public float pickupDropChance = 0.25f; // 25% chance

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        // Edge detection
        Vector2 edgeCheckPos = (Vector2)transform.position + new Vector2(direction * 0.55f, -0.6f);
        bool groundAhead = Physics2D.Raycast(edgeCheckPos, Vector2.down, edgeCheckDistance, groundLayer);

        if (!groundAhead)
        {
            direction *= -1;
        }

        // Movement
        rb.linearVelocity = new Vector2(direction * moveSpeed, 0f); // Clamp Y to 0

        // Flip
        sr.flipX = direction > 0;

        // Enemy shooting
        if (Time.time >= nextEnemyFireTime && player != null)
        {
            nextEnemyFireTime = Time.time + enemyFireRate;

            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            Vector3 spawnPos = enemyFirePoint.position;

            GameObject bullet = Instantiate(enemyBulletPrefab, spawnPos, Quaternion.identity);
            bullet.GetComponent<EnemyBullet>().Initialise(dirToPlayer);
        }
    }

    public void TakeDamage(int damage = 1)
    {
        currentHealth -= damage;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    void Die()
    {
        // Spawn health pickup (25% chance)
        if (healthPickupPrefab != null && Random.value < pickupDropChance)
        {
            Instantiate(healthPickupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        // Death effect
        ParticleSystem deathEffect = GetComponentInChildren<ParticleSystem>();
        if (deathEffect != null)
        {
            deathEffect.transform.parent = null;
            deathEffect.Stop();
            deathEffect.Clear();
            deathEffect.Play();
            Destroy(deathEffect.gameObject, deathEffect.main.duration);
        }
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (groundLayer == 0) return;
        Vector2 pos = transform.position + new Vector3(direction * 0.55f, -0.6f, 0);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(pos, Vector2.down * edgeCheckDistance);
    }
}