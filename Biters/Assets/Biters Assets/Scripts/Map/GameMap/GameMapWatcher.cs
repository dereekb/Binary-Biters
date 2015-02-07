using System;
using System.Collections;
using System.Collections.Generic;

namespace Biters
{
	#region Interfaces

	/*
	 * Class for watching a game map and changes that may occur. Has access to a Game Map's event system.
	 */
	public interface IGameMapWatcher<T, E, O>
		where T : class, IMapTile
		where E : class, IGameMapEntity
		where O : IGameMapWatcherObservation<T, E> 
	{

		O Observe(GameMap<T, E> Map);

	}

	/*
	 * Items that the watcher has seen.
	 */
	public interface IGameMapWatcherObservation<T, E> 
		where T : class, IMapTile
		where E : class, IGameMapEntity
	{
		//Entity Entered Tile
		IEnumerable<TileEntityTuple<T, E>> EntitiesEnteredTile { get; }

		//Entity Exited Tile
		IEnumerable<TileEntityTuple<T, E>> EntitiesExitedTile { get; }

		//Custom Events
		IEnumerable<GameMapEventInfoBuilder> CustomEvents { get; }

	}

	#endregion

	#region Default Implementation

	public class GameMapWatcher<T, E> : IGameMapWatcher<T, E, GameMapWatcherObservation<T, E>>
		where T : class, IMapTile
		where E : class, IGameMapEntity
	{
		
		public GameMapWatcherObservation<T, E> Observe(GameMap<T, E> Map) {
			//TODO: Add Observation.
			return null;
		}

	}

	public class GameMapWatcherObservation<T, E> : IGameMapWatcherObservation<T, E> 
		where T : class, IMapTile
		where E : class, IGameMapEntity
	{
		public IEnumerable<TileEntityTuple<T, E>> EntitiesEnteredTile { get; set; }
		public IEnumerable<TileEntityTuple<T, E>> EntitiesExitedTile { get; set; }
		public IEnumerable<GameMapEventInfoBuilder> CustomEvents { get; set; }
	}

	#endregion

	public struct TileEntityTuple<T, E> 
		where T : class, IMapTile
		where E : class, IGameMapEntity
	{
		public readonly T Tile;
		public readonly E Entity;

		public TileEntityTuple(T Tile, E Entity) {
			this.Tile = Tile;
			this.Entity = Entity;
		}

	}

}

