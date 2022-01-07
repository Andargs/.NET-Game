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
	public class AddImage
	{
		public record Request(int ImageId, string ImageName, string ImageMap) : IRequest<Response>;

        public record Response(bool Success);

		public class Handler : IRequestHandler<Request, Response>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}
            //Adds an image to the database.
			public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
			{
                var image = new Image {
                    ImageName = request.ImageName,
                    ImageId = request.ImageId,
                    ImageMap = request.ImageMap
                };
                if(image is not null)
                {
                    _db.Images.Add(image);
                    await _db.SaveChangesAsync(cancellationToken);
                    return new Response(Success: true);   
                }
                return new Response(Success: false);
            }
				 
				
		}
	}
}