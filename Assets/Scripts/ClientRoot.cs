using Unity.Netcode;
using UnityEngine;

namespace MultiplayerPractice1.Assets.Scripts
{
    public class ClientRoot : NetworkBehaviour
    {
        public GameObject _playerPrefab;

        private DataManager _dataManager;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                SpawnPropServerRpc();
            }

            _dataManager = FindAnyObjectByType<DataManager>();
        }

        [ServerRpc]
        private void SpawnPropServerRpc(ServerRpcParams rpcParams = default)
        {
            GameObject propInstance = Instantiate(_playerPrefab);
            propInstance.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        }

        void Update()
        {
            if (!IsSpawned) return;
            if (IsOwner && Input.anyKeyDown)
            {
                string keyPressed = Input.inputString;
                if (!string.IsNullOrEmpty(keyPressed))
                {
                    _dataManager.UpdateInputServerRpc(keyPressed);
                }
            }
        }
    }
}