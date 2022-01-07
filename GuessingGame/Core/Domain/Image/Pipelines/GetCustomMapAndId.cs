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
	public class GetCustomMapAndId
	{
		public record Request(string Name) : IRequest<Response>;

		public record Response(string folderName, int imageId);

		public class Handler : IRequestHandler<Request, Response>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}
			
			public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
			{
				string foldername = "Custom_1";
				//Retrieves the latest uploaded image.
                var images = await _db.Images.OrderByDescending(i => i.Id).ToListAsync(cancellationToken);
                if(images[0].ImageMap.Split('_')[0] == "Custom") //Checking if the last uploaded image is a custom image.
                {
                    int NewCustomId = Int32.Parse(images[0].ImageMap.Split('_')[1]) + 1; //Incrementing custom Id
                    foldername = "Custom_" + NewCustomId.ToString();
                }
				//Checks if there are any images in the db with the same image name and if 
				//there are the new image will be given the same Id.
				var image = images.Find(i => i.ImageName == request.Name);
				if(image is not null)
				{
					return new Response(foldername, image.ImageId);
				}
				//If not increment highest id by 1.
				return new Response(foldername, images.Max(i => i.ImageId) + 1); 
            }
				
		}
	}
}