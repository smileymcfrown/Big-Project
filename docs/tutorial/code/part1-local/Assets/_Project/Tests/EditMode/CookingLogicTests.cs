using DumplingKitchen.Cooking;
using NUnit.Framework;

namespace DumplingKitchen.Tests.EditMode
{
    public sealed class CookingLogicTests
    {
        private const float ToCook = 8f;
        private const float ToBurn = 10f;

        [TestCase(0f, CookState.Raw)]
        [TestCase(7.9f, CookState.Raw)]
        [TestCase(8f, CookState.Cooked)]
        [TestCase(17.9f, CookState.Cooked)]
        [TestCase(18f, CookState.Burnt)]
        [TestCase(100f, CookState.Burnt)]
        public void Evaluate_ReturnsStateForHeat(float heatSeconds, CookState expected)
        {
            Assert.That(CookingLogic.Evaluate(heatSeconds, ToCook, ToBurn), Is.EqualTo(expected));
        }
    }
}
