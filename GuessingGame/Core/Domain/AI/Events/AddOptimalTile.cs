using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Game;

namespace GuessingGame.Core.Domain.AI.Events
{
	public record AddOptimalTile : BaseDomainEvent
	{
		//An event taking in game id and the tile number as a string. This will help set the optimal tile for each image.
		public AddOptimalTile(int gameid, string tile)
		{
			GameId = gameid;
            Tile = tile;
		}

		public int GameId { get; set; }
        public string Tile {get; set;}
	}
}