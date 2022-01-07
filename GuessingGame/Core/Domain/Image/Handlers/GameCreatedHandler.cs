using GuessingGame.Core.Domain.Game.Events;
using MediatR;
using System.Collections.Generic;
using GuessingGame.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Image.Pipelines;
using System.Linq;

namespace GuessingGame.Core.Domain.Image.Handlers
{
    public class GameCreatedHandler
    {
        public class Handler:INotificationHandler<GameCreated>
        {
            private readonly  ApplicationDbContext _db;
            private readonly IMediator _mediator;

            public Handler(ApplicationDbContext db, IMediator mediator) 
            { 
                _db = db ?? throw new ArgumentNullException(nameof(db));
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            public async Task Handle(GameCreated notification, CancellationToken cancellationToken)
            {
                Game.Game currentGame = await _mediator.Send(new GetGame.Request(notification.GameId));
                List<Image> Images = await _mediator.Send(new GetImages.Request());
                Random r = new Random();
                int rIndex= r.Next(0, Images.Count-1);
                Image newImage = Images[rIndex];

                Console.WriteLine($"The object in the image: {newImage.ImageName}"); 
                // Got this here so it is easy for you to test and to guess the object in the image.

                currentGame.Image = newImage;
                _db.Update(currentGame);
                //Checks to see if the starting player is a proposer, if not it also asigns the first tile
                //the asigning should maybe be handled by the oracle instead, but check should happen here still.
                if(!currentGame.Players[0].IsProposer)
                {
                    var currentDir = System.IO.Directory.GetCurrentDirectory();
                    int numTiles = System.IO.Directory.GetFiles($"./wwwroot/img/"+currentGame.Image.ImageMap+"_scattered").Length;
                    int rTile = r.Next(0, numTiles-1);
                    currentGame.UsedTiles = rTile.ToString();
                }
                await _db.SaveChangesAsync();
                await _db.SaveChangesAsync(new CancellationTokenSource().Token);
            }
        }
    }
}