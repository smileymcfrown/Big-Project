namespace DumplingKitchen.Cooking
{
    /// <summary>
    /// Anything that can cook food: a gas stove, an electric hob, a rice cooker...
    /// Pots don't care WHICH kind of heat source they sit on, only whether it is hot.
    /// That is the point of an interface: code against the capability, not the class.
    /// </summary>
    public interface IHeatSource
    {
        bool IsHeating { get; }

        /// <summary>Cooking-seconds added per real second. 1 = normal speed.</summary>
        float HeatPerSecond { get; }
    }
}
