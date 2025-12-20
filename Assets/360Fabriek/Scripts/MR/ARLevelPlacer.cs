using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; // Required for AR
using UnityEngine.XR.ARSubsystems; // Required for Raycast hits
using UnityEngine.InputSystem;     // Required for Input Actions

public class ARLevelPlacer : MonoBehaviour
{
    [Header("References")]
    public GameObject levelParent;       // Drag your entire "LevelMap" object here
    public ARRaycastManager raycastManager;
    public Transform placementSource;    // Drag your Right Hand / Controller here

    [Header("Input")]
    public InputActionProperty placeAction; // The button to press (e.g., Trigger or Pinch)

    private bool isLevelPlaced = false;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        // Hide the level initially so it doesn't float in the void
        if (levelParent != null) levelParent.SetActive(false);
    }

    void Update()
    {
        // If we already placed the level, stop updating logic (optional: allow moving it later)
        if (isLevelPlaced) return;

        // 1. Raycast into the real world
        Ray ray = new Ray(placementSource.position, placementSource.forward);

        // We check for "PlaneWithinPolygon" to ensure we hit a valid detected surface
        if (raycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
        {
            // 2. Get the hit position (The first hit is the closest)
            Pose hitPose = hits[0].pose;

            // 3. Show the ghost level at that spot
            if (!levelParent.activeSelf) levelParent.SetActive(true);

            levelParent.transform.position = hitPose.position;

            // OPTIONAL: Make the level face the player
            Vector3 cameraPos = Camera.main.transform.position;
            Vector3 direction = cameraPos - levelParent.transform.position;
            direction.y = 0; // Keep it flat
            if (direction != Vector3.zero)
            {
                levelParent.transform.rotation = Quaternion.LookRotation(-direction);
            }

            // 4. Check for Input to "Lock" it
            if (placeAction.action.WasPressedThisFrame())
            {
                PlaceLevel();
            }
        }
        else
        {
            // Hide ghost if pointing at nothing valid
            if (levelParent.activeSelf) levelParent.SetActive(false);
        }
    }

    private void PlaceLevel()
    {
        isLevelPlaced = true;

        // Optional: Disable plane visuals once placed so the room looks clean
        ARPlaneManager planeManager = raycastManager.GetComponent<ARPlaneManager>();
        if (planeManager != null)
        {
            planeManager.enabled = false; // Stop searching for new planes
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false); // Hide existing planes
            }
        }

        Debug.Log("Level Placed Successfully!");
    }
}