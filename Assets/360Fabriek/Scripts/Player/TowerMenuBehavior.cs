using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class TowerMenuBehavior : MonoBehaviour
{
    private Vector3 originalScale;
    private float menuScale;
    private bool isGrabbed = false;
    private XRGrabInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        // Save the real scale of the prefab
        originalScale = transform.localScale;
    }

    public void Initialize(float scaleMultiplier)
    {
        menuScale = scaleMultiplier;
        // Shrink down for the menu
        transform.localScale = originalScale * menuScale;

        // Setup XRI Listeners
        interactable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isGrabbed) return;
        isGrabbed = true;

        // 1. Restore full size
        transform.localScale = originalScale;

        // 2. Unparent completely (so it doesn't move with the wrist anymore)
        transform.SetParent(null);

        // 3. Cleanup: Remove this script so it stops acting like a menu item
        //    (Optional, but good for performance)
        Destroy(this);
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnGrabbed);
        }
    }
}