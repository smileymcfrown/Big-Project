using DumplingKitchen.Cooking;
using UnityEngine;

namespace DumplingKitchen.Sabotage
{
    /// <summary>
    /// Dumplings yank a plug out of the wall. Works with ANY ElectricAppliance:
    /// hob, rice cooker, kettle... (that is what the base class buys us).
    /// </summary>
    public sealed class PowerPlug : SabotageTarget
    {
        [SerializeField] private ElectricAppliance appliance;
        [SerializeField] private Transform plugVisual;
        [SerializeField] private Vector3 unpluggedOffset = new(0f, -0.15f, 0.1f);

        private Vector3 _pluggedInPosition;

        private void Awake()
        {
            if (plugVisual != null)
                _pluggedInPosition = plugVisual.localPosition;
        }

        protected override void OnSabotaged()
        {
            appliance.SetPower(false);
            if (plugVisual != null)
                plugVisual.localPosition = _pluggedInPosition + unpluggedOffset;
        }

        protected override void OnRestored()
        {
            appliance.SetPower(true);
            if (plugVisual != null)
                plugVisual.localPosition = _pluggedInPosition;
        }
    }
}
