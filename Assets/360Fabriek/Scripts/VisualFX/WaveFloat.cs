using UnityEngine;

public class StylizedWobble : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float verticalAmplitude = 0.1f; // How high it floats
    public float verticalFrequency = 1.0f;  // How fast it bobs

    [Header("Rotation Settings")]
    public float rotationAmplitude = 2.0f;  // Degrees of tilt
    public float rotationFrequency = 0.5f; // Speed of tilt

    [Header("Randomization")]
    public bool randomizeStart = true;

    private Vector3 startPosition;
    private float offset;

    void Start()
    {
        startPosition = transform.localPosition;
        if (randomizeStart) offset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float time = Time.time + offset;

        // 1. Vertical Bobbing
        float newY = Mathf.Sin(time * verticalFrequency) * verticalAmplitude;

        // 2. Subtle Rotation (Tilt)
        // We use slightly different speeds for X and Z to avoid a robotic feel
        float tiltX = Mathf.Sin(time * rotationFrequency) * rotationAmplitude;
        float tiltZ = Mathf.Cos(time * rotationFrequency * 1.2f) * rotationAmplitude;

        transform.localPosition = startPosition + new Vector3(0, newY, 0);
        transform.localRotation = Quaternion.Euler(tiltX, transform.localRotation.eulerAngles.y, tiltZ);
    }
}