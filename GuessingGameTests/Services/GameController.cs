using Xunit;
using Moq;
using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Data;
using MediatR;
using System.Collections.Generic;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using GuessingGame.Core.Domain.AI.Events;
using GuessingGame.Core.Domain.AI.Handlers;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Hubs.Services;
using GuessingGame.Core.Domain.AI;
using GuessingGame.Core.Domain.Player.Events;
using GuessingGame.Core.Domain.Player.Handlers;

namespace GuessingGameTests
{


    public class GameControllerTest : DbTest
    {
        public GameControllerTest(ITestOutputHelper output) : base(output)
        {
        }



        // Here we are testing the gamecontroller. We are mainly testing if the gamecontroller can handle both a false and true guess. Again we are using a in memory database and mocking the mediator.

        [Theory]
        [MemberData(nameof(gameData))]
        public void GameController(int gameid, string guess, string username,bool boolV)
        {
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {

                var mediator = new Mock<IMediator>();
                var oracleService = new Mock<IOracleService>();
                var notifiactionService = new Mock<INotificationService>();

                var _gameMode = new GameMode();
                var _players = new List<Player> { new Player("chris", "chrisid", 3) };
                _gameMode = GameMode.SingelPlayer;
                var fakeGame1 = new Game(_players, _gameMode);
                fakeGame1.Players = _players;
                fakeGame1.numplayers = 1;
                fakeGame1.Image = new Image();
                fakeGame1.Id = gameid;
                fakeGame1.GameStatus = GameStatus.Finished;
                fakeGame1.GameMode = GameMode.SingelPlayer;
                fakeGame1.UsedTiles = " ";
                fakeGame1.Image.Id = gameid;


                var ai = new AIData(gameid);
                var _guess = new List<Guess> { new Guess(guess, " ", true) };
                ai.Guesses = _guess;

                contextInMemory.Database.Migrate();
                contextInMemory.AI.Add(ai);
                contextInMemory.Games.Add(fakeGame1);
                contextInMemory.SaveChanges();



                var gameController = new GameController(contextInMemory, mediator.Object, oracleService.Object, notifiactionService.Object);

                GetPlayerByGameIdName.Request _GetPlayerByGameIdName = new GetPlayerByGameIdName.Request(gameid, username);

                var _GetPlayerByGameIdNamehandler = new GetPlayerByGameIdName.Handler(contextInMemory);

                mediator.Setup(m => m.Send(It.IsAny<GetPlayerByGameIdName.Request>(), It.IsAny<CancellationToken>()))
                .Returns<GetPlayerByGameIdName.Request, CancellationToken>((notification, cToken) =>
                    _GetPlayerByGameIdNamehandler.Handle(notification, cToken));

                var handler_getGame = new GetGame.Handler(contextInMemory);
                mediator.Setup(x => x.Send(It.IsAny<GetGame.Request>(), It.IsAny<CancellationToken>())).Returns<GetGame.Request, CancellationToken>((notification, cToken) =>
                   handler_getGame.Handle(notification, cToken));

                var eventAIDataGuess = new AIDataGuess(guess, fakeGame1.UsedTiles, boolV, gameid);

                var eventAIDataGuessHandler = new AIDataGuessHandler(contextInMemory, mediator.Object);

                mediator.Setup(m => m.Publish(It.IsAny<AIDataGuess>(), It.IsAny<CancellationToken>()))
                 .Returns<AIDataGuess, CancellationToken>(async (eventAIDataGuess, cancelation) =>
                 {
                     await eventAIDataGuessHandler.Handle(eventAIDataGuess, cancelation);
                 });

                var eventCalculateSingleScore = new CalculateSingleScore(gameid, username);

                var eventCalculateSingleScoreHandler = new CalculateSingleScoreHandler(contextInMemory, mediator.Object);

                mediator.Setup(m => m.Publish(It.IsAny<CalculateSingleScore>(), It.IsAny<CancellationToken>()))
                 .Returns<CalculateSingleScore, CancellationToken>(async (CreateGame, cancelation) =>
                 {
                     await eventCalculateSingleScoreHandler.Handle(CreateGame, cancelation);
                 });



            }
        }

        public static IEnumerable<object[]> gameData => new List<object[]>
        {
            new object[]
            {
                1, "guess", "chrisid", true

            },
            new object[]
            {
                1, "guess", "chrisid", false

            }



        };



    }


}

