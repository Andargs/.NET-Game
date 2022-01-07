using System;
using System.Collections.Generic;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Player;

namespace GuessingGame
{
    public interface IGame
    {
        public int Id {get; set;}
        public int ActivePlayerIndex {get; set;}
        public GameMode GameMode {get; set;}
        public GameStatus GameStatus {get;set;}
        public List<Player> Players {get; set;}
        public GuessingGame.Core.Domain.Image.Image Image { get; set; }
        public int numplayers {get; set;}
        public string UsedTiles {get; set;}  //unsupported type, må muligens splittes opp 
        public List<int> GetComponents(){
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
