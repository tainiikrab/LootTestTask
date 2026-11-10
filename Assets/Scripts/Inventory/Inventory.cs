using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LootTestTask.Inventory
{
public class Inventory : MonoBehaviour
{
    [field: SerializeField, Min(0)] public int MaxWeight { get; private set; } = 10;

    [field: SerializeField] public List<InventorySlot> Slots { get; private set; } = new();

    public int CurrentWeight => Slots.Sum(slot => slot.Item.Weight * slot.Amount);

    public event Action InventoryChangedEvent;
    public event Action OverencumberedEvent;

    public int TryAddItems(ItemSO item, int amount = 1)
    {
        return TryAddItemsInternal(item, amount, true);
    }

    public bool TryRemoveFromSlot(InventorySlot slot, int amount = 1)
    {
        return TryRemoveFromSlotInternal(slot, amount, true);
    }

    public bool TryBreakDown(InventorySlot slot)
    {
        if (slot == null || !Slots.Contains(slot) || slot.Item == null || slot.Amount < 1)
            return false;

        ItemSO item = slot.Item;
        if (!item.HasBreakdownComponents)
            return false;

        int freeWeight = GetFreeWeight() + item.Weight;
        foreach (BreakdownComponent component in item.BreakdownComponents)
        {
            if (!BreakdownComponent.IsValid(component)) continue;

            int amountFits = ClampAmountByWeight(component.Item, component.Amount, freeWeight);
            if (amountFits < component.Amount)
            {
                OverencumberedEvent?.Invoke();
                return false;
            }

            freeWeight -= component.Item.Weight * component.Amount;
        }

        if (!TryRemoveFromSlotInternal(slot, 1, false))
            return false;

        foreach (BreakdownComponent component in item.BreakdownComponents)
        {
            if (!BreakdownComponent.IsValid(component)) continue;

            int added = TryAddItemsInternal(component.Item, component.Amount, false);
            if (added != component.Amount)
            {
                Debug.LogError(
                    $"TryBreakDown: expected to add {component.Amount} of '{component.Item.name}', added {added}");
            }
        }

        InventoryChangedEvent?.Invoke();
        return true;
    }

    public int GetFreeWeight()
    {
        return MaxWeight - CurrentWeight;
    }

    public bool CanAddAny(ItemSO item)
    {
        if (item == null) return false;
        return ClampAmountByWeight(item, 1) > 0;
    }

    private int TryAddItemsInternal(ItemSO item, int amount, bool notify)
    {
        if (item == null)
        {
            Debug.LogError("Item is null");
            return 0;
        }

        if (amount < 1)
        {
            Debug.LogError("Amount is less than 1");
            return 0;
        }

        int amountToAdd = ClampAmountByWeight(item, amount);
        if (amountToAdd == 0)
        {
            if (notify)
            {
                OverencumberedEvent?.Invoke();
            }

            return 0;
        }

        int remainingAmountToAdd = amountToAdd;
        foreach (InventorySlot slot in Slots)
        {
            if (slot.Item != item) continue;

            int freeSpace = slot.Item.MaxStack - slot.Amount;
            if (freeSpace <= 0) continue;

            int addedAmount = Mathf.Min(remainingAmountToAdd, freeSpace);
            if (!slot.TryChangeAmount(addedAmount)) continue;

            remainingAmountToAdd -= addedAmount;
            if (remainingAmountToAdd == 0)
            {
                if (notify) InventoryChangedEvent?.Invoke();
                return amountToAdd;
            }
        }

        while (remainingAmountToAdd > 0)
        {
            int addedAmount = Mathf.Min(remainingAmountToAdd, item.MaxStack);
            remainingAmountToAdd -= addedAmount;
            Slots.Add(new InventorySlot(item, addedAmount));
        }

        if (notify) InventoryChangedEvent?.Invoke();
        return amountToAdd;
    }

    private bool TryRemoveFromSlotInternal(InventorySlot slot, int amount, bool notify)
    {
        if (slot == null || slot.Item == null || amount < 1 || slot.Amount < amount)
            return false;

        if (!Slots.Contains(slot))
            return false;

        if (slot.Amount == amount)
        {
            Slots.Remove(slot);
        }
        else if (!slot.TryChangeAmount(-amount))
        {
            return false;
        }

        if (notify) InventoryChangedEvent?.Invoke();
        return true;
    }

    private int ClampAmountByWeight(ItemSO item, int amount)
    {
        return ClampAmountByWeight(item, amount, GetFreeWeight());
    }

    private static int ClampAmountByWeight(ItemSO item, int amount, int freeWeight)
    {
        if (item.Weight == 0) return amount;
        if (freeWeight <= 0) return 0;
        return Mathf.Min(amount, freeWeight / item.Weight);
    }
}
}
