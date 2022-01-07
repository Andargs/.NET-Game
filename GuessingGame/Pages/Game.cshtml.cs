using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GuessingGame.Data;
using Microsoft.AspNetCore.Identity;
using MediatR;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Game.GameHandler;
using System.IO;
using GuessingGame.Core.Domain.Image.Services;
using GuessingGame.Core.Domain.AI.Events;
using GuessingGame.Core.Domain.AI.Pipelines;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Player.Events;
using GuessingGame.Core.Domain.Player.Handlers;
using GuessingGame.Hubs.Services;

namespace GuessingGame.Pages
{
    public class GameModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;
        private readonly IImageCombiner _imagecombiner;
        private readonly INotificationService _notificationService;
		public GameModel(IMediator mediator, ApplicationDbContext context, UserManager<IdentityUser> userManager, IImageCombiner imagecombiner, INotificationService notificationService ) {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
            _imagecombiner = imagecombiner ?? throw new System.ArgumentNullException(nameof(imagecombiner));
            _notificationService = notificationService ?? throw new System.ArgumentNullException(nameof(notificationService));
        }

        public Game Game { get; set; }
        public int GameId {get; set; }
        public bool GameStarted { get; set; }
        public string Guess { get; set; }
        public Player Player {get;set;}
        public string[] Tiles {get;set;} = null;
        public List<string> Urls {get;set;} = new();
        public List<string> pUrls {get;set;} = new();
        public int Proposal {get;set;} = -1;
        public string imgMap {get; set;}
        public string CurrentPlayerName {get; set;}
        public int X {get; set;}
        public int Y {get;set;}
        public string error {get;set;}
        public bool PlayerQuit {get;set;}
        public string[] optimaltiles {get;set;}
        public string optimaltiles_error {get;set;} = "";
        public string path = "../GuessingGame/wwwroot/img/";
        public string imgproposerurl {get; set;}
        public string imgfullurl {get; set;}
        public int Height {get; set;}
        public int Width {get; set;}
        public List<String> ScatteredImages {get;set;} = new List<string>();

        //yes, there is way to many public properties. this should have been handled better.

