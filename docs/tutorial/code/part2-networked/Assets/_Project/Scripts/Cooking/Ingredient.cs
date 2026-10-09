using System;
using Unity.Netcode;
using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// NETWORKED version. The server counts the heat; only the resulting CookState is
    /// replicated, because that's all the dumplings need to see (raw/cooked/burnt).
    /// Send the least data that still lets every player see the same game.
    ///
    /// Prefab: NetworkObject, NetworkTransform (Authority Mode = Server),
    /// NetworkRigidbody, XR Grab Interactable, this component.
    /// </summary>
    public sealed class Ingredient : NetworkBehaviour
    {
        [SerializeField] private IngredientDefinition definition;
        [SerializeField] private Renderer meshRenderer;

        private readonly NetworkVariable<CookState> _state = new(CookState.Raw);
        private float _heatSeconds; // server only, never sent

        public IngredientDefinition Definition => definition;
        public CookState State => _state.Value;

        public event Action<Ingredient, CookState> StateChanged;

        private void Awake()
        {
            if (meshRenderer == null)
                meshRenderer = GetComponentInChildren<Renderer>();
        }

        public override void OnNetworkSpawn()
        {
            _state.OnValueChanged += HandleStateValueChanged;
            ApplyLook();
        }

        public override void OnNetworkDespawn() => _state.OnValueChanged -= HandleStateValueChanged;

        /// <summary>Server only (HeatZone only runs on the server).</summary>
        public void AddHeat(float heatSeconds)
        {
            if (!IsServer || heatSeconds <= 0f || State == CookState.Burnt)
                return;

            _heatSeconds += heatSeconds;
            _state.Value = CookingLogic.Evaluate(_heatSeconds, definition.SecondsToCook, definition.SecondsToBurn);
        }

        private void HandleStateValueChanged(CookState previous, CookState next)
        {
            ApplyLook();
            StateChanged?.Invoke(this, next);
        }

        private void ApplyLook()
        {
            if (meshRenderer == null || definition == null)
                return;

            Material material = definition.MaterialFor(State);
            if (material != null)
                meshRenderer.sharedMaterial = material;
        }
    }
}
