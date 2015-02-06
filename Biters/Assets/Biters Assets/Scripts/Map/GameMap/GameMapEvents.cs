using System;

namespace Biters
{
	public enum GameMapEvent : int {
		
		/*
		 * A custom game map event.
		 */
		Custom = -1,

		//Entities
		/*
		 * Called when a map tile is added.
		 */
		AddEntity = 10,
		
		/*
		 * Called when a map tile is removed.
		 */
		RemoveEntity = 11,

		//TODO: Add other game map event types.
		
	}
	
	public static class GameMapEventEnumInfo
	{
		
		//Extension of WorldDirection enum with static functinon.
		public static string EventName(this GameMapEvent Event)
		{
			string name;
			
			switch (Event) {
			case GameMapEvent.AddEntity: name = "game_map_init_event"; break;
			case GameMapEvent.RemoveEntity: name = "game_map_reset_event"; break;
			default:
				name = "game_map_custom_event"; break;
			}
			
			return name;
		}
		
	}
	
	public class GameMapEventInfo : EventInfo {
		
		private GameMapEvent mapEvent;
		private GameMap<IMapTile, IGameMapEntity> map;

		//Optional Entity associated with the event.
		private IGameMapEntity entity;
		
		public GameMapEventInfo(GameMapEvent GameMapEvent, GameMap<IMapTile, IGameMapEntity> Map) {
			this.mapEvent = GameMapEvent;
			this.map = Map;
		}

		public GameMapEventInfo(GameMapEvent GameMapEvent, GameMap<IMapTile, IGameMapEntity> Map, IGameMapEntity Entity) : this (GameMapEvent, Map) {
			this.entity = Entity;
		}
		
		public string EventName {
			get {
				return mapEvent.EventName();
			}
		}

		public IGameMapEntity Entity {
			
			get {
				return entity;
			}
			
		}

		public GameMap<IMapTile, IGameMapEntity> Map {

			get {
				return map;
			}

		}
		
	}

}

