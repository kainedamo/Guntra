using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 12f; // Tweak in prefab
    public float lifetime = 2f; // Auto-destroy after 2 seconds (safe off-screen)
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called by PlayerController when spawning
    public void Initialise(Vector2 direction)
    {
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    // Optional: destroy on leaving screen (extra safety)
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit regular enemy (mechs, bots)
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(1);
            Destroy(gameObject); // Bullet disappears on hit
        }
        // Hit boss (new script name)
        else if (other.TryGetComponent<EnemyBossNew>(out EnemyBossNew boss))
        {
            boss.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}