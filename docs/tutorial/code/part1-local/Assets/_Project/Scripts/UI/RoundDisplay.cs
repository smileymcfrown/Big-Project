using DumplingKitchen.Core;
using DumplingKitchen.Game;
using TMPro;
using UnityEngine;

namespace DumplingKitchen.UI
{
    /// <summary>
    /// Shows the timer, phase message and score. Use one on the desktop overlay canvas
    /// and another on a world-space wall clock for the chef. Same script, two canvases.
    ///
    /// UI only READS game state and reacts to events. It never changes the game.
    /// </summary>
    public sealed class RoundDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text phaseText;
        [SerializeField] private TMP_Text scoreText;

        private RoundManager _round;
        private int _lastShownSecond = -1;

        // Start (not Awake/OnEnable) because RoundManager.Instance is set in its Awake,
        // and Unity guarantees every Awake has run before any Start.
        private void Start()
        {
            _round = RoundManager.Instance;
            if (_round == null)
            {
                Debug.LogWarning("RoundDisplay: no RoundManager in the scene.", this);
                enabled = false;
                return;
            }

            _round.PhaseChanged += HandlePhaseChanged;
            _round.ScoreChanged += RefreshScore;
            HandlePhaseChanged(_round.Phase);
            RefreshScore();
        }

        private void OnDestroy()
        {
            if (_round == null)
                return;

            _round.PhaseChanged -= HandlePhaseChanged;
            _round.ScoreChanged -= RefreshScore;
        }

        private void Update()
        {
            float seconds = _round.Phase == GamePhase.WaitingForPlayers
                ? _round.Config.RoundDurationSeconds
                : _round.SecondsRemaining;

            // Only rebuild the string when the visible number changes: building strings
            // every frame creates garbage, and garbage collection causes hitches (in VR
            // a hitch is a dropped frame the chef can feel).
            int whole = Mathf.CeilToInt(seconds);
            if (whole == _lastShownSecond)
                return;

            _lastShownSecond = whole;
            if (timerText != null)
                timerText.text = RoundTimer.Format(seconds);
        }

        private void HandlePhaseChanged(GamePhase phase)
        {
            if (phaseText == null)
                return;

            phaseText.text = phase switch
            {
                GamePhase.WaitingForPlayers => "Dumplings: press Start / Enter to join. Chef: ring the bell!",
                GamePhase.Countdown => "Get ready...",
                GamePhase.Playing => string.Empty,
                GamePhase.RoundOver => _round.Winner == Team.Chef ? "Dinner is served! Chef wins." : "Kitchen chaos! Dumplings win.",
                _ => string.Empty
            };
        }

        private void RefreshScore()
        {
            if (scoreText != null)
                scoreText.text = $"Dishes {_round.DishesServed}/{_round.Config.DishesToWin}   Sabotages {_round.Sabotages}";
        }
    }
}
