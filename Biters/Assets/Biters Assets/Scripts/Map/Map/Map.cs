using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biters
{
	/*
	 * Represents a Tile on a map.
	 */
	public interface IMapTile : IGameElement, IUpdatingElement {
		
		WorldPosition MapTilePosition { get; }
		
		void AddedToMap(WorldPosition Position);
		
		void RemovedFromMap();
		
	}

	/*
	 * Represents a map made up of tiles.
	 */
	public interface IMap<T> : IMapEventSystem where T : class, IMapTile {

		//World
		void ResetWorld();
		void ClearWorld();

		//Position
		IWorldPositionMapper PositionMapper { get; }
		Vector3 GetPositionVector (WorldPosition Position);

		//Tiles
		T this [WorldPosition Position] { get; }
		T GetTile (WorldPosition Position);
		void SetTile (T Element, WorldPosition Position);
		T RemoveTile (WorldPosition Position);

	}

	/*
	 * Default map implementation.
	 */
	public class Map<T> : Entity, IMap<T>
		where T : class, IMapTile
	{
		private IWorldPositionMapper positionMapper;
		private EventSystem<MapEvent, MapEventInfo> mapEvents;

		protected World<T> World;
		
		public Map(GameObject GameObject) : this(GameObject, new WorldPositionMapper()) {}

		public Map(GameObject GameObject, IWorldPositionMapper PositionMapper) : base(GameObject) {
			this.positionMapper = PositionMapper;
			this.mapEvents = new EventSystem<MapEvent, MapEventInfo> ();
		}

		#region World

		/*
		 * Resets the world to its original state.
		 */
		public virtual void ResetWorld() {
			this.ClearWorld();

			World<T> newWorld = new World<T> (); 
			this.World = newWorld;
		}

		/*
		 * Deletes the current world.
		 */
		public void ClearWorld() {
			if (this.World != null) {
				foreach (WorldPosition Position in World.Positions) {
					this.RemoveTile(Position);
				}
			}
		}

		#region World Position Accessors

		public IWorldPositionMapper PositionMapper 
		{
			get {
				return this.positionMapper;
			}
		}
		
		public Vector3 GetPositionVector(WorldPosition Position) {
			return this.positionMapper.VectorForPosition (Position);
		}

		public T this[WorldPosition Position]
		{

			get
			{
				return this.GetTile(Position);
			}
			
			set
			{
				this.SetTile(value, Position);
			}

		}

		public T GetTile(WorldPosition Position) {
			T result = World [Position];
			return result;
		}
		
		public void SetTile(T Element, WorldPosition Position) {
			this.RemoveTile (Position);
			
			if (Element != null) {
				this.InsertTileAtPosition(Element, Position);
			}
		}

		public T RemoveTile(WorldPosition Position) {
			T removed = this.GetTile(Position);
			
			if (removed != null) {
				this.RemoveTileFromPosition(removed, Position);
			}

			return removed;
		}
		
		protected virtual void InsertTileAtPosition(T Element, WorldPosition Position) {
			this.World.SetAtPosition(Element, Position);
			Element.AddedToMap(Position);
		}

		protected virtual void RemoveTileFromPosition(T Removed, WorldPosition Position) {
			this.World.RemoveFromPosition (Position);
			Removed.RemovedFromMap();
		}

		#endregion

		#region World Accessors

		//TODO: Add accessors to retrieve neighbor elements, etc. Add those to the interface too.

		#endregion

		#endregion

		#region Update

		public override void Update() {
			this.UpdateTiles ();
		}

		protected void UpdateTiles() {
			foreach (T tile in this.World) {
				tile.Update();
			}
		}

		#endregion

		#region mapEvents

		protected EventSystem<MapEvent, MapEventInfo> MapEvents {

			get {
				return this.mapEvents;
			}

		}

		public void RegisterForEvent(IEventListener Listener, MapEvent EventType) {
			this.mapEvents.AddObserver (Listener, EventType);
		}
		
		public void UnregisterForEvent(IEventListener Listener, MapEvent EventType) {
			this.mapEvents.RemoveObserver (Listener, EventType);
		}
		
		public void UnregisterForEvents(IEventListener Listener) {
			this.mapEvents.RemoveObserver (Listener);
		}

		public void BroadcastEvent(MapEventInfoBuilder Builder) {
			this.mapEvents.BroadcastEvent (Builder.MapEvent, Builder.Make ());
		}

		public MapEventInfoBuilder CustomMapEventBuilder {
			get {
				return this.MapEventInfoBuilder(MapEvent.Custom);
			}
		}
		
		public MapEventInfoBuilder MapEventInfoBuilder(MapEvent MapEvent) {
			return new MapEventInfoBuilder(MapEvent, this as IMap<IMapTile>);
		}

		#endregion
	}

}

