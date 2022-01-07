using GuessingGame.Core.Domain.Image;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GuessingGame.Core.Domain.Image.Services
{
    public interface IImageCombiner
    {

        Task<int> Startup();

        string FindTile(int x, int y, string map, int Height, int Width);

    }
}