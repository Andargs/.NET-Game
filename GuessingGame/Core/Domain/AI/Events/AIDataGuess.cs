using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Game;

namespace GuessingGame.Core.Domain.AI.Events
{
	public record AIDataGuess : BaseDomainEvent
	{
		//This is an event where each guess is added to an AIData context, making for easy retrieval and manipulation of data
		//for a data wrangler. It will store the guess the user made, along with tiles available at the time of the guess
		//along with the game id(Which makes finding the image easy), and a boolean which shows if the answer was correct or not.
		public AIDataGuess(string guess, string usedtiles, bool correctanswer, int gameid)
		{
			GameId = gameid;
            Guess = guess;
            UsedTiles = usedtiles;
            CorrectAnswer = correctanswer;
		}

		public int GameId { get; set; }
        public string Guess {get; set;}
        public string UsedTiles {get; set;}
        public bool CorrectAnswer {get; set;}
	}
}