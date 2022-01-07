using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Game.Services;

namespace GuessingGame.Core.Domain.Game.Pipelines
{
	public class CreateGame
	{
		public record Request(
            string displayname,
			string userid,
			string gamemode,
            int players) : IRequest<Unit>;

		public record Response(bool Success, string[] Errors);

		public class Handler : IRequestHandler<Request>
		{
            private readonly IGameCreation _gameCreation;

			private readonly ApplicationDbContext _db; 
			private readonly IMediator _mediator;

			public Handler(ApplicationDbContext db, IMediator mediator, IGameCreation gameCreation) { 
				_db = db ?? throw new ArgumentNullException(nameof(db));
				_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _gameCreation = gameCreation ?? throw new ArgumentNullException(nameof(gameCreation));
			}
			public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
			{
                //Sendes til GameCreation.cs
                var noe = await _gameCreation.CreateGame(request.displayname, request.userid, request.gamemode, request.players);
                await _db.SaveChangesAsync(cancellationToken);

				return Unit.Value;
                }
			}

		}
	}