using UnityEngine;

public static class ArcUtil
{
    public static void DrawArc(
        LineRenderer lr,
        float radius,
        float startDeg,
        float endDeg,
        int steps = 32)
    {
        lr.positionCount = steps;

        for (int i = 0; i < steps; i++)
        {
            float t = i / (steps - 1f);
            float angle =
                Mathf.Lerp(startDeg, endDeg, t) * Mathf.Deg2Rad;

            lr.SetPosition(i, new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            ));
        }
    }
}
