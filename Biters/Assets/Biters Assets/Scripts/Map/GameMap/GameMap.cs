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

		#region Accessors

		public void AddEntity(E Entity, WorldPosition Position) {
			if (this.ContainsEntity(Entity) == false) {
				this.entities.Add(Entity);
				MoveEntityToPosition(Position);
				Entity.AddedToGameMap(this as GameMap<IMapTile, IGameMapEntity>, Position);
			}
		}

		public void RemoveEntity(E Entity) {
			if (this.ContainsEntity(Entity)) {
				this.entities.Remove(Entity);
				Entity.RemovedFromGameMap(this as GameMap<IMapTile, IGameMapEntity>);
			}
		}
		
		public bool ContainsEntity(E Entity) {
			return this.entities.Contains(Entity);
		}

		#endregion

		#region Entity Positions

		protected void MoveEntityToPosition(E Entity, WorldPosition Position) {

			//TODO: Retrieve the tile's position, then apply that position to the entity.

		}

		protected WorldPosition? PositionForEntity(E Entity) {
			WorldPosition? position;

			if (this.ContainsEntity(Entity)) {
				//TODO: Retrieve element position...
			}

			return position;
		}

		//Get Entities within Position...
		public List<E> GetEntitiesAtPosition(WorldPosition Position) {
			List<E> entities = new List<E> ();

			//TODO: Retrieve entities within a position.

			return entities;
		}

		#endregion	
		
		#endregion

		#region Game
		
		#region Update

		public override void Update(Time time) {

			foreach (E entity in this.entities) {
				entity.Update(time);
			}

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
	public interface IGameMapEntity : IGameElement, IUpdatingElement {

		void AddedToGameMap(GameMap<IMapTile, IGameMapEntity> Map, WorldPosition Position);
		
		void RemovedFromGameMap(GameMap<IMapTile, IGameMapEntity> Map);

	}

}

