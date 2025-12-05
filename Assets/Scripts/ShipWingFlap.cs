using UnityEngine;

[RequireComponent(typeof(ShipMovementController))]
public class ShipWingFlap : MonoBehaviour
{
    [Header("Wing References")]
    [SerializeField] private Transform leftWing;
    [SerializeField] private Transform rightWing;

    [Header("Flap Settings")]
    [SerializeField] private float maxFlapAngle = 20f; // max wing rotation
    [SerializeField] private float flapFrequency = 2f; // flaps per second

    private ShipMovementController movement;
    private Quaternion leftInitial;
    private Quaternion rightInitial;
    private float flapTimer = 0f;

    private void Awake()
    {
        movement = GetComponentInParent<ShipMovementController>();
        leftInitial = leftWing.localRotation;
        rightInitial = rightWing.localRotation;
    }

    private void Update()
    {
        float turnDir = movement.GetTurnDirection(); // -1, 0, 1

        // get isThrusting from movement controller
        if (movement.IsThrusting())
        {
            // reset wings to initial position if thrusting
            leftWing.localRotation = leftInitial;
            rightWing.localRotation = rightInitial;
            flapTimer = 0f; // reset timer
            return;
        }

        // advance timer based on flapFrequency
        flapTimer += Time.deltaTime * flapFrequency * Mathf.PI * 2f; // full sine cycle
        float flapAngle = Mathf.Sin(flapTimer) * maxFlapAngle;

        if (turnDir < 0f) // turning left
        {
            leftWing.localRotation = leftInitial * Quaternion.Euler(0, 0, flapAngle);
            rightWing.localRotation = rightInitial;
        }
        else if (turnDir > 0f) // turning right
        {
            leftWing.localRotation = leftInitial;
            rightWing.localRotation = rightInitial * Quaternion.Euler(0, 0, -flapAngle);
        }
        else // no turn
        {
            leftWing.localRotation = leftInitial;
            rightWing.localRotation = rightInitial;
        }
    }
}
