using System.Collections;
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
                StartCoroutine(SetupPlayerDelayed());
            }

            _dataManager = FindAnyObjectByType<DataManager>();
        }

        private IEnumerator SetupPlayerDelayed()
        {
            // Wait a frame to ensure all NetworkObjects are properly spawned
            yield return null;
            
            var playerCatalog = FindAnyObjectByType<PlayerCatalog>();
            if (playerCatalog != null && playerCatalog.IsSpawned)
            {
                playerCatalog.SetupPlayerServerRpc();
            }
            else
            {
                Debug.LogError("PlayerCatalog not found or not spawned in scene!");
            }
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