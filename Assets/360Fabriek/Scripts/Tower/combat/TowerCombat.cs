using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tower))]
public class TowerCombat : MonoBehaviour
{
    [SerializeField] private SphereCollider rangeTrigger;
    [SerializeField] private LayerMask enemyLayer;

    private Tower tower;
    private readonly LinkedList<NPC> queue = new();
    private readonly Dictionary<NPC, LinkedListNode<NPC>> nodeByNpc = new();
    private Coroutine attackLoop;
    private NPC currentTarget;

    private void Awake()
    {
        tower = GetComponent<Tower>();
        if (!rangeTrigger) rangeTrigger = GetComponent<SphereCollider>();
    }

    // test range entering of enemy npcs
    private void Start()
    {
        ApplyRangeFromTower();   // force correct radius at scene start
        
        Debug.Log($"[TowerCombat] Start on {name}. Range set to {rangeTrigger.radius} (tower.range={tower.range})");
    }


    private void OnEnable()
    {
        ApplyRangeFromTower();
        tower.StatsChanged += OnTowerStatsChanged;
    }

    private void OnDisable()
    {
        tower.StatsChanged -= OnTowerStatsChanged;
        StopAttackLoop();
        ClearAllTargets();
    }

    private void OnTowerStatsChanged(Tower _)
    {
        ApplyRangeFromTower();
    }

    private void ApplyRangeFromTower()
    {
        rangeTrigger.isTrigger = true;

        float desiredWorldRange = tower.range;

        Vector3 s = rangeTrigger.transform.lossyScale;
        float maxScale = Mathf.Max(s.x, s.y, s.z);

        rangeTrigger.radius = maxScale > 0f ? (desiredWorldRange / maxScale) : desiredWorldRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer.value) == 0) return;

        if (!other.TryGetComponent(out NPC npc)) return;
        if (npc.IsDead) return;

        // Prevent friendly fire
        if (npc.Data != null && npc.Data.type != NPCType.enemy) return;

        Debug.Log($"[TowerCombat] ENTER: {npc.name} -> queue={queue.Count} (tower={name})");
        AddToQueue(npc);
        EnsureAttackLoopRunning();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out NPC npc)) return;
        RemoveFromQueue(npc);
        
        Debug.Log($"[TowerCombat] EXIT: {npc.name} -> queue={queue.Count} (tower={name})");
    }

    private void AddToQueue(NPC npc)
    {
        if (nodeByNpc.ContainsKey(npc)) return;

        var node = queue.AddLast(npc);
        nodeByNpc[npc] = node;

        npc.OnDeath += OnNpcDied;
    }

    private void RemoveFromQueue(NPC npc)
    {
        if (!nodeByNpc.TryGetValue(npc, out var node)) return;

        npc.OnDeath -= OnNpcDied;

        nodeByNpc.Remove(npc);
        queue.Remove(node);

        if (currentTarget == npc)
        {
            currentTarget = null;
        }

        if (queue.Count == 0)
        {
            StopAttackLoop();
        }
    }

    private void OnNpcDied(NPC npc)
    {      
        Debug.Log($"[TowerCombat] DIED: {npc.name} (tower={name})");
        RemoveFromQueue(npc);
    }

    private void EnsureAttackLoopRunning()
    {
        if (attackLoop != null) return;
        attackLoop = StartCoroutine(AttackLoop());
    }

    private void StopAttackLoop()
    {
        if (attackLoop == null) return;
        StopCoroutine(attackLoop);
        attackLoop = null;
        currentTarget = null;
    }

    private void ClearAllTargets()
    {
        foreach (var npc in queue)
        {
            if (npc != null) npc.OnDeath -= OnNpcDied;
        }
        queue.Clear();
        nodeByNpc.Clear();
        currentTarget = null;
    }

    private NPC GetNextValidTarget()
    {
        while (queue.First != null)
        {
            var npc = queue.First.Value;
            if (npc == null || npc.IsDead)
            {
                RemoveFromQueue(npc);
                continue;
            }
            return npc;
        }
        return null;
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            if (tower == null || tower.IsDead)
            {
                StopAttackLoop();
                yield break;
            }

            currentTarget ??= GetNextValidTarget();

            if (currentTarget == null)
            {
                StopAttackLoop();
                yield break;
            }

            Debug.Log($"[TowerCombat] HIT: {currentTarget.name} for {tower.damage} (tower={name})");

            currentTarget.TakeDamage(tower.damage);

            float interval = Mathf.Max(tower.attackSpeed, 0.01f);
            yield return new WaitForSeconds(interval);


            if (currentTarget != null && (currentTarget.IsDead || !nodeByNpc.ContainsKey(currentTarget)))
            {
                currentTarget = null;
            }
        }
    }
}
