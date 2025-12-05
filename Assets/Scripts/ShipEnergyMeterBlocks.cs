using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class ShipEnergyMeterBlocks : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShipCore shipCore;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject emptyBlockPrefab;

    [Header("Visual Settings")]
    [SerializeField] private int totalBlocks = 10;
    [SerializeField] private float spacing = 0.15f;
    [SerializeField] private bool vertical = true;
    [SerializeField] private Vector3 baseOffset = new(0.5f, 0.5f, 0);

    private readonly List<GameObject> filledBlocks = new();
    private readonly List<GameObject> emptyBlocks = new();
    private readonly CompositeDisposable disposables = new();

    private Quaternion initialRotation;
    private Vector3 localOffset;
    private Transform shipTransform;

    private void Start()
    {
        if (shipCore == null)
            shipCore = GetComponentInParent<ShipCore>();

        if (blockPrefab == null || emptyBlockPrefab == null)
        {
            Debug.LogError("ShipEnergyMeterBlocks: Missing block prefab references!");
            return;
        }

        shipTransform = shipCore.transform;
        localOffset = transform.position - shipTransform.position;
        initialRotation = transform.rotation;

        // Create visual blocks (empty behind, filled in front)
        for (int i = 0; i < totalBlocks; i++)
        {
            Vector3 offset = vertical
                ? new Vector3(0, i * spacing, 0)
                : new Vector3(i * spacing, 0, 0);
            Vector3 pos = baseOffset + offset;

            var empty = Instantiate(emptyBlockPrefab, transform);
            empty.transform.localPosition = pos;
            emptyBlocks.Add(empty);

            var filled = Instantiate(blockPrefab, transform);
            filled.transform.localPosition = pos;
            filledBlocks.Add(filled);
        }

        // Subscribe to energy updates
        shipCore.OnStateChanged()
            .Subscribe(UpdateBlocks)
            .AddTo(disposables);
    }

    private void LateUpdate()
    {
        // Keep the meter upright and position-locked relative to ship
        transform.rotation = Quaternion.identity;
        transform.position = shipTransform.position + localOffset;
    }

    private void UpdateBlocks(ShipState state)
    {
        int activeBlocks = Mathf.RoundToInt(state.Energy / state.MaxEnergy * totalBlocks);

        // change alpha of filled blocks
        for (int i = 0; i < filledBlocks.Count; i++)
        {
            var sr = filledBlocks[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = sr.color;
                color.a = (i < activeBlocks) ? 1f : 0.2f;
                sr.color = color;
            }
        }


        // for (int i = 0; i < filledBlocks.Count; i++)
        //     filledBlocks[i].SetActive(i < activeBlocks);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
