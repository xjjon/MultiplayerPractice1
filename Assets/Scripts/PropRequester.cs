using Unity.Netcode;
using UnityEngine;

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
            GameObject propInstance = Instantiate(propPrefab, spawnPos, Quaternion.identity);
            NetworkObject networkObject = propInstance.GetComponent<NetworkObject>();
            networkObject.Spawn(true);
        }
    }
}