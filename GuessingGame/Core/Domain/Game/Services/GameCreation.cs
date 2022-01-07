using System.Threading.Tasks;
using System;
using MediatR;
using GuessingGame.Data;
using System.Collections.Generic;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Game.Events;
using GuessingGame.Core.Domain.AI.Events;

namespace GuessingGame.Core.Domain.Game.Services
{
    public interface IGameCreation
    {
        Task<int> CreateGame(string displayname, string userid, string mode, int numplayers);
    }

    public class GameCreation : IGameCreation
    {
        private readonly ApplicationDbContext context;
        private readonly IMediator _mediator;

        public GameCreation(ApplicationDbContext context, IMediator mediator)
        {
            this._mediator = mediator;
            this.context = context;
        }

        //Spiller legges i liste, Isproposer settes til false(Må endres etter vi får lagt inn two player)
        //Hva som skal skje bestemmes av Gamemode, lager gamemode og gjør klart for oracle.
        //num players er antall spillere til senere
        //om det er mer enn en player og mode er singleplayer vil den returnere til siden uten å lage game
        public async Task<int> CreateGame(string displayname, string userid, string mode, int numplayers)
        {
            var players = new List<GuessingGame.Core.Domain.Player.Player>();
            var player = new GuessingGame.Core.Domain.Player.Player(displayname, userid, 3);
            player.IsProposer = false;
            players.Add(player);
            context.Players.Add(player);

            switch(mode)
            {
                case "singleplayer":
                    var CurrentGameMode = GameMode.SingelPlayer;
                    var game = new Game(players, CurrentGameMode);
                    game.numplayers = numplayers;
                    context.Games.Add(game);
                    break;
                case "twoplayer":
                    CurrentGameMode = GameMode.TwoPlayer;
                    player.IsProposer = true;
                    game = new Game(players, CurrentGameMode);
                    game.numplayers = numplayers;
                    context.Games.Add(game);
                    break;
                case "multiplayer":
                    CurrentGameMode = GameMode.MultiPlayer;
                    player.IsProposer = true;
                    game = new Game(players, CurrentGameMode);
                    game.numplayers = numplayers;
                    context.Games.Add(game);
                    break;
                case "omultiplayer" :
                    CurrentGameMode = GameMode.MultiPlayerOracle;
                    player.IsProposer = false;
                    game = new Game(players, CurrentGameMode);
                    game.numplayers = numplayers;
                    context.Games.Add(game);
                    break;       
            }

            await context.SaveChangesAsync();
            var GameId = await _mediator.Send(new GetGameId.Request());
            player.GameId = GameId;
            await _mediator.Publish(new GameCreated(GameId));
            await context.SaveChangesAsync();
            var Game = await _mediator.Send( new GetGame.Request(GameId));
            int imgid1 = Game.Image.Id;
            await _mediator.Publish(new CreateAIData(Game.Id, imgid1));
            return await Task.FromResult(0);  
        }
    }
}