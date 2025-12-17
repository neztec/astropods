using UnityEngine;
using UniRx;

public enum AbilityInputState
{
    Idle,
    Targeting
}

[RequireComponent(typeof(PolygonCollider2D))]
public class AbilitySegment : MonoBehaviour
{
    public readonly Subject<AbilityFireEvent> FireStream = new();
    public readonly ReactiveProperty<AbilityInputState> InputState = new(AbilityInputState.Idle);

    Transform center;
    float baseRadius;
    float maxPull;
    string id;
    float a0, a1;

    LineRenderer line;      // arc
    LineRenderer stemLine;  // stem from ring center to pull point
    Camera cam;
    PolygonCollider2D polyCollider;

    bool dragging;
    Vector2 currentDir;
    float currentPull;

    const int ARC_POINTS = 32;

    AbilityRing ringParent;

    public void Init(Transform ship, float radius, float pull, AbilityRing.Slot slot, AbilityRing ring)
    {
        center = ship;
        baseRadius = radius;
        maxPull = pull;
        id = slot.id;
        a0 = slot.startAngle;
        a1 = slot.endAngle;
        ringParent = ring;

        cam = Camera.main;

        // Arc line
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = 1.12f;
        line.endWidth = 1.12f;
        line.useWorldSpace = false;
        line.startColor = slot.color;
        line.endColor = slot.color;

        // Stem line
        stemLine = new GameObject("StemLine").AddComponent<LineRenderer>();
        stemLine.transform.SetParent(transform);
        stemLine.transform.localPosition = Vector3.zero;
        stemLine.material = new Material(Shader.Find("Sprites/Default"));
        stemLine.startWidth = 0.08f;
        stemLine.endWidth = 0.08f;
        stemLine.startColor = slot.color;
        stemLine.endColor = slot.color;
        stemLine.positionCount = 2;
        stemLine.enabled = false;

        polyCollider = GetComponent<PolygonCollider2D>();
        polyCollider.isTrigger = true;

        UpdateCollider(baseRadius, baseRadius + 0.5f);
    }

    void Update()
    {
        if (!center) return;

        Vector2 mouse = GetMouseWorld();

        if (Input.GetMouseButtonDown(0) && Hit(mouse))
        {
            if (!InputRouter.Instance.TryClaim(InputMode.AbilityTargeting))
                return;

            dragging = true;
            InputState.Value = AbilityInputState.Targeting;
        }

        float radius = baseRadius;

        if (!dragging)
        {
            DrawArc(radius);
            UpdateCollider(radius, radius + 0.5f);
            stemLine.enabled = false;
            return;
        }

        // Dragging logic
        Vector2 raw = mouse - (Vector2)center.position;
        currentPull = Mathf.Clamp(raw.magnitude, 0, maxPull);
        currentDir = raw.normalized;

        radius = baseRadius + currentPull;
        DrawArc(radius);
        UpdateCollider(baseRadius, radius, 0.2f);

        // Update stem in local space of ring
        stemLine.enabled = false;
        Vector3 localDir = transform.parent.InverseTransformVector(currentDir * radius);
        stemLine.SetPosition(0, Vector3.zero);
        stemLine.SetPosition(1, localDir);

        // Rotate the ring so the dragged segment mid-point points toward the pointer
        if (dragging && ringParent != null)
        {
            float targetAngle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;
            float segmentMidAngle = (a0 + a1) / 2f;
            float rotationDelta = targetAngle - segmentMidAngle;
            ringParent.transform.rotation = Quaternion.Euler(0, 0, rotationDelta);
        }


        // Fire on release
        if (Input.GetMouseButtonUp(0))
        {
            FireStream.OnNext(new AbilityFireEvent
            {
                AbilityId = id,
                Direction = currentDir,
                Strength = currentPull / maxPull
            });

            dragging = false;
            InputState.Value = AbilityInputState.Idle;
            InputRouter.Instance.Release(InputMode.AbilityTargeting);
            stemLine.enabled = false;
        }
    }

    void OnDisable()
    {
        InputState.Value = AbilityInputState.Idle;
        InputRouter.Instance?.Release(InputMode.AbilityTargeting);
        stemLine.enabled = false;
    }

    Vector2 GetMouseWorld()
    {
        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(cam.transform.position.z);
        return cam.ScreenToWorldPoint(mp);
    }

    bool Hit(Vector2 world)
    {
        Vector2 local = world - (Vector2)center.position;
        float d = local.magnitude;
        if (d < baseRadius - 0.2f || d > baseRadius + maxPull)
            return false;

        float ang = Mathf.Atan2(local.y, local.x) * Mathf.Rad2Deg;
        ang = (ang + 360f) % 360f;
        return ang >= a0 && ang <= a1;
    }

    void DrawArc(float radius)
    {
        line.positionCount = ARC_POINTS;
        for (int i = 0; i < ARC_POINTS; i++)
        {
            float t = i / (ARC_POINTS - 1f);
            float a = Mathf.Lerp(a0, a1, t) * Mathf.Deg2Rad;
            Vector3 p = new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * radius;
            line.SetPosition(i, p);
        }
    }

    void UpdateCollider(float innerRadius, float outerRadius, float padding = 0.2f)
    {
        innerRadius = Mathf.Max(innerRadius - padding * 3, 0);
        int pointsCount = ARC_POINTS * 2;
        Vector2[] points = new Vector2[pointsCount];

        for (int i = 0; i < ARC_POINTS; i++)
        {
            float t = i / (ARC_POINTS - 1f);
            float a = Mathf.Lerp(a0, a1, t) * Mathf.Deg2Rad;
            points[i] = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * (outerRadius + padding);
        }

        for (int i = 0; i < ARC_POINTS; i++)
        {
            float t = i / (ARC_POINTS - 1f);
            float a = Mathf.Lerp(a0, a1, 1 - t) * Mathf.Deg2Rad;
            points[ARC_POINTS + i] = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * innerRadius;
        }

        polyCollider.pathCount = 1;
        polyCollider.SetPath(0, points);
    }
}
