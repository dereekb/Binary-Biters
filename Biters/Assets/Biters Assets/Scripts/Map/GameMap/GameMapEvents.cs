using System;
using Biters.Utility;

namespace Biters
{
	public enum GameMapEvent : int {
		
		/*
		 * A custom game map event.
		 */
		Custom = -1,

		//Map
		/*
		 * Called when the map is going to be deleted.
		 */
		Delete = 0,

		//Entities
		/*
		 * Called when a map tile is added.
		 */
		AddEntity = 10,
		
		/*
		 * Called when a map tile is removed.
		 */
		RemoveEntity = 11,
		
		/*
		 * Entity entered a tile.
		 */
		EntityEnteredTile = 12,
		
		/*
		 * Entity exited a tile.
		 */
		EntityExitedTile = 13,

		/*
		 * Entity went outside the bounds of the world.
		 */
		EntityOutsideWorld = 14

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

	public class GameMapEventInfoBuilder : IFactory<GameMapEventInfo> {
		
		public readonly GameMapEvent MapEvent;
		public readonly IGameMap<IGameMapTile, IGameMapEntity> Map;

		public string EventName;
		public WorldPosition? Position;
		public IGameMapEntity Entity;
		public IMapTile Tile;
		
		public GameMapEventInfoBuilder(GameMapEvent GameMapEvent, IGameMap<IGameMapTile, IGameMapEntity> Map) {
			this.MapEvent = GameMapEvent;
			this.Map = Map;
		}

		public GameMapEventInfo Make() {
			return new GameMapEventInfo (this);
		}

	}

	public class GameMapEventInfo : IEventInfo {

		public const string GameMapEventInfoId = "GAME_MAP_EVENT";
		
		private readonly string name;
		private readonly GameMapEvent mapEvent;
		private readonly IGameMap<IGameMapTile, IGameMapEntity> map;
		
		//Optional Position associed with the event.
		private readonly WorldPosition? position;

		//Optional Entity associated with the event.
		private readonly IGameMapEntity entity;

		//Optional Tile associed with the event.
		private readonly IMapTile tile;

		internal GameMapEventInfo(GameMapEventInfoBuilder Builder) {
			mapEvent = Builder.MapEvent;
			map = Builder.Map;
			position = Builder.Position;
			entity = Builder.Entity;
			tile = Builder.Tile;
			
			string name = Builder.EventName;
			
			if (name == null) {
				name = mapEvent.EventName();
			}
			
			this.name = name;
		}

		public string EventInfoId {
			get {
				return GameMapEventInfoId;
			}
		}

		public string EventName {
			get {
				return name;
			}
		}

		public GameMapEvent GameMapEvent {

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
					position = this.map.GetPositionForEntity(this.entity);
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

		public IGameMap<IGameMapTile, IGameMapEntity> Map {

			get {
				return map;
			}

		}
		
	}

}

