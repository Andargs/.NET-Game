using System.Threading.Tasks;
using System;
using MediatR;
using GuessingGame.Data;
using System.Collections.Generic;
using GuessingGame.Core.Domain.Game.Pipelines;
using System.Linq;
using GuessingGame.Hubs;
using Microsoft.AspNetCore.SignalR;
using GuessingGame.Core.Domain.AI.Events;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Player.Events;
using GuessingGame.Core.Domain.Player.Handlers;
using GuessingGame.Hubs.Services;


namespace GuessingGame.Core.Domain.Game.Services
{
    public interface IGameController
    {
        Task<int> PlayGame( string guess, int gameid, string username, int proposal=-1);
    }
    
    public class GameController :IGameController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly IOracleService _oracleService;
        private readonly INotificationService _notificationService;
        
        public GameController(ApplicationDbContext context, IMediator mediator, IOracleService oracleService, INotificationService notificationService)
        {
            this._mediator = mediator;
            this._context = context;
            this._oracleService = oracleService;
            this._notificationService = notificationService;
        }

        public Oracle Oracle {get; set;}
        public Game Game {get; set;}

        public async Task<int> PlayGame( string guess, int gameid, string username,int proposal){
            Player.Player player = await _mediator.Send(new GetPlayerByGameIdName.Request(gameid, username));
            Game Game = await _mediator.Send(new GetGame.Request(gameid));
            if(player.IsProposer)
            {
                if(Game.UsedTiles==null||Game.UsedTiles==" ")
                {
                    Game.UsedTiles=proposal.ToString();
                }
                else
                {
                    string newUsedTiles=Game.UsedTiles+" "+proposal.ToString();
                    Game.UsedTiles = newUsedTiles;
                }
                Game.ActivePlayerIndex = 1;
                await _context.SaveChangesAsync();
                await  _notificationService.SendNotification(gameid);
                return await Task.FromResult(0);
            }
            else
            {
                if (guess == Game.Image.ImageName)
                {
                    Game.GameStatus = GameStatus.Solved;
                    await _mediator.Publish(new AIDataGuess(proposal.ToString(), Game.UsedTiles, true, gameid));
                    await _mediator.Publish(new CalculateSingleScore(gameid, username));
                    Game.Players[Game.ActivePlayerIndex].AvailableAttempts = 7;
                    await _context.SaveChangesAsync();
                    await _notificationService.SendNotification(gameid);
                    return await Task.FromResult(0);
                }
                else
                {
                    await _mediator.Publish(new AIDataGuess(proposal.ToString(), Game.UsedTiles, false, gameid));
                    if(player.AvailableAttempts<=1)
                    {
                        if(Game.GameMode==GameMode.MultiPlayer || Game.GameMode==GameMode.MultiPlayerOracle)
                        {
                            player.AvailableAttempts=1;
                        }
                        else
                        {
                            player.AvailableAttempts=3;
                        }
                        //calls oracleService for new tile if it is an oracle Game
                        if(Game.GameMode==GameMode.SingelPlayer||Game.GameMode==GameMode.MultiPlayerOracle)
                        {
                            await _oracleService.GetNewTile(gameid);
                        }
                        //updates the  active player index
                        int newActivePlayerIndex = Game.ActivePlayerIndex  +1;
                        if (newActivePlayerIndex>Game.numplayers-1)
                        {
                            Game.ActivePlayerIndex=0;
                        }
                        else
                        {
                            Game.ActivePlayerIndex=newActivePlayerIndex;
                        }
                        await _context.SaveChangesAsync();
                        await  _notificationService.SendNotification(gameid);
                        return await Task.FromResult(0);
                    }
                    else
                    {
                        player.AvailableAttempts--;
                        await _context.SaveChangesAsync();
                        await  _notificationService.SendNotification(gameid);
                        return await Task.FromResult(0);
                    }
                }
            }
        }
    }
}