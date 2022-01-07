using System;
using System.Collections.Generic;
using System.Linq;
using GuessingGame.Core.Domain.Game;

namespace GuessingGame
{
    public class GameValidator : IGameValidator
    {
        
        public string[] IsValid(Game game)
        {
            var list = new List<string>();
            if(game.Id<0){
                list.Add("Error: Game må ha gyldig id.");
            }
            if(game.GameMode != GameMode.SingelPlayer && game.GameMode != GameMode.TwoPlayer && game.GameMode != GameMode.MultiPlayer && game.GameMode != GameMode.MultiPlayerOracle){
                list.Add("Error: Please choose a gamemode.");
            }
            if(game.GameStatus != GameStatus.New && game.GameStatus != GameStatus.Active && game.GameStatus != GameStatus.Created && game.GameStatus != GameStatus.Solved && game.GameStatus != GameStatus.Finished){
                list.Add("Error: Game status must be valid.");
            }
            if (game.numplayers > 6)
            {
                list.Add("Error: Not possible to be more than six players.");
            }
            if (game.Players.Count > 6)
            {
                list.Add("Error: Not possible to be more than 6 players.");
            }
            foreach (var _player in game.Players)
            {
                if(string.IsNullOrWhiteSpace(_player.Name)){
                list.Add("Error: Your display name cannot be null og whitespace. Please type in your display name.");
                }
            }
            if (game.numplayers < 1)
            {
                list.Add("Error: Cannot be less than 1 player.");
            }
            if ((game.numplayers == 1 && game.GameMode != GameMode.SingelPlayer))
            {
                list.Add("Error: You can not be 1 player on selected the game mode");
            }
            // I comment this out so that you can play "Multiplayer with oracle" with 2 persons.
            /*if ((game.numplayers == 2) && (game.GameMode != GameMode.TwoPlayer))
            {
                list.Add("Error: You can not be 2 player on selected game mode");
            }*/
            if ((game.numplayers >= 3))
            {
                if (game.GameMode == GameMode.MultiPlayer){}
                else if (game.GameMode == GameMode.MultiPlayerOracle) {}
                else {
                    list.Add("Error: you can not be as this many player on selected the game mode");
                }
            }
            
            return list.ToArray();
            
        }

    }
}