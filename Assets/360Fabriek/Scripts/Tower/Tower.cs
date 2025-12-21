using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    public TowerData Data;

    public int currentLevel = 1;
    public int currentHP;
    public int damage;
    public float attackSpeed;
    public float range;

    public event System.Action<Tower> StatsChanged;
    public event System.Action<Tower> OnDeath;

    public List<Sprite> levelBadges;

    public bool IsDead => currentHP <= 0;

    void Awake()
    {
        currentHP = Data.levels[currentLevel - 1].maxHP;
        damage = Data.levels[currentLevel - 1].damage;
        attackSpeed = Data.levels[currentLevel - 1].attackSpeed;
        range = Data.levels[currentLevel - 1].range;

        GetComponentInChildren<Image>().sprite = levelBadges[currentLevel - 1];

        Debug.Log($"Placed Tower: {Data.name} with the price of {Data.price} points");
    }

    [ContextMenu("Upgrade Tower")]
    public void UpdateTowerStats()
    {
        int newHPDifference = Data.levels[currentLevel].maxHP - Data.levels[currentLevel - 1].maxHP;

        currentLevel++;

        GetComponentInChildren<Image>().sprite = levelBadges[currentLevel - 1];

        currentHP += newHPDifference;
        damage = Data.levels[currentLevel - 1].damage;
        attackSpeed = Data.levels[currentLevel - 1].attackSpeed;
        range = Data.levels[currentLevel - 1].range;

        StatsChanged?.Invoke(this);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void Upgrade()
    {
        TowerController.instance.UpgradeTower(Data, gameObject, currentLevel);
    }
}
