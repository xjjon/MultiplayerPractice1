using UnityEngine;
using Unity.Netcode;

namespace MultiplayerPractice1.Assets.Scripts
{
    public class AutoConnect : MonoBehaviour
    {
        void Start()
        {
#if UNITY_EDITOR
            if (IsVirtualPlayer())
            {
                // This is a virtual player launched by MPPM, so start as a client.
                NetworkManager.Singleton.StartClient();
            }
            else
            {
                // This is the main editor, so start as the host.
                NetworkManager.Singleton.StartHost();
            }
#endif
        }

#if UNITY_EDITOR
    private bool IsVirtualPlayer()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
                if (args[i] == "-name")
                {
                    return true;
                }
        }
        return false;
    }
#endif
    }
}