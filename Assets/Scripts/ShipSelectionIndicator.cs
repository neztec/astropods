using UnityEngine;
using UniRx;

[RequireComponent(typeof(SpriteRenderer))]
public class ShipSelectionIndicator : MonoBehaviour
{
    [SerializeField] private ShipCore shipCore;

    private SpriteRenderer spriteRenderer;
    private readonly CompositeDisposable disposables = new();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (shipCore == null)
            shipCore = GetComponentInParent<ShipCore>();

        // Hide indicator by default
        spriteRenderer.enabled = false;
    }

    private void Start()
    {
        if (shipCore == null)
        {
            Debug.LogError("ShipSelectionIndicator: Missing ShipCore reference!");
            return;
        }

        shipCore.OnStateChanged()
            .Subscribe(state =>
            {
                spriteRenderer.enabled = state.Selected;
            })
            .AddTo(disposables);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
