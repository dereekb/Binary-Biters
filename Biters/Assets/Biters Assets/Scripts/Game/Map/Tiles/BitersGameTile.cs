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
		public WorldPosition MapTilePosition { get; set; }

		//Game Map of the tile.
		protected GameMap<IGameMapTile, IGameMapEntity> Map;

		public BitersGameTile() : base(GameObject.CreatePrimitive(PrimitiveType.Cube)) {}

		public BitersGameTile(GameObject GameObject) : base(GameObject) {}

		#region Map

		public virtual void AddedToMap(Map<IMapTile> Map, WorldPosition Position) {
			//Override. Use to initialize and/or capture map if necessary. Use AddedAsTileToGameMap for GameMap capture.
		}
		
		public virtual void RemovedFromMap(Map<IMapTile> Map) {
			//TODO: Remove object from view. Discard.
		}

		public virtual void AddedAsTileToGameMap(GameMap<IGameMapTile, IGameMapEntity> Map, WorldPosition Position) {
			//Override to initialize, and/or capture the map reference.
			this.Map = Map;
		}
		
		public virtual void RemovedAsTileFromGameMap(GameMap<IGameMapTile, IGameMapEntity> Map) {
			//Override to deallocated using the GameMap. Use RemovedFromMap otherwise.
			this.Map = null;
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

