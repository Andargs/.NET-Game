using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Game;

namespace GuessingGame.Core.Domain.Game.Events
{
	public record GameCreated : BaseDomainEvent
	{
		public GameCreated(int gameId)
		{
			GameId = gameId;
		}

		public int GameId { get; set; }
        
	}
}