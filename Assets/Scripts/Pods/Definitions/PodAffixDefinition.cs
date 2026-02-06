using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pods/Affix Definition")]
public class PodAffixDefinition : ScriptableObject
{
    public string affixName;
    public AffixRarity rarity;

    public List<PodEffectDefinition> effects;
}