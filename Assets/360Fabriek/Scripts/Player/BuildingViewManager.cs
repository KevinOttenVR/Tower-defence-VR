using UnityEngine;

using _360Fabriek.Player;

/// <summary>
/// Laat de speler "in" een gebouw gaan door een pinch-gebaar te maken
/// terwijl hij naar dat gebouw kijkt. Verplaatst de XR-rig naar een
/// InteriorViewPoint binnen het gebouw en schakelt optioneel locomotion uit.
/// Nog een pinch brengt de speler weer terug naar de originele positie.
/// </summary>
public class BuildingViewManager : MonoBehaviour
{
    [Header("XR Rig / Camera")]
    [Tooltip("Root van de XR rig/origin die verplaatst moet worden.")]
    public Transform xrRigRoot;

    [Tooltip("De hoofd-camera (meestal de child van de XR rig).")]
    public Transform headCamera;

    [Header("Input")]
    [Tooltip("Hand pinch detector die aangeeft wanneer er wordt gepincht.")]
    public HandPinchDetector pinchDetector;

    [Header("Raycast instellingen")]
    [Tooltip("Maximale afstand van de ray om gebouwen te selecteren.")]
    public float maxDistance = 10f;

    [Tooltip("Layers waar betreedbare gebouwen op staan.")]
    public LayerMask buildingLayer;

    [Header("Debug")]
    [Tooltip("Toon visuele debug ray in de Scene view.")]
    public bool showDebugRay = true;

    [Header("Locomotion (optioneel)")]
    [Tooltip("Locomotion-providers die uitgeschakeld worden wanneer je in een gebouw zit.")]
    public UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionProvider[] locomotionToDisable;

    private bool _inBuilding;
    private Vector3 _savedRigPos;
    private Quaternion _savedRigRot;

    private bool _wasPinchingLastFrame;

    private void Reset()
    {
        if (Camera.main != null)
        {
            headCamera = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (pinchDetector == null || headCamera == null || xrRigRoot == null)
        {
            if (pinchDetector == null) Debug.LogWarning("[BuildingViewManager] PinchDetector is niet toegewezen!");
            if (headCamera == null) Debug.LogWarning("[BuildingViewManager] HeadCamera is niet toegewezen!");
            if (xrRigRoot == null) Debug.LogWarning("[BuildingViewManager] XRRigRoot is niet toegewezen!");
            return;
        }

        bool pinchNow = pinchDetector.isPinching;

        // Debug: laat zien wat de raycast raakt
        if (showDebugRay)
        {
            Ray ray = new Ray(headCamera.position, headCamera.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                Debug.DrawRay(headCamera.position, headCamera.forward * hit.distance, Color.green);
                if (hit.collider.TryGetComponent<EnterableBuilding>(out var building))
                {
                    Debug.DrawRay(headCamera.position, headCamera.forward * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(headCamera.position, headCamera.forward * maxDistance, Color.red);
            }
        }

        // Edge detect: alleen reageren wanneer de pinch "start"
        if (pinchNow && !_wasPinchingLastFrame)
        {
            Debug.Log($"[BuildingViewManager] Pinch gedetecteerd! InBuilding: {_inBuilding}");
            
            if (!_inBuilding)
            {
                TryEnterBuilding();
            }
            else
            {
                ExitBuilding();
            }
        }

        _wasPinchingLastFrame = pinchNow;
    }

    private void TryEnterBuilding()
    {
        Ray ray = new Ray(headCamera.position, headCamera.forward);
        Debug.Log($"[BuildingViewManager] Raycast vanaf {headCamera.position} richting {headCamera.forward}, maxDistance: {maxDistance}, layerMask: {buildingLayer.value}");

        // Eerst checken zonder layer mask om te zien of we überhaupt iets raken
        RaycastHit debugHit;
        if (Physics.Raycast(ray, out debugHit, maxDistance))
        {
            Debug.Log($"[BuildingViewManager] Raycast raakt: {debugHit.collider.name} op layer {debugHit.collider.gameObject.layer} (layer mask: {buildingLayer.value})");
        }
        else
        {
            Debug.Log($"[BuildingViewManager] Raycast raakt niets binnen {maxDistance} meter.");
            return;
        }

        // Nu met layer mask checken
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, maxDistance, buildingLayer))
        {
            Debug.LogWarning($"[BuildingViewManager] Raycast raakt wel iets ({debugHit.collider.name}), maar niet op de juiste layer! Gebouw staat op layer {debugHit.collider.gameObject.layer}, maar buildingLayer mask is {buildingLayer.value}");
            return;
        }

        if (!hit.collider.TryGetComponent<EnterableBuilding>(out var building))
        {
            Debug.LogWarning($"[BuildingViewManager] {hit.collider.name} heeft geen EnterableBuilding component!");
            return;
        }

        if (building.interiorViewPoint == null)
        {
            Debug.LogWarning($"EnterableBuilding op {building.name} heeft geen InteriorViewPoint ingesteld.");
            return;
        }

        Debug.Log($"[BuildingViewManager] Betreedt gebouw: {building.name}");

        // Huidige rig-positie bewaren
        _savedRigPos = xrRigRoot.position;
        _savedRigRot = xrRigRoot.rotation;

        // Verplaats rig naar interior-viewpoint
        xrRigRoot.SetPositionAndRotation(
            building.interiorViewPoint.position,
            building.interiorViewPoint.rotation
        );

        SetLocomotionEnabled(false);
        _inBuilding = true;
        
        Debug.Log($"[BuildingViewManager] Rig verplaatst naar {building.interiorViewPoint.position}");
    }

    private void ExitBuilding()
    {
        Debug.Log($"[BuildingViewManager] Verlaat gebouw, terug naar {_savedRigPos}");
        
        // Terug naar oorspronkelijke positie
        xrRigRoot.SetPositionAndRotation(_savedRigPos, _savedRigRot);

        SetLocomotionEnabled(true);
        _inBuilding = false;
    }

    private void SetLocomotionEnabled(bool enabled)
    {
        if (locomotionToDisable == null) return;

        foreach (var loco in locomotionToDisable)
        {
            if (loco != null)
                loco.enabled = enabled;
        }
    }
}
