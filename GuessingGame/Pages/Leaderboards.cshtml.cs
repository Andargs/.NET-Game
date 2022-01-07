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

namespace GuessingGame.Pages
{
	public class LeaderboardsModel : PageModel
	{
        private readonly UserManager<IdentityUser> _userManager;
		private readonly IMediator _mediator;

        private readonly ApplicationDbContext _context;

		public LeaderboardsModel(IMediator mediator, ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
        }

        public List<Player> Players {get; set;}
        public int items {get; set;}
        public int index {get; set;}

        //Shows the leaderboards of all games played, and allows the user to define how many places they wish to see.
        public async Task OnGetAsync(int num){
            Players = await _mediator.Send(new GetPlayerScores.Request());
            items = num;
            index = 1;
        }
        
        public async Task<IActionResult> OnPostAsync(int GameId, string Name){
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            string id = identityUser.Id;
            await _context.SaveChangesAsync();
            return Page();
		}
	}
}