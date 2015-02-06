using System;

namespace Biters
{

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

	public class MapEventInfo : IEventInfo {
		
		public const string MapEventInfoId = "MAP_EVENT";

		private string Name;
		private MapEvent mapEvent;
		private Map<IMapTile> map;

		//Optional Tile associated with the event.
		private IMapTile tile;

		//Optional Position associated with the event.
		private WorldPosition? position;
		
		public MapEventInfo(MapEvent MapEvent, Map<IMapTile> Map, string Name) {
			this.mapEvent = MapEvent;
			this.map = Map;

			if (Name == null) {
				Name = MapEvent.EventName();
			}

			this.Name = Name;
		}
		
		public MapEventInfo(MapEvent MapEvent, Map<IMapTile> Map) : this(MapEvent, Map, MapEvent.EventName()) {
			this.mapEvent = MapEvent;
			this.map = Map;
		}
		
		public MapEventInfo(MapEvent MapEvent, Map<IMapTile> Map, IMapTile tile) : this (MapEvent, Map) {
			this.tile = tile;
		}
		
		public MapEventInfo(MapEvent MapEvent, Map<IMapTile> Map, WorldPosition? position) : this (MapEvent, Map, MapEvent.EventName()) {}
		
		public MapEventInfo(MapEvent MapEvent, Map<IMapTile> Map, WorldPosition? position, string Name) : this (MapEvent, Map, Name) {
			this.position = position;
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

		public Map<IMapTile> Map {

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

