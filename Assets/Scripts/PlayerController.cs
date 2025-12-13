using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Horizontal speed
    public float jumpForce = 10f; // Jump height
    public Transform groundCheck; // Empty child at feet
    public LayerMask groundLayer; // "Ground" layer

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint; // Empty child at gun muzzle
    public float fireRate = 0.2f; // Feels like a machine gun
    private float nextFireTime = 0f;

    private PlayerHealth health; // Self-reference
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
        health = GetComponent<PlayerHealth>(); // Auto-grab (fixed: inside Start)
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsJumping", false); // Airborne state
        animator.SetBool("IsCrouching", false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            health.TakeDamage(1);
        }
    }

    void Update()
    {
        // Input only (frame-rate safe)
        // Crouch toggle
        if (Input.GetKeyDown(KeyCode.S))
        {
            isCrouching = !isCrouching;
            animator.SetBool("IsCrouching", isCrouching);
        }

        // Ground check (light, OK in Update)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        // Shooting (instantiate OK in Update)
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Vector3 spawnPos = firePoint.localPosition;
            spawnPos.x *= spriteRenderer.flipX ? -1f : 1f;
            float muzzleY = isCrouching ? 0.002f : 0.12f;
            spawnPos.y = muzzleY;
            spawnPos = transform.TransformPoint(spawnPos);
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            int facing = spriteRenderer.flipX ? -1 : 1;
            bullet.GetComponent<Bullet>().Initialise(facing);
        }

        // Animations (Update OK)
        float horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetBool("IsRunning", Mathf.Abs(horizontal) > 0 && !isCrouching);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
    }

    void FixedUpdate()
    {
        // Physics movement (fixed timestep = smooth)
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        // Flip (FixedUpdate OK for SpriteRenderer)
        if (horizontal > 0) spriteRenderer.flipX = false;
        else if (horizontal < 0) spriteRenderer.flipX = true;
    }
}