using System;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Exceptions;
using GuessingGame.Core.Domain.AI;
using GuessingGame.Core.Domain.AI.Handlers;
using GuessingGame.Core.Domain.AI.Events;

namespace GuessingGame.Core.Domain.AI.Pipelines
{
	public class GetAIDataByImageId
	{
		public record Request(int id) : IRequest<AIData>;

		public class Handler : IRequestHandler<Request, AIData>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}

			public async Task<AIData> Handle(Request request, CancellationToken cancellationToken)
            {
				//Returns AIData for a specific image using ImageId
                var game = await _db.AI.Include(g => g.Guesses).FirstOrDefaultAsync(g => g.ImageId == request.id, cancellationToken);                
                if (game is null)
                {
                    throw new EntityNotFoundException($"AIData Image with Id {request.id} was not found in the database");
                }
                return game; 
            }	
		}
	}
}