using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    [Header("Data")]
    public NPCData Data;

    [Header("Visual Effects")]
    public Material revealMaterial;
    public string shaderPropertyName = "_Effect_Progress";
    public float effectDuration = 4.0f;

    [Header("Current Stats")]
    public int currentLevel = 1;
    public int currentHP;
    public int damage;
    public float attackSpeed;
    public float range;
    public float movementSpeed;

    public bool IsDead => currentHP <= 0;
    public event System.Action<NPC> OnDeath;

    private Slider hpSlider;
    private Canvas hpCanvas;
    private Animator animator;
    private Collider npcCollider;
    private PathFollower pathFollower;
    private Renderer[] renderers;

    private List<Material[]> originalMaterials = new();
    private bool deathTriggered = false;
    private int effectPropID;

    void Awake()
    {
        hpSlider = GetComponentInChildren<Slider>();
        hpCanvas = GetComponentInChildren<Canvas>();
        animator = GetComponentInChildren<Animator>();
        npcCollider = GetComponent<Collider>();
        pathFollower = GetComponent<PathFollower>();

        renderers = GetComponentsInChildren<Renderer>();

        effectPropID = Shader.PropertyToID(shaderPropertyName);
    }

    void Start()
    {
        if (currentHP == 0)
        {
            InitializeStats();
        }

        StoreOriginalMaterials();

        if (revealMaterial != null)
        {
            if (hpCanvas != null) hpCanvas.enabled = false;
            StartCoroutine(SpawnEffectRoutine());
        }
        else
        {
            if (hpCanvas != null) hpCanvas.enabled = true;
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

        if (hpCanvas != null) hpCanvas.enabled = false;

        if (pathFollower != null) pathFollower.enabled = false;
        if (npcCollider != null) npcCollider.enabled = false;
        if (animator != null) animator.SetBool("isDead01", true);

        ScoreManager.score += Data.levels[currentLevel - 1].destroyOrKillPrice;

        OnDeath?.Invoke(this);

        if (revealMaterial != null)
        {
            StartCoroutine(DeathEffectRoutine());
        }
        else
        {
            Destroy(gameObject, 5f);
        }
    }

    private void StoreOriginalMaterials()
    {
        originalMaterials.Clear();
        foreach (var rend in renderers)
        {
            originalMaterials.Add(rend.sharedMaterials);
        }
    }

    private void ApplyRevealMaterial()
    {
        foreach (var rend in renderers)
        {
            Material[] newMats = new Material[rend.sharedMaterials.Length];
            for (int i = 0; i < newMats.Length; i++)
            {
                newMats[i] = revealMaterial;
            }
            rend.materials = newMats;
        }
    }

    private void RestoreOriginalMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && i < originalMaterials.Count)
            {
                renderers[i].materials = originalMaterials[i];
            }
        }
    }

    private IEnumerator SpawnEffectRoutine()
    {
        ApplyRevealMaterial();

        float timer = 0f;
        float startVal = -1f;
        float endVal = 0.5f;

        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            float t = timer / effectDuration;
            float currentValue = Mathf.Lerp(startVal, endVal, t);

            foreach (var rend in renderers)
            {
                foreach (var mat in rend.materials)
                {
                    mat.SetFloat(effectPropID, currentValue);
                }
            }
            yield return null;
        }

        RestoreOriginalMaterials();

        if (hpCanvas != null) hpCanvas.enabled = true;
    }

    private IEnumerator DeathEffectRoutine()
    {
        ApplyRevealMaterial();

        float timer = 0f;
        float startVal = 0.5f;
        float endVal = -1f;

        while (timer < effectDuration)
        {
            timer += Time.deltaTime;
            float t = timer / effectDuration;
            float currentValue = Mathf.Lerp(startVal, endVal, t);

            foreach (var rend in renderers)
            {
                foreach (var mat in rend.materials)
                {
                    mat.SetFloat(effectPropID, currentValue);
                }
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}