using UnityEngine;

public class SlingshotScript : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform leftAnchor;
    public Transform rightAnchor;
    public Transform handPoint; // The object being dragged

    void Start()
    {
        // Set the line to have 3 points
        lineRenderer.positionCount = 3;
    }

    void Update()
    {
        // Continuously update the line positions to follow the anchors and hand
        lineRenderer.SetPosition(0, leftAnchor.position);
        lineRenderer.SetPosition(1, handPoint.position);
        lineRenderer.SetPosition(2, rightAnchor.position);
    }
}