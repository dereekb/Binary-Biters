using System;
using Biters.Utility;

namespace Biters
{
	
	public interface IGameMapTileExporter<T, E> : IMapTileExporter<T>
		where T : class, IGameMapTile
	where E : class, IGameMapEntity {
		
		void Export(IGameMap<T, E> GameMap);
		
	}

	//TODO: Extend later.
	public interface IGameMapTileImporter<T, E> : IMapTileImporter<T>
		where T : class, IGameMapTile
		where E : class, IGameMapEntity {

	}

	/*
	 * Exports a map.
	 */
	public interface IMapTileExporter<T>
		where T : class, IMapTile 
	{

		void Export(IMap<T> Map);

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

