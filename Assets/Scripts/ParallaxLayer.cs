using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ParallaxLayer : MonoBehaviour
{
    // 0 = far (moves least)
    // 1 = near (moves most)
    public float parallaxStrength = 0.1f;

    // Extra dampening to avoid fast scrolling
    public float uvScale = 0.002f;

    private Renderer rend;
    private Vector2 uvOffset;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        uvOffset = Vector2.zero;
    }

    public void AddMotion(Vector2 shipDelta)
    {
        // Smaller movement for distant layers
        float factor = parallaxStrength * uvScale;

        // Move *against* ship to create parallax drift
        uvOffset -= shipDelta * factor;

        rend.material.SetVector("_UVOffset", uvOffset);
    }
}
