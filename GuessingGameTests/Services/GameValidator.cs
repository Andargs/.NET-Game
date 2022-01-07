using Xunit;
using GuessingGame;
using GuessingGame.Core.Domain.Player;
using GuessingGame.Core.Domain.Game;
using System.Collections.Generic;
using Xunit.Abstractions;


namespace GuessingGameTests
{

    // Here we are testing the gamevalidator, this is the validator that is called when the user is creating the game.
    public class GameValidatorTest 
    {
        public GameValidatorTest()  
         {
     
        }
        [Theory]
        [MemberData(nameof(data))]

        public void GameValidator(List<Player> players, GameMode gameMode, int id, GameStatus gameStatus, int numplayers, int error)
        {
            var fake = new Game(players, gameMode);
            fake.Id = id;
            fake.GameStatus = gameStatus;
            fake.numplayers = numplayers;
            fake.GameMode = gameMode;
            fake.GameStatus = gameStatus;

            var validator = new GameValidator();
            Assert.Equal(error, validator.IsValid(fake).Length);
        }
        public static IEnumerable<object[]> data => new List<object[]>
        {
            new object[]
            {
                new List<Player>{new Player("hm", "chris2", 3)}, GameMode.TwoPlayer, 1, GameStatus.Active, 2, 0


            },
            new object[]
            {
                new List<Player>{new Player("hm", "chris2", 3)}, GameMode.MultiPlayer, 1, GameStatus.Active, 2, 0


            },
            new object[]
            {
                new List<Player>{new Player("hm", "chris2", 3)}, GameMode.SingelPlayer, 1, GameStatus.Active, 1, 0


            },

            
            new object[]
            {
                new List<Player>{new Player("hm", "chris2", 3)}, GameMode.MultiPlayerOracle, 1, GameStatus.Active, 4, 0


            },
            
            new object[]
            {
                new List<Player>{new Player("hm", "chris2", 3)}, GameMode.SingelPlayer, 1, GameStatus.Active, 0, 1


            },
            
            new object[]
            {
                new List<Player>{new Player("hm", "chris2", 3)}, GameMode.MultiPlayer, 1, GameStatus.Active, 1, 1


            },
            
            new object[]
            {
                new List<Player>{new Player("hm", "chris2", 3)}, GameMode.MultiPlayer, 0, GameStatus.Active, 1, 1


            },
            // 
            new object[]
            {
                new List<Player>{new Player("hm", "chris2", 3)}, GameMode.MultiPlayer, 1, GameStatus.Active, 7, 1


            },
            new object[]
            {
                new List<Player>{new Player("hm", "chris100", 3), new Player("hm", "chris99", 3), new Player("hm", "chris2", 3), new Player("hm", "chris7", 3), new Player("hm", "chris6", 3), new Player("hm", "chris5", 3),new Player("hm", "chris4", 3),new Player("hm", "chris3", 3)}, GameMode.MultiPlayer, 1, GameStatus.Active, 7, 2


            },
            new object[]
            {
                new List<Player>{new Player(" ", "chris2", 3)}, GameMode.SingelPlayer, 1, GameStatus.Active, 1, 1

            },
            new object[]
            {
                new List<Player>{new Player(" ", "chris2", 3)}, GameMode.MultiPlayerOracle, 1, GameStatus.Active, 1, 2

            },
            new object[]
            {
                new List<Player>{new Player("yea", "chris2", 3)}, GameMode.MultiPlayerOracle, 1, GameStatus.Active, 4, 0

            },
            new object[]
            {
                new List<Player>{new Player("yea", "chris2", 3)}, GameMode.MultiPlayerOracle, 1, GameStatus.Active, 1, 1

            }
                        
        };
    }

}