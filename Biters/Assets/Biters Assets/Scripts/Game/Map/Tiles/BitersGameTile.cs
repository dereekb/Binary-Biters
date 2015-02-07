using System;
using UnityEngine;
using Biters;

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
		private GameMap<IGameMapTile, IGameMapEntity> map;

		public BitersGameTile() : base(GameObject.CreatePrimitive(PrimitiveType.Cube)) {}

		public BitersGameTile(GameObject GameObject) : base(GameObject) {}

		#region Map

		public WorldPosition MapTilePosition {

			get {
				return this.mapTilePosition;
			}

		}

		public GameMap<IGameMapTile, IGameMapEntity> Map {
			
			get {
				return this.Map;
			}
			
		}

		public virtual void Initialize() {
			//Override to initialize entity further.
		}
		
		public virtual void Destroy() {
			//Override to destroy entity elements.
		}

		public void AddedToMap(Map<IMapTile> Map, WorldPosition Position) {
			this.GameObject.transform.position = Map.GetPositionVector (Position);
			//Override to initialize and/or capture map if necessary.
		}
		
		public void RemovedFromMap(Map<IMapTile> Map) {
			//TODO: Remove object from view?
		}

		public void AddedAsTileToGameMap(GameMap<IGameMapTile, IGameMapEntity> Map, WorldPosition Position) {
			this.mapTilePosition = Position;
			this.map = Map;
			this.Initialize ();
		}
		
		public void RemovedAsTileFromGameMap(GameMap<IGameMapTile, IGameMapEntity> Map) {
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

