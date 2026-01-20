using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandPinchDetector : MonoBehaviour
{
    [Header("Settings")]
    public float pinchThreshold = 0.02f;
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

        XRHand hand = targetHand == Handedness.Left ? handSubsystem.leftHand : handSubsystem.rightHand;

        if (!hand.isTracked)
        {
            isPinching = false;
            return;
        }

        var thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);
        var indexTip = hand.GetJoint(XRHandJointID.IndexTip);

        if (thumbTip.TryGetPose(out Pose thumbPose) && indexTip.TryGetPose(out Pose indexPose))
        {
            currentDistance = Vector3.Distance(thumbPose.position, indexPose.position);

            bool currentlyPinching = currentDistance < pinchThreshold;

            if (currentlyPinching != isPinching)
            {
                isPinching = currentlyPinching;
                if (isPinching)
                {
                    Debug.Log($"{targetHand} Hand PINCH START!");
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
