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

	public class MapEventInfo : EventInfo {

		private MapEvent mapEvent;
		private Map<MapTile> map;

		//Optional Tile associated with the event.
		private MapTile tile;

		//Optional Position associated with the event.
		private WorldPosition? position;
		
		public MapEventInfo(MapEvent MapEvent, Map<MapTile> Map) {
			this.mapEvent = MapEvent;
			this.map = Map;
		}
		
		public MapEventInfo(MapEvent MapEvent, Map<MapTile> Map, MapTile tile) : this (MapEvent, Map) {
			this.tile = tile;
		}
		
		public MapEventInfo(MapEvent MapEvent, Map<MapTile> Map, WorldPosition position) : this (MapEvent, Map) {
			this.position = position;
		}

		public string EventName {
			get {
				return mapEvent.EventName();
			}
		}

		public MapEvent Event {

			get {
				return mapEvent;
			}

		}

		public Map<MapTile> Map {

			get {
				return map;
			}

		}

		public MapTile Tile {

			get {
				return tile;
			}

		}

		public WorldPosition? Position {

			get {
				return position;
			}

		}

	}

}

