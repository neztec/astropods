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
            seg.Init(ship, baseRadius, maxPull, slot);
        }
    }

    void LateUpdate()
    {
        if (ship)
            transform.position = ship.position;
    }
}
