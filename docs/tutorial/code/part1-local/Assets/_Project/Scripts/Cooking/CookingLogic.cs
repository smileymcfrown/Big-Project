namespace DumplingKitchen.Cooking
{
    public enum CookState
    {
        Raw,
        Cooked,
        Burnt
    }

    /// <summary>
    /// The rules of cooking as a pure function: same inputs, same output, no Unity.
    /// Pure functions are the easiest code in a game to unit test, so we pull the
    /// rules out of the MonoBehaviour and keep the MonoBehaviour thin.
    /// </summary>
    public static class CookingLogic
    {
        public static CookState Evaluate(float heatSeconds, float secondsToCook, float secondsToBurn)
        {
            if (heatSeconds >= secondsToCook + secondsToBurn)
                return CookState.Burnt;

            if (heatSeconds >= secondsToCook)
                return CookState.Cooked;

            return CookState.Raw;
        }
    }
}
