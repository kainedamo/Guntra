using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 12f;           // Tweak in prefab
    public float lifetime = 2f;          // Auto-destroy after 2 seconds (safe off-screen)

    private Rigidbody2D rb;
    private int direction = 1;           // 1 = right, -1 = left (set by player)

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called by PlayerController when spawning
    public void Initialise(int facingDirection)
    {
        direction = facingDirection;
        rb.linearVelocity = new Vector2(speed * direction, 0f);
        Destroy(gameObject, lifetime);
    }

    // Optional: destroy on leaving screen (extra safety)
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(1);
            Destroy(gameObject); // Bullet disappears on hit
        }
    }

}