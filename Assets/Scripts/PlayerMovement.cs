using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public CharacterController controller;

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

            UpdateMovementServerRpc(new Vector3(inputX, 0, inputZ));
        }

        if (IsServer)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        moveDirection = new Vector3(inputX, 0, inputZ) * speed * Time.deltaTime;

        controller.Move(moveDirection);
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateMovementServerRpc(Vector3 inputDirection)
    {
        // The server receives the input and updates its state for this player.
        // These values will be used in the server's Update loop to move the character.
        inputX = inputDirection.x;
        inputZ = inputDirection.z;
    }
}
