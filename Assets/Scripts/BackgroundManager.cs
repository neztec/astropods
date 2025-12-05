using UnityEngine;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    public Camera cam;                         // main camera
    public List<ParallaxLayer> layers;         // star layers + grid layer

    private Vector3 lastCamPos;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        lastCamPos = cam.transform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.transform.position - lastCamPos;
        lastCamPos = cam.transform.position;

        Vector2 d2 = new Vector2(delta.x, delta.y);

        foreach (var layer in layers)
            layer.AddMotion(d2);
    }
}
