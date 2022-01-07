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


    public class GetGameTest : DbTest
    {
        // Here we are testing the pipeline GetGame. We are doing this by adding a game to our inmemory database and checking if the handler from the pipelin returns the same game as the one we added.
        public GetGameTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [MemberData(nameof(data))]
        public void GetGame(int id, GameMode GameMode, GameStatus GameStatus, List<Player> Players, Image Image, int numplayers)
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
                var request = new GetGame.Request(id);
                var handler = new GetGame.Handler(context);
                var game = handler.Handle(request, CancellationToken.None).GetAwaiter().GetResult();
                game.Equals(fakeGame);
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
                 2, GameMode.TwoPlayer, GameStatus.Finished, new List<Player>{new Player("hmh", "hmh", 3)}, new Image(), 2


            }
        };
        
    }

    
}

