using System;
using DumplingKitchen.Dumplings;
using DumplingKitchen.Game;
using UnityEngine;

namespace DumplingKitchen.Sabotage
{
    /// <summary>
    /// The base class for everything the dumplings can mess up.
    ///
    /// This is the "Template Method" pattern. The base class owns the RULES that must be
    /// the same for every target (is the round running? already sabotaged? cooling
    /// down? tell the round manager). Subclasses only fill in the parts that differ:
    /// what sabotaging and restoring actually DO.
    ///
    /// Interact() and Restore() are deliberately NOT virtual, so a subclass cannot
    /// accidentally skip the rules. Subclasses override the protected hooks instead.
    /// </summary>
    public abstract class SabotageTarget : MonoBehaviour, IDumplingInteractable
    {
        [Header("Sabotage")]
        [SerializeField] private string displayName = "Thing";
        [Tooltip("Verb shown to the dumpling, e.g. 'Turn off', 'Unplug', 'Knock over'.")]
        [SerializeField] private string sabotageVerb = "Sabotage";
        [Tooltip("Seconds after the chef fixes this before it can be sabotaged again.")]
        [SerializeField, Min(0f)] private float rearmSeconds = 5f;
        [SerializeField] private Transform interactionPoint;

        private float _rearmTime;

        public string DisplayName => displayName;
        public bool IsSabotaged { get; private set; }
        public string Prompt => $"{sabotageVerb} {displayName}";
        public Transform InteractionPoint => interactionPoint != null ? interactionPoint : transform;

        /// <summary>This target changed state. (target, isSabotaged)</summary>
        public event Action<SabotageTarget, bool> StateChanged;

        /// <summary>ANY target was sabotaged / restored. Great for alert UI and audio.</summary>
        public static event Action<SabotageTarget> AnySabotaged;
        public static event Action<SabotageTarget> AnyRestored;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            AnySabotaged = null;
            AnyRestored = null;
        }

        // Unity messages in a base class: make them protected virtual so subclasses can
        // extend them (and remember to call base.OnEnable()).
        protected virtual void OnEnable() => RoundManager.RoundStarting += HandleRoundStarting;
        protected virtual void OnDisable() => RoundManager.RoundStarting -= HandleRoundStarting;

        // ------------------------------------------------------------------
        // The fixed flow (same for every target)
        // ------------------------------------------------------------------

        public virtual bool CanInteract(DumplingInteractor dumpling) =>
            RoundManager.IsPlaying && !IsSabotaged && Time.time >= _rearmTime;

        public void Interact(DumplingInteractor dumpling)
        {
            if (!CanInteract(dumpling))
                return;

            OnSabotagedBy(dumpling);
            SetState(true);
            RoundManager.Instance.ReportSabotage();
        }

        /// <summary>The chef fixes it. Called by ChefRestoreHandle (or anything else).</summary>
        public void Restore()
        {
            if (!IsSabotaged)
                return;

            _rearmTime = Time.time + rearmSeconds;
            SetState(false);
        }

        private void HandleRoundStarting()
        {
            _rearmTime = 0f;
            if (IsSabotaged)
                SetState(false);
        }

        private void SetState(bool sabotaged)
        {
            IsSabotaged = sabotaged;

            if (sabotaged) OnSabotaged();
            else OnRestored();

            StateChanged?.Invoke(this, sabotaged);
            if (sabotaged) AnySabotaged?.Invoke(this);
            else AnyRestored?.Invoke(this);
        }

        // ------------------------------------------------------------------
        // Hooks for subclasses
        // ------------------------------------------------------------------

        /// <summary>
        /// One-off physical effect caused by a specific dumpling (e.g. a shove).
        /// Runs once, where the game's authority is. Optional.
        /// </summary>
        protected virtual void OnSabotagedBy(DumplingInteractor dumpling) { }

        /// <summary>Make the world reflect "sabotaged" (gas off, lights off...).</summary>
        protected abstract void OnSabotaged();

        /// <summary>Make the world reflect "working again".</summary>
        protected abstract void OnRestored();
    }
}
