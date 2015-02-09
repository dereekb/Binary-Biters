using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters.Utility;

namespace Biters
{
	/*
	 * Represents a Game Map that contains entities in addition to map tiles.
	 * 
	 * Contains only support functions for updating the map. 
	 * 
	 * Game Logic should be handled through available accessors.
	 */
	public interface IGameMap<T, E> : IMap<T>, IGameMapEventSystem<T, E>, IEntityWorldPositionQueriable<E>
		where T : class, IGameMapTile
		where E : class, IGameMapEntity {

		//Entities
		IEnumerable<E> Entities { get; }

		void AddEntity (E Entity, WorldPosition Position);
		void RemoveEntity (E Entity);
		bool ContainsEntity (E Entity);
		
		WorldPosition? GetPositionForEntity (E Entity);

		//World
		T GetTileUnderEntity (E Entity);

		//Query
		IEntityWorldPositionQuery<E> EntityPositionQuery { get; }

		//Watcher
		IGameMapWatcher<T, E> Watcher { get; set; }

	}
	
	/*
	 * Game Map Delegate.
	 */
	public interface IGameMapDelegate<T, E> 
		where T : class, IGameMapTile
		where E : class, IGameMapEntity {
		
		/*
		 * Generates a new world for the passed map.
		 * 
		 * Map elements that depend on the map to function should be set properly.
		 */
		World<T> GenerateNewWorld(IGameMap<T, E> GameMap);
		
	}

	/*
	 * Default GameMap Implementation.
	 */
	public class GameMap<T, E> : Map<T>, IGameMap<T, E>
		where T : class, IGameMapTile
		where E : class, IGameMapEntity
	{
		private HashSet<E> entities;
		private IGameMapWatcher<T, E> watcher;
		protected IGameMapDelegate<T, E> mapDelegate;
		private EventSystem<GameMapEvent, GameMapEventInfo> gameMapEvents;
		
		public GameMap(GameObject GameObject, IGameMapDelegate<T, E> MapDelegate) : this (GameObject, MapDelegate, new GameMapWatcher<T, E>(), new WorldPositionMapper()) {}

		public GameMap(GameObject GameObject, IGameMapDelegate<T, E> MapDelegate, IWorldPositionMapper PositionMapper) : this (GameObject, MapDelegate, new GameMapWatcher<T, E>(), PositionMapper) {}

		public GameMap(GameObject GameObject, IGameMapDelegate<T, E> MapDelegate, IGameMapWatcher<T, E> Watcher, IWorldPositionMapper PositionMapper) : base(GameObject, PositionMapper) {
			this.gameMapEvents = new EventSystem<GameMapEvent, GameMapEventInfo> ();
			this.entities = new HashSet<E> ();
			this.mapDelegate = MapDelegate;
			this.Watcher = Watcher;
		}

		#region World
		
		/*
		 * Resets the world to its original state.
		 */
		public override void ResetWorld() {
			base.ResetWorld ();

			World<T> newWorld = this.mapDelegate.GenerateNewWorld(this);
			
			if (newWorld == null) {
				throw new Exception("World was reset with a null world.");
			}
			
			foreach (KeyValuePair<WorldPosition, T> pair in newWorld.ElementPairs) {
				this.InsertTileAtPosition(pair.Value, pair.Key);
			}
		}
		
		protected override void InsertTileAtPosition(T Element, WorldPosition Position) {
			base.InsertTileAtPosition (Element, Position);
			Element.AddedToGameMap (Position);
		}
		
		protected override void RemoveTileFromPosition(T Removed, WorldPosition Position) {
			base.RemoveTileFromPosition (Removed, Position);
			Removed.RemovedFromGameMap ();
		}

		#endregion
		
		#region Entity

		public IEnumerable<E> Entities {
			get {
				return this.entities;
			}
		}

		public ICollection<E> QueriableEntities {
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

				Entity.AddedToGameMap(Position);

				GameMapEventInfoBuilder<T, E> builder = this.GameMapEventInfoBuilder(GameMapEvent.AddEntity);
				builder.Entity = Entity;
				builder.Position = Position;
				this.BroadcastGameMapEvent(builder);
			}
		}

