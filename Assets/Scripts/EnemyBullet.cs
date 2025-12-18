using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 12f; // Higher for consistency — tweak prefab
    public float lifetime = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialise(Vector2 direction)
    {
        rb.WakeUp(); // Ensure physics active
        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Optionally ignore collision with enemies
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}