using Xunit;
using Moq;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Data;
using MediatR;
using System.Collections.Generic;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using GuessingGame.Core.Domain.Image;

namespace GuessingGameTests
{


    public class GetGameIdTest : DbTest
    {
        public GetGameIdTest(ITestOutputHelper output) : base(output)
        { 
        }

        // Here we are testing the GetGameId pipeline. We do this by adding a game to our inmemory database,
        //  and checking if the return value (Gameid) from the handler is the same as the id we gave the game. 

        [Theory]
        [MemberData(nameof(data))]
        public void GetGameId(int id, GameMode GameMode, GameStatus GameStatus, List<Player> Players, Image Image, int numplayers)
        {
            var mediator = new Mock<Mediator>();
            var fakeGame = new Game(Players, GameMode);
            fakeGame.Id = id;
            fakeGame.GameMode = GameMode;
            fakeGame.Image = Image;
            fakeGame.Players = Players;
            fakeGame.numplayers = numplayers;
            fakeGame.GameStatus = GameStatus;
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {


                contextInMemory.Database.Migrate();
                contextInMemory.Games.Add(fakeGame);
                contextInMemory.SaveChanges();

            }
            // Using a new context to prevent cached entities from short circuiting the database. 
            using (var context = new ApplicationDbContext(ContextOptions, null))
            {
                var request = new GetGameId.Request();
                var handler = new GetGameId.Handler(context);
                var gameid = handler.Handle(request, CancellationToken.None).GetAwaiter().GetResult();
                Assert.Equal(gameid, id);


            }
        }

        public static IEnumerable<object[]> data => new List<object[]>
        {

            new object[]
            {
                1, GameMode.SingelPlayer, GameStatus.Active, new List<Player>{new Player("hmh", "hmh", 3)}, new Image(), 1
            },
            new object[]
            {
                 2,GameMode.TwoPlayer, GameStatus.Finished, new List<Player>{new Player("hmh", "hmh", 3)}, new Image(), 2
            }
            


        };
        
    }

    
}

