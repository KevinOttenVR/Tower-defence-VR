using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.Events;

public class HandMenuToggleOnly : MonoBehaviour
{
    [Header("Settings")]
    public float palmFacingThreshold = 0.6f;
    public float activationDelay = 0.2f;

    [Header("References")]
    public Transform headCamera;
    public GameObject menuObject; // Drag your Menu Canvas/Object here

    [Header("Events")]
    public UnityEvent OnMenuOpen;
    public UnityEvent OnMenuClose;

    [Header("Debug")]
    public bool showDebugGizmos = true;

    private XRHandSubsystem handSubsystem;
    private bool isMenuOpen = false;
    private float holdTimer = 0f;

    void Start()
    {
        GetHandSubsystem();
        if (headCamera == null) headCamera = Camera.main.transform;

        // Ensure it starts closed
        if (menuObject != null) menuObject.SetActive(false);
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
        {
            GetHandSubsystem();
            return;
        }

        XRHand leftHand = handSubsystem.leftHand;
        if (!leftHand.isTracked)
        {
            UpdateMenuState(false);
            return;
        }

        var palmJoint = leftHand.GetJoint(XRHandJointID.Palm);

        if (palmJoint.TryGetPose(out Pose palmPose))
        {
            // Vector pointing out of palm (Use palmPose.forward if Blue line pointed at face)
            Vector3 palmNormal = -palmPose.up;
            // Vector3 palmNormal = palmPose.forward; // SWAP TO THIS IF NEEDED

            Vector3 handToHead = (headCamera.position - palmPose.position).normalized;
            float facingScore = Vector3.Dot(palmNormal, handToHead);

            if (facingScore > palmFacingThreshold)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= activationDelay) UpdateMenuState(true);
            }
            else
            {
                holdTimer = 0f;
                UpdateMenuState(false);
            }

            if (showDebugGizmos)
            {
                Debug.DrawRay(palmPose.position, palmNormal * 0.1f, Color.green);
            }
        }
    }

    private void UpdateMenuState(bool shouldBeOpen)
    {
        if (isMenuOpen == shouldBeOpen) return;

        isMenuOpen = shouldBeOpen;

        if (menuObject != null) menuObject.SetActive(isMenuOpen);

        if (isMenuOpen) OnMenuOpen?.Invoke();
        else OnMenuClose?.Invoke();
    }

    private void GetHandSubsystem()
    {
        var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
        if (loader == null) return;
        handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
    }
}