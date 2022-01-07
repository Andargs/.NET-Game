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

namespace GuessingGame.Core.Domain.AI.Handlers
{
    public class CreateAIHandler : INotificationHandler<CreateAIData>
    {
        private readonly ApplicationDbContext _db;
        private readonly IMediator _mediator;

        public CreateAIHandler(ApplicationDbContext db, IMediator mediator){
			_db = db ?? throw new System.ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }
        
        public async Task Handle(CreateAIData notification, CancellationToken cancellationToken)
		{ // This create the basic AIData object. To make it easily trackable to the game and or image in question, it takes in an image id and a game id
            var ai = new AIData(notification.GameId);
            ai.ImageId = notification.ImageId;
            ai.Correct = false;
            _db.Add(ai);
            await _db.SaveChangesAsync(cancellationToken);
		}

    }

}