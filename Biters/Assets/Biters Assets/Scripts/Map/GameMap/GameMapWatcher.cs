using System;
using System.Collections;
using System.Collections.Generic;

namespace Biters
{
	#region Interfaces

	/*
	 * Class for watching a game map and any changes that may occur in regards to entites and their movements.
	 * 
	 * Has access to a Game Map's event system.
	 */
	public interface IGameMapWatcher<T, E>
		where T : class, IGameMapTile
		where E : class, IGameMapEntity
	{
		
		void AttachToMap(GameMap<T, E> Map);
		
		void DeattachFromMap(GameMap<T, E> Map);

		/*
		 * Takes a look at the current map state.
		 */
		IEnumerable<GameMapEventInfoBuilder> Observe(GameMap<T, E> Map);

	}

	#endregion

	#region Default Implementation

	/*
	 * Default implementation that checks entity positions every 
	 */
	public class GameMapWatcher<T, E> : IEventListener, IGameMapWatcher<T, E>
		where T : class, IGameMapTile
		where E : class, IGameMapEntity
	{
		public Dictionary<E, WorldPosition> TrackingMap = new Dictionary<E, WorldPosition>();
		public GameMap<T, E> Map;
		
		#region Attach
		
		public void AttachToMap(GameMap<T, E> Map) {
			this.Map = Map;
			Map.RegisterForEvent (this, GameMapEvent.AddEntity);
		}
		
		public void DeattachFromMap(GameMap<T, E> Map) {
			Map.UnregisterForEvents (this);
		}
		
		#endregion

		#region Tracking Map
		
		public void HandleEvent(IEventInfo EventInfo) {
			GameMapEventInfo info = EventInfo as GameMapEventInfo;

			switch (info.GameMapEvent) {
			case GameMapEvent.Delete:
				this.TrackingMap.Clear();
				break;
			case GameMapEvent.AddEntity: 
				this.InsertEntity(info.Entity as E, info.Position.Value);
				break;
			case GameMapEvent.RemoveEntity:
				this.RemoveEntity(info.Entity as E);
				break;
			}
			
		}
		
		private void InsertEntity(E Entity, WorldPosition Position) {
			this.TrackingMap.Add (Entity, Position);
		}

		private void MoveEntity(E Entity, WorldPosition Position) {
			this.TrackingMap.Remove(Entity);
			this.TrackingMap.Add (Entity, Position);
		}

		private void RemoveEntity(E Entity) {
			this.TrackingMap.Remove(Entity);
		}

		#endregion

		public IEnumerable<GameMapEventInfoBuilder> Observe(GameMap<T, E> Map) {
			List<GameMapEventInfoBuilder> events = new List<GameMapEventInfoBuilder>();

			foreach (KeyValuePair<E, WorldPosition> pair in TrackingMap) {
				E entity = pair.Key;

				WorldPosition? position = Map.GetPositionForEntity(entity);
				if (position.HasValue) {
					if (position.Value.Equals(pair.Value) == false) {

						//Entered Tile Event
						GameMapEventInfoBuilder enteredTile = Map.GameMapEventInfoBuilder(GameMapEvent.EntityEnteredTile);
						enteredTile.Entity = entity;
						enteredTile.Position = position.Value;
						events.Add(enteredTile);

						//Exited Tile Event
						GameMapEventInfoBuilder exitedTile = Map.GameMapEventInfoBuilder(GameMapEvent.EntityEnteredTile);
						exitedTile.Entity = entity;
						exitedTile.Position = pair.Value;
						events.Add(exitedTile);

						this.MoveEntity(entity, position.Value);
					}
				} else {
					GameMapEventInfoBuilder outOfBounds = Map.GameMapEventInfoBuilder(GameMapEvent.EntityOutsideWorld);
					outOfBounds.Entity = entity;
					events.Add(outOfBounds);
				}
			}

			return events;
		}

	}

	#endregion

	public struct TileEntityTuple<E>
		where E : class, IGameMapEntity
	{
		public readonly E Entity;
		public readonly WorldPosition Position;

		public TileEntityTuple(E Entity, WorldPosition Position) {
			this.Entity = Entity;
			this.Position = Position;
		}

	}

}

