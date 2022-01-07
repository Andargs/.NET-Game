using System.Collections.Generic;

namespace GuessingGame.SharedKernel
{
	public abstract class BaseEntity
	{
		public List<BaseDomainEvent> Events = new();
	}
}