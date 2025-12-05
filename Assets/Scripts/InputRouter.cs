using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;

public enum InputMode
{
    None,
    AbilityTargeting,
    ShipThrusting,
    CameraPanning
}

public class InputRouter : MonoBehaviour
{
    public static InputRouter Instance { get; private set; }

    private readonly ReactiveProperty<InputMode> currentMode =
        new ReactiveProperty<InputMode>(InputMode.None);

    public IObservable<InputMode> ModeStream =>
        currentMode.DistinctUntilChanged();

    void Awake()
    {
        Instance = this;
    }

    // --- Claims ---
    public bool TryClaim(InputMode mode)
    {
        if (currentMode.Value != InputMode.None)
            return false;

        currentMode.Value = mode;
        return true;
    }

    public void Release(InputMode mode)
    {
        if (currentMode.Value == mode)
            currentMode.Value = InputMode.None;
    }

    public bool IsActive(InputMode mode)
        => currentMode.Value == mode;
}
