using System;
using UnityEngine;
using Biters;
using Biters.Utility;

namespace Biters.Game
{
	/*
	 * Base class for Biters game tiles. 
	 */
	public abstract class BitersGameTile : BitersGameEntity, IGameMapTile 
	{

		public const float BitersGameTileZOffset = 2.0f;

		//Position in the map's world. Should only be changed by the Map.
		private WorldPosition mapTilePosition;

		public BitersGameTile() : base(GameObject.CreatePrimitive(PrimitiveType.Cube)) {}

		public BitersGameTile(GameObject GameObject) : base(GameObject) {}

		#region Accessors
		
		public WorldPosition MapTilePosition {
			
			get {
				return this.mapTilePosition;
			}
			
		}

		#endregion

		#region Map

		public virtual void AddedToMap(WorldPosition Position) {}

		public virtual void RemovedFromMap() {}

		public sealed override void AddedToGameMap(WorldPosition Position) {
			Vector3 position = Map.GetPositionVector (Position);
			position.z = BitersGameTileZOffset;
			this.GameObject.transform.position = position;

			//Override to initialize and/or capture map if necessary.
			this.mapTilePosition = Position;
			base.AddedToGameMap (Position);
		}

		#endregion

	}

}

