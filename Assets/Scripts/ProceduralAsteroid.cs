using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ProceduralAsteroid : MonoBehaviour
{
    [Header("Shape")]
    public int minPoints = 12;
    public int maxPoints = 22;
    public float majorDeform = 0.35f;
    public float microDeform = 0.12f;
    public float angleJitter = 0.15f;

    [Header("Motion")]
    public Vector2 velocity;
    public float angularVelocity;

    [Header("Material")]
    public Material asteroidMaterial;

    [Header("World")]
    public float worldSize = 200f; // quad width & height

    Mesh mesh;
    PolygonCollider2D poly;
    Rigidbody2D rb;

    float radius;
    float halfWorld;

    void Start()
    {
        mesh = new Mesh();
        poly = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        GetComponent<MeshFilter>().mesh = mesh;

        if (asteroidMaterial != null)
            GetComponent<MeshRenderer>().material = asteroidMaterial;
        else
            GetComponent<MeshRenderer>().material =
                new Material(Shader.Find("Sprites/Default"));

        // Rigidbody setup
        rb.gravityScale = 0f;
        rb.angularDrag = 0f;
        rb.drag = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        halfWorld = worldSize * 0.5f;

        Generate();
    }

    void Generate()
    {
        int count = Random.Range(minPoints, maxPoints + 1);
        radius = Random.Range(0.7f, 6.2f);

        // modify mass based on size
        rb.mass = radius * radius * 0.8f;

        Vector3[] verts = new Vector3[count];
        Vector2[] polyPts = new Vector2[count];
        Color[] colors = new Color[count];
        int[] tris = new int[(count - 2) * 3];

        float shapeSeed = Random.Range(0f, 1000f);
        Vector2 lightDir = new Vector2(0.6f, 0.8f).normalized;

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)count;

            float angle = t * Mathf.PI * 2f +
                          Mathf.PerlinNoise(shapeSeed, t * 3f) * angleJitter;

            float deform =
                Mathf.PerlinNoise(shapeSeed + t * 4f, 0f) * majorDeform +
                Mathf.PerlinNoise(shapeSeed + t * 10f, 3.3f) * microDeform;

            float r = radius * (1f + deform);

            Vector2 v = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;

            verts[i] = v;
            polyPts[i] = v;

            // ---- CLEAN FACET SHADING ----
            float light = Vector2.Dot(v.normalized, lightDir);
            float edge = r / radius;

            float facet =
                Mathf.Round(
                    Mathf.PerlinNoise(shapeSeed + i * 1.7f, 9.1f) * 4f
                ) / 4f;

            float shade =
                0.55f +
                light * 0.22f +
                facet * 0.12f -
                edge * 0.35f;

            shade = Mathf.Clamp01(shade);
            colors[i] = new Color(shade, shade, shade, 1f);
        }

        int ti = 0;
        for (int i = 1; i < count - 1; i++)
        {
            tris[ti++] = 0;
            tris[ti++] = i;
            tris[ti++] = i + 1;
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        poly.points = polyPts;

        // Spin
        angularVelocity = Random.Range(-40f, 40f);
        rb.angularVelocity = angularVelocity;
    }

    void FixedUpdate()
    {
        rb.position += velocity * Time.fixedDeltaTime;
        Wrap();
    }

    void Wrap()
    {
        Vector2 p = rb.position;
        float r = radius;

        if (p.x > halfWorld + r) p.x = -halfWorld - r;
        else if (p.x < -halfWorld - r) p.x = halfWorld + r;

        if (p.y > halfWorld + r) p.y = -halfWorld - r;
        else if (p.y < -halfWorld - r) p.y = halfWorld + r;

        rb.position = p;
    }
}
