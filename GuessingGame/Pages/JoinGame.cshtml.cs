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
using GuessingGame.Core.Domain.Game.GameHandler;

namespace GuessingGame.Pages
{
	public class JoinGameModel : PageModel
	{
        private readonly UserManager<IdentityUser> _userManager;
		private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

		public JoinGameModel(IMediator mediator, ApplicationDbContext context, UserManager<IdentityUser> userManager, IHubContext<NotificationHub> hubContext) {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
            _hubContext = hubContext;
        }

        public List<Game> Games {get; set;}
        public int GameId {get; set;}
        public string Name {get; set;}
        public int index {get; set;}

        public async Task OnGetAsync(){
			Games = await _mediator.Send(new GetGames.Request());
            index = 1;
        }

        public  async Task<IActionResult> OnPostJoinAsync(int GameId, string Name){
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            string userId = identityUser.Id;
            await _mediator.Send(new JoinGameHandler.Request(GameId,Name,userId));
            return RedirectToPage("Game", new {GameId});  
        }       
	}
}