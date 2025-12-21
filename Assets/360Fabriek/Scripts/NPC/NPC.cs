using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    private Animator animator;
    private Collider npcCollider;
    private PathFollower pathFollower;

    private bool deathTriggered = false;

    void Awake()
    {
        hpSlider = GetComponentInChildren<Slider>();
        animator = GetComponentInChildren<Animator>();
        npcCollider = GetComponent<Collider>();
        pathFollower = GetComponent<PathFollower>();
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

        if (hpSlider != null)
        {
            hpSlider.maxValue = currentHP;
            hpSlider.value = currentHP;
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || deathTriggered) return;

        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        if (hpSlider != null)
        {
            hpSlider.value = currentHP;
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathTriggered) return;
        deathTriggered = true;

        if (pathFollower != null)
            pathFollower.enabled = false;

        if (npcCollider != null)
            npcCollider.enabled = false;

        if (animator != null)
            animator.SetBool("isDead01", true);

        ScoreManager.score += Data.levels[currentLevel - 1].destroyOrKillPrice;

        OnDeath?.Invoke(this);

        StartCoroutine(DestroyAfterDelay(5f));
    }


    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
