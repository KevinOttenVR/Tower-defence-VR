using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float currentSpeed = 0f;
    public float targetSpeed = 20f;
    public float acceleration = 10f;

    [Header("Life Settings")]
    public bool lookRotation = true; // Keeps the object pointed where it's going

    void Update()
    {
        // 1. Handle Acceleration
        if (currentSpeed < targetSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }

        // 2. Move the projectile forward
        // Space.Self ensures it moves based on its own orientation
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
    }
}