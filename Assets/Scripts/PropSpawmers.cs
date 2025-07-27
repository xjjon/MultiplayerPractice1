using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PropSpawner : NetworkBehaviour
{
    // Assign your prop prefab in the Unity Inspector
    public GameObject propPrefab;

    void Start()
    { 
        StartCoroutine(SpawnPropsRoutine());
    }

    public IEnumerator SpawnPropsRoutine()
    {
        // Wait for the server to be ready
        while (!IsServer)
        {
            Debug.Log("Waiting for server to be ready...");
            yield return null; // Wait for the next frame
        }

        while (true)
        {
            // Calculate a random spawn position
            Vector3 spawnPos = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));

            // Call the ServerRpc to ask the server to perform the spawn
            SpawnProp(spawnPos);

            yield return new WaitForSeconds(3f);
        }
    }

    // [ServerRpc(RequireOwnership = false)] // RequireOwnership=false allows any client to call this
    private void SpawnProp(Vector3 position)
    {
        // --- THIS CODE IS EXECUTED ON THE SERVER ---

        // 1. Instantiate the prefab
        GameObject propInstance = Instantiate(propPrefab, position, Quaternion.identity);

        // 2. Get the NetworkObject and spawn it
        NetworkObject networkObject = propInstance.GetComponent<NetworkObject>();
        networkObject.Spawn(true); // 'true' makes it destroy with the scene
    }
}