using DumplingKitchen.Core;
using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// A pot, wok or steamer. Needs a TRIGGER collider covering its inside so it knows
    /// which ingredients are in it. A HeatZone passes heat to it; it passes the heat on
    /// to everything inside.
    /// </summary>
    public sealed class CookingVessel : MonoBehaviour
    {
        private readonly TriggerTracker<Ingredient> _contents = new();

        public int IngredientCount => _contents.Count;

        public void ApplyHeat(float heatSeconds)
        {
            _contents.RemoveDestroyed();
            foreach (Ingredient ingredient in _contents.Items)
                ingredient.AddHeat(heatSeconds);
        }

        private void OnTriggerEnter(Collider other) => _contents.Add(other);
        private void OnTriggerExit(Collider other) => _contents.Remove(other);
    }
}
