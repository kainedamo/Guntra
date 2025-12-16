using UnityEngine;
using System.Collections;

public class FlyingBot : MonoBehaviour
{
    public float swoopSpeed = 3f;
    public int maxHealth = 3;
    public float arcHeight = 2f;
    public float pauseTime = 1f;
    [Header("Pickups")]
    public GameObject healthPickupPrefab;
    public float pickupDropChance = 0.25f;

    private Transform player;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float swoopProgress = 0f;
    private bool swooping = false;
    private float pauseTimer = 0f;
    private int currentHealth;
    private SpriteRenderer sr;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position;
        targetPos = new Vector3(player ? player.position.x - 3f : 0, -2f, 0);
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>(); // For FlashRed
    }

    void Update()
    {
        if (player == null) return;

        if (!swooping)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseTime)
            {
                swooping = true;
                swoopProgress = 0f;
                targetPos.x = player.position.x + Random.Range(-2f, 2f);
            }
        }
        else
        {
            swoopProgress += swoopSpeed * Time.deltaTime;
            float sinCurve = Mathf.Sin(swoopProgress * Mathf.PI);
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, swoopProgress);
            currentPos.y += sinCurve * arcHeight;
            transform.position = currentPos;

            if (swoopProgress >= 1f)
            {
                swooping = false;
                pauseTimer = 0f;
                startPos = transform.position;
            }
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

    void OnTriggerEnter2D(Collider2D other)
    {
        // Bullet damage
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            Die(); // Plays DeathEffect + score/pickup before destroy
        }
    }

    void Die()
    {
        // Health pickup drop
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

        // Score
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(100);
        }

        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}