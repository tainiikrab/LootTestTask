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

    public int TryAddItems(ItemSO item, int amount = 1)
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
        if (amountToAdd == 0) return 0;

        int remainingAmountToAdd = amountToAdd;
        foreach (InventorySlot slot in Slots)
        {
            if (slot.Item != item) continue;

            int freeSpace = slot.Item.StackSize - slot.Amount;
            if (freeSpace <= 0) continue;

            int addedAmount = Mathf.Min(remainingAmountToAdd, freeSpace);
            if (!slot.TryChangeAmount(addedAmount)) continue;

            remainingAmountToAdd -= addedAmount;
            if (remainingAmountToAdd == 0) return amountToAdd;
        }

        while (remainingAmountToAdd > 0)
        {
            int addedAmount = Mathf.Min(remainingAmountToAdd, item.StackSize);
            remainingAmountToAdd -= addedAmount;
            Slots.Add(new InventorySlot(item, addedAmount));
        }

        return amountToAdd;
    }

    public int GetFreeWeight()
    {
        return MaxWeight - CurrentWeight;
    }

    private int ClampAmountByWeight(ItemSO item, int amount)
    {
        if (item.Weight == 0) return amount;

        int free = GetFreeWeight();
        if (free <= 0) return 0;
        return Mathf.Min(amount, free / item.Weight);
    }
}
}