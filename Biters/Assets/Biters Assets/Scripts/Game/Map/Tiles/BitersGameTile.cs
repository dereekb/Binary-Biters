using System;
using UnityEngine;
using Biters;

namespace Biters.Game
{
	/*
	 * Base class for Biters game tiles. 
	 */
	public abstract class BitersGameTile : Entity, IMapTile, IEventListener 
	{
		//Position in the map's world. Should only be changed by the Map.
		public WorldPosition MapTilePosition { get; set; }

		public BitersGameTile() : base(GameObject.CreatePrimitive(PrimitiveType.Cube)) {}

		public BitersGameTile(GameObject GameObject) : base(GameObject) {}

		#region Map

		public virtual void AddedToMap(Map<IMapTile> Map, WorldPosition Position) {
			//Override. Use to initialize and capture map if necessary.
		}
		
		public virtual void RemovedFromMap(Map<IMapTile> Map) {
			//TODO: Remove from view. Discard.
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

