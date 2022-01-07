namespace GuessingGame.Core.Domain.Game
{
    public enum GameMode
    {
        SingelPlayer = 0,  //Singleplayer gamemode
        TwoPlayer = 1,  // Twoplayer
        MultiPlayer = 2, //Multiplayer with human proposer
        MultiPlayerOracle = 3  //Multiplayer with oracle proposer
    }
}