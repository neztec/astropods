using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PodEffectDefinition : ScriptableObject
{
    public abstract void Apply(ShipCore ship, PodInstance pod);
    public abstract void Remove(ShipCore ship, PodInstance pod);
}