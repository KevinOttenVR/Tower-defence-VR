using UnityEngine;
using System.Collections;
using System.Linq;

public class Barracks : MonoBehaviour
{
    [Header("Spawning Configuration")]
    public NPCData unitToSpawn;
    public float spawnInterval = 10f;
    public int unitLevel = 1;

    [Header("Setup")]
    public Transform spawnPoint;
    public bool startOnAwake = true;

    private PathManager pathManager;
    private Coroutine spawnRoutine;

    private void Start()
    {
        if (pathManager == null)
        {
            pathManager = FindFirstObjectByType<PathManager>();
        }

        if (startOnAwake)
            BeginSpawning();
    }

    public void BeginSpawning()
    {
        if (spawnRoutine != null) return;
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            SpawnUnit();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnUnit()
    {
        if (unitToSpawn == null || unitToSpawn.NPCPrefab == null) return;

        Transform mapParent = transform.parent;

        GameObject newUnit = Instantiate(unitToSpawn.NPCPrefab, spawnPoint.position, spawnPoint.rotation, mapParent);

        if (newUnit.TryGetComponent<NPC>(out var npcComponent))
        {
            npcComponent.currentLevel = unitLevel;
            npcComponent.UpdateNpcStats(unitLevel);
        }

        PathFollower follower = newUnit.GetComponent<PathFollower>();
        if (follower != null && pathManager != null)
        {
            follower.Initialize(pathManager.waypoints.Reverse().ToArray(), npcComponent.movementSpeed);
        }
    }
}
