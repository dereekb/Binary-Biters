using System;
using UnityEngine;

namespace Biters
{
	public class Map<T> : Entity where T : MapTile
	{
		protected EventSystem<MapEvent, MapEventInfo> events;
		protected MapTileFactory<T> TileFactory;
		protected World<T> World;

		public Map(GameObject GameObject, MapTileFactory<T> TileFactory) : base(GameObject) {
			this.TileFactory = TileFactory;
			this.events = new EventSystem<MapEvent, MapEventInfo> ();
		}

		#region World

		public void resetWorld() {
			//Observer event: World Will Reset.
		}

		/* TODO: Set Tile Functions
		 * - GetTile
		 * - SetTile
		 * - ResetTile
		 * - ClearTile
		 */

		#endregion

		#region Entity

		/*
		 * TODO: Add Entity Functions
		 * - AddEntity
		 * - RemoveEntity
		 */

		#endregion

		#region Events

		/*
		 * TODO: Add Event Functions
		 * - Send Event
		 */

		#endregion
	}

	/*
	 * Entity Placed on a given map.
	 */
	public interface MapEntity : GameElement {
		//Added to Map
	}
	
	//Represents a Tile on a map.
	public interface MapTile : GameElement, WorldElement {

		//TODO: Complete.

	}

	/*
	 * Factory for building maps tiles.
	 * 
	 * Should only build the tiles.
	 */
	public interface MapTileFactory<T> : Factory<T> 
		where T : MapTile {

		World<T> makeWorld();
		
	}

	/*
	 * 
	 */
	public interface MapObserver {

	}

}

