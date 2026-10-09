using System.Text;
using DumplingKitchen.Cooking;
using TMPro;
using UnityEngine;

namespace DumplingKitchen.UI
{
    /// <summary>The order ticket pinned above the pass: what the chef must cook next.</summary>
    public sealed class OrderTicketDisplay : MonoBehaviour
    {
        [SerializeField] private ServingCounter counter;
        [SerializeField] private TMP_Text ticketText;

        private readonly StringBuilder _builder = new();

        private void OnEnable()
        {
            counter.OrderChanged += Refresh;
            Refresh(counter.CurrentOrder);
        }

        private void OnDisable() => counter.OrderChanged -= Refresh;

        private void Refresh(RecipeDefinition order)
        {
            if (order == null)
            {
                ticketText.text = "No orders";
                return;
            }

            _builder.Clear();
            _builder.AppendLine($"<b>{order.DishName}</b>");
            foreach (IngredientDefinition ingredient in order.Ingredients)
                _builder.AppendLine($"- cooked {ingredient.DisplayName}");

            ticketText.text = _builder.ToString();
        }
    }
}
