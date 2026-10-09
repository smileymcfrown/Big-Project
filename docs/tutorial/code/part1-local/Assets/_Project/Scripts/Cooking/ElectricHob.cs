using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// An electric hob / rice cooker. It is BOTH an ElectricAppliance (inheritance: it
    /// can be unplugged) AND an IHeatSource (interface: pots can cook on it).
    /// </summary>
    public sealed class ElectricHob : ElectricAppliance, IHeatSource
    {
        [SerializeField, Min(0f)] private float heatPerSecond = 1.5f;

        [Header("Presentation (optional)")]
        [SerializeField] private Renderer glowRenderer;
        [SerializeField] private Light glowLight;

        public bool IsHeating => HasPower;
        public float HeatPerSecond => heatPerSecond;

        private void Start() => OnPowerChanged(HasPower);

        protected override void OnPowerChanged(bool hasPower)
        {
            if (glowRenderer != null) glowRenderer.enabled = hasPower;
            if (glowLight != null) glowLight.enabled = hasPower;
        }
    }
}
