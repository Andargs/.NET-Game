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
using GuessingGame.Core.Domain.AI;
using GuessingGame.Core.Domain.AI.Pipelines;

namespace GuessingGame.Pages
{
	public class AIDataModel : PageModel
	{
        private readonly UserManager<IdentityUser> _userManager;
		private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

		public AIDataModel(IMediator mediator, ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
        }

        public List<AIData> AIData {get; set;}

        public async Task OnGetAsync(){  // Shows the AIData collected for the games played.
            AIData = await _mediator.Send(new GetAIDataResults.Request());
        }
        
        public async Task<IActionResult> OnPostAsync(int GameId, string Name){
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            string id = identityUser.Id;
            await _context.SaveChangesAsync();
            return Page();
		}
	}
}