using System;
using Xunit;
using Moq;
using GuessingGame;
using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using GuessingGame.Core.Domain.Game.GameHandler;
using Microsoft.AspNetCore.Hosting;
using GuessingGame.Core.Domain.Image.Services;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using GuessingGame.Core.Domain.Game.Events;
using System.Linq;
using System.Threading;
using Shouldly;
using GuessingGame.Core.Domain.Image.Pipelines;
using GuessingGame.Core.Domain.AI.Handlers;
using GuessingGame.Core.Domain.Image.Handlers;
using GuessingGame.Core.Domain.Image;

namespace GuessingGameTests
{


    public class CreateGameTest : DbTest
    {
        


        public CreateGameTest(ITestOutputHelper output) : base(output)
        {
           
        }

        // Here we are getting the image from the game context, checking if the imageid is set to the gameid.

        [Theory]
        [MemberData(nameof(data))]
         public async Task CreateGame_TestIsProposer_With_Image(string displayname, string userid, string mode, int numplayers, int gameid)
		{

            
            
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {

			
                var fakeImage = new Image();
                fakeImage.Id = gameid;
                fakeImage.ImageId = gameid;
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


                var eventGameCreated = new GameCreated(gameid);
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

                var request = new CreateGame.Request(displayname, userid,  mode, numplayers);
				var handler_Create = new CreateGame.Handler(contextInMemory,  mediator.Object, gameCreation);
				var game = handler_Create.Handle(request, CancellationToken.None).GetAwaiter().GetResult();

                await contextInMemory.SaveChangesAsync();
                
                

                var gameid1 = await contextInMemory.Games.Select(x => x.Image).SingleOrDefaultAsync(CancellationToken.None);
                gameid1.ImageId.ShouldBe(gameid);
                
        }}
			
        public static IEnumerable<object[]> data => new List<object[]>
        {
            new object[]
            {
                "ja", "jaid", "twoplayer", 2, 1
                 
            },
            new object[]
            {
                "ja", "jaid", "multiplayer", 3, 2
                 
            }


        };
        // Here we are checking if gameid matches the given id.

        [Theory]
        [MemberData(nameof(dataB))]
         public async Task CreateGame_TestIsProposer(string displayname, string userid, string mode, int numplayers, int gameid)
		{   
            
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {

			
                var fakeImage = new Image();
                fakeImage.Id = gameid;
                fakeImage.ImageId = gameid;
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


                var eventGameCreated = new GameCreated(gameid);
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

                var request = new CreateGame.Request(displayname, userid,  mode, numplayers);
				var handler_Create = new CreateGame.Handler(contextInMemory,  mediator.Object, gameCreation);
				var game = handler_Create.Handle(request, CancellationToken.None).GetAwaiter().GetResult();

                await contextInMemory.SaveChangesAsync();
                
                
                 // Using a new context to prevent cached entities from short circuiting the database.

                using (var context = new ApplicationDbContext(ContextOptions, null))
                {
                    var _requestId = new GetGame.Request(gameid);
                    var _handlerId = new GetGame.Handler(context);
                    var _gameId = _handlerId.Handle(_requestId, CancellationToken.None).GetAwaiter().GetResult();
                    _gameId.Id.ShouldBe(gameid);
                }

                var _game = await contextInMemory.Games.Select(g => g.Id == gameid).SingleOrDefaultAsync(CancellationToken.None);  
                _game.ShouldBe(true);
                
                
        }}
			
        public static IEnumerable<object[]> dataB => new List<object[]>
        {
            new object[]
            {
                "ja", "jaid", "twoplayer", 2, 1
                 
            },
            new object[]
            {
                "ja", "jaid", "multiplayer", 3, 1
                 
            }


        };
        
    }

    
}

