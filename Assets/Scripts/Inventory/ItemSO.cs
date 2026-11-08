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

    [Min(0)] public int Weight;
    [Min(1)] public int StackSize = 1;
    public List<BreakdownComponent> BreakdownComponents;
}

[Serializable]
public class BreakdownComponent
{
    [SerializeField] private ItemSO _item;
    [SerializeField, Min(1)] private int _amount;

    public ItemSO Item => _item;
    public int Amount => _amount;
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