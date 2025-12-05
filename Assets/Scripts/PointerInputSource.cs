using UnityEngine;
using UniRx;
using System;

public enum PointerTarget
{
    None,
    UI,
    AbilitySegment,
    AbilityHandle,
    Ship,
    EmptySpace
}

public struct PointerEvent
{
    public Vector2 WorldPosition;
    public PointerTarget Target;
    public Vector2 Delta; // for drags
}

public class PointerInputSource : MonoBehaviour
{
    public static PointerInputSource Instance { get; private set; }

    private Camera cam;

    // Streams
    public readonly Subject<PointerEvent> OnPointerDown = new();
    public readonly Subject<PointerEvent> OnPointerUp = new();
    public readonly Subject<PointerEvent> OnPointerDrag = new();

    // internal state
    private bool isDown;
    private Vector2 startPos;
    private Vector2 lastPos;
    private PointerTarget currentTarget;
    private const float DragThreshold = 5f; // pixels

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    void Update()
    {
        Vector2 mouseScreen = Input.mousePosition;
        Vector3 mouseWorld3 = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, Mathf.Abs(cam.transform.position.z)));
        Vector2 mouseWorld = mouseWorld3;

        // POINTER DOWN
        if (Input.GetMouseButtonDown(0))
        {
            isDown = true;
            startPos = mouseWorld;
            lastPos = mouseWorld;
            currentTarget = RaycastTarget(mouseWorld);

            OnPointerDown.OnNext(new PointerEvent
            {
                WorldPosition = mouseWorld,
                Target = currentTarget,
                Delta = Vector2.zero
            });
        }

        // POINTER DRAG
        if (isDown && Input.GetMouseButton(0))
        {
            Vector2 deltaScreen = mouseScreen - (Vector2)cam.WorldToScreenPoint(startPos);
            if (deltaScreen.magnitude > DragThreshold)
            {
                OnPointerDrag.OnNext(new PointerEvent
                {
                    WorldPosition = mouseWorld,
                    Target = currentTarget,
                    Delta = mouseWorld - lastPos
                });
            }
            lastPos = mouseWorld;
        }

        // POINTER UP
        if (isDown && Input.GetMouseButtonUp(0))
        {
            OnPointerUp.OnNext(new PointerEvent
            {
                WorldPosition = mouseWorld,
                Target = currentTarget,
                Delta = mouseWorld - lastPos
            });
            isDown = false;
        }
    }

    // Centralized hit test: determines what is under the pointer
    private PointerTarget RaycastTarget(Vector2 worldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (!hit.collider) return PointerTarget.EmptySpace;

        if (hit.collider.GetComponentInParent<AbilitySegment>()) return PointerTarget.AbilitySegment;
        if (hit.collider.GetComponentInParent<AbilityHandle>()) return PointerTarget.AbilityHandle;
        if (hit.collider.GetComponentInParent<ShipCore>()) return PointerTarget.Ship;
        if (hit.collider.GetComponentInParent<UnityEngine.UI.Graphic>()) return PointerTarget.UI;

        return PointerTarget.EmptySpace;
    }
}
