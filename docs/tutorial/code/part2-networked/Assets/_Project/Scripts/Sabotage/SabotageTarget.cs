using System;
using DumplingKitchen.Dumplings;
using DumplingKitchen.Game;
using Unity.Netcode;
using UnityEngine;

namespace DumplingKitchen.Sabotage
{
    /// <summary>
    /// NETWORKED version of the sabotage base class.
    ///
    /// The payoff of the Template Method design: GasValve, LightSwitch, PowerPlug,
    /// Knockable and ChefHat DO NOT CHANGE AT ALL. Only this base class learns about
    /// the network:
    ///  - "is it sabotaged?" becomes a NetworkVariable the server owns,
    ///  - a dumpling's Interact() becomes a request (RPC) to the server,
    ///  - the server re-checks everything before agreeing (never trust a client),
    ///  - every machine runs OnSabotaged()/OnRestored() when the variable changes.
    ///
    /// Every target now needs a NetworkObject on itself or a parent.
    /// </summary>
    public abstract class SabotageTarget : NetworkBehaviour, IDumplingInteractable
    {
        [Header("Sabotage")]
        [SerializeField] private string displayName = "Thing";
        [SerializeField] private string sabotageVerb = "Sabotage";
        [SerializeField, Min(0f)] private float rearmSeconds = 5f;
        [SerializeField] private Transform interactionPoint;

        private readonly NetworkVariable<bool> _isSabotaged = new();
        private readonly NetworkVariable<double> _rearmAt = new(); // server time

        public string DisplayName => displayName;
        public bool IsSabotaged => _isSabotaged.Value;
        public string Prompt => $"{sabotageVerb} {displayName}";
        public Transform InteractionPoint => interactionPoint != null ? interactionPoint : transform;

        public event Action<SabotageTarget, bool> StateChanged;
        public static event Action<SabotageTarget> AnySabotaged;
        public static event Action<SabotageTarget> AnyRestored;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            AnySabotaged = null;
            AnyRestored = null;
        }

        protected virtual void OnEnable() => RoundManager.RoundStarting += HandleRoundStarting;
        protected virtual void OnDisable() => RoundManager.RoundStarting -= HandleRoundStarting;

        public override void OnNetworkSpawn()
        {
            _isSabotaged.OnValueChanged += HandleSabotagedValueChanged;

            // Joined mid-game and this is already broken? Show it, quietly.
            if (_isSabotaged.Value)
                OnSabotaged();
        }

        public override void OnNetworkDespawn() => _isSabotaged.OnValueChanged -= HandleSabotagedValueChanged;

        // ------------------------------------------------------------------
        // The fixed flow
        // ------------------------------------------------------------------

        /// <summary>Runs on the dumpling's PC (for the prompt) AND on the server (to validate).</summary>
        public virtual bool CanInteract(DumplingInteractor dumpling) =>
            IsSpawned
            && RoundManager.IsPlaying
            && !IsSabotaged
            && NetworkManager.ServerTime.Time >= _rearmAt.Value;

        /// <summary>Called on the dumpling's own PC. Asks the server to do it.</summary>
        public void Interact(DumplingInteractor dumpling)
        {
            if (CanInteract(dumpling))
                RequestSabotageRpc();
        }

        // Runs ON THE SERVER. RpcParams tells us who sent it; we never take the
        // client's word for who they are or where they're standing.
        [Rpc(SendTo.Server)]
        private void RequestSabotageRpc(RpcParams rpcParams = default)
        {
            ulong senderId = rpcParams.Receive.SenderClientId;
            if (!NetworkManager.ConnectedClients.TryGetValue(senderId, out NetworkClient sender)
                || sender.PlayerObject == null
                || !sender.PlayerObject.TryGetComponent(out DumplingInteractor dumpling))
            {
                return;
            }

            if (!CanInteract(dumpling) || !dumpling.IsInReach(this))
                return; // late, cheating, or someone else got there first

            OnSabotagedBy(dumpling);  // one-off physics, server only
            _isSabotaged.Value = true; // replicates; fires OnSabotaged everywhere
            RoundManager.Instance.ReportSabotage();
        }

        /// <summary>The chef fixes it. The chef is the host, so this runs on the server.</summary>
        public void Restore()
        {
            if (!IsServer || !IsSabotaged)
                return;

            _rearmAt.Value = NetworkManager.ServerTime.Time + rearmSeconds;
            _isSabotaged.Value = false;
        }

        private void HandleRoundStarting()
        {
            if (!IsServer)
                return;

            _rearmAt.Value = 0d;
            _isSabotaged.Value = false;
        }

        private void HandleSabotagedValueChanged(bool previous, bool sabotaged)
        {
            if (sabotaged) OnSabotaged();
            else OnRestored();

            StateChanged?.Invoke(this, sabotaged);
            if (sabotaged) AnySabotaged?.Invoke(this);
            else AnyRestored?.Invoke(this);
        }

        // ------------------------------------------------------------------
        // Hooks: identical to Part 1, so the subclasses don't change
        // ------------------------------------------------------------------

        protected virtual void OnSabotagedBy(DumplingInteractor dumpling) { }
        protected abstract void OnSabotaged();
        protected abstract void OnRestored();
    }
}
