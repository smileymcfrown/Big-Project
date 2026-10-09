using System;
using System.Collections.Generic;
using DumplingKitchen.Core;
using DumplingKitchen.Game;
using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// The serving hatch. Holds the current order; when the cooked ingredients for that
    /// order are all on the counter at once, they are "served" (removed) and the chef
    /// scores a dish. Needs a TRIGGER collider covering the counter top.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class ServingCounter : MonoBehaviour
    {
        [Tooltip("Orders are served in this order, looping round.")]
        [SerializeField] private RecipeDefinition[] menu = Array.Empty<RecipeDefinition>();
        [SerializeField] private AudioSource servedSound;

        private readonly TriggerTracker<Ingredient> _onCounter = new();
        private readonly List<Ingredient> _matched = new();
        private int _menuIndex;

        public RecipeDefinition CurrentOrder => menu.Length > 0 ? menu[_menuIndex % menu.Length] : null;

        public event Action<RecipeDefinition> OrderChanged;
        public event Action<RecipeDefinition> DishServed;

        private void Awake() => GetComponent<Collider>().isTrigger = true;
        private void OnEnable() => RoundManager.RoundStarting += HandleRoundStarting;
        private void OnDisable() => RoundManager.RoundStarting -= HandleRoundStarting;
        private void Start() => OrderChanged?.Invoke(CurrentOrder);

        private void OnTriggerEnter(Collider other)
        {
            if (_onCounter.Add(other))
                TryServe();
        }

        private void OnTriggerExit(Collider other) => _onCounter.Remove(other);

        private void HandleRoundStarting()
        {
            _menuIndex = 0;
            OrderChanged?.Invoke(CurrentOrder);
        }

        private void TryServe()
        {
            RecipeDefinition order = CurrentOrder;
            if (!RoundManager.IsPlaying || order == null)
                return;

            _onCounter.RemoveDestroyed();
            if (!TryMatch(order, _matched))
                return;

            foreach (Ingredient ingredient in _matched)
                Destroy(ingredient.gameObject);

            if (servedSound != null)
                servedSound.Play();

            RoundManager.Instance.ReportDishServed();
            DishServed?.Invoke(order);

            _menuIndex++;
            OrderChanged?.Invoke(CurrentOrder);
        }

        /// <summary>Finds one cooked ingredient on the counter for each one the recipe needs.</summary>
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
