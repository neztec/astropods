using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PodCategory
{
    Weapon,
    Utility,
    Cargo,
    Defense
}

public enum AffixRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(menuName = "Pods/Pod Definition")]
public class PodDefinition : ScriptableObject
{
    public bool isEnabled;
    public string baseName;  // e.g., "Missile Pod", "Turret Pod"
    public Sprite icon;
    public Color iconColor;
    public PodCategory category;
    public List<PodCategory> allowedSlots;
    public List<PodEffectDefinition> baseEffects;
}

// LaunchMissiles,
// TractorBeam,
// ScanRegion,
// DeployShield,
// EMPBlast,
// Teleport