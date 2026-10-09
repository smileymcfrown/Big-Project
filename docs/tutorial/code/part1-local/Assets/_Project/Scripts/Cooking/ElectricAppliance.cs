using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// Base class for anything that plugs into the wall: hob, rice cooker, kettle, radio.
    /// The PowerPlug sabotage target talks to THIS type, so every new appliance you
    /// make by inheriting from it can be unplugged for free.
    /// </summary>
    public abstract class ElectricAppliance : MonoBehaviour
    {
        public bool HasPower { get; private set; } = true;

        public void SetPower(bool hasPower)
        {
            if (HasPower == hasPower)
                return;

            HasPower = hasPower;
            OnPowerChanged(hasPower);
        }

        /// <summary>Subclasses decide what losing power looks and sounds like.</summary>
        protected abstract void OnPowerChanged(bool hasPower);
    }
}
