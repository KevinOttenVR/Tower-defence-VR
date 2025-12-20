using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class TowerHandHandler : MonoBehaviour
{
    [Header("Data")]
    public TowerData towerData;
    public GameObject ghostPrefab;
    public TowerController towerController;

    [Header("Map Settings")]
    public string mapTag = "Ground"; // Tag your Floor/Table object with this

    private GameObject currentGhost;
    private TowerPlacementGhost ghostScript;
    private XRGrabInteractable interactable;
    private Transform mapTransform; // The parent object

    private void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        if (towerController == null) towerController = FindFirstObjectByType<TowerController>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGrab);
        interactable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (currentGhost == null)
        {
            currentGhost = Instantiate(ghostPrefab);
            ghostScript = currentGhost.GetComponent<TowerPlacementGhost>();
        }
    }

    private void Update()
    {
        if (interactable.isSelected && currentGhost != null)
        {
            // Pass the rotation so the ghost rotates with your hand
            ghostScript.UpdateGhostState(transform.position, transform.rotation);

            // OPTIONAL: If you want the ghost to scale dynamically based on the map 
            // (e.g. shrinking when you hover over the mini-map), logic goes here.
            // For now, we assume the ghostPrefab is already the correct 'Map Scale'.
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (currentGhost != null)
        {
            if (ghostScript.isValidPosition)
            {
                // 1. Get the object the ghost is standing on
                Transform hitObject = ghostScript.hitParent;

                // 2. SEARCH UPWARDS to find the real Map Parent
                // (This fixes the issue of parenting to "FloorTile" instead of "LevelMap")
                Transform mapRoot = GetMapRoot(hitObject);

                if (mapRoot != null)
                {
                    // Success! Place it under the main Map object
                    towerController.PlaceTower(towerData, currentGhost.transform.position, currentGhost.transform.rotation, mapRoot);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning($"Could not find a parent tagged '{mapTag}' above {hitObject.name}. Placing in world space.");
                    // Fallback: Place in world space if map tag is missing
                    towerController.PlaceTower(towerData, currentGhost.transform.position, currentGhost.transform.rotation, null);
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.Log("Invalid Placement Position");
            }

            Destroy(currentGhost);
            currentGhost = null;
        }
    }

    private Transform GetMapRoot(Transform startTransform)
    {
        if (startTransform == null) return null;

        Transform current = startTransform;

        // Loop 10 times max to prevent infinite loops (safety)
        for (int i = 0; i < 10; i++)
        {
            // check if THIS object is the map
            if (current.CompareTag(mapTag))
            {
                return current;
            }

            // Move up one level
            if (current.parent != null)
            {
                current = current.parent;
            }
            else
            {
                // No more parents, and we didn't find the tag
                return null;
            }
        }
        return null;
    }
}