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
	public class GetLastImageId
	{
		public record Request() : IRequest<int>;

		public class Handler : IRequestHandler<Request, int>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}

			public async Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
				//Returns the last game's image id
                var game = await _db.Games.Include(i => i.Image).OrderByDescending(i => i.Id).FirstOrDefaultAsync(cancellationToken);                
                if (game is null)
                {
                    throw new EntityNotFoundException("No Games where found in the database");
                }
                return game.Image.Id; 
            }
			
				
		}
	}
}