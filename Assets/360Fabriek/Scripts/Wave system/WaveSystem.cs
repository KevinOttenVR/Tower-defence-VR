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

    private int currentWaveIndex = 0;
    private bool isWaveActive = false;

    private void Start()
    {
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

            // Post-wave delay (Customize logic here if you want to wait for all enemies to be dead instead)
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
        // Pick a random spawn point from the NPC's own data
        int index = Random.Range(0, data.spawnPoints.Length);
        Vector3 spawnPosition = data.spawnPoints[index].spawnPoint;

        // Apply a small randomization spread
        Vector2 randomOffset = Random.insideUnitCircle * 2f;
        spawnPosition += new Vector3(randomOffset.x, 0, randomOffset.y);

        GameObject npcObj = Instantiate(data.NPCPrefab, spawnPosition, Quaternion.identity);

        // Initialize Components
        NPC npcComponent = npcObj.GetComponent<NPC>();
        PathFollower follower = npcObj.GetComponent<PathFollower>();

        if (npcComponent != null)
        {
            npcComponent.currentLevel = level;
            // Force re-initialization of stats based on the new level
            npcComponent.UpdateNpcStats(level);
        }

        if (follower != null && pathManager != null)
        {
            follower.Initialize(pathManager.waypoints, npcComponent.movementSpeed);
        }
    }
}