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
	public class GetPlayerByGameIdName
	{
		public record Request(int id, string name) : IRequest<Player.Player>;

		public class Handler : IRequestHandler<Request, Player.Player>
		{
			private readonly ApplicationDbContext _db;

			public Handler(ApplicationDbContext db)
			{
				_db = db ?? throw new ArgumentNullException(nameof(db));
			}

			public async Task<Player.Player> Handle(Request request, CancellationToken cancellationToken)
            {
                //Retrieves a player through the game id and the players username.
                var game = await _db.Games.Include(g => g.Players).SingleOrDefaultAsync(g => g.Id == request.id, cancellationToken);                
                if (game is null)
                {
                    throw new EntityNotFoundException($"Game with Id {request.id} was not found in the database");
                }
    
                foreach(var player in game.Players){
                    if(request.name == player.Name){
                        return player;
                    } 
                }
                throw new EntityNotFoundException($"No player without the name {request.name} in Game with Id {request.id}");
                
            }
			
				
		}
	}
}