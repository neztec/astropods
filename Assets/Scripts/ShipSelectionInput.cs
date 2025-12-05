using UnityEngine;
using UniRx;

[RequireComponent(typeof(ShipCore))]
public class ShipSelectionInput : MonoBehaviour
{
    private ShipCore core;
    private bool isSelected;

    private Vector3 mouseDownWorld;
    private bool isDraggingShip;
    private const float DragThreshold = 0.1f;

    private CameraController cam;

    private InputRouter router;


    private void Awake()
    {
        core = GetComponent<ShipCore>();
        cam = Camera.main.GetComponent<CameraController>();
        router = InputRouter.Instance;

    }

    private void Start()
    {
        core.OnStateChanged()
            .Subscribe(state => isSelected = state.Selected)
            .AddTo(this);
    }

    private Vector3 GetWorldMouse()
    {
        // Guarantee consistent depth for 2D point
        float z = Mathf.Abs(Camera.main.transform.position.z);
        return Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, z)
        );
    }

    private void Update()
    {
        if (InputRouter.Instance.IsActive(InputMode.AbilityTargeting))
            return;

        Vector3 mouseWorld = GetWorldMouse();

        if (Input.GetMouseButtonDown(0))
        {
            mouseDownWorld = mouseWorld;
            isDraggingShip = false;

            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);
            bool clickedThisShip = hit.collider && hit.collider.gameObject == gameObject;

            if (clickedThisShip && !isSelected)
            {
                core.SetSelected(true);
                cam.SetTarget(transform);
            }
        }


        if (Input.GetMouseButton(0))
        {
            float dist = Vector2.Distance(mouseWorld, mouseDownWorld);
            if (dist > DragThreshold)
                isDraggingShip = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);
            bool clickedThisShip = hit.collider && hit.collider.gameObject == gameObject;

            if (!isDraggingShip)
            {
                if (clickedThisShip)
                {
                    bool newState = !isSelected;
                    core.SetSelected(newState);

                    // Use correct camera call
                    cam.SetTarget(newState ? transform : null);
                }
                else if (isSelected)
                {
                    core.SetSelected(false);
                    cam.SetTarget(null);
                }
            }

            isDraggingShip = false;
        }
    }
}
