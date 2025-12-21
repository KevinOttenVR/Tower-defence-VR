using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSystem : MonoBehaviour
{
    [Header("Data")]
    public WaveDatabase waveDatabase;
    public PathManager pathManager;

    [Header("Settings")]
    public bool autoStartNextWave = false;
    public Transform levelParent;

    private int currentWaveIndex = 0;
    private bool isWaveActive = false;

    private void Start()
    {
        if (levelParent == null)
        {
            GameObject mapObj = GameObject.FindGameObjectWithTag("Ground");
            if (mapObj != null) levelParent = mapObj.transform;
        }

        if (waveDatabase != null && waveDatabase.waves.Count > 0)
        {
            StartCoroutine(LevelRoutine());
        }
    }

    private IEnumerator LevelRoutine()
    {
        while (currentWaveIndex < waveDatabase.waves.Count)
        {
            yield return StartCoroutine(SpawnWave(waveDatabase.waves[currentWaveIndex]));

            Debug.Log($"Wave {currentWaveIndex + 1} Spawning Complete.");

            yield return new WaitForSeconds(waveDatabase.waves[currentWaveIndex].postWaveDelay);

            currentWaveIndex++;
        }
        Debug.Log("All Waves Completed!");
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        isWaveActive = true;

        foreach (var group in wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnNPC(group.enemyType, group.enemyLevel);
                yield return new WaitForSeconds(group.timeBetweenSpawns);
            }
        }

        isWaveActive = false;
    }

    private void SpawnNPC(NPCData data, int level)
    {
        int index = Random.Range(0, data.spawnPoints.Length);

        Vector3 localSpawnPos = data.spawnPoints[index].spawnPoint;

        Vector3 worldSpawnPos = levelParent.TransformPoint(localSpawnPos);

        Vector2 randomOffset = Random.insideUnitCircle * 2f * levelParent.localScale.x;

        Vector3 finalPosition = worldSpawnPos + (levelParent.rotation * new Vector3(randomOffset.x, 0, randomOffset.y));

        GameObject npcObj = Instantiate(data.NPCPrefab, finalPosition, Quaternion.identity, levelParent);

        NPC npcComponent = npcObj.GetComponent<NPC>();
        PathFollower follower = npcObj.GetComponent<PathFollower>();

        if (npcComponent != null)
        {
            npcComponent.currentLevel = level;
            npcComponent.UpdateNpcStats(level);
        }

        if (follower != null && pathManager != null)
        {
            follower.Initialize(pathManager.waypoints, npcComponent.movementSpeed);
        }
    }
}