using UnityEngine;
using UnityEngine.XR.Hands; // Requires 'XR Hands' package
using UnityEngine.XR.Management;

public class HandPinchDetector : MonoBehaviour
{
    [Header("Settings")]
    public float pinchThreshold = 0.02f; // 2cm distance to trigger pinch
    public Handedness targetHand = Handedness.Right;

    [Header("Debug")]
    public bool isPinching;
    public float currentDistance;

    private XRHandSubsystem handSubsystem;

    void Start()
    {
        GetHandSubsystem();
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
        {
            GetHandSubsystem();
            return;
        }

        // Get the correct hand (Left or Right)
        XRHand hand = targetHand == Handedness.Left ? handSubsystem.leftHand : handSubsystem.rightHand;

        if (!hand.isTracked)
        {
            isPinching = false;
            return;
        }

        // Get joint positions
        var thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);
        var indexTip = hand.GetJoint(XRHandJointID.IndexTip);

        if (thumbTip.TryGetPose(out Pose thumbPose) && indexTip.TryGetPose(out Pose indexPose))
        {
            // Calculate distance
            currentDistance = Vector3.Distance(thumbPose.position, indexPose.position);

            // Check threshold
            bool currentlyPinching = currentDistance < pinchThreshold;

            if (currentlyPinching != isPinching)
            {
                isPinching = currentlyPinching;
                if (isPinching)
                {
                    Debug.Log($"{targetHand} Hand PINCH START!");
                    // Trigger your game events here (e.g., spawn tower)
                }
                else
                {
                    Debug.Log($"{targetHand} Hand PINCH END!");
                }
            }
        }
    }

    private void GetHandSubsystem()
    {
        var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
        if (loader == null) return;

        handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
    }
}