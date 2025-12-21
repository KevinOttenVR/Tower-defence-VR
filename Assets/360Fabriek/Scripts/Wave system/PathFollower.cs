using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform[] path;
    public float speed = 2f;
    public float rotationSpeed = 10f;

    public float reachThreshold = 0.01f;

    private int currentIndex = 0;
    private Animator animator;

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
            if (animator != null) animator.SetBool("isWalking", true);
        }
    }

    void Update()
    {
        if (path == null || path.Length == 0 || currentIndex >= path.Length)
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
                // Reached the end
            }
        }
    }
}