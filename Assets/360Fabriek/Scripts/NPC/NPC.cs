using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCData Data;

    public int currentLevel = 1;
    public int currentHP;
    public int damage;
    public float attackSpeed;
    public float range;

    void Start()
    {
        currentHP = Data.levels[currentLevel - 1].maxHP;
        damage = Data.levels[currentLevel - 1].damage;
        attackSpeed = Data.levels[currentLevel - 1].attackSpeed;
        range = Data.levels[currentLevel - 1].range;

        Debug.Log($"Placed NPC: {Data.name} with the price of {Data.price} points");
    }

    [ContextMenu("Upgrade NPC")]
    public void UpdateNPCStats()
    {
        int newHPDifference = Data.levels[currentLevel].maxHP - Data.levels[currentLevel - 1].maxHP;

        currentLevel++;

        currentHP += newHPDifference;
        damage = Data.levels[currentLevel - 1].damage;
        attackSpeed = Data.levels[currentLevel - 1].attackSpeed;
        range = Data.levels[currentLevel - 1].range;
    }
}