using Unity.Netcode;

namespace MultiplayerPractice1.Assets.Scripts
{
    public struct PlayerData : INetworkSerializable
    {
        public uint Number;
        public string Word;
        public string LastPressedKey;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Number);
            serializer.SerializeValue(ref Word);
            serializer.SerializeValue(ref LastPressedKey);
        }
    }
}