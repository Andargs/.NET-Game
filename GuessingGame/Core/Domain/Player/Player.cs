using System;
using System.Collections.Generic;
using GuessingGame.SharedKernel;

namespace GuessingGame.Core.Domain.Player
{
    public class Player : BaseEntity
    {
        private Player(){}
        public Player(string name, string userid, int Attempts){
            Name = name;
            UserId = userid;
            AvailableAttempts = Attempts;
        }
        public int Id {get;set;}
        public string UserId {get;set;}  //Related identity userid
        public string Name {get;set;}  //Chosen display name
        public int GameId {get; set;}  //Related GameId
        public int AvailableAttempts {get;set;}  //Remaining available attempts
        public Team Team {get;set;}   //The players team if gamemode == Multiplayer
        public bool IsProposer {get; set;}   //Wether the player is a proposer or not
        public int Score {get;set;}   //The players score in a specific game
    }
}