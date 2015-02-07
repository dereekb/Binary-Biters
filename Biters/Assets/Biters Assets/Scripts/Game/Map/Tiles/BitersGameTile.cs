using System;
using UnityEngine;
using Biters;
using Biters.Utility;

namespace Biters.Game
{
	/*
	 * Base class for Biters game tiles. 
	 */
	public abstract class BitersGameTile : Entity, IGameMapTile, IEventListener 
	{
		//Position in the map's world. Should only be changed by the Map.
		private WorldPosition mapTilePosition;

		//Game Map of the tile.
		private IGameMap<BitersGameTile, BitersMapEntity> map;

		public BitersGameTile() : base(GameObject.CreatePrimitive(PrimitiveType.Cube)) {}

		public BitersGameTile(GameObject GameObject) : base(GameObject) {}

		#region Map

		public WorldPosition MapTilePosition {

			get {
				return this.mapTilePosition;
			}

		}

		public IGameMap<BitersGameTile, BitersMapEntity> Map {
			
			get {
				return this.map;
			}
		
			set {
				if (this.map == null) {
					this.map = value;
				}
			}

		}

		public virtual void Initialize() {
			//Override to initialize entity further.
		}
		
		public virtual void Destroy() {
			//Override to destroy entity elements.
		}
		
		public virtual void RegisterForEvents() {
			//Override to register for events. Is called after initialize.
		}
		
		public virtual void UnregisterFromEvents() {
			//Override to unregister from other events. Is called before destroy.
		}
		
		private void UnregisterFromMapEvents() {
			//Automatically unregisters from the map.
			this.map.UnregisterForEvents (this);
		}

		public void AddedToMap(WorldPosition Position) {
			this.GameObject.transform.position = Map.GetPositionVector (Position);
			//Override to initialize and/or capture map if necessary.
			this.mapTilePosition = Position;
			this.Initialize ();
			this.RegisterForEvents ();
		}
		
		public void RemovedFromMap() {
			//TODO: Remove object from view?
			this.UnregisterFromMapEvents ();
			this.UnregisterFromEvents ();
			this.map = null;
			this.Destroy ();
		}

		#endregion
		
		#region Update

		public override void Update() {
			//Override to do something with the update.	
		}
		
		#endregion

		#region Events

		/*
		 * Helper function that automatically casts the known event types.
		 */
		public void HandleEvent(IEventInfo eventInfo) {

			switch (eventInfo.EventInfoId) {
			case MapEventInfo.MapEventInfoId: 
				this.HandleMapEvent(eventInfo as MapEventInfo); break;
			case GameMapEventInfo.GameMapEventInfoId:
				this.HandleGameMapEvent(eventInfo as GameMapEventInfo); break;
			}

		}

		protected virtual void HandleMapEvent(MapEventInfo Info) {
			//Override in sub class.
		}
		
		protected virtual void HandleGameMapEvent(GameMapEventInfo Info) {
			//Override in sub class.
		}

		#endregion

	}

}

