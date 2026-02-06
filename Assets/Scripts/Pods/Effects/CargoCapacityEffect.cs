using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pods/Effects/Cargo Capacity")]
public class CargoCapacityEffect : PodEffectDefinition
{
    public int extraCapacity;

    public override void Apply(ShipCore ship, PodInstance pod)
        => ship.cargoCapacity += extraCapacity;


    public override void Remove(ShipCore ship, PodInstance pod)
        => ship.cargoCapacity -= extraCapacity;
}
