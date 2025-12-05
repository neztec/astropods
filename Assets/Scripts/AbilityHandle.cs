using UnityEngine;
using UniRx;

public struct AbilityFireEvent
{
    public Vector2 Direction;
    public float Strength;

    public string AbilityId;
}

public class AbilityHandle : MonoBehaviour
{
    // cooldown
    public float cooldownTime = 0.25f;
    private float lastFireTime = 0f;

    public Transform parent;
    public float defaultDistance = 3.5f;
    public float maxDragDistance = 5.5f;

    public readonly Subject<AbilityFireEvent> FireStream = new Subject<AbilityFireEvent>();

    private bool dragging = false;
    private Camera cam;
    private LineRenderer line;

    private ShipCore core;

    void Awake()
    {
        core = GetComponentInParent<ShipCore>();

        cam = Camera.main;
        line = gameObject.AddComponent<LineRenderer>();
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;

        // material
        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        line.material = lineMaterial;

        // color with some transparency
        line.startColor = new Color(1, 1, 1, 0.5f);
        line.endColor = new Color(1, 1, 1, 0.5f);

    }

    void Update()
    {
        // if the ship core is thrusting or null, disable ability handle


        if (parent == null)
            return;

        // CLICK START (raycast-based check)
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mp = Input.mousePosition;
            mp.z = Mathf.Abs(cam.transform.position.z);
            Vector3 mw = cam.ScreenToWorldPoint(mp);

            RaycastHit2D hit = Physics2D.Raycast(mw, Vector2.zero);

            if (hit.collider && hit.collider.gameObject == gameObject)
            {
                dragging = true;
            }
        }

        // DRAG END
        if (dragging && Input.GetMouseButtonUp(0))
        {
            dragging = false;

            // Vector3 mp = Input.mousePosition;
            // mp.z = Mathf.Abs(cam.transform.position.z);
            // Vector3 mw = cam.ScreenToWorldPoint(mp);

            // Vector2 delta = mw - ship.position;
            // float dist = Mathf.Clamp(delta.magnitude, 0, maxDragDistance);

            // FireStream.OnNext(new AbilityFireEvent
            // {
            //     Direction = (ship.position - transform.position).normalized,
            //     Strength = dist / maxDragDistance
            // });
        }

        // RESTING POSITION
        if (!dragging)
        {
            transform.position = parent.position - parent.right * defaultDistance;
            return;
        }

        // ACTIVE DRAGGING
        {
            Vector3 mp = Input.mousePosition;
            mp.z = Mathf.Abs(cam.transform.position.z);
            Vector3 mw = cam.ScreenToWorldPoint(mp);

            Vector2 raw = mw - parent.position;
            float dist = Mathf.Clamp(raw.magnitude, 0, maxDragDistance);
            Vector2 offset = raw.normalized * dist;

            transform.position = parent.position + (Vector3)offset;

            /// <summary>
            /// Fire the ability with the calculated direction and strength.
            /// </summary>

            // account for cooldown
            if (Time.time - lastFireTime < cooldownTime)
                return;

            lastFireTime = Time.time;
            FireStream.OnNext(new AbilityFireEvent
            {
                Direction = (parent.position - transform.position).normalized,
                Strength = dist / maxDragDistance
            });
        }
    }

    void LateUpdate()
    {
        if (line == null || parent == null) return;

        line.SetPosition(0, parent.position);
        line.SetPosition(1, transform.position);
        transform.rotation = Quaternion.identity;
    }

}
