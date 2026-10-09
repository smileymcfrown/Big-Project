using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace DumplingKitchen.Net
{
    /// <summary>
    /// Internet play through Unity Relay, using the Multiplayer Services "Sessions" API.
    /// Relay means nobody has to open ports on their router: everyone connects out to
    /// Unity's relay servers, and the host shares a short join code.
    ///
    /// The Sessions SDK starts Netcode for you: CreateSessionAsync calls StartHost()
    /// and JoinSessionByCodeAsync calls StartClient() on NetworkManager.Singleton.
    /// Needs: the project linked to a Unity Cloud project, with Relay enabled.
    /// </summary>
    public sealed class SessionConnector : MonoBehaviour
    {
        [Tooltip("Including the chef.")]
        [SerializeField, Range(2, 5)] private int maxPlayers = 5;

        private ISession _session;

        public string JoinCode => _session?.Code;

        public async Task<string> HostAsync()
        {
            await EnsureSignedInAsync();

            SessionOptions options = new SessionOptions { MaxPlayers = maxPlayers }.WithRelayNetwork();
            _session = await MultiplayerService.Instance.CreateSessionAsync(options);
            return _session.Code;
        }

        public async Task JoinAsync(string joinCode)
        {
            await EnsureSignedInAsync();
            _session = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode.Trim().ToUpperInvariant());
        }

        public async Task LeaveAsync()
        {
            if (_session == null)
                return;

            await _session.LeaveAsync();
            _session = null;
        }

        private static async Task EnsureSignedInAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            // Anonymous sign-in: no accounts needed. Each install gets a player ID.
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}
