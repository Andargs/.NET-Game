using System.Collections.Generic;
using GuessingGame.Core.Domain.Game;

namespace GuessingGame
{
    public interface IGameValidator
    {
        string[] IsValid(Game game);
        
    }
}
