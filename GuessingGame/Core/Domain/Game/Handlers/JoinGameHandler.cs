using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Data;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Hubs.Services;
using GuessingGame.Core.Domain.Game.Pipelines;

namespace GuessingGame.Core.Domain.Game.GameHandler
{
    public class JoinGameHandler
    {
        public record Request(
            int gameId,
            string name,
            string userId
        ):IRequest<Unit>;

        public record Response(bool Success);

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator _mediator;
            private readonly ApplicationDbContext _context;
            private readonly INotificationService _notificationService;

            public Handler(IMediator mediator, ApplicationDbContext context, INotificationService notificationService)
            {
                _mediator = mediator;
                _context = context;
                _notificationService = notificationService;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                Game gameToJoin = await _mediator.Send(new GetGame.Request(request.gameId));
                int attempts= 1;
                if(gameToJoin.GameMode == GameMode.TwoPlayer)
                {
                    attempts =3;
                }
                Player.Player joiningPlayer= new Player.Player(request.name,request.userId,attempts);
                gameToJoin.Players.Add(joiningPlayer);
                if(gameToJoin.Players.Count==gameToJoin.numplayers)
                {
                    gameToJoin.GameStatus=GameStatus.Active;
                }
                await _context.SaveChangesAsync();
                await _notificationService.SendNotification(request.gameId);
                return Unit.Value;
            }
        }
    }
}