using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;

    [Header("Aim Muzzle Offsets")]
    public Vector2 muzzleStand = new Vector2(0.6f, 0.12f);
    public Vector2 muzzleCrouch = new Vector2(0.6f, 0.002f);
    public Vector2 muzzleUp = new Vector2(0.45f, 0.35f);
    public Vector2 muzzleDown = new Vector2(0.55f, -0.15f);
    public Vector2 muzzleUpDiag = new Vector2(0.55f, 0.25f);
    public Vector2 muzzleDownDiag = new Vector2(0.6f, -0.05f);
    public Vector2 muzzleCrawl = new Vector2(0.08f, -0.12f);

    [Header("Spread Shot")]
    public float spreadShotFireRate = 0.15f;
    public GameObject spreadIconUI; // Drag SpreadIconUI GameObject

    private bool isSpreadShotActive = false;

    private PlayerHealth health;
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
        health = GetComponent<PlayerHealth>();

        animator.SetBool("IsRunning", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsCrouching", false);

        if (spreadIconUI != null)
        {
            spreadIconUI.SetActive(false);
        }
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

        // Mouse aim preview
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        float mouseYRelative = mousePos.y - transform.position.y;
        float mouseXRelative = mousePos.x - transform.position.x;

        if (mouseYRelative > 0.5f)
        {
            currentAimDir = Mathf.Abs(mouseXRelative) > 1f ? AimDirection.UpDiag : AimDirection.Up;
        }
        else if (mouseYRelative < -0.5f)
        {
            currentAimDir = Mathf.Abs(mouseXRelative) > 1f ? AimDirection.DownDiag : AimDirection.Down;
        }
        else
        {
            currentAimDir = AimDirection.Forward;
        }

        // Standing aim trigger
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Shoot" + currentAimDir);
        }

        // Running aim Bools
        float horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetBool("IsRunUpDiag", Mathf.Abs(horizontal) > 0 && currentAimDir == AimDirection.UpDiag);
        animator.SetBool("IsRunDownDiag", Mathf.Abs(horizontal) > 0 && currentAimDir == AimDirection.DownDiag);

        // Crawl Bool
        animator.SetBool("IsCrawl", isCrouching && Mathf.Abs(horizontal) > 0);

        // RunAim layer weight
        bool runAimActive = animator.GetBool("IsRunUpDiag") || animator.GetBool("IsRunDownDiag");
        animator.SetLayerWeight(1, runAimActive ? 1f : 0f);

        // Shooting
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (isSpreadShotActive ? spreadShotFireRate : fireRate);

            Vector2 selectedMuzzle = muzzleStand;
            if (isCrouching)
            {
                selectedMuzzle = animator.GetBool("IsCrawl") ? muzzleCrawl : muzzleCrouch;
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

            Vector2 bulletDir = Vector2.right;
            switch (currentAimDir)
            {
                case AimDirection.Up: bulletDir = Vector2.up; break;
                case AimDirection.Down: bulletDir = Vector2.down; break;
                case AimDirection.UpDiag: bulletDir = new Vector2(0.7f, 0.7f).normalized; break;
                case AimDirection.DownDiag: bulletDir = new Vector2(0.8f, -0.6f).normalized; break;
            }
            if (spriteRenderer.flipX) bulletDir.x *= -1f;

            if (isSpreadShotActive)
            {
                float[] angles = { -15f, 0f, 15f };
                foreach (float angle in angles)
                {
                    Vector2 dir = Quaternion.Euler(0, 0, angle) * bulletDir;
                    GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                    bullet.GetComponent<Bullet>().Initialise(dir);
                }
            }
            else
            {
                GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                bullet.GetComponent<Bullet>().Initialise(bulletDir);
            }
        }

        // Animations
        animator.SetBool("IsRunning", Mathf.Abs(horizontal) > 0 && !isCrouching);
        animator.SetBool("IsJumping", !isGrounded);
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        if (horizontal > 0) spriteRenderer.flipX = false;
        else if (horizontal < 0) spriteRenderer.flipX = true;
    }

    // Called by pickup
    public void ActivateSpreadShot()
    {
        isSpreadShotActive = true;
        if (spreadIconUI != null)
        {
            spreadIconUI.SetActive(true);
        }
    }

    // Called by PlayerHealth on death
    public void DeactivateSpreadShot()
    {
        isSpreadShotActive = false;
        if (spreadIconUI != null)
        {
            spreadIconUI.SetActive(false);
        }
    }
}