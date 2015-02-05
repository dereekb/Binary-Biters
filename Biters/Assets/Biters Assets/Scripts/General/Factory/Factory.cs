using System;

namespace Biters
{
	/*
	 * Factory for making new elements of type T.
	 */
	public interface Factory<T> {

		T make();

	}

}

