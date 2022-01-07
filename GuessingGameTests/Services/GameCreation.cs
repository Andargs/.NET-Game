using Xunit;
using Moq;
using GuessingGame;
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
using GuessingGame.Core.Domain.Game.Events;
using System.Linq;
using System.Threading;
using GuessingGame.Core.Domain.AI.Handlers;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Image.Pipelines;
using GuessingGame.Core.Domain.Image.Handlers;

namespace GuessingGameTests
{


    public class GameCreationTest : DbTest
    {

        public GameCreationTest(ITestOutputHelper output) : base(output)
        {
        }


        // Here we are testing the gamecreation sequence. Testing if we are able to create the game when the player is a proposer. 
        // and that the gamemode is matching the gamemode that is given.

        [Theory]
        [MemberData(nameof(gameData))]
        public async Task GameCreationTest_ForIsProposer(string displayname, string userid, string mode, int numplayers, GameMode gameMode)
        {
            // We are using a in memorydatabase, and also mocking mediator and setting it up with the pipelinerequest, handlers and event which the sequence require.
            
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {
                var fakeImage = new Image();
                fakeImage.Id = 0;
                fakeImage.ImageId = 0;
                fakeImage.ImageMap = "custom1";
                fakeImage.ImageName = "Test";


                contextInMemory.Database.Migrate();
                contextInMemory.Images.Add(fakeImage);
                contextInMemory.SaveChanges();
                

                
                var mediator = new  Mock<IMediator>();

                var gameCreation = new GameCreation(contextInMemory, mediator.Object);

                GetGameId.Request _getGameId = new GetGameId.Request();

                var handler = new GetGameId.Handler(contextInMemory);
                GetLastGame.Request _getLastGame = new GetLastGame.Request();

                var handler_getGame = new GetGame.Handler(contextInMemory);

                GetImages.Request _getImages = new GetImages.Request();
                var handler_getImages = new GetImages.Handler(contextInMemory);

                GetLastImageId.Request _getLastImageId = new GetLastImageId.Request();
                var handler_getLastImageId = new GetLastImageId.Handler(contextInMemory);
                var handler_createAI = new CreateAIHandler(contextInMemory, mediator.Object);


                var eventGameCreated = new GameCreated(1);
                var eventGameCreatedHandler =  new GameCreatedHandler.Handler(contextInMemory, mediator.Object);

                

                

               mediator.Setup(m => m.Send(It.IsAny<GetGameId.Request>(), It.IsAny<CancellationToken>()))
                .Returns<GetGameId.Request, CancellationToken>((notification, cToken) => 
                    handler.Handle(notification, cToken));

                mediator.Setup(x => x.Send(It.IsAny<GetGame.Request>(),  It.IsAny<CancellationToken>())).Returns<GetGame.Request, CancellationToken>((notification, cToken) => 
                    handler_getGame.Handle(notification, cToken));

                mediator.Setup(x => x.Send(It.IsAny<GetLastImageId.Request>(),  It.IsAny<CancellationToken>())).Returns<GetLastImageId.Request, CancellationToken>((notification, cToken) => 
                    handler_getLastImageId.Handle(notification, cToken));
                    
                mediator.Setup(x => x.Send(It.IsAny<GetImages.Request>(),  It.IsAny<CancellationToken>()))
                .Returns<GetImages.Request, CancellationToken>((notification, cToken) => 
                    handler_getImages.Handle(notification, cToken));


                
                
                 mediator.Setup(m => m.Publish(It.IsAny<GameCreated>(), It.IsAny<CancellationToken>()))
                 .Returns<GameCreated, CancellationToken>(async (CreateGame, cancelation) => 
                 {
                     await eventGameCreatedHandler.Handle(CreateGame, cancelation);
                 });
                
                    
                    await gameCreation.CreateGame(displayname, userid, mode, numplayers);
                    await contextInMemory.SaveChangesAsync();

                    var gameid1 = await contextInMemory.Games.OrderByDescending(i => i.Id).FirstOrDefaultAsync(CancellationToken.None);
                    var game1 = await contextInMemory.Games.Include(g => g.Players).SingleOrDefaultAsync(g => g.Id == gameid1.Id, CancellationToken.None);
                    


                    Assert.Equal(game1.GameMode, gameMode);
            }
        }


