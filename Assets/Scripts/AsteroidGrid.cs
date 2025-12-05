using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGrid : MonoBehaviour
{
    [Header("Grid Config")]
    public int width = 6;
    public int height = 6;
    public float cellSize = 0.5f;

    [Header("Blocks")]
    public GameObject blockPrefab;   // assign a square block prefab

    [Header("Debris")]
    public GameObject debrisPrefab;  // small block with Rigidbody2D (optional)
    public float debrisForce = 3f;

    private AsteroidBlock[,] blocks;

    void Start()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        blocks = new AsteroidBlock[width, height];

        Vector2 origin = transform.position;
        Vector2 startOffset = new Vector2(-width * cellSize * 0.5f, -height * cellSize * 0.5f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Random.value < 0.2f) continue; // optional: add holes

                Vector2 pos = origin + startOffset + new Vector2(
                    x * cellSize + cellSize * 0.5f,
                    y * cellSize + cellSize * 0.5f
                );

                GameObject b = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                blocks[x, y] = b.GetComponent<AsteroidBlock>();
            }
        }
    }

    public void DestroyBlock(AsteroidBlock block)
    {
        // optional debris
        if (debrisPrefab)
        {
            GameObject d = Instantiate(debrisPrefab, block.transform.position, Quaternion.identity);
            var rb = d.GetComponent<Rigidbody2D>();
            if (rb)
            {
                Vector2 force = Random.insideUnitCircle.normalized * debrisForce;
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

        Destroy(block.gameObject);
    }
}
