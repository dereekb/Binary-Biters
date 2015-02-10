using System;
using Biters.Utility;

namespace Biters
{
	public interface IGameMapEventSystem<T, E>
		where T : class, IGameMapTile
		where E : class, IGameMapEntity {
		
		//Events
		void RegisterForGameMapEvent (IEventListener Listener, GameMapEvent EventType);
		void UnregisterFromGameMapEvent (IEventListener Listener, GameMapEvent EventType);
		void UnregisterFromGameMapEvents (IEventListener Listener);
		void BroadcastCustomGameMapEvent (GameMapEventInfoBuilder<T,E> Builder);
		
		GameMapEventInfoBuilder<T,E> CustomGameMapEventBuilder { get; }
		GameMapEventInfoBuilder<T,E> GameMapEventInfoBuilder (GameMapEvent MapEvent);
		
	}

	public enum GameMapEvent : int {
		
		/*
		 * A custom game map event.
		 */
		Custom = -1,

		//Map
		/*
		 * Called when the map is going to be deleted.
		 */
		MapDeleted = 0,

		//Entities
		/*
		 * Called when an entity is added to the map.
		 */
		EntityAdded = 10,
		
		/*
		 * Called when an entity is removed from the map.
		 */
		EntityRemoved = 11,
		
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

	}
	
	public static class GameMapEventEnumInfo
	{
		
		//Extension of WorldDirection enum with static functinon.
		public static string EventName(this GameMapEvent Event)
		{
			string name;
			
			switch (Event) {
			case GameMapEvent.EntityAdded: name = "game_map_init_event"; break;
			case GameMapEvent.EntityRemoved: name = "game_map_reset_event"; break;
			default:
				name = "game_map_custom_event"; break;
			}
			
			return name;
		}
		
	}

	public sealed class GameMapEventInfoBuilder<T, E> : IFactory<GameMapEventInfo> 
		where T : class, IGameMapTile
		where E : class, IGameMapEntity 
	{
		
		public readonly GameMapEvent GameMapEvent;
		public readonly IGameMap<T, E> Map;

		private string eventName;
		private WorldPosition? position;
		private E entity;
		private T tile;

		public string EventName {

			get {
				return this.eventName;
			}

			set {
				string name = value;
				
				if (name == null) {
					name = this.GameMapEvent.EventName();
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
				} else if (this.entity != null) {
					position = this.Map.GetPositionForEntity(this.entity);
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

		public E Entity {

			get {
				return this.entity;
			}

			set {
				this.entity = value;
			}

		}

		public GameMapEventInfoBuilder(GameMapEvent GameMapEvent, IGameMap<T, E> Map) {
			this.GameMapEvent = GameMapEvent;
			this.Map = Map;
		}

		public GameMapEventInfo Make() {
			return new GameMapEventInfo (GameMapEvent, EventName, Map, Position, Entity, Tile);
		}

	}

	public sealed class GameMapEventInfo : IEventInfo {

		public const string GameMapEventInfoId = "GAME_MAP_EVENT";
		
		private readonly string eventName;
		public readonly GameMapEvent GameMapEvent;
		public readonly object Map;
		
		//Optional Position associed with the event.
		public readonly WorldPosition? Position;

		//Optional Entity associated with the event.
		public readonly IGameMapEntity Entity;

		//Optional Tile associed with the event.
		public readonly IMapTile Tile;

		internal GameMapEventInfo(GameMapEvent GameMapEvent, string EventName, object Map, WorldPosition? Position, IGameMapEntity Entity, IMapTile Tile) {
			this.eventName = EventName;
			this.GameMapEvent = GameMapEvent;
			this.Map = Map;

			this.Position = Position;
			this.Entity = Entity;
			this.Tile = Tile;
		}

		public string EventInfoId {
			get {
				return GameMapEventInfoId;
			}
		}

		public string EventName {
			get {
				return this.eventName;
			}
		}

	}

}

