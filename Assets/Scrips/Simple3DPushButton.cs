using UnityEngine;
using UnityEngine.Events;


public class Simple3DPushButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform buttonCap;
    [SerializeField] private Collider pressVolume;

    [Header("Motion")]
    [SerializeField] private Vector3 localPressAxis = Vector3.down;
    [SerializeField] private float pressDistance = 0.01f;
    [SerializeField] private float pressSpeed = 20f;
    [SerializeField] private float returnSpeed = 25f;

    [Header("Activation")]
    [SerializeField] private float pressThreshold = 0.85f; // 0..1
    [SerializeField] private bool fireOnceUntilRelease = true;

    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private Vector3 startLocalPos;
    private bool isTouching;
    private bool isPressed;

    private void Reset()
    {
        localPressAxis = Vector3.down;
        pressDistance = 0.01f;
        pressThreshold = 0.85f;
        fireOnceUntilRelease = true;
    }

    private void Awake()
    {
        if (buttonCap == null)
        {
            Debug.LogError("[Simple3DPushButton] ButtonCap reference missing.");
            enabled = false;
            return;
        }

        startLocalPos = buttonCap.localPosition;

        if (pressVolume != null)
            pressVolume.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detect XR poke interactor tip
        if (other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor>() != null)
            isTouching = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor>() != null)
            isTouching = false;
    }

    private void Update()
    {
        var axis = localPressAxis.normalized;

        // Target press amount
        float target01 = isTouching ? 1f : 0f;

        // Current press amount based on cap position
        float current01 = GetPressAmount01(axis);

        // Move towards target
        float speed = isTouching ? pressSpeed : returnSpeed;
        float new01 = Mathf.MoveTowards(current01, target01, speed * Time.deltaTime);

        SetPressAmount01(axis, new01);

        bool shouldBePressed = new01 >= pressThreshold;

        if (shouldBePressed && !isPressed)
        {
            isPressed = true;
            onPressed?.Invoke();

            if (fireOnceUntilRelease)
                isTouching = false; // prevents repeat while held in
        }
        else if (!shouldBePressed && isPressed)
        {
            isPressed = false;
            onReleased?.Invoke();
        }
    }

    private float GetPressAmount01(Vector3 axis)
    {
        var delta = buttonCap.localPosition - startLocalPos;
        float along = Vector3.Dot(delta, axis); // negative if axis is down and moved down
        float amount = Mathf.Clamp01(Mathf.Abs(along) / Mathf.Max(pressDistance, 0.0001f));
        return amount;
    }

    private void SetPressAmount01(Vector3 axis, float amount01)
    {
        float dist = Mathf.Clamp(amount01, 0f, 1f) * pressDistance;
        buttonCap.localPosition = startLocalPos + axis * dist;
    }
}