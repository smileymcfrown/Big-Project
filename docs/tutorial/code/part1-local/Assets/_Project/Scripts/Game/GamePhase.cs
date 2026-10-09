namespace DumplingKitchen.Game
{
    /// <summary>The states of the round's state machine, in the order they happen.</summary>
    public enum GamePhase
    {
        WaitingForPlayers, // dumplings joining, chef getting ready; chef rings the bell to start
        Countdown,         // 3, 2, 1...
        Playing,           // the 5 minute round
        RoundOver          // results on screen, then back to WaitingForPlayers
    }

    public enum Team
    {
        None,
        Chef,
        Dumplings
    }
}
