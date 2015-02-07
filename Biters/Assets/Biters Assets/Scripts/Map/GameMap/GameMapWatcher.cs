using System;
using System.Collections;
using System.Collections.Generic;
using Biters.Utility;

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
		protected List<EntityPositionTuple<E>> JustAdded = new List<EntityPositionTuple<E>>();
		protected Dictionary<E, WorldPosition> TrackingMap = new Dictionary<E, WorldPosition>();
		private GameMap<T, E> map;

		public GameMap<T, E> Map {
		
			get {
				return this.map;
			}
		
		}

		#region Attach
		
		public virtual void AttachedToMap(GameMap<T, E> Map) {
			//Override in subclass to continue initialization.
		}
		
		public virtual void DeattachedFromMap(GameMap<T, E> Map) {
			//Override in subclass to continue initialization.
		}

		public void AttachToMap(GameMap<T, E> Map) {
			this.map = Map;
			Map.RegisterForEvent (this, GameMapEvent.AddEntity);
			this.AttachedToMap (Map);
		}
		
		public void DeattachFromMap(GameMap<T, E> Map) {
			Map.UnregisterForEvents (this);
			this.DeattachFromMap (Map);
		}
		
		#endregion

		#region Tracking Map

		public virtual void HandleEvent(IEventInfo EventInfo) {
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
			this.JustAdded.Add (new EntityPositionTuple<E>(Entity, Position));
		}

		private void MoveEntity(E Entity, WorldPosition Position) {
			this.TrackingMap.Remove(Entity);
			this.TrackingMap.Add (Entity, Position);
		}

		private void RemoveEntity(E Entity) {
			this.TrackingMap.Remove(Entity);
		}

		#endregion

		//TODO: Instead of this, can possible add a "flag" that will cause the entity to call Entity Entered Tile.
		private void ObserveMapAdditions(GameMap<T, E> Map, List<GameMapEventInfoBuilder> events) {
			if (this.JustAdded.Count > 0) {
				foreach (EntityPositionTuple<E> tuple in this.JustAdded) {

					GameMapEventInfoBuilder enteredTile = Map.GameMapEventInfoBuilder(GameMapEvent.EntityEnteredTile);
					enteredTile.Entity = tuple.Entity;
					enteredTile.Position = tuple.Position;
					events.Add(enteredTile);
				}

				this.JustAdded.Clear();
			}
		}

		private void ObserverTileChanges(GameMap<T, E> Map, List<GameMapEventInfoBuilder> events) {
			
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
						GameMapEventInfoBuilder exitedTile = Map.GameMapEventInfoBuilder(GameMapEvent.EntityExitedTile);
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

		}

		public virtual IEnumerable<GameMapEventInfoBuilder> Observe(GameMap<T, E> Map) {
			List<GameMapEventInfoBuilder> events = new List<GameMapEventInfoBuilder>();
			
			this.ObserveMapAdditions (Map, events);
			this.ObserverTileChanges (Map, events);

			return events;
		}

	}

	#endregion

	public struct EntityPositionTuple<E>
		where E : class, IGameMapEntity
	{
		public readonly E Entity;
		public readonly WorldPosition Position;

		public EntityPositionTuple(E Entity, WorldPosition Position) {
			this.Entity = Entity;
			this.Position = Position;
		}

	}

}

