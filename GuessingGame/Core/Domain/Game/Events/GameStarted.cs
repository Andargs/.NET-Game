using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Game;

namespace GuessingGame.Core.Domain.Game.Events
{
	public record GameStarted : BaseDomainEvent
	{
		public GameStarted(int gameId, int[] UserIds)
		{
			ImageId = gameId;
            Users =  UserIds;
		}

		public int ImageId { get; set; }
        public int[] Users {get; set;}
	}
}