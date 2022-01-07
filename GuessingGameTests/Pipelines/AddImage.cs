using Xunit;
using GuessingGame.Data;
using System.Collections.Generic;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Shouldly;
using GuessingGame.Core.Domain.Image.Pipelines;
using GuessingGame.Core.Domain.Image;

namespace GuessingGameTests
{


    public class AddImageTest : DbTest
    {
        public AddImageTest(ITestOutputHelper output) : base(output)
        {  
        }

        // here we are testing if we can add an image with the AddImage pipeline.

        [Theory]
        [MemberData(nameof(data))]
        public void AddImage_Should_Return_True(int ImageId, string ImageName, string ImageMap)
        {

            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {
                contextInMemory.Database.Migrate();
                contextInMemory.SaveChanges();
            }
            // Using a new context to prevent cached entities from short circuiting the database.

            using (var context = new ApplicationDbContext(ContextOptions, null))
            {
                var request = new AddImage.Request(ImageId, ImageName, ImageMap);
                var handler = new AddImage.Handler(context);
                var game = handler.Handle(request, CancellationToken.None).GetAwaiter().GetResult();

                game.Success.ShouldBe(true);
            }
        }

        public static IEnumerable<object[]> data => new List<object[]>
        {

            new object[]
            {
                1, "test", " "


            }, 
            new object[]
            {
                2, "test", "test"


            },
            new object[]
            {
                null, null, null


            }

    
        };

    }

    
}

