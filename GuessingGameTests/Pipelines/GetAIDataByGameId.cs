using Xunit;
using Moq;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Data;
using MediatR;
using System.Collections.Generic;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Shouldly;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.AI.Pipelines;
using GuessingGame.Core.Domain.AI;


namespace GuessingGameTests
{
    public class GetAIDataByGameIdTest : DbTest
    {
        // in this test we are testing the GetAIDataByGameId pipeline. 
        // We are adding a game, and a AIData to the inmemory database with guesses.
        // Then we are using the pipelines handler to check the count of the guesses, and comparing it to have many guesses we added
        public GetAIDataByGameIdTest(ITestOutputHelper output) : base(output)
        {  
        }

        [Theory]
        [MemberData(nameof(data))]
        public void GetAIDataByGameId_ShouldBe_One(int id,  int numplayers, string guess)
        {
            var _gameMode = new GameMode();
            var _players = new List<Player> { new Player("chris", "chrisid", 3) };
            _gameMode = GameMode.SingelPlayer;
            var fakeGame1 = new Game(_players, _gameMode);
            fakeGame1.Players = _players;
            fakeGame1.numplayers = numplayers;
            fakeGame1.Image = new Image();
            fakeGame1.Id = id;
            fakeGame1.GameStatus = GameStatus.Finished;
            fakeGame1.GameMode = GameMode.SingelPlayer;
            fakeGame1.UsedTiles = " ";
            fakeGame1.Image.Id = id;
            var ai = new AIData(id);
            var _guess = new List<Guess> { new Guess(guess, " ", true) };
            ai.Guesses = _guess;

            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {
                contextInMemory.Database.Migrate();
                contextInMemory.Games.Add(fakeGame1);
                contextInMemory.AI.Add(ai);
                contextInMemory.SaveChanges();
            }

            // Using a new context to prevent cached entities from short circuiting the database.

            using (var context = new ApplicationDbContext(ContextOptions, null))
            {
                var request = new GetAIDataByGameId.Request(id);
                var handler = new GetAIDataByGameId.Handler(context);
                var game = handler.Handle(request, CancellationToken.None).GetAwaiter().GetResult();
                game.Guesses.Count.ShouldBe(1);
            }
        }

        public static IEnumerable<object[]> data => new List<object[]>
        {
            new object[]
            {
                 2,  2, "guess"

            }

        };
        
        [Theory]
        [MemberData(nameof(dataB))]
        public void GetAIDataByGameId_ShouldBe_Two(int id,  int numplayers, string guess)
        {
            var mediator = new Mock<Mediator>();
            var _gameMode = new GameMode();
            var _players = new List<Player> { new Player("chris", "chrisid", 3) };
            _gameMode = GameMode.SingelPlayer;
            var fakeGame1 = new Game(_players, _gameMode);
            fakeGame1.Players = _players;
            fakeGame1.numplayers = numplayers;
            fakeGame1.Image = new Image();
            fakeGame1.Id = id;
            fakeGame1.GameStatus = GameStatus.Finished;
            fakeGame1.GameMode = GameMode.SingelPlayer;
            fakeGame1.UsedTiles = " ";
            fakeGame1.Image.Id = id;
            var ai = new AIData(id);
            var _guess = new List<Guess> { new Guess(guess, " ", true), new Guess(guess, " ", false) };
            ai.Guesses = _guess;

            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {
                contextInMemory.Database.Migrate();
                contextInMemory.Games.Add(fakeGame1);
                contextInMemory.AI.Add(ai);
                contextInMemory.SaveChanges();
            }

            using (var context = new ApplicationDbContext(ContextOptions, null))
            {
                var request = new GetAIDataByGameId.Request(id);
                var handler = new GetAIDataByGameId.Handler(context);
                var game = handler.Handle(request, CancellationToken.None).GetAwaiter().GetResult();
                game.Guesses.Count.ShouldBe(2);
            }
        }

        public static IEnumerable<object[]> dataB => new List<object[]>
        {
            new object[]
            {
                 2,  2, "guess"

            }

        };
    }

    
}

