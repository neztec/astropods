using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public static class ShipConstants
{
    public const float MaxEnergy = 100f;
    public const float EnergyRegenRate = 15f; // per second
}

public struct ShipState
{
    public bool Selected;
    public float Energy;
    public float MaxEnergy;
    public float EnergyRegenRate;
    public Vector2 Position;

    public bool AbilityInputActive;
}

public class ShipCore : MonoBehaviour
{
    public AbilityRing ring;
    private ShipState shipState;
    public readonly Subject<ShipState> ShipStateStream = new Subject<ShipState>();

    CompositeDisposable abilityDisposables = new CompositeDisposable();


    private void Start()
    {
        ring = GetComponentInChildren<AbilityRing>();

        shipState = new ShipState
        {
            Selected = false,
            AbilityInputActive = false,
            Energy = ShipConstants.MaxEnergy,
            MaxEnergy = ShipConstants.MaxEnergy,
            EnergyRegenRate = ShipConstants.EnergyRegenRate,
            Position = transform.position
        };

        // subscribe to selected state changes from stream
        ShipStateStream.Subscribe(state =>
        {
            if (ring != null)
                ring.gameObject.SetActive(state.Selected);
        });

        EmitState();

        foreach (var ability in GetComponentsInChildren<AbilitySegment>())
        {
            ability.InputState
                .Select(s => s == AbilityInputState.Targeting)
                .DistinctUntilChanged()
                .Subscribe(active =>
                {
                    shipState.AbilityInputActive = active;
                    EmitState();
                })
                .AddTo(this);
        }

    }

    private void Update()
    {
        if (shipState.Energy < shipState.MaxEnergy)
        {
            shipState.Energy += shipState.EnergyRegenRate * Time.deltaTime;
            shipState.Energy = Mathf.Min(shipState.Energy, shipState.MaxEnergy);

            shipState.Position = transform.position;

            EmitState();
        }
    }

    private void OnDestroy()
    {
        ShipStateStream?.OnCompleted();
    }

    public bool TryUseEnergy(float amount)
    {
        if (shipState.Energy < amount)
            return false;

        shipState.Energy -= amount;
        EmitState();
        return true;
    }

    public void SetSelected(bool selected)
    {
        shipState.Selected = selected;
        EmitState();
    }

    public void EmitState()
    {
        ShipStateStream.OnNext(shipState);
    }

    public IObservable<ShipState> OnStateChanged()
    {
        return ShipStateStream.AsObservable();
    }
}
