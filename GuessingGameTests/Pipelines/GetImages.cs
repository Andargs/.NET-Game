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
using GuessingGame.Core.Domain.Image.Pipelines;
using GuessingGame.Core.Domain.Image;

namespace GuessingGameTests
{


    public class GetImagesTest : DbTest
    {
        
        public GetImagesTest(ITestOutputHelper output) : base(output)
        {
            
        }
        //This test checks if the pipeline GetImages returns multiple images. We do this by adding two games with two images. And then we check the count from the handler. 
        [Theory]
        [MemberData(nameof(data))]
        public void GetImages(int id,  GameMode GameMode, GameStatus GameStatus, List<Player> Players, Image Image, int numplayers)
        {
            var mediator = new Mock<Mediator>();
            var fakeGame = new Game(Players, GameMode);
            fakeGame.Id = id;
            fakeGame.GameMode = GameMode;
            fakeGame.Image = Image;
            fakeGame.Players = Players;
            fakeGame.numplayers = numplayers;
            fakeGame.GameStatus = GameStatus;

            var fakeGame2 = new Game(Players, GameMode);
            fakeGame2.Id = 3;
            fakeGame2.GameMode = GameMode.SingelPlayer;
            fakeGame2.Image = new Image();
            fakeGame2.Players = new List<Player>{new Player("hmh1", "hmh1", 3)};
            fakeGame2.numplayers = 1;
            fakeGame2.GameStatus = GameStatus.New;
            // var _image = new Image();
            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {


                contextInMemory.Database.Migrate();
                contextInMemory.Games.Add(fakeGame);
                contextInMemory.Games.Add(fakeGame2);
                contextInMemory.SaveChanges();

            }
            // Using a new context to prevent cached entities from short circuiting the database. 

            using (var context = new ApplicationDbContext(ContextOptions, null))
            {
                var request = new GetImages.Request();
                var handler = new GetImages.Handler(context);
                var _Images = handler.Handle(request, CancellationToken.None).GetAwaiter().GetResult();

                _Images.Count.ShouldBe(2);


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
                 2,  GameMode.TwoPlayer, GameStatus.Finished, new List<Player>{new Player("hmh", "hmh", 3)}, new Image(), 2

            }
            


        };
        
    }

    
}

