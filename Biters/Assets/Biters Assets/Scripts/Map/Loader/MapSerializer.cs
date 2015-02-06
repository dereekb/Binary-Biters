using System;

namespace Biters
{

	/*
	 * Exports a map.
	 */
	public interface IMapTileExporter<T>
		where T : class, IMapTile 
	{

		void Export(Map<T> Map);

	}

	/*
	 * Imports a map by name.
	 */
	public interface IMapTileImporter<T>
		where T : class, IMapTile
	{

		//Returns a factory for recreating the loaded map.
		IImportedMap<T> Import(string map);

	}

	public interface IImportedMap<T> : IFactory<World<T>> {}

}

