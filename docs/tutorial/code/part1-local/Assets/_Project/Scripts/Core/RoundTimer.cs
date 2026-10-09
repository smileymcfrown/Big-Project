using System;

namespace DumplingKitchen.Core
{
    /// <summary>
    /// A plain C# countdown timer. It is NOT a MonoBehaviour, so:
    ///  - it can be unit tested without a scene,
    ///  - it can be reused anywhere (round timer, cooldowns, power-ups),
    ///  - whoever owns it decides when time passes by calling Tick().
    /// </summary>
    public sealed class RoundTimer
    {
        public float Duration { get; private set; }
        public float Remaining { get; private set; }
        public bool IsRunning { get; private set; }

        /// <summary>1 at the start, 0 when finished. Handy for UI fill bars.</summary>
        public float Normalized => Duration > 0f ? Remaining / Duration : 0f;

        /// <summary>Raised once, on the tick where the timer reaches zero.</summary>
        public event Action Expired;

        public void Start(float durationSeconds)
        {
            if (durationSeconds <= 0f)
                throw new ArgumentOutOfRangeException(nameof(durationSeconds), "Duration must be positive.");

            Duration = durationSeconds;
            Remaining = durationSeconds;
            IsRunning = true;
        }

        public void Stop() => IsRunning = false;

        public void Tick(float deltaTime)
        {
            if (!IsRunning)
                return;

            Remaining -= deltaTime;
            if (Remaining > 0f)
                return;

            Remaining = 0f;
            IsRunning = false;
            Expired?.Invoke();
        }

        /// <summary>Formats seconds as m:ss, rounding up so "0:00" only shows when time is really up.</summary>
        public static string Format(float seconds)
        {
            int whole = Math.Max(0, (int)Math.Ceiling(seconds));
            return $"{whole / 60}:{whole % 60:00}";
        }
    }
}
