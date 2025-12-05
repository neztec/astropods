using UnityEngine;

public class ProjectileHitAsteroid : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        AsteroidBlock block = other.GetComponent<AsteroidBlock>();
        if (block)
        {
            AsteroidGrid grid = block.GetComponentInParent<AsteroidGrid>();
            if (grid)
                grid.DestroyBlock(block);

            Destroy(gameObject); // bullet despawn
        }
    }
}
