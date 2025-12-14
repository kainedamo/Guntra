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

    [Header("Aim Muzzle Offsets")]
    public Vector2 muzzleStand = new Vector2(0.6f, 0.12f);     // Forward/Idle
    public Vector2 muzzleCrouch = new Vector2(0.6f, 0.002f);   // Crouch forward
    public Vector2 muzzleUp = new Vector2(0.45f, 0.35f);      // Gun up
    public Vector2 muzzleDown = new Vector2(0.55f, -0.15f);   // Gun down
    public Vector2 muzzleUpDiag = new Vector2(0.55f, 0.25f);   // Up-right diag
    public Vector2 muzzleDownDiag = new Vector2(0.6f, -0.05f); // Down-right diag
    public Vector2 muzzleCrawl = new Vector2(0.08f, -0.12f); //Crawl forward

    private PlayerHealth health; // Self-reference
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool isCrouching;
    private enum AimDirection { Forward, Up, Down, UpDiag, DownDiag }
    private AimDirection currentAimDir = AimDirection.Forward;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<PlayerHealth>(); // Auto-grab
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsJumping", false);
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
        // Crouch toggle
        if (Input.GetKeyDown(KeyCode.S))
        {
            isCrouching = !isCrouching;
            animator.SetBool("IsCrouching", isCrouching);
        }

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        // Mouse aim preview (standing poses — Trigger on MouseDown only)
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            float mouseYRelative = mousePos.y - transform.position.y;
            float mouseXRelative = mousePos.x - transform.position.x;

            if (mouseYRelative > 0.5f)
            {
                if (Mathf.Abs(mouseXRelative) > 1f) currentAimDir = AimDirection.UpDiag;
                else currentAimDir = AimDirection.Up;
            }
            else if (mouseYRelative < -0.5f)
            {
                if (Mathf.Abs(mouseXRelative) > 1f) currentAimDir = AimDirection.DownDiag;
                else currentAimDir = AimDirection.Down;
            }
            else
            {
                currentAimDir = AimDirection.Forward;
            }

            animator.SetTrigger("Shoot" + currentAimDir);
        }

        // Running aim Bools (loop while running + aim)
        float horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetBool("IsRunUpDiag", Mathf.Abs(horizontal) > 0 && currentAimDir == AimDirection.UpDiag);
        animator.SetBool("IsRunDownDiag", Mathf.Abs(horizontal) > 0 && currentAimDir == AimDirection.DownDiag);

        // Crawl Bool
        animator.SetBool("IsCrawl", isCrouching && Mathf.Abs(horizontal) > 0);

        // Layer weights (RunAim always active)
        animator.SetLayerWeight(1, 1f); // RunAim layer index 1

        // Shooting
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            // Muzzle offset per aim state
            Vector2 selectedMuzzle = muzzleStand;
            if (isCrouching)
            {
                if (animator.GetBool("IsCrawl"))
                {
                    selectedMuzzle = muzzleCrawl;
                }
                else
                {
                    selectedMuzzle = muzzleCrouch;
                }
            }
            else
            {
                switch (currentAimDir)
                {
                    case AimDirection.Up: selectedMuzzle = muzzleUp; break;
                    case AimDirection.Down: selectedMuzzle = muzzleDown; break;
                    case AimDirection.UpDiag: selectedMuzzle = muzzleUpDiag; break;
                    case AimDirection.DownDiag: selectedMuzzle = muzzleDownDiag; break;
                }
            }

            Vector3 spawnPos = selectedMuzzle;
            spawnPos.x *= spriteRenderer.flipX ? -1f : 1f;
            spawnPos = transform.TransformPoint(spawnPos);

            // Bullet direction matching aim
            Vector2 bulletDir = Vector2.right; // Default forward
            switch (currentAimDir)
            {
                case AimDirection.Up: bulletDir = Vector2.up; break;
                case AimDirection.Down: bulletDir = Vector2.down; break;
                case AimDirection.UpDiag: bulletDir = new Vector2(0.7f, 0.7f).normalized; break;
                case AimDirection.DownDiag: bulletDir = new Vector2(0.8f, -0.6f).normalized; break;
            }
            if (spriteRenderer.flipX) bulletDir.x *= -1f;

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            bullet.GetComponent<Bullet>().Initialise(bulletDir);
        }

        // Animations
        float animHorizontal = Input.GetAxisRaw("Horizontal"); // Renamed to avoid duplicate
        animator.SetBool("IsRunning", Mathf.Abs(animHorizontal) > 0 && !isCrouching);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetLayerWeight(1, !isGrounded ? 1f : 0f); // JumpPriority layer
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        if (horizontal > 0) spriteRenderer.flipX = false;
        else if (horizontal < 0) spriteRenderer.flipX = true;
    }
}