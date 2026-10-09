using Unity.Netcode;

namespace DumplingKitchen.Net
{
    /// <summary>
    /// "Should THIS machine run the game rules?" True on the host/server, and also when
    /// no network session is running at all (handy for quick offline tests of a scene).
    /// </summary>
    public static class Authority
    {
        public static bool IsAuthority
        {
            get
            {
                NetworkManager network = NetworkManager.Singleton;
                return network == null || !network.IsListening || network.IsServer;
            }
        }
    }
}
