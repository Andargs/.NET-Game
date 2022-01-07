using System;
using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Player;
using System.Collections.Generic;
using GuessingGame.Core.Domain.Game.Events;
using Microsoft.EntityFrameworkCore;
using GuessingGame.Core.Domain.Image;

namespace GuessingGame.Core.Domain.AI
{
    public class AIData : BaseEntity 
    {
        private AIData(){}
        public AIData(int gameId){
            GameId = gameId;
        }
        public int Id {get; set;}
        public int GameId {get; set;}   //Set to gameid for the related game
        public int ImageId {get; set;}  //Imageid is used to make retrieval of all data on one specific image easy
        public List<Guess> Guesses {get; set;} //AI guess, Used tiles when guess occured, Correct or not
        public int Attempts {get; set;}    //incremented for each guess
        public Boolean Correct {get; set;}  //Set to true if the last guess is correct
        public string OptimalUserTiles {get; set;}      //This is set to the optimal tiles the user selects after finishing the game.
        //This will help the data wrangler look at percieved differences in what a person says would be the optimal tiles, and what actually is.

    }
}