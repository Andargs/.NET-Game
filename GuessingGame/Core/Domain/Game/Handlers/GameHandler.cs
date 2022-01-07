using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using GuessingGame.SharedKernel;
using GuessingGame.Data;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Game.Services;

namespace GuessingGame.Core.Domain.Game.GameHandler
{
	public class GameHandler
	{
		public record Request(
            string guess,
            string username,
			int gameid,
			int proposal) : IRequest<Unit>;

		public record Response(bool Success, string[] Errors);

		public class Handler : IRequestHandler<Request>
		{
            private readonly ApplicationDbContext _context;
			private readonly IMediator _mediator;
            private readonly IOracleService _oracleService;
			private readonly IGameController _gameController;

			public Handler(ApplicationDbContext db, IMediator mediator, IOracleService oracleService, IGameController gameController)
			{ 
				_context = db ?? throw new ArgumentNullException(nameof(db));
				_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
				_gameController = gameController ?? throw new ArgumentNullException(nameof(gameController));
			}

			public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
			{
				var game = await _mediator.Send(new GetGame.Request(request.gameid));
                await _gameController.PlayGame(request.guess, request.gameid, request.username, request.proposal);
                await _context.SaveChangesAsync(cancellationToken);
				return Unit.Value;
            }
		}
	}
}