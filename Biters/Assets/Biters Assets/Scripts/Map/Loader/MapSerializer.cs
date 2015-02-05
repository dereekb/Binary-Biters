using System;

namespace Biters
{

	/*
	 * Exports a map.
	 */
	public interface MapExporter<T> 
		where T : MapTile 
	{

		void Export(Map<T> Map);

	}

	/*
	 * Imports a map by name.
	 */
	public interface MapImporter<T>
		where T : MapTile
	{
		
		Map<T> Import(string map);

	}

}

