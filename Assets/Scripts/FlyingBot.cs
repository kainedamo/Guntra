using UnityEngine;

public class FlyingBot : MonoBehaviour
{
    public float swoopSpeed = 3f;
    public float arcHeight = 2f; // Swoop arc peak
    public float pauseTime = 1f; // Hover at top before swoop

    private Transform player;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float swoopProgress = 0f;
    private bool swooping = false;
    private float pauseTimer = 0f;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPos = transform.position;
        targetPos = new Vector3(player.position.x - 3f, -2f, 0); // Swoop below player
    }

    void Update()
    {
        if (player == null) return;

        if (!swooping)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseTime)
            {
                swooping = true;
                swoopProgress = 0f;
                targetPos.x = player.position.x + Random.Range(-2f, 2f); // Random X for next swoop
            }
        }
        else
        {
            // Arcing swoop: sin wave Y for curve
            swoopProgress += swoopSpeed * Time.deltaTime;
            float sinCurve = Mathf.Sin(swoopProgress * Mathf.PI); // 0 to 1 to 0
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, swoopProgress);
            currentPos.y += sinCurve * arcHeight;
            transform.position = currentPos;

            if (swoopProgress >= 1f)
            {
                swooping = false;
                pauseTimer = 0f;
                startPos = transform.position; // Ready for next swoop
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}