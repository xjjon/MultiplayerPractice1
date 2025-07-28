using Unity.Netcode;
using UnityEngine;

namespace MultiplayerPractice1.Assets.Scripts
{
    public class ClientRoot : NetworkBehaviour
    {
        public GameObject _playerPrefab;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                SpawnPropServerRpc();
            }
        }

        [ServerRpc]
        private void SpawnPropServerRpc(ServerRpcParams rpcParams = default)
        {
            GameObject propInstance = Instantiate(_playerPrefab);
            propInstance.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        }
    }
}