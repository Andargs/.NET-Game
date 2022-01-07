using System;
using System.Collections.Generic;
using GuessingGame.SharedKernel;

namespace GuessingGame.Core.Domain.Image
{
    public class Image : BaseEntity 
    {

        public int Id {get;set;}

        public string ImageName {get; set;}  //Image label

        public int ImageId {get; set;}  //Image numer which shares the same label

        public string ImageMap {get; set;} = "";  //Image path


    }
}