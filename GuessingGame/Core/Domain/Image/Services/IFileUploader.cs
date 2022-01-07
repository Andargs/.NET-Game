using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Drawing;
namespace GuessingGame.Core.Domain.Image.Services
{
    public interface IFileUploader
    {
        Task<int> SliceImage(IFormFile FormFile, string imageName, int numslices);
        Task<int> SliceCustomImage(Bitmap bmp, string ImageName);
        Task<int> FloodFill(Bitmap bmp, string ImageName, Point pt);
    }
}