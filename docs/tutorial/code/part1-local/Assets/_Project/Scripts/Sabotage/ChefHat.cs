using UnityEngine;

namespace DumplingKitchen.Sabotage
{
    /// <summary>
    /// The chef's hat. A dumpling that gets close enough (jumping from a counter!) can
    /// knock it down over the chef's eyes. The chef pushes it back up by grabbing it.
    ///
    /// This class only moves the hat. Blinding the chef is ChefVisionBlocker's job,
    /// which listens for this target's state changes. One class, one job.
    /// </summary>
    public sealed class ChefHat : SabotageTarget
    {
        [SerializeField] private Transform hatVisual;
        [SerializeField] private Vector3 overEyesRotation = new(35f, 0f, 0f);
        [SerializeField] private Vector3 overEyesOffset = new(0f, -0.12f, 0.03f);

        private Vector3 _uprightPosition;
        private Quaternion _uprightRotation;

        private void Awake()
        {
            if (hatVisual == null)
                return;

            _uprightPosition = hatVisual.localPosition;
            _uprightRotation = hatVisual.localRotation;
        }

        protected override void OnSabotaged()
        {
            if (hatVisual == null)
                return;

            hatVisual.localPosition = _uprightPosition + overEyesOffset;
            hatVisual.localRotation = _uprightRotation * Quaternion.Euler(overEyesRotation);
        }

        protected override void OnRestored()
        {
            if (hatVisual == null)
                return;

            hatVisual.localPosition = _uprightPosition;
            hatVisual.localRotation = _uprightRotation;
        }
    }
}
