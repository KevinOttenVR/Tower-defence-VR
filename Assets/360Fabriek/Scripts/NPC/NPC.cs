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
        if (hpSlider == null)
        {
            hpSlider = GetComponentInChildren<Slider>();
        }
    }

    void Start()
    {
        if (currentHP == 0)
        {
            InitializeStats();
        }
    }

    public void UpdateNpcStats(int newLevel)
    {
        currentLevel = newLevel;
        InitializeStats();
    }

    private void InitializeStats()
    {
        if (Data == null || Data.levels == null || Data.levels.Length == 0) return;

        int dataIndex = Mathf.Clamp(currentLevel - 1, 0, Data.levels.Length - 1);
        var stats = Data.levels[dataIndex];

        currentHP = stats.maxHP;
        damage = stats.damage;
        attackSpeed = stats.attackSpeed;
        range = stats.range;
        movementSpeed = stats.movementSpeed;

        if (hpSlider == null)
        {
            hpSlider = GetComponentInChildren<Slider>();
        }

        if (hpSlider != null)
        {
            hpSlider.maxValue = currentHP;
            hpSlider.value = currentHP;
            Canvas.ForceUpdateCanvases();
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
