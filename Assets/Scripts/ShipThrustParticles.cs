using UnityEngine;

[RequireComponent(typeof(ShipMovementController))]
public class ShipThrustParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem thrustParticles;
    private ShipMovementController movement;

    private void Awake()
    {
        movement = GetComponentInParent<ShipMovementController>();
        if (thrustParticles == null)
            thrustParticles = GetComponent<ParticleSystem>();
    }
    [SerializeField] private float particlesPerSecond = 10f;
    private float emitAccumulator = 0f;

    private void Update()
    {
        if (movement.IsThrusting())
        {
            emitAccumulator += particlesPerSecond * Time.deltaTime;
            while (emitAccumulator >= 1f)
            {
                thrustParticles.Emit(1);
                emitAccumulator -= 1f;
            }
        }
        else
        {
            emitAccumulator = 0f; // reset when not thrusting
        }
    }
}
