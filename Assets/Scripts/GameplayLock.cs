using LootTestTask.Inventory;
using UnityEngine;

namespace LootTestTask
{
public class GameplayLock : MonoBehaviour
{
    [SerializeField] private Behaviour _lookInput;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private ItemCollector _itemCollector;
    [SerializeField] private CursorVisibilityToggle _cursor;

    public void SetLocked(bool locked)
    {
        _lookInput.enabled = !locked;
        _inputManager.TogglePlayerInput(!locked);
        _itemCollector.enabled = !locked;
        _cursor.SetGameplayLocked(locked);
    }
}
}