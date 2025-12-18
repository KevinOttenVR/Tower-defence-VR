using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NPC_Data", menuName = "GameData/NPC")]
public class NPCData : ScriptableObject
{
    [Header("Prefab data")]
    public GameObject NPCPrefab;
    public GameObject projectilePrefab;

    [Header("NPC data")]
    public new string name;
    [Min(0)] public int price;

    public NPCType type;

    public Vector3 scale;

    public WeaponType weaponType;

    [Header("Level data")]
    public TroopSpawnPointData[] spawnPoints;

    public LevelStats[] levels;
}

[Serializable]
public class TroopSpawnPointData
{
    public Transform spawnPoint;

    [Min(0f)]
    [Tooltip("The higher the weight, the higher the chance to use this spawn point")]
    public int weight = 1;

    [Min(0)]
    [Tooltip("The wave you have to reach before this spawnpoint can be used")]
    public int minWave = 0;
}

[Serializable]
public enum NPCType
{
    player,
    enemy
}