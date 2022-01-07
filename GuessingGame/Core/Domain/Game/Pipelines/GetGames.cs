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
	public class GetGames
	{
		public record Request : IRequest<List<Game>> { }

		public class Handler : IRequestHandler<Request, List<Game>>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}
			//Retrieves all games
			public async Task<List<Game>> Handle(Request request, CancellationToken cancellationToken)
				=> await _db.Games.OrderByDescending(i => i.Id).Include(i => i.Players).ToListAsync(cancellationToken: cancellationToken);
		}
	}
}