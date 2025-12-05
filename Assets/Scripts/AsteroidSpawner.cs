using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public ProceduralAsteroid prefab;
    public int count = 10;
    public float speedMin = 0.5f;
    public float speedMax = 2f;

    // size controlled by transform.localScale
    // so you can scale it visually in the editor

    void Start()
    {
        for (int i = 0; i < count; i++)
            SpawnOne();
    }

    void SpawnOne()
    {
        Vector2 half = transform.localScale / 2f;

        var a = Instantiate(prefab);

        a.transform.position = new Vector2(
            Random.Range(-half.x, half.x) + transform.position.x,
            Random.Range(-half.y, half.y) + transform.position.y
        );

        float speed = Random.Range(speedMin, speedMax);
        float angle = Random.Range(0f, Mathf.PI * 2f);

        a.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.25f);     // transparent fill
        Gizmos.DrawCube(transform.position, transform.localScale);

        Gizmos.color = Color.yellow;                 // outline
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
