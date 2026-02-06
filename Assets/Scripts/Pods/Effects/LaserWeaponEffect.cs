using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeaponEffect : PodEffectDefinition
{
    public float damage;
    public float range;

    public override void Apply(ShipCore ship, PodInstance pod)
    {
        // Implement laser weapon effect application logic
    }

    public override void Remove(ShipCore ship, PodInstance pod)
    {

    }

}
