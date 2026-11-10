using System;
using UnityEngine;

namespace LootTestTask.Inventory
{
[Serializable]
public class InventorySlot
{
    public InventorySlot(ItemSO item, int amount = 1)
    {
        SetItem(item, amount);
    }

    [field: SerializeField] public ItemSO Item { get; private set; }

    [field: SerializeField] public int Amount { get; private set; }

    public void SetItem(ItemSO item, int amount = 1)
    {
        Item = item;
        Amount = amount;
    }

    public bool TryChangeAmount(int delta)
    {
        if (delta == 0) return false;

        if (Amount + delta > 0 && Amount + delta <= Item.MaxStack)
        {
            Amount += delta;
            return true;
        }

        return false;
    }

    public void Clear()
    {
        Item = null;
        Amount = 0;
    }
}
}