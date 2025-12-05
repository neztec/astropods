using UnityEngine;
using UniRx;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;
    public float followLerp = 5f;

    [Header("Speed-based Zoom")]
    public float minZoom = 6f;
    public float maxZoom = 14f;
    public float maxShipSpeed = 20f;
    public float zoomLerp = 3f;

    private Camera cam;
    private Rigidbody2D targetRb;

    private bool isPanning = false;
    private Vector3 lastMouseScreen;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandlePan();
        HandleZoom();
    }

    void LateUpdate()
    {
        HandleFollow();
    }

    void HandleZoom()
    {
        if (followTarget == null || targetRb == null)
            return;

        float speed = targetRb.velocity.magnitude;

        float t = Mathf.InverseLerp(0f, maxShipSpeed, speed);
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, t);

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetZoom,
            zoomLerp * Time.deltaTime
        );
    }
    void HandlePan()
    {
        // Block camera if another input mode owns input
        if (InputRouter.Instance.IsActive(InputMode.AbilityTargeting) ||
            InputRouter.Instance.IsActive(InputMode.ShipThrusting))
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(
                new Vector3(
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    Mathf.Abs(cam.transform.position.z)
                )
            );

            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);

            bool clickedHandle = hit.collider && hit.collider.GetComponentInParent<AbilityHandle>();
            bool clickedSegment = hit.collider && hit.collider.GetComponentInParent<AbilitySegment>();
            bool clickedShip = hit.collider && hit.collider.GetComponentInParent<ShipCore>();

            if (!clickedShip && !clickedHandle && !clickedSegment)
            {
                if (!InputRouter.Instance.TryClaim(InputMode.CameraPanning))
                    return;

                followTarget = null;
                targetRb = null;
                isPanning = true;
                lastMouseScreen = Input.mousePosition;
            }
        }

        if (Input.GetMouseButtonUp(0) && isPanning)
        {
            isPanning = false;
            InputRouter.Instance.Release(InputMode.CameraPanning);
            return;
        }

        if (!isPanning)
            return;

        Vector3 deltaScreen = Input.mousePosition - lastMouseScreen;

        Vector3 worldDelta =
            cam.ScreenToWorldPoint(
                new Vector3(deltaScreen.x, deltaScreen.y, Mathf.Abs(cam.transform.position.z))
            )
            - cam.ScreenToWorldPoint(
                new Vector3(0, 0, Mathf.Abs(cam.transform.position.z))
            );

        transform.position -= worldDelta;
        lastMouseScreen = Input.mousePosition;
    }
    void HandleFollow()
    {
        if (isPanning ||
            followTarget == null ||
            InputRouter.Instance.IsActive(InputMode.CameraPanning))
            return;

        Vector3 target = followTarget.position;
        target.z = transform.position.z;

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            followLerp * Time.deltaTime
        );
    }
    public void SetTarget(Transform t)
    {
        if (isPanning ||
            InputRouter.Instance.IsActive(InputMode.CameraPanning))
            return;

        followTarget = t;
        targetRb = t ? t.GetComponent<Rigidbody2D>() : null;
    }

}
