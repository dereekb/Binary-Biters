using System;
using UnityEngine;

namespace Biters
{
	public class Map<T> : Entity where T : MapTile
	{
		private EventSystem<MapEvent, MapEventInfo> events;
		protected MapTileFactory<T> TileFactory;
		protected World<T> World;

		public Map(GameObject GameObject, MapTileFactory<T> TileFactory) : base(GameObject) {
			this.TileFactory = TileFactory;
			this.events = new EventSystem<MapEvent, MapEventInfo> ();
		}

		#region World

		public void resetWorld() {

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

		protected EventSystem<MapEvent, MapEventInfo> Events {

			get {
				return this.events;
			}

		}

		/*
		 * TODO: Add Event Functions
		 * - Send Event
		 * - Register for Event
		 * - Unregister for Event
		 */

		/*
		 * Convenience function for broadcasting an event with default arguments. 
		 */
		private void BroadcastEvent(MapEvent MapEvent) {
			MapEventInfo info = this.DefaultMapEventInfo (MapEvent);
			this.events.BroadcastEvent (MapEvent, info);
		}

		private MapEventInfo DefaultMapEventInfo(MapEvent MapEvent) {
			return new MapEventInfo(MapEvent, this as Map<MapTile>);
		}

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

}

