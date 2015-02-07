using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biters
{

	/*
	 * Represents a map made up of tiles.
	 */
	public class Map<T> : Entity 
		where T : class, IMapTile
	{
		private IWorldPositionMapper positionMapper;
		private EventSystem<MapEvent, MapEventInfo> mapEvents;
		protected IFactory<World<T>> WorldFactory;

		protected World<T> World;

		public Map(GameObject GameObject, IFactory<World<T>> WorldFactory, IWorldPositionMapper PositionMapper) : base(GameObject) {
			this.WorldFactory = WorldFactory;
			this.positionMapper = PositionMapper;
			this.mapEvents = new EventSystem<MapEvent, MapEventInfo> ();
		}

		#region World

		/*
		 * Resets the world to its original state.
		 */
		public void ResetWorld() {
			this.ClearWorld();

			World<T> newWorld = WorldFactory.Make();
			Map<IMapTile> map = this as Map<IMapTile>;

			foreach (KeyValuePair<WorldPosition, T> pair in newWorld.ElementPairs) {
				WorldPosition position = pair.Key;
				T element = pair.Value;

				element.MapTilePosition = pair.Key;
				element.AddedToMap(map, position);
			}

			this.World = newWorld;
		}

		/*
		 * Deletes the current world.
		 */
		public void ClearWorld() {
			foreach (WorldPosition Position in World.Positions) {
				this.RemoveTile(Position);
			}
		}

		#region World Position Accessors

		public IWorldPositionMapper PositionMapper 
		{
			get {
				return this.positionMapper;
			}
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

		public Vector3 GetPositionVector(WorldPosition Position) {
			return this.positionMapper.VectorForPosition (Position);
		}

		public T SetTile(T Element, WorldPosition Position) {
			T replaced = this.RemoveTile (Position);
			
			if (Element != null) {
				this.InsertTileAtPosition(Element, Position);
			}
			
			return replaced;
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
			Element.MapTilePosition = Position;
			Element.AddedToMap(this as Map<IMapTile>, Position);
		}

		protected virtual void RemoveTileFromPosition(T Removed, WorldPosition Position) {
			this.World.RemoveFromPosition (Position);
			Removed.RemovedFromMap (this as Map<IMapTile>);
		}

		#endregion

		#region World Accessors

		//TODO: Add accessors to retrieve neighbor elements, etc.

		#endregion

		#endregion

		#region Update

		public override void Update() {
			this.UpdateTiles ();
		}

		protected void UpdateTiles() {
			foreach (T tile in World) {
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
			return new MapEventInfoBuilder(MapEvent, this as Map<IMapTile>);
		}

		#endregion
	}

	//Represents a Tile on a map.
	public interface IMapTile : IGameElement, IUpdatingElement {
		
		WorldPosition MapTilePosition { get; set; }

		void AddedToMap(Map<IMapTile> Map, WorldPosition Position);
			
		void RemovedFromMap(Map<IMapTile> Map);

	}

}

