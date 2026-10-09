using System;
using DumplingKitchen.Core;
using Unity.Netcode;
using UnityEngine;

namespace DumplingKitchen.Game
{
    /// <summary>
    /// NETWORKED version of the RoundManager. Compare it with the Part 1 file:
    ///  - The public API (Phase, SecondsRemaining, PhaseChanged, StartRound...) is the
    ///    same, so RoundDisplay and every other listener work unchanged.
    ///  - State lives in NetworkVariables, written only by the server (the chef's PC).
    ///  - Instead of ticking a countdown and sending "time left" 60 times a second, the
    ///    server sends ONE number: the server time when the phase ends. Every machine
    ///    works out the time left from its own copy of the server clock.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RoundManager : NetworkBehaviour
    {
        [SerializeField] private GameConfig config;

        public static RoundManager Instance { get; private set; }
        public static bool IsPlaying => Instance != null && Instance.IsSpawned && Instance.Phase == GamePhase.Playing;
        public static event Action RoundStarting;

        public event Action<GamePhase> PhaseChanged;
        public event Action ScoreChanged;

        // Declared BEFORE _phase on purpose: NGO sends a behaviour's variables in
        // declaration order, so clients already know the winner when RoundOver arrives.
        private readonly NetworkVariable<Team> _winner = new(Team.None);
        private readonly NetworkVariable<int> _dishesServed = new();
        private readonly NetworkVariable<int> _sabotages = new();
        private readonly NetworkVariable<int> _dumplingCount = new();
        private readonly NetworkVariable<double> _phaseEndsAt = new();
        private readonly NetworkVariable<GamePhase> _phase = new(GamePhase.WaitingForPlayers);

        public GameConfig Config => config;
        public GamePhase Phase => _phase.Value;
        public int DishesServed => _dishesServed.Value;
        public int Sabotages => _sabotages.Value;
        public int DumplingCount => _dumplingCount.Value;
        public Team Winner => _winner.Value;

        public float SecondsRemaining => IsSpawned && _phaseEndsAt.Value > 0d
            ? Mathf.Max(0f, (float)(_phaseEndsAt.Value - NetworkManager.ServerTime.Time))
            : 0f;

        public bool CanStartRound =>
            Phase == GamePhase.WaitingForPlayers && DumplingCount >= config.MinDumplingsToStart;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
            RoundStarting = null;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError($"Two RoundManagers found. Removing the one on '{name}'.", this);
                Destroy(this);
                return;
            }

            Instance = this;
        }

        public override void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            base.OnDestroy(); // NetworkBehaviour has its own OnDestroy: always call base
        }

        public override void OnNetworkSpawn()
        {
            _phase.OnValueChanged += HandlePhaseValueChanged;
            _dishesServed.OnValueChanged += HandleScoreValueChanged;
            _sabotages.OnValueChanged += HandleScoreValueChanged;

            // Late joiners: show the current state straight away.
            PhaseChanged?.Invoke(Phase);
            ScoreChanged?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            _phase.OnValueChanged -= HandlePhaseValueChanged;
            _dishesServed.OnValueChanged -= HandleScoreValueChanged;
            _sabotages.OnValueChanged -= HandleScoreValueChanged;
        }

        private void Update()
        {
            if (!IsServer || _phaseEndsAt.Value <= 0d)
                return;

            if (NetworkManager.ServerTime.Time >= _phaseEndsAt.Value)
                HandlePhaseTimeUp();
        }

        // ------------------------------------------------------------------
        // Commands (server only: the chef's PC is the server)
        // ------------------------------------------------------------------

        public void StartRound()
        {
            if (!IsServer || !CanStartRound)
                return;

            _dishesServed.Value = 0;
            _sabotages.Value = 0;
            _winner.Value = Team.None;
            EnterPhase(GamePhase.Countdown, config.CountdownSeconds);
        }

        public void SetDumplingCount(int count)
        {
            if (IsServer)
                _dumplingCount.Value = Mathf.Max(0, count);
        }

        public void ReportDishServed()
        {
            if (!IsServer || Phase != GamePhase.Playing)
                return;

            _dishesServed.Value++;
            if (_dishesServed.Value >= config.DishesToWin)
                EndRound(Team.Chef);
        }

        public void ReportSabotage()
        {
            if (IsServer && Phase == GamePhase.Playing)
                _sabotages.Value++;
        }

        // ------------------------------------------------------------------
        // State machine (server only)
        // ------------------------------------------------------------------

        private void HandlePhaseTimeUp()
        {
            switch (Phase)
            {
                case GamePhase.Countdown:
                    EnterPhase(GamePhase.Playing, config.RoundDurationSeconds);
                    break;
                case GamePhase.Playing:
                    EndRound(DishesServed >= config.DishesToWin ? Team.Chef : Team.Dumplings);
                    break;
                case GamePhase.RoundOver:
                    EnterPhase(GamePhase.WaitingForPlayers, 0f);
                    break;
            }
        }

        private void EndRound(Team winner)
        {
            _winner.Value = winner;
            EnterPhase(GamePhase.RoundOver, config.ResultsSeconds);
        }

        private void EnterPhase(GamePhase next, float durationSeconds)
        {
            _phaseEndsAt.Value = durationSeconds > 0f ? NetworkManager.ServerTime.Time + durationSeconds : 0d;
            _phase.Value = next; // fires HandlePhaseValueChanged on the server AND every client

            if (durationSeconds <= 0f && next != GamePhase.WaitingForPlayers)
                HandlePhaseTimeUp();
        }

        // ------------------------------------------------------------------
        // Runs on EVERY machine when the replicated values change
        // ------------------------------------------------------------------

        private void HandlePhaseValueChanged(GamePhase previous, GamePhase next)
        {
            if (next == GamePhase.Countdown)
                RoundStarting?.Invoke();

            PhaseChanged?.Invoke(next);
        }

        private void HandleScoreValueChanged(int previous, int next) => ScoreChanged?.Invoke();
    }
}
