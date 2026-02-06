using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodSlot : MonoBehaviour
{
    public PodCategory slotCategory;
    public PodInstance installedPod;

    public bool CanInstall(PodDefinition pod)
        => pod.allowedSlots.Contains(slotCategory);

    public void Install(PodInstance pod, ShipCore ship)
    {
        installedPod = pod;
        foreach (var effect in pod.GetAllEffects())
            effect.Apply(ship, pod);
    }

    public void Remove(ShipCore ship)
    {
        if (installedPod == null) return;

        foreach (var effect in installedPod.GetAllEffects())
            effect.Remove(ship, installedPod);

        installedPod = null;
    }
}
