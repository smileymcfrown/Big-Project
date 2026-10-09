using DumplingKitchen.Core;
using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// A trigger volume on top of a burner. Any CookingVessel resting in it gets heat
    /// while the heat source (found on this object or a parent) is on.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class HeatZone : MonoBehaviour
    {
        private readonly TriggerTracker<CookingVessel> _vessels = new();
        private IHeatSource _source;

        private void Awake()
        {
            // Unity can't show interface fields in the Inspector, so we look the source up.
            _source = GetComponentInParent<IHeatSource>();
            if (_source == null)
                Debug.LogError("HeatZone needs an IHeatSource (Stove, ElectricHob...) on itself or a parent.", this);

            GetComponent<Collider>().isTrigger = true;
        }

        private void FixedUpdate()
        {
            if (_source == null || !_source.IsHeating || _vessels.Count == 0)
                return;

            float heat = _source.HeatPerSecond * Time.fixedDeltaTime;
            _vessels.RemoveDestroyed();
            foreach (CookingVessel vessel in _vessels.Items)
                vessel.ApplyHeat(heat);
        }

        private void OnTriggerEnter(Collider other) => _vessels.Add(other);
        private void OnTriggerExit(Collider other) => _vessels.Remove(other);
    }
}
