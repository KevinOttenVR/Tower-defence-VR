using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform[] path;
    public float speed = 2f;
    public float rotationSpeed = 10f;

    public float reachThreshold = 0.01f;
    public bool stopped;

    private int currentIndex = 0;
    private Animator animator;
    public NPC npc;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Initialize(Transform[] waypoints, float moveSpeed)
    {
        path = waypoints;
        speed = moveSpeed;

        if (path != null && path.Length > 0)
        {
            currentIndex = 0;

            for(var i = 0; i < waypoints.Length; i++)
            {
                var distance = Vector3.Distance(waypoints[i].position, transform.position);
                if (distance >= Vector3.Distance(waypoints[currentIndex].position, transform.position)) continue;
                currentIndex = i;
            }

            if (animator != null) animator.SetBool("isWalking", true);
        }
    }

    void Update()
    {
        if (path == null || path.Length == 0 || currentIndex >= path.Length || stopped)
        {
            if (animator != null) animator.SetBool("isWalking", false);
            return;
        }

        Vector3 targetPosition = path[currentIndex].position;
        Vector3 currentPosition = transform.position;
        float distX = targetPosition.x - currentPosition.x;
        float distZ = targetPosition.z - currentPosition.z;

        float distanceSquared = (distX * distX) + (distZ * distZ);
        float thresholdSquared = reachThreshold * reachThreshold;

        if (distanceSquared > thresholdSquared)
        {
            if (animator != null) animator.SetBool("isWalking", true);

            Vector3 direction = (targetPosition - currentPosition).normalized;

            transform.position += speed * Time.deltaTime * direction;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            currentIndex++;

            if (currentIndex >= path.Length)
            {
                if (animator != null) animator.SetBool("isWalking", false);
                npc.Die();
                if (npc.Data.type == NPCType.enemy) HeadQuartersManager.Instance.TakeDamage(npc.damage);
            }
        }
    }
}