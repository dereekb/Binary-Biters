using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biters
{
	/*
	 * Represents a Game Map that contains entities in addition to map tiles.
	 */
	public class GameMap<T, E> : Map<T> 
		where T : IMapTile
		where E : IGameMapEntity 
	{
		private HashSet<E> entities;
		private EventSystem<GameMapEvent, GameMapEventInfo> gameMapEvents;

		public GameMap(GameObject GameObject, IFactory<World<T>> WorldFactory) : base(GameObject, WorldFactory) {
			this.gameMapEvents = new EventSystem<GameMapEvent, GameMapEventInfo> ();
			this.entities = new HashSet<E> ();
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

		#region Game
		
		#region Update

		public override void Update(Time time) {

			//TODO: Update elements before board pieces.

			base.Update (time);
		}

		
		#endregion

		#endregion

		#region Game Map Events
		
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
			return new GameMapEventInfo(GameMapEvent, this as GameMap<IMapTile, IGameMapEntity>);
		}
		
		#endregion
	}
	
	/*
	 * Entity Placed on an Entity Map.
	 */
	public interface IGameMapEntity : IGameElement {

		//TODO: Add anything..? Set/Get Map?

	}

}

