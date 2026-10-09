using UnityEngine;

namespace DumplingKitchen.Sabotage
{
    /// <summary>
    /// Dumplings flick the kitchen lights off. For this to actually get dark, the
    /// kitchen lights must be Realtime or Mixed (not Baked) and ambient light low.
    /// </summary>
    public sealed class LightSwitch : SabotageTarget
    {
        [SerializeField] private Light[] lights;
        [SerializeField] private Transform switchRocker;
        [SerializeField] private Vector3 rockerOffRotation = new(-30f, 0f, 0f);

        private Quaternion _rockerOnRotation;

        private void Awake()
        {
            if (switchRocker != null)
                _rockerOnRotation = switchRocker.localRotation;
        }

        protected override void OnSabotaged() => SetLights(false);
        protected override void OnRestored() => SetLights(true);

        private void SetLights(bool on)
        {
            foreach (Light kitchenLight in lights)
            {
                if (kitchenLight != null)
                    kitchenLight.enabled = on;
            }

            if (switchRocker != null)
                switchRocker.localRotation = on ? _rockerOnRotation : _rockerOnRotation * Quaternion.Euler(rockerOffRotation);
        }
    }
}
