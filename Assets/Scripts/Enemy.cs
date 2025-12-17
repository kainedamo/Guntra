using UnityEngine;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public static event Action OnDestroyed;

    public float moveSpeed = 1.5f;
    public int maxHealth = 3;
    public float edgeCheckDistance = 0.6f;
    public LayerMask groundLayer;

    [Header("Shooting")]
    public GameObject enemyBulletPrefab;
    public Transform enemyFirePoint;
    public float enemyFireRate = 2.5f;
    private float nextEnemyFireTime;

    [Header("Pickups")]
    public GameObject healthPickupPrefab;
    public float pickupDropChance = 0.25f;

    private int currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int direction = -1;
    private Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        nextEnemyFireTime = Time.time;
    }

    void FixedUpdate()
    {
        // Ground check
        Vector2 groundCheckPos = (Vector2)transform.position + Vector2.down * 0.6f;
        bool onGround = Physics2D.OverlapCircle(groundCheckPos, 0.1f, groundLayer);

        if (onGround)
        {
            // Face player only if far away (>5 units — no jitter)
            if (player != null && Mathf.Abs(player.position.x - transform.position.x) > 5f)
            {
                direction = player.position.x > transform.position.x ? 1 : -1;
            }

            // Edge detection fallback
            Vector2 edgeCheckPos = (Vector2)transform.position + new Vector2(direction * 0.55f, -0.6f);
            bool groundAhead = Physics2D.Raycast(edgeCheckPos, Vector2.down, edgeCheckDistance, groundLayer);
            if (!groundAhead)
            {
                direction *= -1;
            }

            // Movement (preserve Y)
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

            // Flip (prefab orientation)
            sr.flipX = direction < 0;

            // Shooting (grounded only)
            if (Time.time >= nextEnemyFireTime && player != null && enemyBulletPrefab != null && enemyFirePoint != null)
            {
                nextEnemyFireTime = Time.time + enemyFireRate;
                Vector2 dirToPlayer = (player.position - transform.position).normalized;
                Vector3 spawnPos = enemyFirePoint.position;
                GameObject bullet = Instantiate(enemyBulletPrefab, spawnPos, Quaternion.identity);
                bullet.GetComponent<EnemyBullet>().Initialise(dirToPlayer);
            }
        }
        else
        {
            // Falling — no horizontal movement
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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
        if (healthPickupPrefab != null && UnityEngine.Random.value < pickupDropChance)
        {
            Instantiate(healthPickupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        ParticleSystem deathEffect = GetComponentInChildren<ParticleSystem>();
        if (deathEffect != null)
        {
            deathEffect.transform.parent = null;
            deathEffect.Stop();
            deathEffect.Clear();
            deathEffect.Play();
            Destroy(deathEffect.gameObject, deathEffect.main.duration);
        }

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(100);
        }

        OnDestroyed?.Invoke();
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