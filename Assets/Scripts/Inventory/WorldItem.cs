using UnityEngine;

namespace LootTestTask.Inventory
{
public class WorldItem : MonoBehaviour
{
    [field: SerializeField] public ItemSO ItemSO { get; private set; }
    [field: SerializeField, Min(1)] public int Amount { get; private set; }

    public void SetItem(ItemSO item, int amount)
    {
        (ItemSO, Amount) = (item, amount);
    }

    public void SetAmount(int amount)
    {
        Amount = amount;
    }
}
}