using UnityEngine;
using UnityEngine.InputSystem; // New Input System

[RequireComponent(typeof(Rigidbody2D))]
public class ShipHandleController : MonoBehaviour
{
    [Header("Handle Settings")]
    public GameObject handlePrefab;
    public float maxDragDistance = 2f;
    public float thrustStrength = 8f;
    public float damping = 0.3f;

    Rigidbody2D rb;
    Camera cam;
    Transform handle;
    LineRenderer line;
    bool dragging = false;
    Vector2 dragPos;

    public bool IsSelected = false; // Hook this to your selection logic

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update()
    {
        if (!IsSelected) HideHandle();

        HandleInput();
        UpdateHandleVisuals();
        ApplyThrust();
    }

    void HandleInput()
    {
        if (!IsSelected) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            StartDrag();
        }
        else if (mouse.leftButton.isPressed && dragging)
        {
            dragPos = cam.ScreenToWorldPoint(mouse.position.ReadValue());
        }
        else if (mouse.leftButton.wasReleasedThisFrame)
        {
            EndDrag();
        }
    }

    void StartDrag()
    {
        if (handle == null)
        {
            handle = Instantiate(handlePrefab).transform;
            line = handle.GetComponent<LineRenderer>();
        }
        dragging = true;
        dragPos = transform.position;
        handle.gameObject.SetActive(true);
    }

    void EndDrag()
    {
        dragging = false;
        HideHandle();
    }

    void HideHandle()
    {
        if (handle != null) handle.gameObject.SetActive(false);
    }

    void UpdateHandleVisuals()
    {
        if (!dragging) return;

        Vector2 shipPos = transform.position;
        Vector2 rawOffset = dragPos - shipPos;

        float distance = Mathf.Clamp(rawOffset.magnitude, 0f, maxDragDistance);
        Vector2 clampedOffset = rawOffset.normalized * distance;

        Vector2 handlePos = shipPos + clampedOffset;
        handle.position = handlePos;

        // Draw stem
        if (line != null)
        {
            line.positionCount = 2;
            line.SetPosition(0, shipPos);
            line.SetPosition(1, handlePos);
        }
    }

    void ApplyThrust()
    {
        if (!dragging) return;

        Vector2 shipPos = transform.position;
        Vector2 offset = shipPos - (Vector2)handle.position; // Opposite drag
        float distance = offset.magnitude;

        // spring force - damping
        Vector2 springForce = offset.normalized * (distance * thrustStrength);
        Vector2 dampForce = -rb.velocity * damping;

        rb.AddForce(springForce + dampForce);
    }
}
