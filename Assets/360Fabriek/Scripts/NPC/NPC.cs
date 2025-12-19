using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCData Data;

    public int currentLevel = 1;
    public int currentHP;
    public int damage;
    public float attackSpeed;
    public float range;

    public bool IsDead => currentHP <= 0;
    public event System.Action<NPC> OnDeath;

    void Start()
    {
        currentHP = Data.levels[currentLevel - 1].maxHP;
        damage = Data.levels[currentLevel - 1].damage;
        attackSpeed = Data.levels[currentLevel - 1].attackSpeed;
        range = Data.levels[currentLevel - 1].range;

        Debug.Log($"Placed npc: {Data.name} with the price of {Data.price} points");
    }

    [ContextMenu("Upgrade npc")]
    public void UpdateNpcStats()
    {
        int newHPDifference = Data.levels[currentLevel].maxHP - Data.levels[currentLevel - 1].maxHP;

        currentLevel++;

        currentHP += newHPDifference;
        damage = Data.levels[currentLevel - 1].damage;
        attackSpeed = Data.levels[currentLevel - 1].attackSpeed;
        range = Data.levels[currentLevel - 1].range;
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
