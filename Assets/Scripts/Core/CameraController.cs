using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minHeight = 5f;
    [SerializeField] private float maxHeight = 30f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 0.3f;

    private void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector3 direction = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
            direction += Vector3.forward;

        if (Keyboard.current.sKey.isPressed)
            direction += Vector3.back;

        if (Keyboard.current.dKey.isPressed)
            direction += Vector3.right;

        if (Keyboard.current.aKey.isPressed)
            direction += Vector3.left;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll == 0)
            return;

        Vector3 position = transform.position;

        position.y -= scroll * zoomSpeed * Time.deltaTime;

        position.y = Mathf.Clamp(
            position.y,
            minHeight,
            maxHeight
        );

        transform.position = position;
    }

    private void HandleRotation()
    {
        if (!Mouse.current.middleButton.isPressed)
            return;

        Vector2 mouseDelta =
            Mouse.current.delta.ReadValue();

        transform.Rotate(
            Vector3.up,
            mouseDelta.x * rotationSpeed,
            Space.World
        );
    }
}