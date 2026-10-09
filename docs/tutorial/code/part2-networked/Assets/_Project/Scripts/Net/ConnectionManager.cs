using System;
using System.Collections;
using System.Threading.Tasks;
using DumplingKitchen.Game;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DumplingKitchen.Net
{
    /// <summary>
    /// Everything about connecting, in one place. Lives on the NetworkManager object in
    /// the Bootstrap scene (so it survives scene loads with it).
    ///
    /// Topology (see "Decisions to review"): the CHEF is always the HOST. The host is
    /// both server and a player, so the VR player's physics and grabbing run with zero
    /// network delay, and dumplings send requests to the chef's machine.
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public sealed class ConnectionManager : MonoBehaviour
    {
        [SerializeField] private string menuSceneName = "Menu";
        [SerializeField] private string kitchenSceneName = "Kitchen";
        [SerializeField] private ushort port = 7777;
        [SerializeField, Range(1, 4)] private int maxDumplings = 4;
        [SerializeField] private SessionConnector sessionConnector;

        private NetworkManager _network;

        public event Action<string> StatusChanged;

        /// <summary>Last message, so a freshly loaded menu can show why we're back.</summary>
        public string LastStatus { get; private set; } = string.Empty;

        /// <summary>The Relay join code while hosting online, otherwise empty.</summary>
        public string JoinCode { get; private set; } = string.Empty;

        private void Awake()
        {
            _network = GetComponent<NetworkManager>();
            // Also tick "Connection Approval" on the NetworkManager component.
            _network.ConnectionApprovalCallback = ApproveConnection;
        }

        private void OnEnable() => _network.OnClientDisconnectCallback += HandleClientDisconnect;

        private void OnDisable()
        {
            if (_network != null)
                _network.OnClientDisconnectCallback -= HandleClientDisconnect;
        }

        // The Bootstrap scene only exists to create the NetworkManager once.
        // Never load Bootstrap again, or you get a second NetworkManager.
        private void Start() => SceneManager.LoadScene(menuSceneName);

        // ------------------------------------------------------------------
        // LAN / direct IP
        // ------------------------------------------------------------------

        public void HostAsChef() => StartCoroutine(HostAsChefRoutine());

        public void JoinAsDumpling(string address)
        {
            Transport.SetConnectionData(string.IsNullOrWhiteSpace(address) ? "127.0.0.1" : address.Trim(), port);
            if (_network.StartClient())
                SetStatus($"Connecting to {address}...");
            else
                SetStatus("Could not start the client.");
        }

        private IEnumerator HostAsChefRoutine()
        {
            yield return StartChefXR();
            if (!XRStartup.IsRunning)
                yield break;

            // Listen on every network card (0.0.0.0) so other PCs on the LAN can join.
            Transport.SetConnectionData("127.0.0.1", port, "0.0.0.0");
            if (!_network.StartHost())
            {
                SetStatus($"Could not host. Is port {port} already in use?");
                XRStartup.StopXR();
                yield break;
            }

            LoadKitchen();
        }

        // ------------------------------------------------------------------
        // Online through Unity Relay (join codes)
        // ------------------------------------------------------------------

        public void HostOnlineAsChef() => StartCoroutine(HostOnlineRoutine());

        public void JoinOnlineAsDumpling(string joinCode) => StartCoroutine(JoinOnlineRoutine(joinCode));

        private IEnumerator HostOnlineRoutine()
        {
            yield return StartChefXR();
            if (!XRStartup.IsRunning)
                yield break;

            SetStatus("Creating online session...");
            Task<string> hosting = sessionConnector.HostAsync();
            yield return new WaitUntil(() => hosting.IsCompleted);

            if (hosting.IsFaulted)
            {
                SetStatus($"Online hosting failed: {hosting.Exception?.GetBaseException().Message}");
                XRStartup.StopXR();
                yield break;
            }

            JoinCode = hosting.Result;
            SetStatus($"Join code: {JoinCode}");
            LoadKitchen();
        }

        private IEnumerator JoinOnlineRoutine(string joinCode)
        {
            SetStatus("Joining online session...");
            Task joining = sessionConnector.JoinAsync(joinCode);
            yield return new WaitUntil(() => joining.IsCompleted);

            if (joining.IsFaulted)
                SetStatus($"Could not join: {joining.Exception?.GetBaseException().Message}");
        }

        // ------------------------------------------------------------------
        // Shared
        // ------------------------------------------------------------------

        public void LeaveGame()
        {
            if (sessionConnector != null)
                _ = sessionConnector.LeaveAsync(); // fire and forget

            JoinCode = string.Empty;
            _network.Shutdown();
            XRStartup.StopXR();
            SceneManager.LoadScene(menuSceneName);
        }

        private IEnumerator StartChefXR()
        {
            SetStatus("Starting VR...");
            yield return XRStartup.StartXR();
            if (!XRStartup.IsRunning)
                SetStatus("No headset found. Connect the Quest with Link (or start SteamVR) and try again.");
        }

        private void LoadKitchen()
        {
            // NetworkSceneManager: the server loads the scene and every client follows,
            // including clients that join later.
            _network.SceneManager.LoadScene(kitchenSceneName, LoadSceneMode.Single);
        }

        /// <summary>Server-side gatekeeper. Runs for every connection, including the host's own.</summary>
        private void ApproveConnection(NetworkManager.ConnectionApprovalRequest request,
                                       NetworkManager.ConnectionApprovalResponse response)
        {
            bool isHost = request.ClientNetworkId == NetworkManager.ServerClientId;
            int dumplings = Mathf.Max(0, _network.ConnectedClientsIds.Count - 1); // everyone except the host
            bool roundRunning = RoundManager.Instance != null && RoundManager.Instance.Phase != GamePhase.WaitingForPlayers;

            response.Approved = isHost || (dumplings < maxDumplings && !roundRunning);
            response.Reason = response.Approved ? string.Empty
                : roundRunning ? "A round is in progress. Try again in a minute."
                : "The kitchen is full.";

            // We spawn player objects ourselves (KitchenSpawner), because the chef and the
            // dumplings need different prefabs.
            response.CreatePlayerObject = false;
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            // On a client, this fires for ourselves when we are kicked or the host quits.
            if (_network.IsServer || clientId != _network.LocalClientId)
                return;

            string reason = string.IsNullOrEmpty(_network.DisconnectReason) ? "Lost connection to the chef." : _network.DisconnectReason;
            SceneManager.LoadScene(menuSceneName);
            SetStatus(reason);
        }

        private UnityTransport Transport => (UnityTransport)_network.NetworkConfig.NetworkTransport;

        private void SetStatus(string message)
        {
            Debug.Log($"[Connection] {message}");
            LastStatus = message;
            StatusChanged?.Invoke(message);
        }
    }
}
