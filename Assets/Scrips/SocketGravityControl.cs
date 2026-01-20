using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[DisallowMultipleComponent]
public class SocketGravityControl : MonoBehaviour
{
    [SerializeField] private bool setKinematic = true;
    [SerializeField] private bool disableGravity = true;

    private XRBaseInteractable interactable;
    private Rigidbody rb;

    private struct RigidbodyState
    {
        public bool useGravity;
        public bool isKinematic;
    }

    private RigidbodyState defaultState;
    private bool hasDefaultState;
    private Coroutine applyRoutine;
    private bool pendingRestore;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        if (interactable == null)
            Debug.LogError("[SocketGravityControl] XRBaseInteractable missing.");

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("[SocketGravityControl] Rigidbody missing.");
        else
            CacheDefaultState();
    }

    private void OnEnable()
    {
        if (interactable == null)
            return;

        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
        interactable.lastSelectExited.AddListener(OnLastSelectExited);
    }

    private void OnDisable()
    {
        if (interactable == null)
            return;

        interactable.selectEntered.RemoveListener(OnSelectEntered);
        interactable.selectExited.RemoveListener(OnSelectExited);
        interactable.lastSelectExited.RemoveListener(OnLastSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (interactable == null || rb == null)
            return;

        if (args.interactorObject is not XRSocketInteractor socketInteractor)
            return;

        StopApplyRoutine();
        pendingRestore = false;

        // Wait one frame so the socket has finished its selection update.
        applyRoutine = StartCoroutine(ApplySocketPhysicsNextFrame(socketInteractor));
    }

    private IEnumerator ApplySocketPhysicsNextFrame(XRSocketInteractor socketInteractor)
    {
        yield return null; // wait 1 frame

        if (socketInteractor == null || interactable == null || rb == null || !socketInteractor.IsSelecting(interactable))
            yield break;

        if (disableGravity)
            rb.useGravity = false;

        if (setKinematic)
            rb.isKinematic = true;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (interactable == null || rb == null)
            return;

        if (args.interactorObject is not XRSocketInteractor)
            return;

        StopApplyRoutine();
        pendingRestore = true;

        if (!interactable.isSelected)
            RestoreIfPending();
    }

    private void CacheDefaultState()
    {
        defaultState = new RigidbodyState
        {
            useGravity = rb.useGravity,
            isKinematic = rb.isKinematic
        };
        hasDefaultState = true;
    }

    private void RestoreState()
    {
        if (!hasDefaultState)
            return;

        if (!ReferenceEquals(rb, null))
        {
            if (disableGravity)
                rb.useGravity = defaultState.useGravity;

            if (setKinematic)
                rb.isKinematic = defaultState.isKinematic;
        }
    }

    private void OnLastSelectExited(SelectExitEventArgs args)
    {
        RestoreIfPending();
    }

    private void StopApplyRoutine()
    {
        if (applyRoutine != null)
        {
            StopCoroutine(applyRoutine);
            applyRoutine = null;
        }
    }

    private void RestoreIfPending()
    {
        if (!pendingRestore)
            return;

        RestoreState();
        pendingRestore = false;
    }
}
