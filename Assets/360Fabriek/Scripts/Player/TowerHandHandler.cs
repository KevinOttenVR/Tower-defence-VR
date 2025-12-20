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
    public string mapTag = "Ground";

    private GameObject currentGhost;
    private TowerPlacementGhost ghostScript;
    private XRGrabInteractable interactable;
    private Transform mapTransform;

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
            ghostScript.UpdateGhostState(transform.position, transform.rotation);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (currentGhost != null)
        {
            if (ghostScript.isValidPosition)
            {
                Transform hitObject = ghostScript.hitParent;

                Transform mapRoot = GetMapRoot(hitObject);

                if (mapRoot != null)
                {
                    towerController.PlaceTower(towerData, currentGhost.transform.position, currentGhost.transform.rotation, mapRoot);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning($"Could not find a parent tagged '{mapTag}' above {hitObject.name}. Placing in world space.");
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

        for (int i = 0; i < 10; i++)
        {
            if (current.CompareTag(mapTag))
            {
                return current;
            }

            if (current.parent != null)
            {
                current = current.parent;
            }
            else
            {
                return null;
            }
        }
        return null;
    }
}