using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Horizontal speed
    public float jumpForce = 10f; // Jump height
    public Transform groundCheck; // Empty child at feet
    public LayerMask groundLayer; // "Ground" layer

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isCrouching;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsCrouching", false);
    }

    void Update()
    {
        // Crouch input (hold S)
        isCrouching = Input.GetKey(KeyCode.S);
        Debug.Log("Crouching Input: " + isCrouching); // Temp: Confirm S press

        // Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        // Flip
        if (horizontal > 0) spriteRenderer.flipX = false;
        else if (horizontal < 0) spriteRenderer.flipX = true;

        // Jump
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        // Animations (Run only if not crouching)
        animator.SetBool("IsRunning", Mathf.Abs(horizontal) > 0 && !isCrouching);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
    }
}