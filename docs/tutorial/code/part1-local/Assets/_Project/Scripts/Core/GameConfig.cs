using UnityEngine;

namespace DumplingKitchen.Core
{
    /// <summary>
    /// Every tunable number for a round lives here, in one asset.
    /// Designers (future you) tweak this in the Inspector without touching code.
    /// Create one via: Assets > Create > Dumpling Kitchen > Game Config.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Dumpling Kitchen/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        [Header("Round timing (seconds)")]
        [Tooltip("Length of the playing phase. 300 = 5 minutes.")]
        [SerializeField, Min(10f)] private float roundDurationSeconds = 300f;

        [Tooltip("3-2-1 countdown before play starts. 0 = start immediately.")]
        [SerializeField, Min(0f)] private float countdownSeconds = 3f;

        [Tooltip("How long the results show before the kitchen resets.")]
        [SerializeField, Min(1f)] private float resultsSeconds = 10f;

        [Header("Win condition")]
        [Tooltip("Dishes the chef must serve before time runs out.")]
        [SerializeField, Min(1)] private int dishesToWin = 3;

        [Header("Players")]
        [SerializeField, Range(1, 4)] private int maxDumplings = 4;
        [SerializeField, Range(0, 4)] private int minDumplingsToStart = 1;

        public float RoundDurationSeconds => roundDurationSeconds;
        public float CountdownSeconds => countdownSeconds;
        public float ResultsSeconds => resultsSeconds;
        public int DishesToWin => dishesToWin;
        public int MaxDumplings => maxDumplings;
        public int MinDumplingsToStart => minDumplingsToStart;
    }
}
