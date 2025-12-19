using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SineWaveFlag : MonoBehaviour
{
    [Header("Wave Settings")]
    public float amplitude = 0.5f;
    public float frequency = 2.0f;
    public float speed = 3.0f;

    [Header("Pinning Settings")]
    [Tooltip("Which side should stay still?")]
    public bool pinLeft = true;
    public float waveEdgeStart = 0f;

    private Mesh originalMesh;
    private Mesh workingMesh;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;

    void Start()
    {
        // Clone the mesh to avoid modifying the original asset
        originalMesh = GetComponent<MeshFilter>().sharedMesh;
        workingMesh = Instantiate(originalMesh);
        GetComponent<MeshFilter>().mesh = workingMesh;

        originalVertices = originalMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
    }

    void Update()
    {
        float time = Time.time * speed;
        float minX = 0f;
        float maxX = 0f;

        // Find the bounds if we need dynamic pinning
        foreach (var v in originalVertices)
        {
            if (v.x < minX) minX = v.x;
            if (v.x > maxX) maxX = v.x;
        }

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];

            // Calculate distance from the 'pinned' side
            // Normalized weight (0 at the pin, 1 at the far edge)
            float weight = pinLeft ?
                Mathf.InverseLerp(minX, maxX, vertex.x) :
                Mathf.InverseLerp(maxX, minX, vertex.x);

            // Sine wave math
            float wave = Mathf.Sin(vertex.x * frequency + time) * amplitude;

            // Apply wave only to the Y axis, multiplied by weight
            vertex.y += wave * weight;

            displacedVertices[i] = vertex;
        }

        workingMesh.vertices = displacedVertices;
        workingMesh.RecalculateNormals(); // Ensures lighting looks correct
    }
}