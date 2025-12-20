using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class EnemyGroup
{
    public NPCData enemyType;
    public int count;
    public int enemyLevel = 1;
    public float timeBetweenSpawns = 1f;
}

[CreateAssetMenu(fileName = "Wave_", menuName = "GameData/Wave")]
public class WaveData : ScriptableObject
{
    public List<EnemyGroup> enemyGroups;
    public float postWaveDelay = 10f;
}