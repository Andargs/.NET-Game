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
	public class CreateGameModel : PageModel
	{
        private readonly IGameValidator _validator;
        private readonly UserManager<IdentityUser> _userManager;
		private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;

		public CreateGameModel(IMediator mediator, ApplicationDbContext context, UserManager<IdentityUser> userManager, IGameValidator gameValidator) 
        {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
            _validator = gameValidator;
        }  
        
        //This page allows the user to initialize a game based on their own inputs and specifications.
        public string Name {get; set; } ="";
        public string mode {get; set; }
        public int players {get; set;}
        
        //Sender Display name, userid og game mode til Creategame.cs
        // Dette er for å å vise error meldingene som validatoren gir
        [BindProperty]
        public string[] Errors { get; set; }

        public async Task<IActionResult> OnPostAsync(string Name, string mode, int players){
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            string id = identityUser.Id;
            var fakePlayers = new List<Player>{ new Player(Name, id, 3) };
            var fakeMode = new GameMode();
            if (mode.Equals("singplayer", StringComparison.OrdinalIgnoreCase))
            {
                fakeMode = GameMode.SingelPlayer;
            }
            else if (mode.Equals("multiplayer", StringComparison.OrdinalIgnoreCase))
            {
                fakeMode = GameMode.MultiPlayer;
            }
            else if (mode.Equals("twoplayer", StringComparison.OrdinalIgnoreCase))
            {
                fakeMode = GameMode.TwoPlayer;
            }
            else if (mode.Equals("omultiplayer", StringComparison.OrdinalIgnoreCase))
            {
                fakeMode = GameMode.MultiPlayerOracle;
            }
            var fakeGame = new Game(fakePlayers, fakeMode);
            fakeGame.numplayers = players;
            Errors = _validator.IsValid(fakeGame);
            if(Errors.Length!=0){
                return Page(); 
            }
            else
            {
                await _mediator.Send(new CreateGame.Request(Name, id, mode, players));
                await _context.SaveChangesAsync();
                var GameId = await _mediator.Send(new GetGameId.Request());
                var Game = await _mediator.Send(new GetGame.Request(GameId));
                return RedirectToPage("Game", new { GameId });
            }
		}
	}
}