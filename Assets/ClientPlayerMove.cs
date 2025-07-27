using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarterAssets;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerPractice1.Assets
{
    public class ClientPlayerMove : NetworkBehaviour
    {
        [SerializeField]
        private PlayerInput _playerInput;

        [SerializeField]
        private StarterAssetsInputs _starterAssetsInputs;

        [SerializeField]
        private ThirdPersonController _thirdPersonController;

        private void Awake()
        {
            _playerInput.enabled = false;
            _starterAssetsInputs.enabled = false;
            _thirdPersonController.enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                _playerInput.enabled = true;
                _starterAssetsInputs.enabled = true;
            }

            if (IsServer)
            {
                _thirdPersonController.enabled = true;
            }
        }

        [Rpc(SendTo.Server)]
        private void UpdateInputServerRpc(Vector2 moveInput, Vector2 lookInput, bool jump, bool sprint)
        {
            _starterAssetsInputs.MoveInput(moveInput);
            _starterAssetsInputs.LookInput(lookInput);
            _starterAssetsInputs.JumpInput(jump);
            _starterAssetsInputs.SprintInput(sprint);
        }

        private void LateUpdate()
        {
            if (!IsOwner)
            {
                return;
            }

            UpdateInputServerRpc(
                _starterAssetsInputs.move,
                _starterAssetsInputs.look,
                _starterAssetsInputs.jump,
                _starterAssetsInputs.sprint
            );
        }
    }
}