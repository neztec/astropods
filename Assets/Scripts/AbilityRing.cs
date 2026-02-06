using UnityEngine;

public class AbilityRing : MonoBehaviour
{
    public Transform ship;

    public float baseRadius = 3.5f;
    public float maxPull = 6.0f;

    private float rotationOffset = 0f;

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
        transform.Rotate(0, 0, rotationOffset);
    }

    public void AimSegment(Vector2 direction, float segmentMidAngle)
    {
        float targetAngle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rotationOffset = targetAngle - segmentMidAngle;
    }


    public float GetRotationOffset()
    {
        return rotationOffset;
    }

}
