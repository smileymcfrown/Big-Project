using DumplingKitchen.Cooking;
using UnityEngine;

namespace DumplingKitchen.Sabotage
{
    /// <summary>Dumplings turn the gas off at the wall; the stove goes out.</summary>
    public sealed class GasValve : SabotageTarget
    {
        [SerializeField] private Stove stove;
        [SerializeField] private Transform handle;
        [SerializeField] private Vector3 handleOffRotation = new(0f, 0f, 90f);

        private Quaternion _handleOnRotation;

        private void Awake()
        {
            if (handle != null)
                _handleOnRotation = handle.localRotation;
        }

        protected override void OnSabotaged()
        {
            stove.SetGasSupply(false);
            if (handle != null)
                handle.localRotation = _handleOnRotation * Quaternion.Euler(handleOffRotation);
        }

        protected override void OnRestored()
        {
            stove.SetGasSupply(true);
            if (handle != null)
                handle.localRotation = _handleOnRotation;
        }
    }
}
