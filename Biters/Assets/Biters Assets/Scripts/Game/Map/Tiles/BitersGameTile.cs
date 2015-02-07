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
		public const float BitersGameTileZOffset = 2.0f;

		//Position in the map's world. Should only be changed by the Map.
		private WorldPosition mapTilePosition;

		//Game Map of the tile.
		private IGameMap<BitersGameTile, BitersMapEntity> map = null;

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
		
		private void UnregisterFromGameMapEvents() {
			//Automatically unregisters from the map.
			this.map.UnregisterFromGameMapEvents (this);
		}

		public void AddedToMap(WorldPosition Position) {
			Vector3 position = Map.GetPositionVector (Position);
			position.z = BitersGameTileZOffset;
			this.GameObject.transform.position = position;

			//Override to initialize and/or capture map if necessary.
			this.mapTilePosition = Position;
			this.Initialize ();
			this.RegisterForEvents ();
		}
		
		public void RemovedFromMap() {
			//TODO: Remove object from view?
			this.UnregisterFromGameMapEvents ();
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
				this.HandleMapEvent((MapEventInfo) eventInfo); break;
			case GameMapEventInfo.GameMapEventInfoId:
				this.HandleGameMapEvent((GameMapEventInfo) eventInfo); break;
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

