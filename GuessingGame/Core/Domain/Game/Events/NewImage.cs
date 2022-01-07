using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Game;

namespace GuessingGame.Core.Domain.Game.Events
{
	public record NewImage : BaseDomainEvent
	{
		public NewImage(int imageId)
		{
			ImageId = imageId;
		}

		public int ImageId { get; set; }
	}
}