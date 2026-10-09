using DumplingKitchen.Dumplings;
using DumplingKitchen.Game;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DumplingKitchen.Local
{
    /// <summary>
    /// Couch co-op: when a keyboard or gamepad presses Join, PlayerInputManager spawns a
    /// dumpling prefab and gives it that device. We then place it, colour it, and tell
    /// the round how many dumplings there are. Joining is closed while a round is on.
    /// </summary>
    [RequireComponent(typeof(PlayerInputManager))]
    public sealed class LocalPlayerJoiner : MonoBehaviour
    {
        private const string KeyboardMouseScheme = "Keyboard&Mouse";

        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private Color[] dumplingColours =
        {
            new(1.00f, 0.86f, 0.62f), // classic
            new(0.62f, 0.85f, 0.55f), // spinach
            new(0.95f, 0.55f, 0.50f), // chilli
            new(0.70f, 0.62f, 0.90f)  // purple yam
        };

        private PlayerInputManager _manager;
        private RoundManager _round;

        private void Awake() => _manager = GetComponent<PlayerInputManager>();

        private void OnEnable()
        {
            _manager.onPlayerJoined += HandlePlayerJoined;
            _manager.onPlayerLeft += HandlePlayerLeft;
        }

        private void OnDisable()
        {
            _manager.onPlayerJoined -= HandlePlayerJoined;
            _manager.onPlayerLeft -= HandlePlayerLeft;
            if (_round != null)
                _round.PhaseChanged -= HandlePhaseChanged;
        }

        private void Start()
        {
            _round = RoundManager.Instance;
            _round.PhaseChanged += HandlePhaseChanged;
            UpdateDumplingCount();
        }

        private void HandlePlayerJoined(PlayerInput player)
        {
            int index = player.playerIndex;
            Transform spawn = spawnPoints[index % spawnPoints.Length];

            // Move the Rigidbody AND the Transform, so physics and interpolation agree.
            if (player.TryGetComponent(out Rigidbody body))
            {
                body.position = spawn.position;
                body.rotation = spawn.rotation;
            }
            player.transform.SetPositionAndRotation(spawn.position, spawn.rotation);

            if (player.TryGetComponent(out DumplingPlayer dumpling))
                dumpling.Initialise(index, dumplingColours[index % dumplingColours.Length]);

            if (player.currentControlScheme == KeyboardMouseScheme)
                Cursor.lockState = CursorLockMode.Locked;

            UpdateDumplingCount();
        }

        private void HandlePlayerLeft(PlayerInput player) => UpdateDumplingCount();

        private void HandlePhaseChanged(GamePhase phase)
        {
            if (phase == GamePhase.WaitingForPlayers) _manager.EnableJoining();
            else _manager.DisableJoining();
        }

        private void UpdateDumplingCount()
        {
            if (_round != null)
                _round.SetDumplingCount(_manager.playerCount);
        }
    }
}
