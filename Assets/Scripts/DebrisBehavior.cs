using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class DebrisBehavior : MonoBehaviour
{
    public float lifetime = 4f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
