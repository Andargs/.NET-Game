using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace GuessingGame.Core.Domain.AI.Pipelines
{
	public class GetAIDataResults
	{
		public record Request : IRequest<List<AIData>> { }

		public class Handler : IRequestHandler<Request, List<AIData>>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}
			//Retrieves all AIData-sets created
			public async Task<List<AIData>> Handle(Request request, CancellationToken cancellationToken)
				=> await _db.AI.OrderByDescending(i => i.Id).Include(s => s.Guesses).ToListAsync(cancellationToken: cancellationToken);
		}
	}
}