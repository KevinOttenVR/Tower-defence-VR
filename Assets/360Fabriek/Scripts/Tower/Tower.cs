using UnityEngine;

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

    public bool IsDead => currentHP <= 0;

    void Awake()
    {
        currentHP = Data.levels[currentLevel - 1].maxHP;
        damage = Data.levels[currentLevel - 1].damage;
        attackSpeed = Data.levels[currentLevel - 1].attackSpeed;
        range = Data.levels[currentLevel - 1].range;

        Debug.Log($"Placed Tower: {Data.name} with the price of {Data.price} points");
    }

    [ContextMenu("Upgrade Tower")]
    public void UpdateTowerStats()
    {
        int newHPDifference = Data.levels[currentLevel].maxHP - Data.levels[currentLevel - 1].maxHP;

        currentLevel++;

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
}
