using UnityEngine;

public class DragHandler : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;

    void Update()
    {
        // --- TOUCH INPUT ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
            worldPos.z = 0;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Collider2D hit = Physics2D.OverlapPoint(worldPos);
                    if (hit && hit.transform == transform)
                    {
                        isDragging = true;
                        offset = transform.position - worldPos;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                        transform.position = worldPos + offset;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }

        // --- MOUSE INPUT (for Editor or Desktop) ---
        else
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            if (Input.GetMouseButtonDown(0))
            {
                Collider2D hit = Physics2D.OverlapPoint(mousePos);
                if (hit && hit.transform == transform)
                {
                    isDragging = true;
                    offset = transform.position - mousePos;
                }
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                transform.position = mousePos + offset;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
    }
}
