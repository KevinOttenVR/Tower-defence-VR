using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform[] path;
    public float speed = 2f;
    public float pathRadius = 50f;

    public float rotationSpeed = 180f;

    private int currentIndex = 0;
    private Vector3 currentTarget;

    public void Initialize(Transform[] waypoints, float moveSpeed)
    {
        this.path = waypoints;
        this.speed = moveSpeed;

        if (path != null && path.Length > 0)
        {
            SetNextTarget();
        }
    }

    void Update()
    {
        if (path == null || path.Length == 0 || currentIndex >= path.Length) return;

        Vector3 direction = (currentTarget - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, currentTarget) < 0.2f)
        {
            currentIndex++;
            if (currentIndex < path.Length)
                SetNextTarget();
        }
    }

    private void SetNextTarget()
    {
        Vector3 basePosition = path[currentIndex].position;
        Vector2 offset = Random.insideUnitCircle * pathRadius;
        currentTarget = basePosition + new Vector3(offset.x, 0, offset.y);
    }
}
