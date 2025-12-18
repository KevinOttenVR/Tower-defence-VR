using System;
using UnityEngine;

[Serializable]
public class LevelStats
{
    [Min(0)] public int level;
    [Min(0)] public int upgradePrice;
    [Min(0)] public int destroyOrKillPrice;
    [Min(0)] public int maxHP;
    [Min(0)] public int damage;
    [Min(0f)] public float attackSpeed;
    [Min(0f)] public float range;
}