        public async Task<IActionResult> OnGetAsync(int GameId)
        {
            Game = await _mediator.Send(new GetGame.Request(GameId));
            IdentityUser user =  await _userManager.GetUserAsync(User);
            if(user != null) //quick fix to ensure that to many error messages wont be shown if not authenticated
            {
                Player = Game.Players.Find(delegate(Player p){return p.UserId==user.Id;});
                if(Game.UsedTiles != null) //quick fix to check if game is lost, i.e  all tiles spent, will end the game. This realy should be a set of event and handler handled in the game class
                {
                    Tiles = Game.UsedTiles.Split(" ");
                    if(Game.UsedTiles.Split(" ").Length == System.IO.Directory.GetFiles("./wwwroot/img/"+Game.Image.ImageMap+"_scattered").Length -1)
                    {
                        Game.GameStatus = GameStatus.Finished;
                        Game.ActivePlayerIndex = 0;
                        await _context.SaveChangesAsync();  
                    }
                }
                
                if(Game.GameStatus == GameStatus.Solved) //sets up for proposing better tiles
                {
                    int FileCount = Directory.GetFiles(Path.Combine(path,Game.Image.ImageMap + "_scattered"), "*", SearchOption.TopDirectoryOnly).Length - 1;
                    for(var i = 0; i< FileCount+1; i++)
                    {
                        Urls.Add(Path.Combine("/img", Game.Image.ImageMap + "_scattered", i.ToString() + ".png"));
                    }
                    imgproposerurl = ($"/img/{Game.Image.ImageMap}_full/proposer.png");
                    imgMap = Game.Image.ImageMap;
                }
                if(Game.GameStatus == GameStatus.Finished) //gets the full picture to show if the game is over
                {
                    int FileCount = Directory.GetFiles(Path.Combine(path,Game.Image.ImageMap + "_scattered"), "*", SearchOption.TopDirectoryOnly).Length - 1;
                    for(var i = 0; i< FileCount+1; i++)
                    {
                        Urls.Add(Path.Combine("/img", Game.Image.ImageMap + "_scattered", i.ToString() + ".png"));
                    }
                    imgfullurl = ($"/img/{Game.Image.ImageMap}_full/full.png");
                }
                
                if(Player.IsProposer) //gets the proposer view  when needed
                {
                    imgproposerurl = ($"/img/{Game.Image.ImageMap}_full/proposer.png");
                    imgMap = Game.Image.ImageMap;
                }
                if(Game.UsedTiles != null)
                {
                    foreach(string tile in Tiles)
                    {
                        string path = "/img/"+Game.Image.ImageMap+"_scattered/"+tile+".png";
                        Urls.Add(path);
                    }
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostGuess(int GameId, string CurrentPlayerName, string Guess)
        {   
            IdentityUser user =  await _userManager.GetUserAsync(User);
            await _mediator.Send(new GameHandler.Request(Guess,CurrentPlayerName,GameId,Proposal));
            return RedirectToPage();    
        }

        public async Task<IActionResult> OnPostPropose(int GameId, string CurrentPlayerName, int X, int Y, string imgMap, int Height, int Width){ // kanskje ikke ha denne imgMap sammen navn  som propertyen oppe.
            Guess = " "; // a quick fix to make gameHandler.cs handle both  guess and proposal . should be two different handlers
            if(imgMap != "")
            {
                this.imgMap = imgMap;
                var tilepath = _imagecombiner.FindTile(X, Y, imgMap, Height, Width); //finds the tile based on x,y cords
                string[] tilepathparts = tilepath.Split("."); // converts the full path to a int
                Proposal = Int32.Parse(tilepathparts[0]);  // function should just have returned an int in the first place
            }
            Game = await _mediator.Send(new GetGame.Request(GameId));
            imgproposerurl = ($"/img/{Game.Image.ImageMap}_full/proposer.png");
            IdentityUser user =  await _userManager.GetUserAsync(User);
            Player = Game.Players.Find(delegate(Player p){return p.UserId==user.Id;});
            if(Player.IsProposer) 
            {   
                if( Game.UsedTiles != null){  //quick fix to ensure that same tile cant be proposed several times, should also have included an error message
                    if(Game.UsedTiles.Split(" ").Contains(Proposal.ToString())){
                        return RedirectToPage();
                    }
                    
                }
                await _mediator.Send(new GameHandler.Request(Guess,CurrentPlayerName,GameId,Proposal));
            }else{ // for proposing better images when game has been won
                var AIdata = await _mediator.Send(new GetAIDataByGameId.Request(Game.Id)); 
                if (AIdata.OptimalUserTiles is not null){
                    optimaltiles = AIdata.OptimalUserTiles.Split(" ");
                    if(optimaltiles.Contains(Proposal.ToString()))
                    {
                        optimaltiles_error = "Tile " + Proposal.ToString() + " already chosen. Please choose another one.";
                        return Page();
                    }
                }
                await _mediator.Publish(new AddOptimalTile(GameId,Proposal.ToString())); //ToDoo sort out proposal handeling, decide if it is a string or int
                Player.AvailableAttempts --;
                if(Player.AvailableAttempts==0){
                    Game.GameStatus=GameStatus.Finished;
                }
                _context.SaveChanges();
            }
            imgfullurl = ($"/img/{Game.Image.ImageMap}_full/full.png");
            return Page();
        }

        public async Task<IActionResult> OnPostShowOneMore(int GameId, string CurrentPlayerName, string Guess)
        {
            IdentityUser user =  await _userManager.GetUserAsync(User);
            var player = await _mediator.Send(new GetPlayerByGameIdName.Request(GameId, CurrentPlayerName));
            player.AvailableAttempts = 1;
            await _mediator.Send(new GameHandler.Request(Guess,CurrentPlayerName,GameId,Proposal));
            return RedirectToPage();  
        }

        public async Task<IActionResult> OnPostQuit(int GameId, string CurrentPlayerName, string Guess)
        {
            IdentityUser user =  await _userManager.GetUserAsync(User);
            var player = await _mediator.Send(new GetPlayerByGameIdName.Request(GameId, CurrentPlayerName));
            Game = await _mediator.Send(new GetGame.Request(GameId));
            //following part should also have had its own pipeline.
            Game.GameStatus = GameStatus.Finished;
            PlayerQuit = true;
            imgfullurl = ($"/img/{Game.Image.ImageMap}_full/full.png");
            await _context.SaveChangesAsync();
            await _notificationService.SendNotification(GameId);
            return Page();  
        }

    }  
}