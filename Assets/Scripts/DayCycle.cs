using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace MultiplayerPractice1.Assets.Scripts
{
    public class DayCycle : NetworkBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI dayTimeText;
        public float dayLength = 60f; // Length of a day in seconds
        private NetworkVariable<float> timeOfDay = new NetworkVariable<float>(
            0f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                StartDayCycle();
            }

            timeOfDay.OnValueChanged += OnTimeOfDayChanged;
        }

        private void OnTimeOfDayChanged(float previousValue, float newValue)
        {
            dayTimeText.text = $"Day Time: {newValue:F2} / {dayLength:F2} seconds";
        }

        private async void StartDayCycle()
        {
            while (true)
            {
                timeOfDay.Value += Time.deltaTime;
                if (timeOfDay.Value >= dayLength)
                {
                    timeOfDay.Value = 0f;
                }
                await Task.Yield();
            }
        }
    }
}