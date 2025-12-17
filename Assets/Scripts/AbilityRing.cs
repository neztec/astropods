using UnityEngine;

public class AbilityRing : MonoBehaviour
{
    public Transform ship;

    public float baseRadius = 3.5f;
    public float maxPull = 2.0f;

    [System.Serializable]
    public struct Slot
    {
        public string id;
        public float startAngle;
        public float endAngle;
        public Color color;
    }

    public Slot[] slots;

    void Start()
    {
        foreach (var slot in slots)
        {
            GameObject go = new GameObject($"Ability_{slot.id}");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;

            var seg = go.AddComponent<AbilitySegment>();
            seg.Init(ship, baseRadius, maxPull, slot, this);
        }
    }

    void LateUpdate()
    {
        if (ship)
            transform.position = ship.position;

        // ring should stay upright in world space and only rotate when AimSegment is called
        transform.rotation = Quaternion.identity;

    }

    public void AimSegment(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
