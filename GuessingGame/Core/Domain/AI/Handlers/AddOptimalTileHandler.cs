using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using GuessingGame.Data;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.AI;
using GuessingGame.Core.Domain.AI.Events;
using GuessingGame.Core.Domain.AI.Pipelines;

namespace GuessingGame.Core.Domain.AI.Handlers
{
    public class AddOptimalTileHandler : INotificationHandler<AddOptimalTile>
    {
        private readonly ApplicationDbContext _db;
        private readonly IMediator _mediator;

        public AddOptimalTileHandler(ApplicationDbContext db, IMediator mediator){
			_db = db ?? throw new System.ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }
        
        public async Task Handle(AddOptimalTile notification, CancellationToken cancellationToken)
		{ //This method takes the game in question, and adds optimal tiles selected by user to the optimalusertiles. 
            //This might come in handy for an ai. There might be a difference in what players say the optimal tiles would be,
            //and what they actually are.
            var CurrentAI = await _mediator.Send(new GetAIDataByGameId.Request(notification.GameId));
            var NewTiles = CurrentAI.OptimalUserTiles;
            if (NewTiles is null){
                NewTiles = notification.Tile;
                CurrentAI.OptimalUserTiles = NewTiles;
                await _db.SaveChangesAsync(cancellationToken);
            } else {
                NewTiles = NewTiles + $" {notification.Tile}";
                CurrentAI.OptimalUserTiles = NewTiles;
                await _db.SaveChangesAsync(cancellationToken);
            }
		}

    }

}