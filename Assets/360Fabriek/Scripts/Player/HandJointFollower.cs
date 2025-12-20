using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandJointFollower : MonoBehaviour
{
    public XRHandJointID jointToFollow = XRHandJointID.Palm; // Palm is best for grabbing
    public bool isRightHand = true;

    private XRHandSubsystem handSubsystem;

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
        {
            GetHandSubsystem();
            return;
        }

        // 1. Get the hand
        XRHand hand = isRightHand ? handSubsystem.rightHand : handSubsystem.leftHand;

        if (!hand.isTracked) return;

        // 2. Get the joint pose
        var joint = hand.GetJoint(jointToFollow);
        if (joint.TryGetPose(out Pose pose))
        {
            transform.position = pose.position;
            transform.rotation = pose.rotation;
        }
    }

    private void GetHandSubsystem()
    {
        var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
        if (loader == null) return;
        handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
    }
}