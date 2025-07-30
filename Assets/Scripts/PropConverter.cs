using MultiplayerPractice1.Assets.Scripts;
using Unity.Netcode;
using UnityEngine;

public class PropConverter : NetworkBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Only process collisions on the server since that's where movement is authoritative
        if (!IsServer)
        {
            return;
        }

        if (hit.gameObject.TryGetComponent(out OwnableCube cube))
        {
            NetworkObject cubeNetworkObject = cube.GetComponent<NetworkObject>();
            cubeNetworkObject.ChangeOwnership(OwnerClientId);
        }
    }
}