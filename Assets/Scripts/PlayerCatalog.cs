using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MultiplayerPractice1.Assets.Scripts
{
    public class PlayerCatalog : NetworkBehaviour
    {
        [SerializeField]
        private List<NetworkObject> _playerObjects = new List<NetworkObject>();

        [ServerRpc(RequireOwnership = false)]
        public void SetupPlayerServerRpc(ServerRpcParams rpcParams = default)
        {
            // instantiate random player object for the client
            NetworkObject playerObject = Instantiate(_playerObjects[Random.Range(0, _playerObjects.Count)]);
            playerObject.gameObject.name = rpcParams.Receive.SenderClientId.ToString();
            playerObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        }
    }
}