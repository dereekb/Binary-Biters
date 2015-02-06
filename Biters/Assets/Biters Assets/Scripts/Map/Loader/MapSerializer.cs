using System;

namespace Biters
{

	/*
	 * Exports a map.
	 */
	public interface IMapExporter<T> 
		where T : IMapTile 
	{

		void Export(Map<T> Map);

	}

	/*
	 * Imports a map by name.
	 */
	public interface IMapImporter<T>
		where T : IMapTile
	{
		
		Map<T> Import(string map);

	}

}

