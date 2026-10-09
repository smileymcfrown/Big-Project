using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace DumplingKitchen.Net
{
    /// <summary>
    /// Shows the Relay join code inside the kitchen (e.g. on the order board), so the
    /// chef can read it out from inside the headset. Hidden on LAN games.
    /// </summary>
    public sealed class JoinCodeDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text codeText;

        private void Start()
        {
            NetworkManager network = NetworkManager.Singleton;
            ConnectionManager connection = network != null ? network.GetComponent<ConnectionManager>() : null;
            string code = connection != null && network.IsServer ? connection.JoinCode : string.Empty;

            codeText.text = string.IsNullOrEmpty(code) ? string.Empty : $"Join code: {code}";
        }
    }
}
