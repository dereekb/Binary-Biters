using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biters
{
	/*
	 * Represents a Game Map that contains entities in addition to map tiles.
	 * 
	 * Contains only support functions for updating the map. 
	 * 
	 * Game Logic should be handled through available accessors.
	 */
	public class GameMap<T, E> : Map<T> 
		where T : class, IMapTile
		where E : class, IGameMapEntity
	{

		private HashSet<E> entities;
		private EventSystem<GameMapEvent, GameMapEventInfo> gameMapEvents;

		public GameMap(GameObject GameObject, IFactory<World<T>> WorldFactory, IWorldPositionMapper PositionMapper) : base(GameObject, WorldFactory, PositionMapper) {
			this.gameMapEvents = new EventSystem<GameMapEvent, GameMapEventInfo> ();
			this.entities = new HashSet<E> ();
		}
		
		#region Entity

		#region Accessors

		public void AddEntity(E Entity, WorldPosition Position) {
			if (this.ContainsEntity(Entity) == false) {
				this.entities.Add(Entity);

				//TODO: Consider other things, such as setting the entity's position parent to the map's game object.

				MoveEntityToPosition(Entity, Position);
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

		#region World

		public T TileUnderEntity(E Entity) {
			T tile = null;
			WorldPosition? position = this.PositionForEntity (Entity);

			if (position.HasValue) {
				tile = base.GetTile(position.Value);
			}

			return tile;
		}

		#endregion

		#region Entity Positions

		protected void MoveEntityToPosition(E Entity, WorldPosition Position) {
			Vector3 entityPosition = this.PositionMapper.VectorForPosition (Position);
			Entity.Transform.position = entityPosition;
		}

		/*
		 * Returns the WorldPosition of an entity.
		 */
		public WorldPosition? PositionForEntity(E Entity) {
			WorldPosition? position = null;

			if (this.ContainsEntity(Entity)) {
				Vector3 entityPosition = Entity.Transform.position;
				position = this.PositionMapper.PositionForVector(entityPosition);
			}

			return position;
		}

		/*
		 * Returns all entities at a given WorldPosition.
		 */
		public List<E> GetEntitiesAtPosition(WorldPosition Position) {
			List<E> result = new List<E> ();

			foreach (E entity in this.entities) {
				Vector3 entityPosition = entity.Transform.position;
				WorldPosition? position = this.PositionMapper.PositionForVector(entityPosition);

				if (position.HasValue && Position.Equals(position.Value)) {
					result.Add(entity);
				}
			}

			return result;
		}

		#endregion	
		
		#endregion

		#region Game
		
		#region Update

		public override void Update() {
			this.UpdateEntities();
			base.Update();
		}

		private void UpdateEntities() {	
			foreach (E entity in this.entities) {
				entity.Update();
			}
		}

		#endregion

		#endregion

		#region Game Map Events
		
		protected EventSystem<GameMapEvent, GameMapEventInfo> GameMapEvents {
			
			get {
				return this.gameMapEvents;
			}
			
		}
		
		public void RegisterForEvent(EventListener Listener, GameMapEvent EventType) {
			this.gameMapEvents.AddObserver (Listener, EventType);
		}
		
		public void UnregisterForEvent(EventListener Listener, GameMapEvent EventType) {
			this.gameMapEvents.RemoveObserver (Listener, EventType);
		}
		
		public void UnregisterForEvents(EventListener Listener, GameMapEvent EventType) {
			this.gameMapEvents.RemoveObserver (Listener);
		}

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
	public interface IGameMapEntity : IGameElement, ITransformableElement, IUpdatingElement {

		void AddedToGameMap(GameMap<IMapTile, IGameMapEntity> Map, WorldPosition Position);
		
		void RemovedFromGameMap(GameMap<IMapTile, IGameMapEntity> Map);

	}

}

