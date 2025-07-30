using Unity.Netcode;
using UnityEngine;

public class PropColorizer : NetworkBehaviour
{
    // Use an array for efficiency and Inspector access
    [SerializeField]
    private Color[] playerColors = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    // This variable will be automatically synchronized from server to clients
    private readonly NetworkVariable<int> colorIndex = new NetworkVariable<int>(
        // Default value
        0,
        // Who can read the variable
        NetworkVariableReadPermission.Everyone,
        // Only the server can change the value
        NetworkVariableWritePermission.Server
    );

    private Renderer propRenderer;

    private void Awake()
    {
        propRenderer = GetComponent<Renderer>();
    }

    public override void OnNetworkSpawn()
    {
        // Subscribe to value changes
        colorIndex.OnValueChanged += OnColorChanged;
        DetermineColor();
        ApplyColor(colorIndex.Value);
    }

    private void DetermineColor()
    {
        if (IsServer)
        {
            // The server determines the color based on the owner's ID
            int index = (int)OwnerClientId % playerColors.Length;
            colorIndex.Value = index;
        }
    }

    protected override void OnOwnershipChanged(ulong previous, ulong current)
    {
        base.OnOwnershipChanged(previous, current);
        DetermineColor();
        ApplyColor(colorIndex.Value);
    }

    private void OnColorChanged(int previousValue, int newValue)
    {
        ApplyColor(newValue);
    }

    private void ApplyColor(int index)
    {
        if (propRenderer != null && index < playerColors.Length)
        {
            propRenderer.material.color = playerColors[index];
        }
    }

    public override void OnNetworkDespawn()
    {
        colorIndex.OnValueChanged -= OnColorChanged;
    }
}