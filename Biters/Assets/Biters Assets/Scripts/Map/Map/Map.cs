using System;
using UnityEngine;

namespace Biters
{

	/*
	 * Represents a map made up of tiles.
	 */
	public class Map<T> : Entity 
		where T : MapTile
	{
		private EventSystem<MapEvent, MapEventInfo> mapEvents;
		protected MapTileFactory<T> TileFactory;

		protected World<T> World;

		public Map(GameObject GameObject, MapTileFactory<T> TileFactory) : base(GameObject) {
			this.TileFactory = TileFactory;
			this.mapEvents = new EventSystem<MapEvent, MapEventInfo> ();
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

		#region Update

		public override void Update(Time time) {
			//TODO: Call update on all tiles.
		}

		#endregion

		#region mapEvents

		protected EventSystem<MapEvent, MapEventInfo> MapEvents {

			get {
				return this.mapEvents;
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
			this.mapEvents.BroadcastEvent (MapEvent, info);
		}

		private MapEventInfo DefaultMapEventInfo(MapEvent MapEvent) {
			return new MapEventInfo(MapEvent, this as Map<MapTile>);
		}

		#endregion
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

