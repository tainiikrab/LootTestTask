using UnityEngine;

namespace LootTestTask
{
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IForwardProvider
{
    private CharacterController controller;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    public Vector3 Forward { get; private set; } = Vector3.forward;

    public Vector3 VectorTo(Transform other) => other.position - cameraTransform.position;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 inputMovement = inputManager.GetMovement();
        
        Vector3 cameraForward = cameraTransform.forward;
        Forward = cameraForward;
        
        cameraForward.y = 0f;
        cameraForward.Normalize();
        
        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();
        
        Vector3 movement = (cameraForward * inputMovement.z + cameraRight * inputMovement.x).normalized;

        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        controller.SimpleMove(speed * movement);
    }
}

public interface IForwardProvider
{
    Vector3 Forward { get; }
    Vector3 VectorTo(Transform other);
}
}