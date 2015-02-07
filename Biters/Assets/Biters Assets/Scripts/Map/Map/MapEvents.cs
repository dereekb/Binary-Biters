using System;

namespace Biters
{

	public interface IMapEventSystem {
		
		//Events
		void RegisterForEvent (IEventListener Listener, MapEvent EventType);
		void UnregisterForEvent (IEventListener Listener, MapEvent EventType);
		void UnregisterForEvents (IEventListener Listener);
		void BroadcastEvent (MapEventInfoBuilder Builder);
		
		MapEventInfoBuilder CustomMapEventBuilder { get; }
		MapEventInfoBuilder MapEventInfoBuilder (MapEvent MapEvent);
		
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
	
	public class MapEventInfoBuilder : IFactory<MapEventInfo> {
		
		public readonly MapEvent MapEvent;
		public readonly IMap<IMapTile> Map;

		public string EventName;
		public WorldPosition? Position;
		public IMapTile Tile;
		
		public MapEventInfoBuilder(MapEvent MapEvent, IMap<IMapTile> Map) {
			this.MapEvent = MapEvent;
			this.Map = Map;
		}
		
		public MapEventInfo Make() {
			return new MapEventInfo (this);
		}
		
	}

	public class MapEventInfo : IEventInfo {
		
		public const string MapEventInfoId = "MAP_EVENT";

		private readonly string Name;
		private readonly MapEvent mapEvent;
		private readonly IMap<IMapTile> map;

		//Optional Tile associated with the event.
		private readonly IMapTile tile;

		//Optional Position associated with the event.
		private readonly WorldPosition? position;
		
		internal MapEventInfo(MapEventInfoBuilder Builder) {
			this.mapEvent = Builder.MapEvent;
			this.map = Builder.Map;

			string name = Builder.EventName;

			if (name == null) {
				name = mapEvent.EventName();
			}
			
			this.Name = name;
			this.position = Builder.Position;
			this.tile = Builder.Tile;
		}
		
		public string EventInfoId {
			get {
				return MapEventInfoId;
			}
		}

		public string EventName {

			get {
				return Name;
			}

		}

		public MapEvent Event {

			get {
				return mapEvent;
			}

		}

		public IMap<IMapTile> Map {

			get {
				return map;
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
			
		}
		
		public IMapTile Tile {
			get {
				IMapTile tile = null;
				
				if (this.tile != null) {
					tile = this.tile;
				} else if (this.Position.HasValue) {
					tile = this.map.GetTile(this.position.Value);
				}
				
				return tile;
			}
		}

	}

}