        public static IEnumerable<object[]> gameData => new List<object[]>
        {
            new object[]
            {
                "chris", "chrisid", "multiplayer", 3, GameMode.MultiPlayer

            },
            new object[]
            {
                "chris1", "chrisid1", "twoplayer", 2, GameMode.TwoPlayer

            }
            
        
        };

        // here we are checking that the UserId matches when a game is created
        [Theory]
        [MemberData(nameof(gameDataB))]
        public async Task GameCreationTest_ForIsProposer_Check_Userid(string displayname, string userid, string mode, int numplayers)
        {
            // We are using a in memorydatabase, and also mocking mediator and setting it up with the pipelinerequest, handlers and event which the sequence require.
            
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {
                var fakeImage = new Image();
                fakeImage.Id = 0;
                fakeImage.ImageId = 0;
                fakeImage.ImageMap = "custom1";
                fakeImage.ImageName = "Test";


                contextInMemory.Database.Migrate();
                contextInMemory.Images.Add(fakeImage);
                contextInMemory.SaveChanges();
                

                
                var mediator = new  Mock<IMediator>();

                var gameCreation = new GameCreation(contextInMemory, mediator.Object);

                GetGameId.Request _getGameId = new GetGameId.Request();

                var handler = new GetGameId.Handler(contextInMemory);
                GetLastGame.Request _getLastGame = new GetLastGame.Request();

                var handler_getGame = new GetGame.Handler(contextInMemory);

                GetImages.Request _getImages = new GetImages.Request();
                var handler_getImages = new GetImages.Handler(contextInMemory);

                GetLastImageId.Request _getLastImageId = new GetLastImageId.Request();
                var handler_getLastImageId = new GetLastImageId.Handler(contextInMemory);
                var handler_createAI = new CreateAIHandler(contextInMemory, mediator.Object);


                var eventGameCreated = new GameCreated(1);
                var eventGameCreatedHandler =  new GameCreatedHandler.Handler(contextInMemory, mediator.Object);

                

                

               mediator.Setup(m => m.Send(It.IsAny<GetGameId.Request>(), It.IsAny<CancellationToken>()))
                .Returns<GetGameId.Request, CancellationToken>((notification, cToken) => 
                    handler.Handle(notification, cToken));

                mediator.Setup(x => x.Send(It.IsAny<GetGame.Request>(),  It.IsAny<CancellationToken>())).Returns<GetGame.Request, CancellationToken>((notification, cToken) => 
                    handler_getGame.Handle(notification, cToken));

                mediator.Setup(x => x.Send(It.IsAny<GetLastImageId.Request>(),  It.IsAny<CancellationToken>())).Returns<GetLastImageId.Request, CancellationToken>((notification, cToken) => 
                    handler_getLastImageId.Handle(notification, cToken));
                    
                mediator.Setup(x => x.Send(It.IsAny<GetImages.Request>(),  It.IsAny<CancellationToken>()))
                .Returns<GetImages.Request, CancellationToken>((notification, cToken) => 
                    handler_getImages.Handle(notification, cToken));


                
                
                 mediator.Setup(m => m.Publish(It.IsAny<GameCreated>(), It.IsAny<CancellationToken>()))
                 .Returns<GameCreated, CancellationToken>(async (CreateGame, cancelation) => 
                 {
                     await eventGameCreatedHandler.Handle(CreateGame, cancelation);
                 });
                
                    
                    await gameCreation.CreateGame(displayname, userid, mode, numplayers);
                    await contextInMemory.SaveChangesAsync();

                    var gameid1 = await contextInMemory.Games.OrderByDescending(i => i.Id).FirstOrDefaultAsync(CancellationToken.None);
                    var game1 = await contextInMemory.Games.Include(g => g.Players).SingleOrDefaultAsync(g => g.Id == gameid1.Id, CancellationToken.None);
                    


                    Assert.Equal(game1.Players[0].UserId, userid);
            }
        }


        public static IEnumerable<object[]> gameDataB => new List<object[]>
        {
            new object[]
            {
                "chris", "chrisid", "multiplayer", 3

            },
            new object[]
            {
                "chris1", "chrisid1", "twoplayer", 2

            }
            
        
        };





        


    }


}

