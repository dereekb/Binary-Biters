using System;

namespace Biters.Utility
{
	/*
	 * Factory for making new elements of type T.
	 */
	public interface IFactory<T> {

		T Make();

	}

}

