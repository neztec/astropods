using UnityEngine;
using UniRx;

public class ShipAbilityController : MonoBehaviour
{
    public ShipCore core;
    public AbilityHandle abilityHandle;
    public GameObject projectilePrefab;

    public float energyCost = 0.5f;
    public float projectileForce = 10f;

    void Start()
    {
        abilityHandle.FireStream
            .Where(_ => core != null)
            .Subscribe(evt =>
            {
                if (!core.TryUseEnergy(energyCost))
                    return;

                FireProjectile(evt.Direction, evt.Strength);
            })
            .AddTo(this);
    }

    void FireProjectile(Vector2 direction, float strength)
    {
        // debug
        Debug.Log($"Firing projectile: Direction={direction}, Strength={strength}");
        Vector2 spawnPos = (Vector2)transform.position + direction * 0.6f;
        var proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * projectileForce * strength, ForceMode2D.Impulse);
    }
}
