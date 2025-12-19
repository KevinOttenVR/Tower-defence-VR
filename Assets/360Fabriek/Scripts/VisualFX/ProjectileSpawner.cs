using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;
    public Transform spawnPoint; // Drag a child object here to set position/direction
    public float spawnInterval = 1.0f;
    public float lifeSpan = 5.0f; // How many seconds until it destroys itself

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnProjectile();
            timer = 0;
        }
    }

    void SpawnProjectile()
    {
        if (prefabToSpawn == null) return;

        // Create the object at the spawnPoint position and rotation
        GameObject projectile = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        // Tell the object to destroy itself after 'lifeSpan' seconds
        Destroy(projectile, lifeSpan);
    }
}