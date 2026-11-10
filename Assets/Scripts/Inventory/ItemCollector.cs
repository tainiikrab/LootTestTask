using System.Collections.Generic;
using UnityEngine;

namespace LootTestTask.Inventory
{
[RequireComponent(typeof(Collider))]
public class ItemCollector : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private float _activeRadius;
    [SerializeField] private float _lookConeAngleDeg = 45;

    [SerializeField] private float _updateCooldown = 0.1f;
    private float _minLookDot;

    private float _updateTimer;

    private readonly List<WorldItem> _closeItems = new();
    public IReadOnlyList<WorldItem> CloseItems => _closeItems;

    private readonly Dictionary<WorldItem, int> _blockedAutoPickups = new();

    private IForwardProvider _forwardProvider;

    public WorldItem BestToPickUp { get; private set; }
    private float _bestDot;

    public bool CanAccept(WorldItem worldItem)
    {
        return worldItem != null && _inventory.CanAddAny(worldItem.ItemSO);
    }

    private void Awake()
    {
        _forwardProvider = GetComponentInParent<IForwardProvider>();
        _minLookDot = Mathf.Cos(_lookConeAngleDeg * Mathf.Deg2Rad);
        _bestDot = _minLookDot;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out WorldItem worldItem))
        {
            return;
        }

        if (_closeItems.Contains(worldItem)) return;
        _closeItems.Add(worldItem);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out WorldItem worldItem))
        {
            return;
        }

        _closeItems.Remove(worldItem);
        ForgetAutoPickupBlock(worldItem);

        if (worldItem == BestToPickUp)
        {
            BestToPickUp = null;
            _bestDot = _minLookDot;
        }
    }

    private void Update()
    {
        TryPickUpBest();

        if (!TickScanCooldown()) return;

        _bestDot = _minLookDot;
        BestToPickUp = null;

        ScanCloseItems();
    }

    private void TryPickUpBest()
    {
        if (CanPickUp(BestToPickUp) && _inputManager.IsPickUpTriggered())
        {
            if (TryPickUpItem(BestToPickUp))
            {
                BestToPickUp = null;
                _bestDot = _minLookDot;
            }
        }
    }

    private bool TickScanCooldown()
    {
        _updateTimer += Time.deltaTime;
        if (_updateTimer < _updateCooldown) return false;
        _updateTimer = 0;
        return true;
    }

    private void ScanCloseItems()
    {
        for (int i = _closeItems.Count - 1; i >= 0; i--)
        {
            WorldItem worldItem = _closeItems[i];

            if (worldItem == null)
            {
                ForgetAutoPickupBlock(worldItem);
                _closeItems.RemoveAt(i);
                continue;
            }

            if (worldItem.ItemSO == null || worldItem.Amount <= 0)
            {
                ForgetAutoPickupBlock(worldItem);
                _closeItems.RemoveAt(i);
                Destroy(worldItem.gameObject);
                continue;
            }

            if (!IsInActiveRadius(worldItem))
            {
                ForgetAutoPickupBlock(worldItem);
                continue;
            }

            if (worldItem.ItemSO.PickUpType == PickUpType.Automatic)
            {
                TryAutoPickUpItem(worldItem);
                continue;
            }

            if (!TryGetLookDot(worldItem, out float dot)) continue;
            if (dot > _bestDot)
            {
                BestToPickUp = worldItem;
                _bestDot = dot;
            }
        }
    }

    private void TryAutoPickUpItem(WorldItem worldItem)
    {
        if (IsAutoPickupBlocked(worldItem)) return;

        if (TryPickUpItem(worldItem, out int taken)) return;
        if (taken > 0) return;

        _blockedAutoPickups[worldItem] = _inventory.GetFreeWeight();
    }

    private bool IsAutoPickupBlocked(WorldItem worldItem)
    {
        if (!_blockedAutoPickups.TryGetValue(worldItem, out int freeWeightWhenBlocked))
        {
            return false;
        }

        if (_inventory.GetFreeWeight() != freeWeightWhenBlocked)
        {
            ForgetAutoPickupBlock(worldItem);
            return false;
        }

        return true;
    }

    private void ForgetAutoPickupBlock(WorldItem worldItem)
    {
        _blockedAutoPickups.Remove(worldItem);
    }

    private bool TryPickUpItem(WorldItem worldItem)
    {
        return TryPickUpItem(worldItem, out _);
    }

    private bool TryPickUpItem(WorldItem worldItem, out int taken)
    {
        taken = _inventory.TryAddItems(worldItem.ItemSO, worldItem.Amount);
        if (taken <= 0) return false;

        if (taken == worldItem.Amount)
        {
            ForgetAutoPickupBlock(worldItem);
            _closeItems.Remove(worldItem);
            Destroy(worldItem.gameObject);
            return true;
        }

        worldItem.SetAmount(worldItem.Amount - taken);
        return false;
    }

    private bool CanPickUp(WorldItem worldItem)
    {
        if (worldItem == null) return false;
        if (!_closeItems.Contains(worldItem)) return false;
        if (worldItem.ItemSO == null || worldItem.Amount <= 0) return false;
        if (!IsInActiveRadius(worldItem)) return false;
        if (!TryGetLookDot(worldItem, out float dot)) return false;
        return dot > _minLookDot;
    }

    private bool IsInActiveRadius(WorldItem worldItem)
    {
        return Vector3.SqrMagnitude(worldItem.transform.position - transform.position)
               <= _activeRadius * _activeRadius;
    }

    private bool TryGetLookDot(WorldItem worldItem, out float dot)
    {
        Vector3 toItem = _forwardProvider.VectorTo(worldItem.transform);
        if (toItem == Vector3.zero)
        {
            dot = 0f;
            return false;
        }

        dot = Vector3.Dot(_forwardProvider.Forward, toItem.normalized);
        return true;
    }
}
}