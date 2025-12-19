using UnityEngine;

[System.Serializable]
public struct ForbiddenZone
{
    public float minX;
    public float maxX;
}

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

    [Header("Boss")]
    public GameObject enemyBossPrefab;
    public float bossSpawnX = 122f;
    private bool bossSpawned = false;

    [Header("Forbidden Spawn Zones")]
    [Tooltip("Define X ranges where enemies should NOT spawn (e.g., moving platforms)")]
    public ForbiddenZone[] forbiddenZones = new ForbiddenZone[]
    {
        new ForbiddenZone { minX = 90f, maxX = 100f }
    };

    private float timer;
    private Camera mainCam;
    private int activeMechCount = 0;
    private Transform player;

    void Start()
    {
        mainCam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Enemy spawning (stops after boss)
        if (!bossSpawned && timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemyOffScreenRight();
        }

        // Boss spawn trigger (one-time)
        if (!bossSpawned && player != null && player.position.x > bossSpawnX)
        {
            bossSpawned = true;
            float bossSpawnXPos = 130f;
            float bossSpawnYPos = 6.5f;

            // Raycast to find exact ground level
            Vector2 rayOrigin = new Vector2(bossSpawnXPos, bossSpawnYPos + 10f);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 50f, groundLayer);

            Vector3 bossPos;
            if (hit.collider != null)
            {
                // Spawn slightly above ground
                bossPos = hit.point + Vector2.up * 0.6f; // Reduced from 1.5f to 0.6f
            }
            else
            {
                // Fallback to fixed Y position
                bossPos = new Vector3(bossSpawnXPos, bossSpawnYPos, 0);
            }

            GameObject boss = Instantiate(enemyBossPrefab, bossPos, Quaternion.identity);
            Debug.Log($"Boss spawned at position: {bossPos}");

            // Lock player in arena
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.EnterBossFight();
            }
        }
    }

    private void SpawnEnemyOffScreenRight()
    {
        float spawnX = mainCam.transform.position.x +
                       (mainCam.orthographicSize * mainCam.aspect) + spawnDistanceFromCamera;  //MATHS CONTENT PRESENT HERE

        // Check if spawnX is in any forbidden zone
        if (IsInForbiddenZone(spawnX))
        {
            return;
        }

        Vector2 rayOrigin = new Vector2(spawnX, mainCam.transform.position.y + 10f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 50f, groundLayer);

        if (Random.value < flyingBotChance && flyingBotPrefab != null) // MATHS CONTENT PRESENT HERE
        {
            Vector3 highSpawn = new Vector3(spawnX, mainCam.transform.position.y + 8f, 0);
            Instantiate(flyingBotPrefab, highSpawn, Quaternion.identity);
        }
        else if (hit.collider != null && enemyMechPrefab != null && activeMechCount < maxMechs)
        {
            Vector3 spawnPos = hit.point;
            spawnPos.y += 0.55f;
            GameObject mech = Instantiate(enemyMechPrefab, spawnPos, Quaternion.identity);
            activeMechCount++;
            Enemy.OnDestroyed += () => activeMechCount--;
        }
    }

    private bool IsInForbiddenZone(float xPosition)
    {
        foreach (ForbiddenZone zone in forbiddenZones)
        {
            if (xPosition >= zone.minX && xPosition <= zone.maxX)
            {
                return true;
            }
        }
        return false;
    }
}