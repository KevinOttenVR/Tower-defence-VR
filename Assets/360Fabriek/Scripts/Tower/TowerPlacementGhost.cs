using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class TowerPlacementGhost : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    public string mapTag = "Ground";
    public float rotationSmoothness = 10f;

    [Tooltip("Lift the tower up if it looks buried. Try 0.5 or 1.0 depending on scale.")]
    public float placementHeightOffset = 0f;

    [Header("Visuals")]
    public Renderer[] renderers;
    public Material validMaterial;
    public Material invalidMaterial;

    [HideInInspector] public bool isValidPosition = false;
    [HideInInspector] public Transform hitParent;

    private BoxCollider boxCollider;
    private Vector3 originalScale;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        originalScale = transform.localScale;
    }

    public void UpdateGhostState(Vector3 handPosition, Quaternion handRotation)
    {
        RaycastHit hit;

        if (Physics.Raycast(handPosition + Vector3.up * 0.5f, Vector3.down, out hit, 10f, groundLayer))
        {
            HandleMapParenting(hit.transform);

            transform.position = hit.point + (hit.normal * placementHeightOffset);

            Vector3 forward = Vector3.ProjectOnPlane(handRotation * Vector3.forward, hit.normal);
            if (forward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(forward, hit.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothness);
            }

            Physics.SyncTransforms();

            isValidPosition = !CheckObstructed();
            SetMaterial(isValidPosition ? validMaterial : invalidMaterial);
        }
        else
        {
            isValidPosition = false;
            hitParent = null;
            UnparentGhost();
            SetMaterial(invalidMaterial);
        }
    }

    private void HandleMapParenting(Transform hitObject)
    {
        hitParent = hitObject;
        Transform mapRoot = GetMapRoot(hitObject);

        if (mapRoot != null && transform.parent != mapRoot)
        {
            transform.SetParent(mapRoot, true);
            transform.localScale = originalScale;
        }
        else if (mapRoot == null && transform.parent != null)
        {
            UnparentGhost();
        }
    }

    private void UnparentGhost()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null, true);
            transform.localScale = originalScale;
        }
    }

    private bool CheckObstructed()
    {
        if (boxCollider == null) return false;

        Collider[] hits = Physics.OverlapBox(
            transform.position + transform.TransformVector(boxCollider.center),
            Vector3.Scale(boxCollider.size, transform.lossyScale) * 0.5f,
            transform.rotation,
            obstacleLayer,
            QueryTriggerInteraction.Ignore
        );

        return hits.Length > 0;
    }

    private Transform GetMapRoot(Transform startTransform)
    {
        if (startTransform == null) return null;
        Transform current = startTransform;

        for (int i = 0; i < 10; i++)
        {
            if (current.CompareTag(mapTag)) return current;
            if (current.parent != null) current = current.parent;
            else return null;
        }
        return null;
    }

    private void SetMaterial(Material mat)
    {
        foreach (var r in renderers) r.material = mat;
    }
}