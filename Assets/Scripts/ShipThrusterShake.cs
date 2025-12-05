using UnityEngine;

[RequireComponent(typeof(ShipMovementController))]
public class ShipThrusterShake : MonoBehaviour
{
    [Header("Thrusters")]
    [SerializeField] private Transform[] thrusters;

    [Header("Shake Settings")]
    [SerializeField] private float maxAmplitude = 0.014f;     // maximum displacement
    [SerializeField] private float baseFrequency = 25f;     // base vibration speed
    [SerializeField] private float lerpSpeed = 0.2f;        // smoothing factor

    private Vector3[] initialPositions;
    private float[] phaseOffsets;
    private ShipMovementController movement;

    private void Awake()
    {
        movement = GetComponentInParent<ShipMovementController>();
        if (thrusters.Length == 0)
            Debug.LogWarning("No thrusters assigned!");
    }

    private void Start()
    {
        int n = thrusters.Length;
        initialPositions = new Vector3[n];
        phaseOffsets = new float[n];

        for (int i = 0; i < n; i++)
        {
            initialPositions[i] = thrusters[i].localPosition;
            phaseOffsets[i] = Random.Range(0f, Mathf.PI * 2f); // random phase per thruster
        }
    }

    private void Update()
    {
        float thrust = movement.GetCurrentThrust();
        if (thrust <= 0f) return;

        for (int i = 0; i < thrusters.Length; i++)
        {
            float freq = baseFrequency * (0.8f + 0.4f * i / thrusters.Length);
            float shakeAmount = Mathf.Sin(Time.time * freq + phaseOffsets[i]) * maxAmplitude * thrust;

            // Shake along local Y/back axis (assumes thrusters point "up")
            Vector3 offset = new Vector3(0f, -shakeAmount, 0f);
            thrusters[i].localPosition = Vector3.Lerp(thrusters[i].localPosition, initialPositions[i] + offset, lerpSpeed);
        }
    }

}
