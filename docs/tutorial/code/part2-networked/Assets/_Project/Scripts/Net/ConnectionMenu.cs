using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace DumplingKitchen.Net
{
    /// <summary>
    /// The Menu scene's UI. It only turns button presses into ConnectionManager calls
    /// and shows status text. No networking logic lives here.
    /// </summary>
    public sealed class ConnectionMenu : MonoBehaviour
    {
        [Header("LAN")]
        [SerializeField] private TMP_InputField addressField;
        [SerializeField] private Button hostLanButton;
        [SerializeField] private Button joinLanButton;

        [Header("Online (Relay)")]
        [SerializeField] private TMP_InputField joinCodeField;
        [SerializeField] private Button hostOnlineButton;
        [SerializeField] private Button joinOnlineButton;

        [Header("Feedback")]
        [SerializeField] private TMP_Text statusText;

        private ConnectionManager _connection;

        private void Start()
        {
            // The ConnectionManager sits on the NetworkManager, which NGO keeps alive
            // across scenes and exposes as NetworkManager.Singleton.
            _connection = NetworkManager.Singleton.GetComponent<ConnectionManager>();
            _connection.StatusChanged += ShowStatus;
            ShowStatus(_connection.LastStatus);

            // Listeners are added ONCE, here. (The 2022 VrWristUI added them in Update,
            // which stacked up a new listener every frame.)
            hostLanButton.onClick.AddListener(() => { SetButtons(false); _connection.HostAsChef(); });
            joinLanButton.onClick.AddListener(() => { SetButtons(false); _connection.JoinAsDumpling(addressField.text); });
            hostOnlineButton.onClick.AddListener(() => { SetButtons(false); _connection.HostOnlineAsChef(); });
            joinOnlineButton.onClick.AddListener(() => { SetButtons(false); _connection.JoinOnlineAsDumpling(joinCodeField.text); });
        }

        private void OnDestroy()
        {
            if (_connection != null)
                _connection.StatusChanged -= ShowStatus;
        }

        private void ShowStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;

            // Anything that isn't progress means we're back at the menu: let them retry.
            if (!message.EndsWith("..."))
                SetButtons(true);
        }

        private void SetButtons(bool interactable)
        {
            hostLanButton.interactable = interactable;
            joinLanButton.interactable = interactable;
            hostOnlineButton.interactable = interactable;
            joinOnlineButton.interactable = interactable;
        }
    }
}
