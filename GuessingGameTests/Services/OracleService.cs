using Xunit;
using Moq;
using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Data;
using MediatR;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using GuessingGame.Core.Domain.AI.Events;
using GuessingGame.Core.Domain.AI.Handlers;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Player.Events;
using GuessingGame.Core.Domain.Player.Handlers;
using GuessingGame.Core.Domain.AI.Pipelines;
using GuessingGame.Core.Domain.AI;


namespace GuessingGameTests
{


    public class OracleServiceTest : DbTest

    {
       
        public OracleServiceTest(ITestOutputHelper output) : base(output)
        {

        }


        // This is a test where we test the OracleService. We are testing if it is possible to use the method PlayGame with various input. 
        

        [Theory]
        [MemberData(nameof(gameData))]
        public async Task OracleServiceTests(string guess, int gameid, string user, int proposal, bool boolV, string userid, GameMode gameMode, int numplayers, GameStatus gameStatus)
        {
            // We are using a in memorydatabase for this test.
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {
                

                // We are mocking the mediator and setting it up the essential pipelines, events and handlers that OracleService Requires. 
                var mediator = new  Mock<IMediator>();

                 var _players = new List<Player>{new Player(user, userid, 3)};
                    var _gameMode = new GameMode();
                    _gameMode = gameMode;
                    var fakeGame1 = new Game(_players, _gameMode);
                    fakeGame1.Players = _players;
                    fakeGame1.numplayers = numplayers;
                    fakeGame1.Image = new Image();
                    fakeGame1.Id = gameid;
                    fakeGame1.GameStatus = gameStatus;
                    fakeGame1.GameMode = gameMode;
                    fakeGame1.UsedTiles = " ";
                    fakeGame1.Image.Id = gameid;


                    var ai = new AIData(gameid);
                    var _guess = new List<Guess>{new Guess(guess, " ", true)};
                    ai.Guesses = _guess;
                    
                    
                    contextInMemory.Database.Migrate();
                    contextInMemory.AI.Add(ai);
                    contextInMemory.Games.Add(fakeGame1);
                    contextInMemory.SaveChanges();


                var OracleService = new OracleService(contextInMemory, mediator.Object);
                var handler_getGame = new GetGame.Handler(contextInMemory);
                mediator.Setup(x => x.Send(It.IsAny<GetGame.Request>(),  It.IsAny<CancellationToken>())).Returns<GetGame.Request, CancellationToken>((notification, cToken) => 
                    handler_getGame.Handle(notification, cToken));
                
                var eventCalculateSingleScore = new CalculateSingleScore(gameid, user);

                var eventCalculateSingleScoreHandler =  new CalculateSingleScoreHandler(contextInMemory, mediator.Object);
                
                mediator.Setup(m => m.Publish(It.IsAny<CalculateSingleScore>(), It.IsAny<CancellationToken>()))
                 .Returns<CalculateSingleScore, CancellationToken>(async (CreateGame, cancelation) => 
                 {
                     await eventCalculateSingleScoreHandler.Handle(CreateGame, cancelation);
                 });

                 
                var eventAIDataGuess = new AIDataGuess(guess, fakeGame1.UsedTiles, boolV, gameid);

                var eventAIDataGuessHandler =  new AIDataGuessHandler(contextInMemory, mediator.Object);
                
                mediator.Setup(m => m.Publish(It.IsAny<AIDataGuess>(), It.IsAny<CancellationToken>()))
                 .Returns<AIDataGuess, CancellationToken>(async (eventAIDataGuess, cancelation) => 
                 {
                     await eventAIDataGuessHandler.Handle(eventAIDataGuess, cancelation);
                 });

                GetAIDataByGameId.Request _GetAIDataByGameId = new GetAIDataByGameId.Request(gameid);
                var _GetAIDataByGameIdhandler = new GetAIDataByGameId.Handler(contextInMemory);
                mediator.Setup(m => m.Send(It.IsAny<GetAIDataByGameId.Request>(), It.IsAny<CancellationToken>()))
                .Returns<GetAIDataByGameId.Request, CancellationToken>((notification, cToken) => 
                    _GetAIDataByGameIdhandler.Handle(notification, cToken));

                GetPlayerByGameIdName.Request _GetPlayerByGameIdName = new GetPlayerByGameIdName.Request(gameid, user);

                var _GetPlayerByGameIdNamehandler = new GetPlayerByGameIdName.Handler(contextInMemory);

                mediator.Setup(m => m.Send(It.IsAny<GetPlayerByGameIdName.Request>(), It.IsAny<CancellationToken>()))
                .Returns<GetPlayerByGameIdName.Request, CancellationToken>((notification, cToken) => 
                    _GetPlayerByGameIdNamehandler.Handle(notification, cToken));



                GetAIDataByImageId.Request _GetAIDataByImageId = new GetAIDataByImageId.Request(fakeGame1.Image.Id);

                var _GetAIDataByImageIdhandler = new GetAIDataByImageId.Handler(contextInMemory);

                mediator.Setup(m => m.Send(It.IsAny<GetAIDataByImageId.Request>(), It.IsAny<CancellationToken>()))
                .Returns<GetAIDataByImageId.Request, CancellationToken>((notification, cToken) => 
                    _GetAIDataByImageIdhandler.Handle(notification, cToken));

                await OracleService.PlayGame(guess, gameid, user, proposal);
                await contextInMemory.SaveChangesAsync();
                var id = contextInMemory.Games.Select(x => x.Id).SingleOrDefault();
            }
        }
        
        public static IEnumerable<object[]> gameData => new List<object[]>
        {
            new object[]
            {
                "guess",  1, "chris", 1, false, "chrisid", GameMode.SingelPlayer, 1, GameStatus.Active
                

            },
            new object[]
            {
                "guess",  2, "ch", 2, true, "id", GameMode.TwoPlayer, 2, GameStatus.Active
                

            },
            new object[]
            {
                "guess",  3, "yee", 2, true, "id", GameMode.MultiPlayer, 3, GameStatus.Active
                

            },
            new object[]
            {
                "guess",  4, "yee", 2, true, "id", GameMode.MultiPlayerOracle, 4, GameStatus.Active
                

            },
            new object[]
            {
                "guess",  3, "yee", 2, true, "id", GameMode.MultiPlayer, 3, GameStatus.Finished
                

            },
            new object[]
            {
                "guess",  3, "yee", 2, true, "id", GameMode.MultiPlayer, 3, GameStatus.Created
                

            },
            new object[]
            {
                "guess",  3, "yee", 2, true, "id", GameMode.MultiPlayer, 3, GameStatus.New
                

            },
            new object[]
            {
                "guess",  3, "yee", 2, true, "id", GameMode.MultiPlayer, 3, GameStatus.Solved
                

            },
            new object[]
            {
                " ",  3, " ", 2, true, " ", GameMode.MultiPlayer, 3, GameStatus.Solved
                

            },
            new object[]
            {
                " ",  3, " ", 2, true, "id", GameMode.MultiPlayer, 1, GameStatus.Solved
                

            },
            // checking if the test fails also
            // new object[]
            // {
            //     " ",  0, " ", 0, false, " ", GameMode.MultiPlayer, 1, GameStatus.Solved
                

            // }

            
            
            
            
            
            
            



        };



    }


}

