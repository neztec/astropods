using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetScroller : MonoBehaviour
{

    public float scrollSpeed;
    public float scaleCount;
    private Renderer renderer;

    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {


        // Camera should be no closer than size:7

        // TODO ask Scott how to get this to execute in the editor while building the game
        // TODO cache the float so that we save on performance

        // Remember, scale properties on a transform dictates width and height

        // This figures out the exact scale to make the transform (quad) to fit the
        // camera width-wise. In a landscape type screen this works perfectly, with some
        // bleed over in the Y axis
        double widthDouble = Camera.main.orthographicSize * 2.0 * Screen.width / Screen.height;
        float width = (float)widthDouble;
        transform.localScale = new Vector3(width, width, width);

        // Split the width (scale size) by our scaleCount so that the texture will repeat
        // that many times over the width of the screen
        float textureScaleW = width / scaleCount;
        renderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(textureScaleW, textureScaleW));

        // Decide how much offset to give the texture, given the current position and the scale
        // I am fuzzy on why scaleRatio needs to be here, but it does...  the "-width/2 * scaleRatio"
        // bit makes is so that "zooming" in and out with the camera's orthographicSize will
        // look like scaling from the center (where the ship is)
        // the "...cameraWorldPosition.x) / scrollSpeed" bit is about setting offset based on position
        Vector3 cameraWorldPosition = Camera.main.transform.position;
        float scaleRatio = scrollSpeed / scaleCount;
        float x = Mathf.Repeat((-width / 2 * scaleRatio + cameraWorldPosition.x) / scrollSpeed, 1);
        float y = Mathf.Repeat((-width / 2 * scaleRatio + cameraWorldPosition.y) / scrollSpeed, 1);
        Vector2 offset = new Vector2(x, y);

        renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
