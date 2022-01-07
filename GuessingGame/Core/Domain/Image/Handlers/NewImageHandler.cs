using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Game.Events;
using GuessingGame.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GuessingGame.Core.Domain.Game;

namespace GuessingGame.Core.Domain.Image.Handlers
{
    public class NewImageHandler : INotificationHandler<NewImage>
    {
        private readonly ApplicationDbContext _db;
        private readonly IMediator _mediator;

        public NewImageHandler(ApplicationDbContext db, IMediator mediator){
            _db = db ?? throw new System.ArgumentNullException(nameof(db));
                _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task Handle(NewImage notification, CancellationToken cancellationToken)
        {   
            var image = await _db.Images.FirstOrDefaultAsync(i => i.ImageId == notification.ImageId);
            
            var game = await _db.Games.OrderByDescending(g => g.Id).FirstOrDefaultAsync();
            
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}