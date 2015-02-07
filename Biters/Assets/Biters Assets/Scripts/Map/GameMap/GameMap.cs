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
		where T : class, IGameMapTile
		where E : class, IGameMapEntity
	{
		private HashSet<E> entities;
		private IGameMapWatcher<T, E> watcher;
		private EventSystem<GameMapEvent, GameMapEventInfo> gameMapEvents;
		
		public GameMap(GameObject GameObject, IFactory<World<T>> WorldFactory) : this (GameObject, WorldFactory, new GameMapWatcher<T, E>(), new WorldPositionMapper()) {}

		public GameMap(GameObject GameObject, IFactory<World<T>> WorldFactory, IWorldPositionMapper PositionMapper) : this (GameObject, WorldFactory, new GameMapWatcher<T, E>(), PositionMapper) {}

		public GameMap(GameObject GameObject, IFactory<World<T>> WorldFactory, IGameMapWatcher<T, E> Watcher, IWorldPositionMapper PositionMapper) : base(GameObject, WorldFactory, PositionMapper) {
			this.gameMapEvents = new EventSystem<GameMapEvent, GameMapEventInfo> ();
			this.entities = new HashSet<E> ();
			this.watcher = Watcher;
		}
		
		#region Entity

		public IEnumerable<E> Entities {
			get {
				return this.entities;
			}
		}

		#region Accessors

		public void AddEntity(E Entity, WorldPosition Position) {
			if (this.ContainsEntity(Entity) == false) {
				this.entities.Add(Entity);
				
				//Set Transform to match map's.
				Entity.Transform.SetParent(this.Transform);

				MoveEntityToPosition(Entity, Position);
				Entity.AddedToGameMap(this as GameMap<IGameMapTile, IGameMapEntity>, Position);

				GameMapEventInfoBuilder builder = this.GameMapEventInfoBuilder(GameMapEvent.AddEntity);
				builder.Entity = Entity;
				builder.Position = Position;
				this.BroadcastEvent(builder);
			}
		}

		public void RemoveEntity(E Entity) {
			if (this.ContainsEntity(Entity)) {
				this.entities.Remove(Entity);

				if (Entity.Transform.parent == this.Transform) {
					Entity.Transform.parent = null;
				}

				GameMapEventInfoBuilder builder = this.GameMapEventInfoBuilder(GameMapEvent.RemoveEntity);
				builder.Entity = Entity;
				this.BroadcastEvent(builder);

				Entity.RemovedFromGameMap(this as GameMap<IGameMapTile, IGameMapEntity>);
			}
		}
		
		public bool ContainsEntity(E Entity) {
			return this.entities.Contains(Entity);
		}

		#endregion

		#region World

		public T GetTileUnderEntity(E Entity) {
			T tile = null;
			WorldPosition? position = this.GetPositionForEntity (Entity);

			if (position.HasValue) {
				tile = base.GetTile(position.Value);
			}

			return tile;
		}
		
		protected override void InsertTileAtPosition(T Element, WorldPosition Position) {
			base.InsertTileAtPosition (Element, Position);
			Element.AddedAsTileToGameMap(this as GameMap<IGameMapTile, IGameMapEntity>, Position);
		}
		
		protected override void RemoveTileFromPosition(T Removed, WorldPosition Position) {
			base.RemoveTileFromPosition (Removed, Position);
			Removed.RemovedAsTileFromGameMap(this as GameMap<IGameMapTile, IGameMapEntity>);
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
		public WorldPosition? GetPositionForEntity(E Entity) {
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

		#region Watcher

		public IGameMapWatcher<T, E> Watcher {

			get {
				return this.watcher;
			}

			set {
				if (value != null) {
					if (this.watcher != null) {
						this.watcher.DeattachFromMap(this);
					}

					this.watcher = value;
					this.watcher.AttachToMap(this);
				}
			}

		}

		#endregion

		#endregion

		#region Game
		
		#region Update

		public override void Update() {
			this.UpdateEntities();
			this.UpdateWatcher();
			base.Update();
		}

		protected void UpdateEntities() {	
			foreach (E entity in this.entities) {
				entity.Update();
			}
		}

		protected void UpdateWatcher() {
			IEnumerable<GameMapEventInfoBuilder> events = this.watcher.Observe (this);

			foreach (GameMapEventInfoBuilder info in events) {
				this.BroadcastEvent(info);
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

		//TODO: If this being public presents itself as an issue, then hide again, and abstract necessary components.
		public GameMapEventInfoBuilder GameMapEventInfoBuilder(GameMapEvent GameMapEvent) {
			return new GameMapEventInfoBuilder(GameMapEvent, this as GameMap<IGameMapTile, IGameMapEntity>);
		}

		#endregion
	}
	
	/*
	 * Entity Placed on an Entity Map.
	 */
	public interface IGameMapEntity : IGameElement, ITransformableElement, IMovingElement, IUpdatingElement {

		void AddedToGameMap(GameMap<IGameMapTile, IGameMapEntity> Map, WorldPosition Position);
		
		void RemovedFromGameMap(GameMap<IGameMapTile, IGameMapEntity> Map);

	}

	public interface IGameMapTile : IMapTile {
		
		void AddedAsTileToGameMap(GameMap<IGameMapTile, IGameMapEntity> Map, WorldPosition Position);
		
		void RemovedAsTileFromGameMap(GameMap<IGameMapTile, IGameMapEntity> Map);

	}

}

