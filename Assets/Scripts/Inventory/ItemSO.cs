using System;
using System.Collections.Generic;
using UnityEngine;

namespace LootTestTask.Inventory
{
[CreateAssetMenu(fileName = "Item", menuName = "LootTestTask/Item")]
public class ItemSO : ScriptableObject
{
    public ItemCategory Category;
    public PickUpType PickUpType;

    public string Name;
    [Min(0)] public int Weight;
    [Min(1)] public int MaxStack = 1;
    public List<BreakdownComponent> BreakdownComponents;

    public bool HasBreakdownComponents
    {
        get
        {
            if (BreakdownComponents == null) return false;

            foreach (BreakdownComponent component in BreakdownComponents)
            {
                if (BreakdownComponent.IsValid(component))
                    return true;
            }

            return false;
        }
    }
}

[Serializable]
public class BreakdownComponent
{
    [SerializeField] private ItemSO _item;
    [SerializeField, Min(1)] private int _amount;

    public ItemSO Item => _item;
    public int Amount => _amount;

    public static bool IsValid(BreakdownComponent component)
    {
        return component != null && component.Item != null && component.Amount > 0;
    }
}

public enum ItemCategory
{
    Consumable,
    RepairResource,
    Junk,
    Weapon
}

public enum PickUpType
{
    Automatic,
    Manual
}
}