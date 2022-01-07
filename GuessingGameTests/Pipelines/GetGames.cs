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
using Shouldly;
using GuessingGame.Core.Domain.Image;

namespace GuessingGameTests
{


    public class GetGamesTest : DbTest
    {
        // In this test we are checking the pipeline GetGames. We are adding two games to the database and checking if the count from the handler returns two.
        public GetGamesTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [MemberData(nameof(data))]
        public void GetGames(int id,  GameMode GameMode, GameStatus GameStatus, List<Player> Players, Image Image, int numplayers)
        {
            var mediator = new Mock<Mediator>();
            var _players = new List<Player> { new Player("fake", "fakeid", 3) };
            var _gameMode = new GameMode();
            _gameMode = GameMode.SingelPlayer;
            var fakeGame1 = new Game(_players, _gameMode);
            fakeGame1.Players = _players;
            fakeGame1.numplayers = 1;
            fakeGame1.Image = new Image();
            fakeGame1.Id = 420;
            fakeGame1.GameStatus = GameStatus.Finished;
            fakeGame1.GameMode = GameMode.SingelPlayer;

            var fakeGame = new Game(Players, GameMode);
            fakeGame.Id = id;
            fakeGame.GameMode = GameMode;
            fakeGame.Image = Image;
            fakeGame.Players = Players;
            fakeGame.numplayers = numplayers;
            fakeGame.GameStatus = GameStatus;
            // Here are we using a inmemory database
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {
                contextInMemory.Database.Migrate();
                contextInMemory.Games.Add(fakeGame);
                contextInMemory.Games.Add(fakeGame1);
                contextInMemory.SaveChanges();

            }

            
            // Using a new context to prevent cached entities from short circuiting the database. 
            using (var context = new ApplicationDbContext(ContextOptions, null))
            {
                var request = new GetGames.Request();
                var handler = new GetGames.Handler(context);
                var game = handler.Handle(request, CancellationToken.None).GetAwaiter().GetResult();
                game.Count.ShouldBe(2);
            }
        }

        public static IEnumerable<object[]> data => new List<object[]>
        {

            new object[]
            {
                1,  GameMode.SingelPlayer, GameStatus.Active, new List<Player>{new Player("hmh", "hmh", 3)}, new Image(), 1

            },
            new object[]
            {
                 2,  GameMode.TwoPlayer, GameStatus.Finished, new List<Player>{new Player("hmh", "hmh", 3)}, new Image(), 2


            },
            new object[]
            {
                 2,  GameMode.MultiPlayer, GameStatus.Finished, new List<Player>{new Player("hmh", "hmh", 3)}, new Image(), 3

            }
        };
        
    }

    
}

