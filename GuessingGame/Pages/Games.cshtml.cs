using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using GuessingGame.Data;
using GuessingGame;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GuessingGame.Pages
{
	public class GamesModel : PageModel
	{
        private readonly UserManager<IdentityUser> _userManager;
		private readonly IMediator _mediator;

        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

		public GamesModel(IMediator mediator, ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<NotificationHub> hubContext) {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
            _hubContext = hubContext;
        }
        public List<Game> Games {get; set;}
        public int GameId {get; set;}
        public string Name {get; set;}

        public async Task OnGetAsync(){
			Games = await _mediator.Send(new GetGames.Request());
        }
        
        public void SendNotification(){
             _hubContext.Clients.All.SendAsync("ReceiveNotification");
        }
        
        public  async Task<IActionResult> OnPostJoinAsync(int GameId, string Name){
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            string userId = identityUser.Id;
            int attempts= 1;
            Game gameToJoin = await _mediator.Send(new GetGame.Request(GameId));
            if(gameToJoin.GameMode == GameMode.TwoPlayer){
                attempts =3;
            }
            Player joiningPlayer= new Player(Name,userId,attempts);
            gameToJoin.Players.Add(joiningPlayer);
            if(gameToJoin.Players.Count==gameToJoin.numplayers){
                gameToJoin.GameStatus=GameStatus.Active;
                SendNotification();
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("Game", new {GameId}); 
        }       
	}
}