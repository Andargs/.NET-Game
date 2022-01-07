using Xunit;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Data;
using System.Collections.Generic;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Shouldly;
using GuessingGame.Core.Domain.Image;

namespace GuessingGameTests
{
    public class GetPlayerScoresTest : DbTest  {
        
        // This test if for checking the GetPlayerScore Pipeline. We do this by checking the count of the list that is returned by GetPlayerScore handler. If we add two players the count of the list is going to be two. 
        public GetPlayerScoresTest(ITestOutputHelper output) : base(output)
        { 
        }

        [Theory]
        [MemberData(nameof(data))]
        public void GetPlayerScores_shouldBeOne(List<Player> players)
        {
            
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {

                var _players = players;
                _players[0].Score = 100;
                var _gameMode = new GameMode();
                _gameMode = GameMode.SingelPlayer;
                var fakeGame1 = new Game(_players, _gameMode);
                fakeGame1.Players = _players;
                
                fakeGame1.numplayers = 1;
                fakeGame1.Image = new Image();
                fakeGame1.Id = 1;
                fakeGame1.GameStatus = GameStatus.Finished;
                fakeGame1.GameMode = GameMode.SingelPlayer;

                contextInMemory.Database.Migrate();
                contextInMemory.Games.Add(fakeGame1);
                contextInMemory.SaveChanges();

            }

            using (var context = new ApplicationDbContext(ContextOptions, null))
            {
                var request = new GetPlayerScores.Request();
                var handler = new GetPlayerScores.Handler(context);
                var game = handler.Handle(request, CancellationToken.None).GetAwaiter().GetResult();

                game.Count.ShouldBe(1);


            }
        }

        public static IEnumerable<object[]> data => new List<object[]>
        {
            new object[]
            {
                new List<Player>{new Player("hm", "chris2", 3)}

            }

        };


        [Theory]
        [MemberData(nameof(dataB))]
        public void GetPlayerScores_shouldBe2(List<Player> players)
        {
            
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {

                var _players = players;
                _players[0].Score = 100;
                _players[1].Score = 100;
                var _gameMode = new GameMode();
                _gameMode = GameMode.SingelPlayer;
                var fakeGame1 = new Game(_players, _gameMode);
                fakeGame1.Players = _players;
                
                fakeGame1.numplayers = 1;
                fakeGame1.Image = new Image();
                fakeGame1.Id = 1;
                fakeGame1.GameStatus = GameStatus.Finished;
                fakeGame1.GameMode = GameMode.SingelPlayer;

                contextInMemory.Database.Migrate();
                contextInMemory.Games.Add(fakeGame1);
                contextInMemory.SaveChanges();

            }
            // Using a new context to prevent cached entities from short circuiting the database. 

            using (var context = new ApplicationDbContext(ContextOptions, null))
            {
                var request = new GetPlayerScores.Request();
                var handler = new GetPlayerScores.Handler(context);
                var score = handler.Handle(request, CancellationToken.None).GetAwaiter().GetResult();

                score.Count.ShouldBe(2);


            }
        }

        public static IEnumerable<object[]> dataB => new List<object[]>
        {
            new object[]
            {
                new List<Player>{new Player("hm", "chris2", 3), new Player("hm2", "chris1", 3)}

            }

        };
        
        
    }

    
}

