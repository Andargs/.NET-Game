using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Exceptions;

namespace GuessingGame.Core.Domain.Game.Pipelines
{
	public class GetLastGame
	{
		public record Request() : IRequest<Game>;

		public class Handler : IRequestHandler<Request, Game>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}

			public async Task<Game> Handle(Request request, CancellationToken cancellationToken)
            {
                var game = await _db.Games.Include(i => i.Image).OrderByDescending(i => i.Id).FirstOrDefaultAsync(cancellationToken);                
                if (game is null)
                {
                    throw new EntityNotFoundException("No Games where found in the database");
                }
                return game; 
            }
			
				
		}
	}
}