using UnityEngine;

[CreateAssetMenu(fileName = "Tower_Data", menuName = "GameData/Tower")]
public class TowerData : ScriptableObject
{
    [Header("Prefab data")]
    public GameObject towerPrefab;
    public GameObject projectilePrefab;

    [Header("Tower data")]
    public new string name;
    [Min(0)] public int price;

    public Vector3 scale;

    public WeaponType weaponType;

    [Header("Level data")]

    public LevelStats[] levels;
}