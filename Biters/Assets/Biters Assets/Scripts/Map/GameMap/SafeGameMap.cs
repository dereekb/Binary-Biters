using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters.Utility;

namespace Biters
{

	public interface ISafeGameMap<T, E>  : IGameMap<T, E>
		where T : class, IGameMapTile
		where E : class, IGameMapEntity 
	{

	}

	/*
	 * Extension of GameMap that protects the Map during the Update/Broadcast stage.
	 * 
	 * Catches changes and queues them up to maintain the order in which they were called.
	 */		
	public class SafeGameMap<T, E> : GameMap<T, E>, ISafeGameMap<T, E>
		where T : class, IGameMapTile
		where E : class, IGameMapEntity
	{

		private bool enabled;
		private Queue<ISafeMapChange<T, E>> Changes = new Queue<ISafeMapChange<T, E>>();

		public SafeGameMap(GameObject GameObject, IMapWorldFactory<T> MapWorldFactory)
			: base (GameObject, MapWorldFactory) {}

		#region Safety

		//Entites
		public override void AddEntity (E Entity, WorldPosition Position)
		{
			if (this.enabled) {
				SafeMapEntityChange<T, E> change = new SafeMapEntityChange<T, E>(Entity, ProtectedMapChange.Add);
				change.Position = Position;
				this.Changes.Enqueue(change);
			} else {
				base.AddEntity (Entity, Position);
			}
		}

		public override void RemoveEntity (E Entity)
		{
			if (this.enabled) {
				SafeMapEntityChange<T, E> change = new SafeMapEntityChange<T, E>(Entity, ProtectedMapChange.Remove);
				this.Changes.Enqueue(change);
			} else {
				base.RemoveEntity (Entity);
			}
		}

		//Tiles
		public override void SetTile (T Tile, WorldPosition Position)
		{
			if (this.enabled) {
				SafeMapTileChange<T, E> change = new SafeMapTileChange<T, E>(Tile, ProtectedMapChange.Add);
				change.Position = Position;
				this.Changes.Enqueue(change);
			} else {
				base.SetTile (Tile, Position);
			}
		}

		public override void RemoveTile (WorldPosition Position)
		{
			if (this.enabled) {
				SafeMapTileChange<T, E> change = new SafeMapTileChange<T, E>(Position, ProtectedMapChange.Remove);
				change.Position = Position;
				this.Changes.Enqueue(change);
			} else {
				base.RemoveTile (Position);
			}
		}

		//Events

		protected override void BroadcastGameMapEvent (GameMapEventInfoBuilder<T, E> Builder)
		{
			if (this.enabled) {
				SafeMapEventChange<T, E> change = new SafeMapEventChange<T, E>(Builder, this, false);
				this.Changes.Enqueue(change);
			} else {
				base.BroadcastGameMapEvent (Builder);
			}
		}
		
		public override void BroadcastCustomGameMapEvent (GameMapEventInfoBuilder<T, E> Builder)
		{
			if (this.enabled) {
				SafeMapEventChange<T, E> change = new SafeMapEventChange<T, E>(Builder, this, true);
				this.Changes.Enqueue(change);
			} else {
				base.BroadcastCustomGameMapEvent(Builder);
			}
		}

		/*
		 * Internal function that gives the SafeMapEventChange class access to sending events.
		 */
		internal void SafelyBroadcastEvent(GameMapEventInfoBuilder<T, E> Builder) {
			base.BroadcastGameMapEvent (Builder);
		}

		#endregion

		#region Update

		/*
		 * Protects against entities being removed from the map accidentally.
		 */
		protected override void UpdateEntities ()
		{
			this.enabled = true;
			base.UpdateEntities ();
			this.enabled = false;
			this.RunQueue ();
		}

		/*
		 * Protects against other events from firing off events that may form an infinite loop.
		 */
		protected override void UpdateWatcher ()
		{
			this.enabled = true;
			base.UpdateWatcher ();
			this.enabled = false;
			this.RunQueue ();
		}

		#endregion

		#region Queue
		
		public void RunQueue() {
			while (this.Changes.Count > 0) {
				ISafeMapChange<T, E> Change = this.Changes.Dequeue();
				Change.RunChange(this);
			}
		}

		public void Enqueue(ISafeMapChange<T, E> Change) {
			this.Changes.Enqueue (Change);
		}

		#endregion

	}

	#region Changes

	internal enum ProtectedMapChange {
		Add,
		Remove
	}

	public interface ISafeMapChange<T, E>
		where T : class, IGameMapTile
		where E : class, IGameMapEntity {

		void RunChange(IGameMap<T, E> Map);

	}

	public abstract class SafeMapChange<A> {
		
		internal ProtectedMapChange Change;
		protected readonly A Element;
		public WorldPosition Position;
		
		protected SafeMapChange(A Element) {
			this.Element = Element;
		}
		
		protected SafeMapChange(WorldPosition Position) {
			this.Position = Position;
		}
		
		protected SafeMapChange(A Element, WorldPosition Position) {
			this.Element = Element;
			this.Position = Position;
		}

	}

	public class SafeMapEntityChange<T, E> : SafeMapChange<E>, ISafeMapChange<T, E> 
		where T : class, IGameMapTile
		where E : class, IGameMapEntity 
	{

		public SafeMapEntityChange (E Entity) : base(Entity) {}

		internal SafeMapEntityChange(E Entity, ProtectedMapChange Change) : base(Entity) {
			this.Change = Change;
		}

		public virtual void RunChange(IGameMap<T, E> Map) {

			switch (this.Change) {
			case ProtectedMapChange.Add:
				Map.AddEntity(this.Element, this.Position);
				break;
			case ProtectedMapChange.Remove:
				Map.RemoveEntity(this.Element);
				break;
			}

		}

	}
	
	public class SafeMapTileChange<T, E> : SafeMapChange<T>, ISafeMapChange<T, E> 
		where T : class, IGameMapTile
			where E : class, IGameMapEntity 
	{
		
		public SafeMapTileChange (T Tile) : base(Tile) {}
		
		public SafeMapTileChange (WorldPosition Position) : base(null) {
			this.Position = Position;
		}
		
		internal SafeMapTileChange(T Tile, ProtectedMapChange Change) : base(Tile) {
			this.Change = Change;
		}
		
		internal SafeMapTileChange(WorldPosition Position, ProtectedMapChange Change) : base(Position) {
			this.Change = Change;
		}
		
		public virtual void RunChange(IGameMap<T, E> Map) {
			
			switch (this.Change) {
			case ProtectedMapChange.Add:
				Map.SetTile(this.Element, this.Position);
				break;
			case ProtectedMapChange.Remove:
				Map.RemoveTile(this.Position);
				break;
			}
			
		}
		
	}

	
	internal class SafeMapEventChange<T, E> : SafeMapChange<GameMapEventInfoBuilder<T, E>>, ISafeMapChange<T, E> 
		where T : class, IGameMapTile
		where E : class, IGameMapEntity 
	{

		internal bool isCustom;
		internal SafeGameMap<T, E> Map;

		internal SafeMapEventChange(GameMapEventInfoBuilder<T, E> Builder, SafeGameMap<T, E> Map, bool isCustom) : base(Builder) {
			this.Map = Map;
			this.isCustom = isCustom;
		}
		
		public virtual void RunChange(IGameMap<T, E> Map) {
			if (this.isCustom) {
				this.Map.BroadcastCustomGameMapEvent(this.Element);
			} else {
				this.Map.SafelyBroadcastEvent(this.Element);
			}
		}
		
	}
		
	#endregion

}



