using System;
using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Player;
using System.Collections.Generic;
using GuessingGame.Core.Domain.Game.Events;
using Microsoft.EntityFrameworkCore;
using GuessingGame.Core.Domain.Image;

namespace GuessingGame.Core.Domain.Game
{
    public class Game : BaseEntity 
    {
        private Game(){}
        public Game(List<Player.Player> players, GameMode gameMode){
            GameMode = gameMode;
            Players = players;
            if(GameMode == GameMode.SingelPlayer)
            {
                GameStatus = GameStatus.Active;
            }else
            {
                GameStatus = GameStatus.Created;
            }
            ActivePlayerIndex = 0;
            GameDate = DateTime.Now;
        }
        public int Id {get; set;}
        public int ActivePlayerIndex {get; set;}  //An index for the active player
        public DateTime GameDate { get; set; }
        public GameMode GameMode {get; set;}  //Enum which defines which gamemode is selected.
        public GameStatus GameStatus {get;set;}  //Enum which defines the status of the game for different events.
        public List<Player.Player> Players {get; set;}  //List of players
        public GuessingGame.Core.Domain.Image.Image Image { get; set; }  //Image for specified game
        public int numplayers {get; set;}  // number of players
        public string UsedTiles {get; set;}    //Currently used tiles in the game

        public List<int> GetComponents(){  //Gets components for a game.
            List<int> listComponents = new List<int>();
            string[]splitComponents = UsedTiles.Split(" ");
            foreach(string component in splitComponents)
            {
                int intComponent = Int32.Parse(component);
                listComponents.Add(intComponent);
            }
            return listComponents;
        }
    }
}