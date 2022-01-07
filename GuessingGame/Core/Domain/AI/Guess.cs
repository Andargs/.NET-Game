using System;
using System.Collections.Generic;
using GuessingGame.SharedKernel;

namespace GuessingGame.Core.Domain.AI
{
    public class Guess
    {
        private Guess(){}
        public Guess(string guess, string usedtiles, bool correct){
            NewGuess = guess;
            UsedTiles = usedtiles;
            Correct = correct;
        }
        public int Id {get;set;}
        public int AIid {get;set;}  //Set to the AIData id.
        public string UsedTiles {get; set;}  //Used tiles on guess
        public string NewGuess {get;set;}  //The users guess
        public bool Correct {get; set;}  // Wether the guess was correct or not
    }
}