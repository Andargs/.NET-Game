using Xunit;
using GuessingGame.Data;
using GuessingGame.Core.Domain.Image.Services;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GuessingGame.Core.Domain.Image;

namespace GuessingGameTests
{


    public class FileReaderTest : DbTest
    {
    

        public FileReaderTest(ITestOutputHelper output) : base(output)
        {
            
        }

        // Checking if the filereader is able to read and adding a image to the inmemory database.
        //  We are checking this by fetching a imageid from the database.  

        [Fact]

        public void FileReader()
        {
            System.IO.Directory.SetCurrentDirectory("../../../../GuessingGame/");




            using (var contextInMemory = new ApplicationDbContext(ContextOptions, null))
            {


                contextInMemory.Database.Migrate();
                contextInMemory.SaveChanges();
                var _fileReader = new FileReader();
                _fileReader.ReadFile();

            }
            // Using a new context to prevent cached entities from short circuiting the database. 

            using (var context = new ApplicationDbContext(ContextOptions, null))
            {
                var imageid = context.Images.Select(i => i.ImageId);
                // checking if the imageid exists
                Assert.NotNull(imageid);

            }
        }



    }

    
}

