using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// A gas burner. It heats while gas is supplied. The GasValve sabotage target
    /// cuts the supply; the stove itself knows nothing about dumplings.
    /// </summary>
    public sealed class Stove : MonoBehaviour, IHeatSource
    {
        [SerializeField, Min(0f)] private float heatPerSecond = 1f;

        [Header("Presentation (optional)")]
        [SerializeField] private ParticleSystem flame;
        [SerializeField] private AudioSource burnerAudio;

        private bool _hasGas = true;

        public bool IsHeating => _hasGas;
        public float HeatPerSecond => heatPerSecond;

        private void Start() => RefreshPresentation();

        public void SetGasSupply(bool hasGas)
        {
            if (_hasGas == hasGas)
                return;

            _hasGas = hasGas;
            RefreshPresentation();
        }

        private void RefreshPresentation()
        {
            if (flame != null)
            {
                if (_hasGas) flame.Play();
                else flame.Stop();
            }

            if (burnerAudio != null)
            {
                if (_hasGas) burnerAudio.Play();
                else burnerAudio.Stop();
            }
        }
    }
}
