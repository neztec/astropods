using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PodInstance
{
    public PodDefinition basePod;
    public List<PodAffixDefinition> affixes = new();

    // Unique / persistent state
    public int seed;
    public int upgradeLevel;
    public float durability;

    public string DisplayName =>
        affixes.Count == 0
            ? basePod.baseName
            : $"{affixes[0].affixName} {basePod.baseName}";

    public IEnumerable<PodEffectDefinition> GetAllEffects()
    {
        foreach (var e in basePod.baseEffects)
            yield return e;

        foreach (var affix in affixes)
            foreach (var e in affix.effects)
                yield return e;
    }
}