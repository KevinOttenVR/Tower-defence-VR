using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public NPCData Data;

    public int currentLevel = 1;
    public int currentHP;
    public int damage;
    public float attackSpeed;
    public float range;
    public float movementSpeed;

    public bool IsDead => currentHP <= 0;
    public event System.Action<NPC> OnDeath;

    private Slider hpSlider;

    void Awake()
    {
        currentHP = Data.levels[currentLevel - 1].maxHP;
        damage = Data.levels[currentLevel - 1].damage;
        attackSpeed = Data.levels[currentLevel - 1].attackSpeed;
        range = Data.levels[currentLevel - 1].range;
        movementSpeed = Data.levels[currentLevel - 1].movementSpeed;

        if (hpSlider == null)
        {
            hpSlider = GetComponentInChildren<Slider>();
        }

        if (hpSlider != null)
        {
            hpSlider.maxValue = currentHP;
            hpSlider.value = currentHP;
        }

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
        movementSpeed = Data.levels[currentLevel - 1].movementSpeed;

        if (hpSlider != null)
        {
            hpSlider.maxValue = currentHP;
            hpSlider.value = currentHP;
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHP -= amount;

        if (currentHP < 0) currentHP = 0;

        if (hpSlider != null)
        {
            hpSlider.value = currentHP;
        }

        if (currentHP <= 0)
        {
            ScoreManager.score += Data.levels[currentLevel - 1].destroyOrKillPrice;

            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
