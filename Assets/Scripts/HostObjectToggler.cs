using Unity.Netcode;
using UnityEngine;

namespace MultiplayerPractice1.Assets.Scripts
{
    public class HostObjectToggler : NetworkBehaviour
    {
        [SerializeField] private GameObject objectToToggle;

        void Update()
        {
            if (!IsHost)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                bool newState = !objectToToggle.activeSelf;

                ToggleObjectClientRpc(newState);
            }
        }

        [ClientRpc]
        private void ToggleObjectClientRpc(bool newState)
        {
            // This code executes on ALL clients, including the host's client.
            if (objectToToggle != null)
            {
                objectToToggle.SetActive(newState);
            }
        }
    }
}