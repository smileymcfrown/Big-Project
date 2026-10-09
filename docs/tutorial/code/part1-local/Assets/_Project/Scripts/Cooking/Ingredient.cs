using System;
using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// One physical piece of food. Put this on a grabbable prop (XR Grab Interactable)
    /// together with a Rigidbody and a collider.
    /// </summary>
    public sealed class Ingredient : MonoBehaviour
    {
        [SerializeField] private IngredientDefinition definition;
        [SerializeField] private Renderer meshRenderer;

        private float _heatSeconds;

        public IngredientDefinition Definition => definition;
        public CookState State { get; private set; } = CookState.Raw;

        public event Action<Ingredient, CookState> StateChanged;

        private void Awake()
        {
            if (meshRenderer == null)
                meshRenderer = GetComponentInChildren<Renderer>();

            ApplyLook();
        }

        /// <summary>Called by whatever is heating us (a pot on a stove).</summary>
        public void AddHeat(float heatSeconds)
        {
            if (heatSeconds <= 0f || State == CookState.Burnt)
                return;

            _heatSeconds += heatSeconds;
            CookState next = CookingLogic.Evaluate(_heatSeconds, definition.SecondsToCook, definition.SecondsToBurn);
            if (next == State)
                return;

            State = next;
            ApplyLook();
            StateChanged?.Invoke(this, next);
        }

        private void ApplyLook()
        {
            if (meshRenderer == null || definition == null)
                return;

            Material material = definition.MaterialFor(State);
            if (material != null)
                meshRenderer.sharedMaterial = material; // sharedMaterial: no per-object material copies
        }
    }
}
