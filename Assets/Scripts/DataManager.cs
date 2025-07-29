using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace MultiplayerPractice1.Assets.Scripts
{
    public class DataManager : NetworkBehaviour
    {
        private List<string> words = new List<string>
        {
            "apple", "banana", "cherry", "date", "elderberry"
        };

        [SerializeField]
        private TextMeshProUGUI playerDataText;

        private NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>(
            new PlayerData
            {
                Number = 0,
                Word = "default",
                LastPressedKey = ""
            },
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);


        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                StartCoroutine(UpdatePlayerData());
            }

            playerData.OnValueChanged += OnPlayerDataChanged;
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateInputServerRpc(string keyPressed)
        {
            if (IsServer)
            {
                var data = playerData.Value;
                data.LastPressedKey = keyPressed;
                playerData.Value = data;
            }
        }

        private void OnPlayerDataChanged(PlayerData previousValue, PlayerData newValue)
        {
            playerDataText.text = $"Number: {newValue.Number}, Word: {newValue.Word}, Last Key: {newValue.LastPressedKey}";
        }

        private IEnumerator UpdatePlayerData()
        {
            while (true)
            {
                yield return new WaitForSeconds(5f);
                var data = playerData.Value;
                data.Number = (uint)Random.Range(1, 100);
                data.Word = words[Random.Range(0, words.Count)];
                playerData.Value = data;
            }
        }
    }
}