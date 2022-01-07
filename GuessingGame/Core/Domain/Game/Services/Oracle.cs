using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using GuessingGame.SharedKernel;
using GuessingGame.Data;

namespace GuessingGame.Core.Domain.Game
{
    public class Oracle
    {
        public int ImageId { get; set; }
        public string ImageName {get; set;}
        public string ImageFolderPath {get; set;}
        public int TileId {get; set;}

        //Verifies the users guess.
        public bool VerifyGuess(string guess)
        {
            if(guess == ImageName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Gets the full path to the folder that contains the images for ImageId
        public void GetImageFolderPath(string value)
        {
            ImageFolderPath = "img/" + value + "_scattered/" + TileId + ".png";
        }
    }
}