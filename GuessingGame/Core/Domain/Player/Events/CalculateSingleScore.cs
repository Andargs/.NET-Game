
using System;
using System.Collections.Generic;
using GuessingGame.SharedKernel;

namespace GuessingGame.Core.Domain.Player.Events
{
	public record CalculateSingleScore : BaseDomainEvent
	{
		//Event that takes in the game id and the person who solved the puzzle.
		public CalculateSingleScore(int GameId, string name){
			Id = GameId;
            Name = name;

		}

		public int Id {get; }
        public string Name {get;}
	}

}