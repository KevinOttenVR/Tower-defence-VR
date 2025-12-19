using UnityEngine;

public class DynamicRotator : MonoBehaviour
{
    [Header("Axis Settings")]
    public Vector3 rotationAxis = Vector3.up; // Set your axis here (e.g., 0,1,0)

    [Header("Speed Settings")]
    public float targetSpeed = 100f;   // The speed you want to reach
    public float acceleration = 50f;  // How fast it reaches that speed

    private float currentSpeed = 0f;   // Starts at 0

    void Update()
    {
        // Smoothly move currentSpeed toward targetSpeed
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // Apply the rotation
        transform.Rotate(rotationAxis * currentSpeed * Time.deltaTime);
    }

    // Call this from other scripts to change speed later
    public void ChangeSpeed(float newSpeed)
    {
        targetSpeed = newSpeed;
    }
}