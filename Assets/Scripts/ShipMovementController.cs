using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(ShipCore), typeof(Rigidbody2D), typeof(Collider2D))]
public class ShipMovementController : MonoBehaviour
{
    private ShipCore core;
    private Rigidbody2D rb;
    private Collider2D col;

    [SerializeField] private float thrustForce = 10f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float damping = 1.5f;
    [SerializeField] private float energyPerThrust = 0.5f;
    [SerializeField] private float alignmentThreshold = 10f; // degrees

    private bool isThrusting;
    private float thrustStrength;
    public bool IsThrusting() => isThrusting;

    // variable to track rotation direction for visual effects (CW, CCW, or not rotating)
    public float rotationDirection { get; private set; }

    private bool isSelected;
    private bool isDraggingShip;
    private readonly CompositeDisposable disposables = new();

    private void Awake()
    {
        core = GetComponent<ShipCore>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        core.OnStateChanged()
            .Subscribe(state => isSelected = state.Selected)
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Subscribe(_ => HandleInput())
            .AddTo(disposables);
    }

    private void HandleInput()
    {
        isThrusting = false;
        thrustStrength = 0f;
        rotationDirection = 0f;

        if (!isSelected)
            return;

        var mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (col.OverlapPoint(worldPos))
            {
                isDraggingShip = true;
            }
        }


        // Handle release
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDraggingShip = false;
        }

        // Only move if dragging started on the ship
        if (!isDraggingShip)
            return;

        Vector2 toTarget = worldPos - rb.position;
        if (toTarget.sqrMagnitude < 0.001f)
            return;

        float targetAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = rb.rotation;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        rotationDirection = Mathf.Sign(Mathf.DeltaAngle(currentAngle, newAngle));
        rb.MoveRotation(newAngle);

        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(newAngle, targetAngle));
        if (angleDiff < alignmentThreshold && core.TryUseEnergy(energyPerThrust))
        {
            Vector2 forward = transform.up;
            rb.AddForce(forward * thrustForce * Time.deltaTime, ForceMode2D.Force);
            isThrusting = true;

            // set thrust strength based how on-alignment we are (visual effect purpose)
            // (in reality here it's either full thrust or no thrust)
            thrustStrength = 1f - (angleDiff / alignmentThreshold);
        }

        rb.velocity *= 1f - (damping * Time.deltaTime);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }

    // returns 0-1 based on current thrust usage
    public float GetCurrentThrust()
    {
        return thrustStrength;
    }

    // optional: returns 0-1 based on rotational velocity
    public float GetRotationMagnitude()
    {
        return Mathf.Abs(rb.angularVelocity) / 360f; // normalize roughly
    }

    // return the current turn direction: -1 (CCW), 0 (none), 1 (CW)
    public float GetTurnDirection()
    {
        return rotationDirection;
    }

}
