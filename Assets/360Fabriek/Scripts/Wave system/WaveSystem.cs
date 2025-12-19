using UnityEngine;
using System.Collections;

public class WaveSystem : MonoBehaviour
{
    [Header("NPC Settings")]
    public NPCData npcData;

    [Header("Spawn Interval")]
    public float minInterval = 1f;
    public float maxInterval = 5f;

    [Header("Path Settings")]
    public PathManager pathManager;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            SpawnNPC();
        }
    }

    private void SpawnNPC()
    {
        if (npcData == null || npcData.NPCPrefab == null || npcData.spawnPoints.Length == 0)
        {
            Debug.LogWarning("NPCData or prefab missing!");
            return;
        }

        int index = Random.Range(0, npcData.spawnPoints.Length);
        Vector3 spawnPosition = npcData.spawnPoints[index].spawnPoint;

        float radius = 3f;
        Vector2 randomOffset = Random.insideUnitCircle * radius;
        spawnPosition += new Vector3(randomOffset.x, 0, randomOffset.y);

        GameObject npcObj = Instantiate(npcData.NPCPrefab, spawnPosition, Quaternion.identity);

        PathFollower follower = npcObj.GetComponent<PathFollower>();
        NPC npcComponent = npcObj.GetComponent<NPC>();

        if (follower != null && npcComponent != null)
        {
            follower.Initialize(pathManager.waypoints, npcComponent.movementSpeed);
        }
    }
}
