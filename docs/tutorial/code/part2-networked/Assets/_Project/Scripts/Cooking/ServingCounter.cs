using System;
using System.Collections.Generic;
using DumplingKitchen.Core;
using DumplingKitchen.Game;
using Unity.Netcode;
using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// NETWORKED version. Changes from Part 1:
    ///  - the menu position is a NetworkVariable, so everyone's order ticket matches,
    ///  - only the server decides a dish is served,
    ///  - served ingredients are Despawned (removed on every machine), not Destroyed.
    /// Needs a NetworkObject (it's placed in the scene).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class ServingCounter : NetworkBehaviour
    {
        [SerializeField] private RecipeDefinition[] menu = Array.Empty<RecipeDefinition>();
        [SerializeField] private AudioSource servedSound;

        private readonly NetworkVariable<int> _menuIndex = new();
        private readonly TriggerTracker<Ingredient> _onCounter = new();
        private readonly List<Ingredient> _matched = new();

        public RecipeDefinition CurrentOrder => menu.Length > 0 ? menu[_menuIndex.Value % menu.Length] : null;

        public event Action<RecipeDefinition> OrderChanged;
        public event Action<RecipeDefinition> DishServed;

        private void Awake() => GetComponent<Collider>().isTrigger = true;
        private void OnEnable() => RoundManager.RoundStarting += HandleRoundStarting;
        private void OnDisable() => RoundManager.RoundStarting -= HandleRoundStarting;

        public override void OnNetworkSpawn()
        {
            _menuIndex.OnValueChanged += HandleMenuIndexChanged;
            OrderChanged?.Invoke(CurrentOrder);
        }

        public override void OnNetworkDespawn() => _menuIndex.OnValueChanged -= HandleMenuIndexChanged;

        private void OnTriggerEnter(Collider other)
        {
            if (IsServer && _onCounter.Add(other))
                TryServe();
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsServer)
                _onCounter.Remove(other);
        }

        private void HandleRoundStarting()
        {
            if (IsServer)
                _menuIndex.Value = 0;
        }

        private void HandleMenuIndexChanged(int previous, int next) => OrderChanged?.Invoke(CurrentOrder);

        private void TryServe()
        {
            RecipeDefinition order = CurrentOrder;
            if (!RoundManager.IsPlaying || order == null)
                return;

            _onCounter.RemoveDestroyed();
            if (!TryMatch(order, _matched))
                return;

            foreach (Ingredient ingredient in _matched)
                ingredient.NetworkObject.Despawn(destroy: true);

            PlayServedFeedbackRpc();
            RoundManager.Instance.ReportDishServed();
            DishServed?.Invoke(order);
            _menuIndex.Value++;
        }

        // A one-off EVENT (not state), so an RPC to everyone is the right tool.
        [Rpc(SendTo.Everyone)]
        private void PlayServedFeedbackRpc()
        {
            if (servedSound != null)
                servedSound.Play();
        }

        private bool TryMatch(RecipeDefinition recipe, List<Ingredient> results)
        {
            results.Clear();
            foreach (IngredientDefinition needed in recipe.Ingredients)
            {
                Ingredient found = null;
                foreach (Ingredient candidate in _onCounter.Items)
                {
                    if (candidate.Definition == needed
                        && candidate.State == CookState.Cooked
                        && !results.Contains(candidate))
                    {
                        found = candidate;
                        break;
                    }
                }

                if (found == null)
                    return false;

                results.Add(found);
            }

            return results.Count > 0;
        }
    }
}
