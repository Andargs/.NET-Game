namespace GuessingGame.Core.Domain.Game
{
    public enum GameStatus
    {
        New = 0,
        Created = 1,  //Created but not enough players to play the game
        Active = 2,  //Not finished
        Solved = 3,  //Guessed correctly but not proposed optimal tiles
        Finished = 4  //Game finished and optimal tiles proposed
    }
}