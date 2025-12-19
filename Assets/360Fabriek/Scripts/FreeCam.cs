using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCam : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float lookSensitivity = 0.1f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotationX = rot.y;
        rotationY = -rot.x;
    }

    void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            rotationX += mouseDelta.x * lookSensitivity;
            rotationY += mouseDelta.y * lookSensitivity;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);

            transform.localRotation = Quaternion.Euler(-rotationY, rotationX, 0f);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        float currentSpeed = moveSpeed;

        Vector3 moveInput = Vector3.zero;
        if (Keyboard.current.wKey.isPressed) moveInput += Vector3.forward;
        if (Keyboard.current.sKey.isPressed) moveInput += Vector3.back;
        if (Keyboard.current.aKey.isPressed) moveInput += Vector3.left;
        if (Keyboard.current.dKey.isPressed) moveInput += Vector3.right;

        transform.Translate(moveInput.normalized * currentSpeed * Time.deltaTime, Space.Self);

        if (Keyboard.current.eKey.isPressed)
            transform.Translate(Vector3.up * currentSpeed * Time.deltaTime, Space.World);

        if (Keyboard.current.qKey.isPressed)
            transform.Translate(Vector3.down * currentSpeed * Time.deltaTime, Space.World);
    }
}