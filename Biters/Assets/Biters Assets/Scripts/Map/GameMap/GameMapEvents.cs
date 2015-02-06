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
		AddEntity = 0,
		
		/*
		 * Called when a map tile is removed.
		 */
		RemoveEntity = 1,
		
		/*
		 * Entity entered a tile.
		 */
		EntityEnteredTile = 10,
		
		/*
		 * Entity exited a tile.
		 */
		EntityExitedTile = 11,

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
	
	public class GameMapEventInfo : IEventInfo {

		public const string GameMapEventInfoId = "GAME_MAP_EVENT";

		private readonly GameMapEvent mapEvent;
		private readonly GameMap<IMapTile, IGameMapEntity> map;
		
		//Optional Position associed with the event.
		private readonly WorldPosition? position;

		//Optional Entity associated with the event.
		private readonly IGameMapEntity entity;

		//Optional Tile associed with the event.
		private readonly IMapTile tile;
		
		public GameMapEventInfo(GameMapEvent GameMapEvent, GameMap<IMapTile, IGameMapEntity> Map) {
			this.mapEvent = GameMapEvent;
			this.map = Map;
		}
		
		public GameMapEventInfo(GameMapEvent GameMapEvent, GameMap<IMapTile, IGameMapEntity> Map, IGameMapEntity Entity) : this (GameMapEvent, Map) {
			this.entity = Entity;
		}
		
		public GameMapEventInfo(GameMapEvent GameMapEvent, GameMap<IMapTile, IGameMapEntity> Map, IGameMapEntity Entity, WorldPosition Position) : this (GameMapEvent, Map, Entity) {
			this.position = Position;
		}

		public GameMapEventInfo(GameMapEvent GameMapEvent, GameMap<IMapTile, IGameMapEntity> Map, WorldPosition Position) : this (GameMapEvent, Map) {
			this.position = Position;
		}
		
		public GameMapEventInfo(GameMapEvent GameMapEvent, GameMap<IMapTile, IGameMapEntity> Map, IMapTile Tile) : this (GameMapEvent, Map) {
			this.tile = Tile;
		}

		public string EventInfoId {
			get {
				return GameMapEventInfoId;
			}
		}

		public string EventName {
			get {
				return mapEvent.EventName();
			}
		}

		public GameMapEvent MapEvent {

			get {
				return mapEvent;
			}

		}

		public IGameMapEntity Entity {
			
			get {
				return entity;
			}
			
		}

		public WorldPosition? Position {

			get {
				WorldPosition? position = null;

				if (this.position.HasValue) {
					position = this.position;
				} else if (this.tile != null) {
					position = this.tile.MapTilePosition;
				} else if (this.entity != null) {
					position = this.map.PositionForEntity(this.entity);
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

		public GameMap<IMapTile, IGameMapEntity> Map {

			get {
				return map;
			}

		}
		
	}

}

