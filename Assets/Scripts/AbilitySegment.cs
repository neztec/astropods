using UnityEngine;
using UniRx;

public enum AbilityInputState
{
    Idle,
    Targeting   // click-drag in progress
}


public class AbilitySegment : MonoBehaviour
{
    public readonly Subject<AbilityFireEvent> FireStream
        = new Subject<AbilityFireEvent>();

    public readonly ReactiveProperty<AbilityInputState> InputState
    = new ReactiveProperty<AbilityInputState>(AbilityInputState.Idle);

    Transform center;
    float baseRadius;
    float maxPull;

    string id;
    float a0, a1;

    LineRenderer line;
    Camera cam;

    bool dragging;
    Vector2 currentDir;
    float currentPull;

    const int ARC_POINTS = 64;

    public void Init(
        Transform ship,
        float radius,
        float pull,
        AbilityRing.Slot slot)
    {
        center = ship;
        baseRadius = radius;
        maxPull = pull;

        id = slot.id;
        a0 = slot.startAngle;
        a1 = slot.endAngle;

        cam = Camera.main;

        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = 1.12f;
        line.endWidth = 1.12f;
        line.useWorldSpace = false;
        line.startColor = slot.color;
        line.endColor = slot.color;
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


        if (!dragging)
        {
            DrawArc(baseRadius);
            return;
        }

        Vector2 raw = mouse - (Vector2)center.position;
        currentPull = Mathf.Clamp(raw.magnitude, 0, maxPull);
        currentDir = raw.normalized;

        DrawArc(baseRadius + currentPull);

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
        }

    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    void OnDisable()
    {
        InputState.Value = AbilityInputState.Idle;
        InputRouter.Instance?.Release(InputMode.AbilityTargeting);
    }



    // ---------- Helpers ----------

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
}
