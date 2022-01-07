using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GuessingGame.Core.Domain.Image;

namespace GuessingGame.Core.Domain.Image.Pipelines
{
	public class GetImages
	{
		public record Request : IRequest<List<Image>> { }

		public class Handler : IRequestHandler<Request, List<Image>>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}

			public async Task<List<Image>> Handle(Request request, CancellationToken cancellationToken)
				//Retrives all Images in the database.
				=> await _db.Images.OrderBy(i => i.ImageId).ToListAsync(cancellationToken);
				
		}
	}
}