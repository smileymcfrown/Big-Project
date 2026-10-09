using DumplingKitchen.Core;
using DumplingKitchen.Net;
using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// NETWORKED version: identical to Part 1 except for ONE line. Cooking is a game
    /// rule, so only the authority (the chef's PC) runs it. Clients still have the
    /// component, it just does nothing there.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class HeatZone : MonoBehaviour
    {
        private readonly TriggerTracker<CookingVessel> _vessels = new();
        private IHeatSource _source;

        private void Awake()
        {
            _source = GetComponentInParent<IHeatSource>();
            if (_source == null)
                Debug.LogError("HeatZone needs an IHeatSource (Stove, ElectricHob...) on itself or a parent.", this);

            GetComponent<Collider>().isTrigger = true;
        }

        private void FixedUpdate()
        {
            if (!Authority.IsAuthority) // <- the one new line
                return;

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
