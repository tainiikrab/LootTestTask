using UnityEngine;

namespace LootTestTask
{
public class InputManager : MonoBehaviour, IInputManager
{
    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }

    public Vector3 GetMovement()
    {
        var vec2 = _inputActions.Player.Move.ReadValue<Vector2>();
        return new Vector3(vec2.x, 0, vec2.y);
    }

    public bool IsJumping()
    {
        return _inputActions.Player.Jump.triggered;
    }
}

public interface IInputManager
{
    Vector3 GetMovement();
    bool IsJumping();
}
}