using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.Events;

public class HandMenuController : MonoBehaviour
{
    [Header("Detection Settings")]
    public float palmFacingThreshold = 0.6f;
    public float activationDelay = 0.2f;

    [Header("Positioning Settings")]
    public Vector3 positionalOffset = new Vector3(0, 0.15f, 0); // Where the menu sits relative to wrist
    public float followSmoothing = 10f; // Higher = tighter snap, Lower = smoother/lazier
    public bool faceHeadset = true;     // If true, menu rotates to face your eyes

    [Header("References")]
    public Transform headCamera;
    public GameObject menuObject;

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
            // --- DEBUG SECTION ---
            if (showDebugGizmos)
            {
                // Green = Up, Blue = Forward, Red = Right
                // Look for the line that is pointing directly at your eyes!
                Debug.DrawRay(palmPose.position, palmPose.up * 0.2f, Color.green);
                Debug.DrawRay(palmPose.position, -palmPose.up * 0.2f, Color.yellow); // Negative Up
                Debug.DrawRay(palmPose.position, palmPose.forward * 0.2f, Color.blue);
                Debug.DrawRay(palmPose.position, -palmPose.forward * 0.2f, Color.cyan); // Negative Forward
                Debug.DrawRay(palmPose.position, palmPose.right * 0.2f, Color.red);
            }

            // --- LOGIC CHANGE HERE ---
            // Try changing this vector based on what you see in the debug rays:
            // options: -palmPose.up, palmPose.forward, -palmPose.right, etc.
            Vector3 palmNormal = -palmPose.up; 
            
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
        }
        
        // ... (Keep the positioning logic the same)
        if (isMenuOpen && menuObject != null) UpdateMenuPosition(leftHand);
    }

    private void UpdateMenuPosition(XRHand hand)
    {
        // We track the WRIST for stability, rather than the palm
        var wristJoint = hand.GetJoint(XRHandJointID.Wrist);

        if (wristJoint.TryGetPose(out Pose wristPose))
        {
            // Calculate target position: Wrist Position + (Wrist Rotation * Offset)
            // This ensures the offset is relative to how your hand is turned
            Vector3 targetPosition = wristPose.position + (wristPose.rotation * positionalOffset);

            // Smoothly move the menu there (Lerp) to remove hand jitter
            menuObject.transform.position = Vector3.Lerp(
                menuObject.transform.position,
                targetPosition,
                Time.deltaTime * followSmoothing
            );

            // Handle Rotation
            if (faceHeadset)
            {
                // Smoothly rotate to face the camera
                Quaternion targetRotation = Quaternion.LookRotation(menuObject.transform.position - headCamera.position);
                menuObject.transform.rotation = Quaternion.Slerp(
                    menuObject.transform.rotation,
                    targetRotation,
                    Time.deltaTime * followSmoothing
                );
            }
            else
            {
                // Lock rotation relative to wrist
                menuObject.transform.rotation = Quaternion.Slerp(
                    menuObject.transform.rotation,
                    wristPose.rotation,
                    Time.deltaTime * followSmoothing
                );
            }
        }
    }

    private void UpdateMenuState(bool shouldBeOpen)
    {
        if (isMenuOpen == shouldBeOpen) return;

        isMenuOpen = shouldBeOpen;

        if (menuObject != null)
            menuObject.SetActive(isMenuOpen);

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