using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Vector3 nextPosition;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Setup Rigidbody2D if not already configured
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        nextPosition = pointB.position;
    }

    void FixedUpdate()
    {
        // Move platform
        Vector3 newPosition = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.fixedDeltaTime);

        // Use Rigidbody2D.MovePosition for smooth physics movement
        rb.MovePosition(newPosition);

        // Switch target when reached
        if (Vector3.Distance(newPosition, nextPosition) < 0.01f)
        {
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Parent player to platform
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Unparent player from platform
            collision.transform.SetParent(null);
        }
    }
}