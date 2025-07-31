using Unity.Netcode;
using UnityEngine;
using Unity.BossRoom.Infrastructure;

namespace MultiplayerPractice1.Assets.Scripts
{
    public class PropRequester : NetworkBehaviour
    {
        public GameObject propPrefab;


        private void Update()
        {
            if (IsOwner && Input.GetKeyDown(KeyCode.R))
            {
                RequestPropSpawnServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestPropSpawnServerRpc()
        {
            Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(-5f, 5f), 5f, UnityEngine.Random.Range(-5f, 5f));
            
            // Use the NetworkObjectPool instead of direct instantiation
            if (NetworkObjectPool.Singleton != null)
            {
                NetworkObject networkObject = NetworkObjectPool.Singleton.GetNetworkObject(propPrefab, spawnPos, Quaternion.identity);
                networkObject.SpawnWithOwnership(OwnerClientId, true);
            }
            else
            {
                Debug.LogError("NetworkObjectPool.Singleton is null! Make sure NetworkObjectPool is in the scene and spawned.");
                // Fallback to direct instantiation if pool is not available
                GameObject propInstance = Instantiate(propPrefab, spawnPos, Quaternion.identity);
                NetworkObject networkObject = propInstance.GetComponent<NetworkObject>();
                networkObject.SpawnWithOwnership(OwnerClientId, true);
            }
        }
    }
}