using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Game;

namespace GuessingGame.Core.Domain.AI.Events
{
	public record CreateAIData : BaseDomainEvent
	{
		//Creates AIData which will be linked to a specific game id, and an image id. This will make use of the data for each
		//Game or image easy to handle.
		public CreateAIData(int gameid, int imgid)
		{
			GameId = gameid;
			ImageId = imgid;
		}

		public int GameId { get; set; }
		public int ImageId {get; set;}
	}
}