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
    public class GetAIDataByImageIdTest : DbTest
    {
        public GetAIDataByImageIdTest(ITestOutputHelper output) : base(output)
        {
        }

        // Here we are checking the GetAIDataByImageId pipeline. We are doing this by making a new game, 
        // guess and aiData, and saving it to our inmemory database. We are setting the guess to be false,
        //  so we can check if the handler return the false guess.
        [Theory]
        [MemberData(nameof(data))]
        public void GetAIDataByImageId(int id, Image Image, int numplayers, string guess)
        {
            
            var _gameMode = new GameMode();
            var _players = new List<Player> { new Player("chris", "chrisid", 3) };
            _gameMode = GameMode.SingelPlayer;
            var fakeGame1 = new Game(_players, _gameMode);
            fakeGame1.Players = _players;
            fakeGame1.numplayers = numplayers;
            var _image = Image;
            _image.ImageId = id;
            _image.Id = id;
            fakeGame1.Image = Image;
            fakeGame1.Id = id;
            fakeGame1.GameStatus = GameStatus.Finished;
            fakeGame1.GameMode = GameMode.SingelPlayer;
            fakeGame1.UsedTiles = " ";
            fakeGame1.Image.Id = id;
            fakeGame1.Image.ImageId = id;
            var ai = new AIData(id);
            var _guess = new List<Guess> { new Guess(guess, " ", false) };
            ai.Guesses = _guess;
            ai.ImageId = id;

            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {
                contextInMemory.Database.Migrate();
                contextInMemory.Games.Add(fakeGame1);
                contextInMemory.Images.Add(_image);
                contextInMemory.AI.Add(ai);
                contextInMemory.SaveChanges();
            }
            // Using a new context to prevent cached entities from short circuiting the database.

            using (var context = new ApplicationDbContext(ContextOptions, null))
            {
                var request = new GetAIDataByImageId.Request(id);
                var handler = new GetAIDataByImageId.Handler(context);
                var game = handler.Handle(request, CancellationToken.None).GetAwaiter().GetResult();
                game.Correct.ShouldBe(false);
            }
        }

        public static IEnumerable<object[]> data => new List<object[]>
        {
            new object[]
            {
                 2,  new Image(), 2, "guess"

            },
            
        };
        
    }

    

    
}

