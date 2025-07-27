using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public CharacterController controller;

    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public float inputX;
    public float inputZ;
    public Vector3 moveDirection;

    void Update()
    {
        if (IsOwner)
        {
            inputX = Input.GetAxis("Horizontal");
            inputZ = Input.GetAxis("Vertical");
        }

        controller.Move(moveDirection);
    }
    
    [Rpc(SendTo.Server)]
    private void UpdateInputServerRpc(Vector3 moveInput)
    {
        moveDirection = moveInput;
    }

    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        UpdateInputServerRpc(
            new Vector3(inputX, 0, inputZ) * speed * Time.deltaTime
        );
    }
}
