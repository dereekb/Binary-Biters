using System;
using UnityEngine;
using Biters.Utility;

namespace Biters
{

	public interface IMapEventSystem<T>  where T : class, IMapTile {
		
		//Events
		void RegisterForMapEvent (IEventListener Listener, MapEvent EventType);
		void UnregisterFromMapEvent (IEventListener Listener, MapEvent EventType);
		void UnregisterFromMapEvents (IEventListener Listener);
		void BroadcastCustomMapEvent (MapEventInfoBuilder<T> Builder);
		
		MapEventInfoBuilder<T> CustomMapEventBuilder { get; }
		MapEventInfoBuilder<T> MapEventInfoBuilder (MapEvent MapEvent);
		
	}

	public enum MapEvent : int {

		/*
		 * A custom map event.
		 */
		Custom = -1,

		//Map
		/*
		 * Called when the map is initialized.
		 */
		Init = 0,

		/*
		 * Called when the map reset itself.
		 */
		Reset = 1,
		
		/*
		 * Called when the map is stopped.
		 */
		Stop = 2,

		//Entities
		/*
		 * Called when a map tile is added.
		 */
		AddTile = 10,
		
		/*
		 * Called when a map tile is removed.
		 */
		RemoveTile = 11,

		//Tiles

		//TODO: Add other possible map event types.

	}
	
	public static class MapEventEnumInfo
	{
		
		//Extension of WorldDirection enum with static functinon.
		public static string EventName(this MapEvent Event)
		{
			string name;

			switch (Event) {
			case MapEvent.Init: name = "map_init_event"; break;
			case MapEvent.Reset: name = "map_reset_event"; break;
			case MapEvent.AddTile: name = "map_add_tile_event"; break;
			case MapEvent.RemoveTile: name = "map_remove_tile_event"; break;
			default:
				name = "map_custom_event"; break;
			}
			
			return name;
		}
		
	}
	
	public class MapEventInfoBuilder<T> : IFactory<MapEventInfo>
		where T : class, IMapTile
	{
		
		public readonly MapEvent MapEvent;
		public readonly IMap<T> Map;

		private string eventName;
		private WorldPosition? position;
		private T tile;
		
		public string EventName {
			
			get {
				return this.eventName;
			}
			
			set {
				string name = value;
				
				if (name == null) {
					name = this.MapEvent.EventName();
				}
				
				this.eventName = name;
			}
			
		}

		public WorldPosition? Position {
			
			get {
				WorldPosition? position = null;
				
				if (this.position.HasValue) {
					position = this.position;
				} else if (this.tile != null) {
					position = this.tile.MapTilePosition;
				}
				
				return position;
			}

			set {
				this.position = value;
			}

		}
		
		public T Tile {

			get {
				T tile = null;
				
				if (this.tile != null) {
					tile = this.tile;
				} else {
					WorldPosition? position = this.Position;
					
					if (position.HasValue) {
						tile = this.Map.GetTile(position.Value);
					}
				}
				
				return tile;
			}

			set {
				this.tile = value;
			}

		}

		public MapEventInfoBuilder(MapEvent MapEvent, IMap<T> Map) {
			this.MapEvent = MapEvent;
			this.Map = Map;
		}
		
		public MapEventInfo Make() {
			return new MapEventInfo (MapEvent, eventName, Map, Position, Tile);
		}
		
	}

	public sealed class MapEventInfo : IEventInfo {
		
		public const string MapEventInfoId = "MAP_EVENT";
		
		
		private readonly string eventName;
		public readonly MapEvent MapEvent;
		public readonly object Map;

		public readonly WorldPosition? Position;
		public readonly IMapTile Tile;
		
		internal MapEventInfo(MapEvent MapEvent, string EventName, object Map, WorldPosition? Position, IMapTile Tile) {
			this.eventName = EventName;
			this.MapEvent = MapEvent;
			this.Map = Map;
			
			this.Position = Position;
			this.Tile = Tile;
		}
		
		public string EventInfoId {
			get {
				return MapEventInfoId;
			}
		}
		
		public string EventName {
			get {
				return this.eventName;
			}
		}

	}

}

