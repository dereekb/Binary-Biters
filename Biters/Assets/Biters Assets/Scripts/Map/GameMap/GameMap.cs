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
		private IGameMapWatcher<T, E, IGameMapWatcherObservation<T, E>> watcher;
		private EventSystem<GameMapEvent, GameMapEventInfo> gameMapEvents;
		
		public GameMap(GameObject GameObject, IFactory<World<T>> WorldFactory, IWorldPositionMapper PositionMapper) : this(GameObject, WorldFactory, null, PositionMapper) {
			this.watcher = new GameMapWatcher<T, E>() as IGameMapWatcher<T, E, IGameMapWatcherObservation<T, E>>;
		}

		public GameMap(GameObject GameObject, IFactory<World<T>> WorldFactory, IGameMapWatcher<T, E, IGameMapWatcherObservation<T, E>> Watcher, IWorldPositionMapper PositionMapper) : base(GameObject, WorldFactory, PositionMapper) {
			this.gameMapEvents = new EventSystem<GameMapEvent, GameMapEventInfo> ();
			this.entities = new HashSet<E> ();
			this.watcher = Watcher;
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
			this.UpdateWatcher ();
			base.Update();
		}

		private void UpdateEntities() {	
			foreach (E entity in this.entities) {
				entity.Update();
			}
		}

		private void UpdateWatcher() {
			this.watcher.Observe (this);
		}

		#endregion

		#endregion

		#region Game Map Events
		
		protected EventSystem<GameMapEvent, GameMapEventInfo> GameMapEvents {
			
			get {
				return this.gameMapEvents;
			}
			
		}
		
		public void RegisterForEvent(IEventListener Listener, GameMapEvent EventType) {
			this.gameMapEvents.AddObserver (Listener, EventType);
		}
		
		public void UnregisterForEvent(IEventListener Listener, GameMapEvent EventType) {
			this.gameMapEvents.RemoveObserver (Listener, EventType);
		}
		
		public void UnregisterForEvents(IEventListener Listener, GameMapEvent EventType) {
			this.gameMapEvents.RemoveObserver (Listener);
		}

		private void BroadcastEvent(GameMapEventInfoBuilder Builder) {
			this.gameMapEvents.BroadcastEvent (Builder.MapEvent, Builder.Make());
		}
		
		public GameMapEventInfoBuilder CustomGameMapEventBuilder {
			get {
				return this.GameMapEventInfoBuilder(GameMapEvent.Custom);
			}
		}

		private GameMapEventInfoBuilder GameMapEventInfoBuilder(GameMapEvent GameMapEvent) {
			return new GameMapEventInfoBuilder(GameMapEvent, this as GameMap<IMapTile, IGameMapEntity>);
		}

		#endregion
	}
	
	/*
	 * Entity Placed on an Entity Map.
	 */
	public interface IGameMapEntity : IGameElement, ITransformableElement, IMovingElement, IUpdatingElement {

		void AddedToGameMap(GameMap<IMapTile, IGameMapEntity> Map, WorldPosition Position);
		
		void RemovedFromGameMap(GameMap<IMapTile, IGameMapEntity> Map);

	}

}

