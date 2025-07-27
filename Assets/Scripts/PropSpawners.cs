using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PropSpawner : NetworkBehaviour
{
    public GameObject propPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(SpawnPropsRoutine());
        }
        else
        {
            Debug.Log("This is not the server, so no props will be spawned.");
        }
    }

    public IEnumerator SpawnPropsRoutine()
    {
        while (true)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
            SpawnProp(spawnPos);
            yield return new WaitForSeconds(3f);
        }
    }

    private void SpawnProp(Vector3 position)
    {
        GameObject propInstance = Instantiate(propPrefab, position, Quaternion.identity);
        NetworkObject networkObject = propInstance.GetComponent<NetworkObject>();
        networkObject.Spawn(true);
    }
}
