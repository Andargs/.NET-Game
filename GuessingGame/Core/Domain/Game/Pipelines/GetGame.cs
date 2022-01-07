using System;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Exceptions;

namespace GuessingGame.Core.Domain.Game.Pipelines
{
	public class GetGame
	{
		public record Request(int id) : IRequest<Game>;

		public class Handler : IRequestHandler<Request, Game>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}

			public async Task<Game> Handle(Request request, CancellationToken cancellationToken)
            {
				//Retrieves a game with the specified id
                var game = await _db.Games.Include(g => g.Players).Include(g => g.Image).SingleOrDefaultAsync(g => g.Id == request.id, cancellationToken);                
                if (game is null)
                {
                    throw new EntityNotFoundException($"Game with Id {request.id} was not found in the database");
                }
                return game; 
            }
			
				
		}
	}
}