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
	public class PersonalGamesModel : PageModel
	{
        private readonly UserManager<IdentityUser> _userManager;
		private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

		public PersonalGamesModel(IMediator mediator, ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
        }
        public List<Player> Players {get; set;}
        
        //Shows the users own games.
        public async Task OnGetAsync(){
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            string id = identityUser.Id;
            Players = await _mediator.Send(new GetPlayersByUserId.Request(id));
        }
        
        public async Task<IActionResult> OnPostAsync(){
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            string id = identityUser.Id;
            await _context.SaveChangesAsync();
            return Page();
		}
	}
}