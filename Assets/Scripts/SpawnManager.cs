using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 6f;          
    public float spawnDistanceFromCamera = 12f;  // How far right of camera to spawn
    public LayerMask groundLayer;             // Ground layer

    private float timer;
    private Camera mainCam;

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
        // X position = right edge of screen + a little buffer
        float spawnX = mainCam.transform.position.x +
                       (mainCam.orthographicSize * mainCam.aspect) + spawnDistanceFromCamera;

        // Raycast down from high above to find the ground Y
        Vector2 rayOrigin = new Vector2(spawnX, mainCam.transform.position.y + 10f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 50f, groundLayer);

        if (hit.collider != null)
        {
            Vector3 spawnPos = hit.point;
            spawnPos.y += 0.6f; // Half enemy height so feet touch ground
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}