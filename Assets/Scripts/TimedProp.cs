
using System;
using System.Threading.Tasks;
using Unity.Netcode;

namespace MultiplayerPractice1.Assets.Scripts
{
    public class TimedProp : NetworkBehaviour
    {

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                StartLifetimeCountdown();
            }
        }

        private async void StartLifetimeCountdown()
        {
            await Task.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(5f, 10f)));
            if (IsServer)
            {
                Despawn();
            }
        }

        private void Despawn()
        {
            NetworkObject.Despawn();
        }
    }
}