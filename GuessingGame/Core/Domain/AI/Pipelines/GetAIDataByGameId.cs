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
	public class GetAIDataByGameId
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
				//Returns AIData for a specific game using GameId
                var AI_data = await _db.AI.Include(g => g.Guesses).SingleOrDefaultAsync(g => g.GameId == request.id, cancellationToken);                
                if (AI_data is null)
                {
                    throw new EntityNotFoundException($"AIData with Id {request.id} was not found in the database");
                }
                return AI_data; 
            }
			
				
		}
	}
}