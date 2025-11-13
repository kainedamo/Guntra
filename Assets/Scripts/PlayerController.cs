using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Horizontal speed
    public float jumpForce = 10f; // Jump height
    public Transform groundCheck; // Empty child object at feet for ground detection
    public LayerMask groundLayer; // Assign "Ground" layer in Inspector

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Movement
        float horizontal = Input.GetAxisRaw("Horizontal"); // WASD/Arrows
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        // Flip sprite direction
        if (horizontal > 0) spriteRenderer.flipX = false; // Right
        else if (horizontal < 0) spriteRenderer.flipX = true; // Left

        // Jump
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        // Animations
        animator.SetBool("IsRunning", Mathf.Abs(horizontal) > 0);
        animator.SetBool("IsJumping", !isGrounded);
    }
}