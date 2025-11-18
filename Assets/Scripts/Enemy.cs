using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public int maxHealth = 3;
    public float edgeCheckDistance = 0.6f;   // How far ahead to check for ground
    public LayerMask groundLayer;            // Assign "Ground" in Inspector

    private int currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int direction = -1;              // -1 = left, 1 = right

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    void FixedUpdate()   // Use FixedUpdate for physics movement
    {
        // Check for edge
        Vector2 edgeCheckPos = (Vector2)transform.position + new Vector2(direction * 0.55f, -0.6f);
        bool groundAhead = Physics2D.Raycast(edgeCheckPos, Vector2.down, edgeCheckDistance, groundLayer);

        if (!groundAhead)
        {
            direction *= -1;   // Turn around!
        }

        // Move
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // Flip sprite to face movement direction
        sr.flipX = direction > 0;   // true = facing right
    }

    public void TakeDamage(int damage = 1)
    {
        currentHealth -= damage;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
            Die();
    }

    private System.Collections.IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    void Die()
    {
        // TODO: blood particles here
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    // Optional: visualise the edge check ray in Scene view
    void OnDrawGizmosSelected()
    {
        if (groundLayer == 0) return;
        Vector2 pos = transform.position + new Vector3(direction * 0.55f, -0.6f, 0);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(pos, Vector2.down * edgeCheckDistance);
    }
}