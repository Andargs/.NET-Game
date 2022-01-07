using System.Threading.Tasks;
using System;
using MediatR;
using GuessingGame.Data;
using System.Collections.Generic;
using GuessingGame.Core.Domain.Game.Pipelines;
using System.Linq;
using GuessingGame.Core.Domain.AI;
using GuessingGame.Core.Domain.AI.Events;
using GuessingGame.Core.Exceptions;
using GuessingGame.Core.Domain.AI.Pipelines;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Player.Events;
using GuessingGame.Core.Domain.Player.Handlers;

namespace GuessingGame.Core.Domain.Game.Services
{
    public interface IOracleService
    {
        Task<int> PlayGame(string guess, int gameid, string user, int proposal);
        Task<int> GetNewTile(int gameId); 
    }

    public class OracleService : IOracleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public OracleService(ApplicationDbContext context, IMediator mediator)
        {
            this._mediator = mediator;
            this._context = context;
        }

        public Oracle Oracle {get; set;}
        public Game Game {get; set;}

        // Mulig vi må endre hva som returneres, men det kan hende vi bare  
        // trenger å sende int for hvilket png som skal vises på game siden
        public async Task<int> PlayGame(string guess, int gameid, string user, int proposal)
        {
            var Game = await _mediator.Send(new GetGame.Request(gameid));
            if (guess == Game.Image.ImageName)
            {
                Game.GameStatus = GameStatus.Finished;
                await _mediator.Publish(new CalculateSingleScore(gameid, user));
                await _context.SaveChangesAsync();
                await _mediator.Publish(new AIDataGuess(guess, Game.UsedTiles, true, gameid));
                await _context.SaveChangesAsync();
                return await Task.FromResult(0);
            }

            if (guess!= Game.Image.ImageName)
            {
                var player = await _mediator.Send(new GetPlayerByGameIdName.Request(gameid, user));
                player.AvailableAttempts--;
                await _mediator.Publish(new AIDataGuess(guess, Game.UsedTiles, false, gameid));
                if(player.AvailableAttempts == 0)
                {
                    var tile = await GetNewTile(gameid);
                    player.AvailableAttempts = 3;
                    await _context.SaveChangesAsync();
                    return await Task.FromResult(0);
                }
                await _context.SaveChangesAsync();
                return await Task.FromResult(0); 
            }
            return await Task.FromResult(0);
        }

        public async Task<int> GetNewTile(int gameId)
        {
            var Game = await _mediator.Send(new GetGame.Request(gameId));
            var imgid = Game.Image.Id;
            int numTiles = System.IO.Directory.GetFiles("./wwwroot/img/"+Game.Image.ImageMap+"_scattered").Length -1;
            if (Game.UsedTiles.Split(" ").Length == numTiles){
                Game.GameStatus = GameStatus.Finished;
                Game.ActivePlayerIndex = 0;
                return await Task.FromResult(0); 
            }
            try
            {
                // Tries to retrieve AIData with the same image as the current game
                var AiData = await _mediator.Send(new GetAIDataByImageId.Request(imgid));
                if (AiData is null)
                { //If no game has been played with the same image
                    throw new EntityNotFoundException("AIData for image not found");
                }
                if (AiData.OptimalUserTiles is null)
                { //If no game with the same image has finished and chosen optimal tiles.
                    throw new EntityNotFoundException("Game not finished");
                }
                var Optimaltiles = AiData.OptimalUserTiles.Split(" ");

                // Checks if the game has used all the optimal tiles in the game it used as a baseline
                if (Optimaltiles.Length >= Game.UsedTiles.Length){  
                    HashSet<int> tilesToExclude = new HashSet<int>();
                    string[] tilesString =Game.UsedTiles.Split(' ');
                    for(int i=0;tilesString.Length>i;i++)
                    {
                        tilesToExclude.Add(Int32.Parse(tilesString[i]));
                    }
                    HashSet<int> tilesToCompare = new HashSet<int>();
                    for(int i=0;Optimaltiles.Length>i;i++)
                    {
                        tilesToCompare.Add(Int32.Parse(Optimaltiles[i]));
                    }
                    var ToUseList = tilesToCompare.Except(tilesToExclude);
                    int newTile = ToUseList.ElementAt(0);
                    Game.UsedTiles += " "+newTile.ToString();
                    return await Task.FromResult(0);
                } 
                else
                {  //If there are no more optimal tiles, the tiles will be chosen at random while excluding the other tiles
                    //int numTiles = System.IO.Directory.GetFiles("./wwwroot/img/"+Game.Image.ImageMap+"_scattered").Length -1;  
                    HashSet<int> tilesToExclude = new HashSet<int>();
                    string[] tilesString =Game.UsedTiles.Split(' ');
                    for(int i=0;tilesString.Length>i;i++)
                    {
                        tilesToExclude.Add(Int32.Parse(tilesString[i]));
                    }
                    var availabeTiles = Enumerable.Range(0,numTiles).Where(i => !tilesToExclude.Contains(i));
                    
                        Random r = new System.Random();
                    int tileIndex = r.Next(0, numTiles - tilesToExclude.Count);
                    int newTile = availabeTiles.ElementAt(tileIndex);
                    Game.UsedTiles += " "+newTile.ToString();
                    await _context.SaveChangesAsync();
                    return await Task.FromResult(0); 
                    
                   
                }
            }
            catch (EntityNotFoundException) //No game with the current image was found, does random selection
            { 
                //int numTiles = System.IO.Directory.GetFiles("./wwwroot/img/"+Game.Image.ImageMap+"_scattered").Length -1;  
                HashSet<int> tilesToExclude = new HashSet<int>();
                string[] tilesString =Game.UsedTiles.Split(' ');
                for(int i=0;tilesString.Length>i;i++)
                {
                    tilesToExclude.Add(Int32.Parse(tilesString[i]));
                }
                var availabeTiles = Enumerable.Range(0,numTiles).Where(i => !tilesToExclude.Contains(i));
                Random r = new System.Random();
                int tileIndex = r.Next(0, numTiles - tilesToExclude.Count);
                int newTile = availabeTiles.ElementAt(tileIndex);
                Game.UsedTiles += " "+newTile.ToString();
                await _context.SaveChangesAsync();
                return await Task.FromResult(0);
            }
        }
    }
}