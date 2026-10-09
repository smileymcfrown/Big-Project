using UnityEngine;

namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// DATA about a kind of ingredient (what a "carrot" is), shared by every carrot
    /// in the scene. The Ingredient component holds the per-object STATE (how cooked
    /// THIS carrot is). Splitting data from state is a ScriptableObject's main job.
    /// </summary>
    [CreateAssetMenu(fileName = "Ingredient", menuName = "Dumpling Kitchen/Ingredient")]
    public sealed class IngredientDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Carrot";
        [SerializeField, Min(0.1f)] private float secondsToCook = 8f;
        [Tooltip("Extra seconds of heat after cooking before it burns.")]
        [SerializeField, Min(0.1f)] private float secondsToBurn = 10f;

        [Header("Look per state")]
        [SerializeField] private Material rawMaterial;
        [SerializeField] private Material cookedMaterial;
        [SerializeField] private Material burntMaterial;

        public string DisplayName => displayName;
        public float SecondsToCook => secondsToCook;
        public float SecondsToBurn => secondsToBurn;

        public Material MaterialFor(CookState state) => state switch
        {
            CookState.Cooked => cookedMaterial,
            CookState.Burnt => burntMaterial,
            _ => rawMaterial
        };
    }
}
