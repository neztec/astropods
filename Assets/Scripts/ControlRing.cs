using UnityEngine;

public class ControlRing : MonoBehaviour
{
    void LateUpdate()
    {
        // Keep this object upright in world space
        transform.rotation = Quaternion.identity;
    }
}
