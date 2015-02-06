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
		where T : IMapTile
	{
		private EventSystem<MapEvent, MapEventInfo> mapEvents;
		protected IFactory<World<T>> WorldFactory;

		protected World<T> World;

		public Map(GameObject GameObject, IFactory<World<T>> WorldFactory) : base(GameObject) {
			this.WorldFactory = WorldFactory;
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

		public T SetTile(T Element, WorldPosition Position) {
			T replaced = this.RemoveTile (Position);
			
			if (Element != null) {
				this.World.SetAtPosition(Element, Position);
				Element.MapTilePosition = Position;
				Element.AddedToMap(this as Map<IMapTile>, Position);
			}
			
			return replaced;
		}

		public T RemoveTile(WorldPosition Position) {
			T removed = this.GetTile(Position);
			
			if (removed != null) {
				this.World.RemoveFromPosition(Position);
				removed.RemovedFromMap(this as Map<IMapTile>);
			}

			return removed;
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

		public void RegisterForEvent(EventListener Listener, MapEvent EventType) {
			this.mapEvents.AddObserver (Listener, EventType);
		}
		
		public void UnregisterForEvent(EventListener Listener, MapEvent EventType) {
			this.mapEvents.RemoveObserver (Listener, EventType);
		}
		
		public void UnregisterForEvents(EventListener Listener) {
			this.mapEvents.RemoveObserver (Listener);
		}

		public void BroadcastCustomMapEvent(String Name) {
			this.BroadcastCustomMapEvent (Name, null);
		}
		
		public void BroadcastCustomMapEvent(String Name, WorldPosition? Position) {
			MapEventInfo mapEventInfo = new MapEventInfo(MapEvent.Custom, this as Map<IMapTile>, Position);
			this.mapEvents.BroadcastEvent(MapEvent.Custom, mapEventInfo);
		}

		/*
		 * Convenience function for broadcasting an internal event with default arguments. 
		 */
		protected void BroadcastMapEvent(MapEvent MapEvent) {
			MapEventInfo info = this.DefaultMapEventInfo (MapEvent);
			this.mapEvents.BroadcastEvent (MapEvent, info);
		}

		protected MapEventInfo DefaultMapEventInfo(MapEvent MapEvent) {
			return new MapEventInfo(MapEvent, this as Map<IMapTile>);
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

