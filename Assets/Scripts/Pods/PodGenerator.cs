using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodGenerator : MonoBehaviour
{
    public List<PodAffixDefinition> possibleAffixes;

    public PodInstance Generate(
        PodDefinition basePod,
        AffixRarity rarity
    )
    {
        var pod = new PodInstance
        {
            basePod = basePod,
            seed = Random.Range(int.MinValue, int.MaxValue)
        };

        int affixCount = rarity switch
        {
            AffixRarity.Common => 0,
            AffixRarity.Rare => 1,
            AffixRarity.Epic => 2,
            AffixRarity.Legendary => 3,
            _ => 0
        };

        for (int i = 0; i < affixCount; i++)
            pod.affixes.Add(
                possibleAffixes[Random.Range(0, possibleAffixes.Count)]
            );

        return pod;
    }
}