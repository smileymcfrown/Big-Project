using System.Collections.Generic;
using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>A dish: a name and the cooked ingredients it needs (repeats allowed).</summary>
    [CreateAssetMenu(fileName = "Recipe", menuName = "Dumpling Kitchen/Recipe")]
    public sealed class RecipeDefinition : ScriptableObject
    {
        [SerializeField] private string dishName = "Stir-fried Vegetables";
        [SerializeField] private List<IngredientDefinition> ingredients = new();

        public string DishName => dishName;
        public IReadOnlyList<IngredientDefinition> Ingredients => ingredients;
    }
}
