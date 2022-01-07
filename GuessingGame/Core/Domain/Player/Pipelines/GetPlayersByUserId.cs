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
	public class GetPlayersByUserId
	{
		public record Request(string id) : IRequest<List<Player.Player>> { }

		public class Handler : IRequestHandler<Request, List<Player.Player>>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}

			//Retrieves all Players created by a user based on the Identity UserId.
			public async Task<List<Player.Player>> Handle(Request request, CancellationToken cancellationToken)
                => await _db.Players.OrderBy(i => i.Id).Where(c => c.UserId == request.id).ToListAsync(cancellationToken);
		}
	}
}