		public void RemoveEntity(E Entity) {
			if (this.ContainsEntity(Entity)) {
				this.entities.Remove(Entity);

				if (Entity.Transform.parent == this.Transform) {
					Entity.Transform.parent = null;
				}

				GameMapEventInfoBuilder<T, E> builder = this.GameMapEventInfoBuilder(GameMapEvent.RemoveEntity);
				builder.Entity = Entity;
				this.BroadcastGameMapEvent(builder);

				Entity.RemovedFromGameMap();
			}
		}
		
		public bool ContainsEntity(E Entity) {
			return this.entities.Contains(Entity);
		}

		#endregion
		
		#region Entity Positions
		
		protected void MoveEntityToPosition(E Entity, WorldPosition Position) {
			Vector3 entityPosition = this.PositionMapper.VectorForPosition (Position);
			Entity.Transform.position = entityPosition;

		}

		public bool EntityIsAtWorldPosition(E Entity, WorldPosition Position) {
			bool isAtPosition = false;
			WorldPosition? entityPosition = this.GetPositionForEntity (Entity);

			if (entityPosition.HasValue) {
				isAtPosition = (entityPosition == Position);
			}

			return isAtPosition;
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
		public List<E> EntitiesAtPosition(WorldPosition Position) {
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

		public IEntityWorldPositionQuery<E> EntityPositionQuery {

			get {
				return new EntityWorldPositionQuery<E>(this);
			}

		}

		#endregion

		#region World Position

		public bool HasTileUnderEntity(E Entity) {
			bool hasTile = false;
			WorldPosition? position = this.GetPositionForEntity (Entity);
			
			if (position.HasValue) {
				hasTile = base.HasTileAtPosition(position.Value);
			}
			
			return hasTile;
		}

		public T GetTileUnderEntity(E Entity) {
			T tile = null;
			WorldPosition? position = this.GetPositionForEntity (Entity);

			if (position.HasValue) {
				tile = base.GetTile(position.Value);
			}

			return tile;
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

		/*
		 *
		 * TODO: Protect against state changes during Update stage.
		 * 
		 * For example, add an "action queue" that is called at the end of update to apply any game-map related changes.
		 */
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
			IEnumerable<GameMapEventInfoBuilder<T, E>> events = this.watcher.Observe (this);

			foreach (GameMapEventInfoBuilder<T, E> info in events) {
				this.BroadcastGameMapEvent(info);
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

		public void RegisterForGameMapEvent(IEventListener Listener, GameMapEvent EventType) {
			this.gameMapEvents.AddObserver (Listener, EventType);
		}
		
		public void UnregisterFromGameMapEvent(IEventListener Listener, GameMapEvent EventType) {
			this.gameMapEvents.RemoveObserver (Listener, EventType);
		}
		
		public void UnregisterFromGameMapEvents(IEventListener Listener) {
			this.gameMapEvents.RemoveObserver (Listener);
		}

		private void BroadcastGameMapEvent(GameMapEventInfoBuilder<T,E> Builder) {
			this.gameMapEvents.BroadcastEvent (Builder.GameMapEvent, Builder.Make());
		}
		
		public void BroadcastCustomGameMapEvent(GameMapEventInfoBuilder<T,E> Builder) {
			if (Builder.GameMapEvent == GameMapEvent.Custom) {
				this.BroadcastGameMapEvent(Builder);
			}
		}

		public GameMapEventInfoBuilder<T,E> CustomGameMapEventBuilder {
			get {
				return this.GameMapEventInfoBuilder(GameMapEvent.Custom);
			}
		}

		//TODO: If this being public presents itself as an issue, then hide again, and abstract necessary components.
		public GameMapEventInfoBuilder<T,E> GameMapEventInfoBuilder(GameMapEvent GameMapEvent) {
			return new GameMapEventInfoBuilder<T,E>(GameMapEvent, this);
		}

		#endregion
	}
	
	/*
	 * Entity Placed on an Entity Map.
	 */
	public interface IGameMapEntity : IGameElement, ITransformableElement, IMovingElement, IUpdatingElement {

		void AddedToGameMap(WorldPosition Position);
		
		void RemovedFromGameMap();

	}

	public interface IGameMapTile : IGameMapEntity, IMapTile {
		//TODO: Add anything if necessary.
	}
	
	#region Game Map Events

	#endregion

}

