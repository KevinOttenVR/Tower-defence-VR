using UnityEngine;
using System.Collections.Generic;

public class VoxelPortalEffect : MonoBehaviour
{
    [Header("Core Settings")]
    public Transform portalCenter; // The central object
    public List<GameObject> voxelCubes; // Assign your cubes here

    [Header("Movement Settings")]
    public float minOrbitSpeed = 20f;
    public float maxOrbitSpeed = 60f;
    public float minRadius = 2f;
    public float maxRadius = 5f;

    [Header("Size & Variation")]
    public float minScale = 0.5f;
    public float maxScale = 1.5f;
    public float scaleSpeed = 2f;
    public float directionChangeInterval = 3f;

    // Internal data to track each voxel's unique movement
    private struct VoxelData
    {
        public Transform transform;
        public Vector3 orbitAxis;
        public float speed;
        public float radius;
        public float scaleSeed;
    }

    private List<VoxelData> voxels = new List<VoxelData>();
    private float lastDirectionChange;

    void Start()
    {
        if (portalCenter == null) portalCenter = this.transform;

        foreach (GameObject cube in voxelCubes)
        {
            if (cube == null) continue;

            VoxelData data = new VoxelData
            {
                transform = cube.transform,
                orbitAxis = Random.onUnitSphere, // Random initial orbit direction
                speed = Random.Range(minOrbitSpeed, maxOrbitSpeed),
                radius = Random.Range(minRadius, maxRadius),
                scaleSeed = Random.value * 10f
            };
            voxels.Add(data);
        }
    }

    void Update()
    {
        // Periodic direction and speed shifts for that "unstable" nether feel
        bool shouldChange = Time.time - lastDirectionChange > directionChangeInterval;

        for (int i = 0; i < voxels.Count; i++)
        {
            VoxelData v = voxels[i];

            // 1. Orbit the center
            v.transform.RotateAround(portalCenter.position, v.orbitAxis, v.speed * Time.deltaTime);

            // 2. Maintain Distance (keep them in a shell around the portal)
            Vector3 direction = (v.transform.position - portalCenter.position).normalized;
            v.transform.position = portalCenter.position + direction * v.radius;

            // 3. Pulse Size (Sin wave for smooth random-ish breathing)
            float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * scaleSpeed + v.scaleSeed) + 1f) / 2f);
            v.transform.localScale = new Vector3(scale, scale, scale);

            // 4. Update direction occasionally
            if (shouldChange)
            {
                v.orbitAxis = Vector3.Lerp(v.orbitAxis, Random.onUnitSphere, 0.5f);
                v.speed = Random.Range(minOrbitSpeed, maxOrbitSpeed);
                voxels[i] = v; // Update the struct in the list
            }
        }

        if (shouldChange) lastDirectionChange = Time.time;
    }
}