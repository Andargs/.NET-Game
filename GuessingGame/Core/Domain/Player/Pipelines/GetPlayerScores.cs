using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace GuessingGame.Core.Domain.Game.Pipelines
{
	public class GetPlayerScores
	{
		public record Request : IRequest<List<Player.Player>> { }

		public class Handler : IRequestHandler<Request, List<Player.Player>>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}

			//Retrieves the highscores of all games played.
			public async Task<List<Player.Player>> Handle(Request request, CancellationToken cancellationToken)
                => await _db.Players.OrderByDescending(i => i.Score).Where(i => i.Score < 999).Where(i => i.Score > 0).ToListAsync(cancellationToken);
		}
	}
}