using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using GuessingGame.Data;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Player.Events;

namespace GuessingGame.Core.Domain.Player.Handlers
{
    public class CalculateSingleScoreHandler : INotificationHandler<CalculateSingleScore>
    {
        private readonly ApplicationDbContext _db;
        private readonly IMediator _mediator;

        public CalculateSingleScoreHandler(ApplicationDbContext db, IMediator mediator){
			_db = db ?? throw new System.ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }
        
        public async Task Handle(CalculateSingleScore notification, CancellationToken cancellationToken)
		{ //Calculates score based on gamemode and the user who sent the correct guess.
            var player = await _mediator.Send(new GetPlayerByGameIdName.Request(notification.Id, notification.Name));
            var game = await _mediator.Send(new GetGame.Request(notification.Id));
            if (game.GameMode == GuessingGame.Core.Domain.Game.GameMode.SingelPlayer) {
                //Calculates the score in the same way as the singleplayer method, only here both players will recieve the same score
                var list = game.Players;
                var player1 = list[0];
                string[] complist = game.UsedTiles.Split(' ');
                var max_score = 150;
                var points = complist.Length*3;
                max_score -= points;
                max_score += 3; //Adds 3 points to make sure max score is still achievable
                max_score -= 3 - player1.AvailableAttempts;
                player1.Score = max_score;
                await _db.SaveChangesAsync(cancellationToken);
            }
            if (game.GameMode == GuessingGame.Core.Domain.Game.GameMode.TwoPlayer){
                //Calculates the score in the same way as the singleplayer method, only here both players will recieve the same score due to 1 being proposer,
                //And the other being a guesser.
                var list = game.Players;
                var player1 = list[0];
                var player2 = list[1];
                string[] complist = game.UsedTiles.Split(' ');
                var max_score = 150;
                var points = complist.Length*3;
                max_score -= points;
                max_score += 3; //Adds 3 points to make sure max score is still achievable
                max_score -= 3 - player2.AvailableAttempts;
                player1.Score = max_score;
                player2.Score = max_score;
                await _db.SaveChangesAsync(cancellationToken);
            }
            if (game.GameMode == GuessingGame.Core.Domain.Game.GameMode.MultiPlayer || game.GameMode == GuessingGame.Core.Domain.Game.GameMode.MultiPlayerOracle){
                //Calculates the score of the winning user, while setting all other users in the game to 0
                //If however this is a multiplayer game with a human proposer, the proposer will recieve the same score as the winner.
                string[] complist = game.UsedTiles.Split(' ');
                var max_score = 150;
                var points = complist.Length*3;
                max_score -= points;
                max_score += 3; //Adds 3 points to make sure max score is still achievable
                max_score -= 3 - player.AvailableAttempts;
                player.Score = max_score;
                await _db.SaveChangesAsync(cancellationToken);

                foreach(var players in game.Players){
                if (players.Score >0){
                    
                } else {
                    if (players.IsProposer){
                        players.Score = max_score;
                    }else {
                        players.Score = 0;
                    }
                }
                await _db.SaveChangesAsync(cancellationToken);
            } 
            }
		}

    }

}