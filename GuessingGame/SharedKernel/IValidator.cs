using System.Collections.Generic;

namespace GuessingGame.SharedKernel
{
	public interface IValidator<T>
	{
		(bool IsValid, string Error) IsValid(T item);
	}
}