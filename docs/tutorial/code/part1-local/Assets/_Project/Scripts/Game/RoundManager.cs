using System;
using DumplingKitchen.Core;
using UnityEngine;

namespace DumplingKitchen.Game
{
    /// <summary>
    /// Owns the round: its state machine, its timer and its score.
    ///
    /// Design rule: other systems REPORT to the RoundManager (ReportDishServed,
    /// ReportSabotage) and LISTEN to it (PhaseChanged, ScoreChanged). The RoundManager
    /// never reaches into stoves, dumplings or UI. That keeps the dependency arrows
    /// pointing one way: gameplay -> round, never round -> gameplay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RoundManager : MonoBehaviour
    {
        [SerializeField] private GameConfig config;

        // A deliberately small singleton. See "Decisions to review" in the tutorial.
        public static RoundManager Instance { get; private set; }
        public static bool IsPlaying => Instance != null && Instance.Phase == GamePhase.Playing;

        /// <summary>
        /// Raised when a new round begins. Static so kitchen objects can reset
        /// themselves without holding a reference to the RoundManager.
        /// </summary>
        public static event Action RoundStarting;

        public event Action<GamePhase> PhaseChanged;
        public event Action ScoreChanged;

        public GameConfig Config => config;
        public GamePhase Phase { get; private set; } = GamePhase.WaitingForPlayers;
        public float SecondsRemaining => _timer.Remaining;
        public int DishesServed { get; private set; }
        public int Sabotages { get; private set; }
        public int DumplingCount { get; private set; }
        public Team Winner { get; private set; } = Team.None;

        public bool CanStartRound =>
            Phase == GamePhase.WaitingForPlayers && DumplingCount >= config.MinDumplingsToStart;

        private readonly RoundTimer _timer = new();

        // With "Enter Play Mode Options" (no domain reload) statics survive between
        // play sessions. This resets them every time you press Play.
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

            if (config == null)
                Debug.LogError("RoundManager needs a GameConfig asset assigned.", this);

            Instance = this;
            _timer.Expired += HandleTimerExpired;
        }

        private void OnDestroy()
        {
            _timer.Expired -= HandleTimerExpired;
            if (Instance == this)
                Instance = null;
        }

        private void Update() => _timer.Tick(Time.deltaTime);

        // ------------------------------------------------------------------
        // Commands: things other systems ask the round to do
        // ------------------------------------------------------------------

        /// <summary>Called by the chef's service bell (wired up with a UnityEvent).</summary>
        public void StartRound()
        {
            if (!CanStartRound)
            {
                Debug.Log($"Can't start yet: phase={Phase}, dumplings={DumplingCount}/{config.MinDumplingsToStart}.");
                return;
            }

            DishesServed = 0;
            Sabotages = 0;
            Winner = Team.None;
            ScoreChanged?.Invoke();
            RoundStarting?.Invoke();

            EnterPhase(GamePhase.Countdown, config.CountdownSeconds);
        }

        public void SetDumplingCount(int count) => DumplingCount = Mathf.Max(0, count);

        public void ReportDishServed()
        {
            if (Phase != GamePhase.Playing)
                return;

            DishesServed++;
            ScoreChanged?.Invoke();

            if (DishesServed >= config.DishesToWin)
                EndRound(Team.Chef);
        }

        public void ReportSabotage()
        {
            if (Phase != GamePhase.Playing)
                return;

            Sabotages++;
            ScoreChanged?.Invoke();
        }

        // ------------------------------------------------------------------
        // State machine
        // ------------------------------------------------------------------

        private void HandleTimerExpired()
        {
            switch (Phase)
            {
                case GamePhase.Countdown:
                    EnterPhase(GamePhase.Playing, config.RoundDurationSeconds);
                    break;

                case GamePhase.Playing:
                    // Time ran out before the chef hit the target: the dumplings win.
                    EndRound(DishesServed >= config.DishesToWin ? Team.Chef : Team.Dumplings);
                    break;

                case GamePhase.RoundOver:
                    EnterPhase(GamePhase.WaitingForPlayers, 0f);
                    break;
            }
        }

        private void EndRound(Team winner)
        {
            Winner = winner;
            EnterPhase(GamePhase.RoundOver, config.ResultsSeconds);
        }

        private void EnterPhase(GamePhase next, float durationSeconds)
        {
            Phase = next;
            _timer.Stop();
            if (durationSeconds > 0f)
                _timer.Start(durationSeconds);

            PhaseChanged?.Invoke(next);

            // A timed phase with zero length (e.g. countdown set to 0) moves straight on.
            if (durationSeconds <= 0f && next != GamePhase.WaitingForPlayers)
                HandleTimerExpired();
        }
    }
}
