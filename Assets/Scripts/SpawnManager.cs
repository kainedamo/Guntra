using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public float spawnInterval = 6f;
    public float spawnDistanceFromCamera = 12f;
    public LayerMask groundLayer;

    [Header("Enemy Types")]
    public GameObject enemyMechPrefab;
    public GameObject flyingBotPrefab;
    public float flyingBotChance = 0.2f;

    [Header("Limits")]
    public int maxMechs = 5;

    private float timer;
    private Camera mainCam;
    private int activeMechCount = 0;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemyOffScreenRight();
        }
    }

    void SpawnEnemyOffScreenRight()
    {
        float spawnX = mainCam.transform.position.x +
                       (mainCam.orthographicSize * mainCam.aspect) + spawnDistanceFromCamera;

        Vector2 rayOrigin = new Vector2(spawnX, mainCam.transform.position.y + 10f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 50f, groundLayer);

        if (Random.value < flyingBotChance && flyingBotPrefab != null)
        {
            Vector3 highSpawn = new Vector3(spawnX, mainCam.transform.position.y + 8f, 0);
            Instantiate(flyingBotPrefab, highSpawn, Quaternion.identity);
        }
        else if (hit.collider != null && enemyMechPrefab != null && activeMechCount < maxMechs)
        {
            Vector3 spawnPos = hit.point;
            spawnPos.y += 0.6f;

            GameObject mech = Instantiate(enemyMechPrefab, spawnPos, Quaternion.identity);
            activeMechCount++;

            // Subscribe to static event (correct syntax)
            Enemy.OnDestroyed += () => activeMechCount--;
        }
    }
}