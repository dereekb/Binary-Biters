using System;
using UnityEngine;

namespace Biters
{
	/*
	 * Represents a Game Map that contains entities in addition to map tiles.
	 */
	public class GameMap<T, E> : Map<T> 
		where T : MapTile
		where E : GameMapEntity 
	{
		private EventSystem<GameMapEvent, GameMapEventInfo> gameMapEvents;

		public GameMap(GameObject GameObject, MapTileFactory<T> TileFactory) : base(GameObject, TileFactory) {
			this.gameMapEvents = new EventSystem<GameMapEvent, GameMapEventInfo> ();
		}
		
		#region Entity
		
		/*
		 * TODO: Add Entity Functions
		 * - AddEntity(Entity, Position)
		 * - RemoveEntity(Entity)
		 * - GetEntitiesAtPosition(WorldPosition)
		 * etc.
		 */
		
		#endregion

		#region gameMapEvents
		
		protected EventSystem<GameMapEvent, GameMapEventInfo> GameMapEvents {
			
			get {
				return this.gameMapEvents;
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
		private void BroadcastEvent(GameMapEvent GameMapEvent) {
			GameMapEventInfo info = this.DefaultMapEventInfo(GameMapEvent);
			this.gameMapEvents.BroadcastEvent (GameMapEvent, info);
		}
		
		private GameMapEventInfo DefaultMapEventInfo(GameMapEvent GameMapEvent) {
			return new GameMapEventInfo(GameMapEvent, this as GameMap<MapTile, GameMapEntity>);
		}
		
		#endregion
	}
	
	/*
	 * Entity Placed on an Entity Map.
	 */
	public interface GameMapEntity : GameElement {

		//TODO: Add anything..?

	}


}

